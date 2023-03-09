using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Model;
using Microsoft.Extensions.DependencyInjection;
using AngleSharp;
using AngleSharp.Dom;
using FluentAssertions;
using Northwind.Web.Tests.TestDataGenerators;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Web.Tests
{
    [TestClass]
    public class CategoriesControllerIntegrationTesting
    {
        private const string AspNetVerificationTokenName = "__RequestVerificationToken";

        [TestMethod]
        public async Task Index_ReturnViewResult_WithAllCategories()
        {
            // Создаем пустой контекст EF в памяти и заносим 10 категорий
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var categories = categoryGenerator.Generate(10).ToList();
            context.SaveChanges();

            // Запускаем наше приложение на основе созданного и заполненного 
            // контекста и получаем HTTP клиент, который будет к этому приложению
            // обращаться
            var client = GetTestHttpClient(
                () => NorthwindContextHelpers.GetInMemoryContext());

            // Делаем GET запрос к списку категорий
            var response = await client.GetStringAsync("/categories");

            // Парсим полученную HTML, достаем данные о категориях
            var result = GetResultCategories(response).ToList();

            // Сверяем полученные в запросе и созданные ранее категории
            result.Should().BeEquivalentTo(categories, 
                options => options
                    .Excluding(c => c.Products)
                    .Excluding(c => c.Picture));
        }

        [TestMethod]
        public async Task Create_AddNewCategory_WithoutPicture_AsUrlEncodedForm_And_RedirectToList()
        {
            // Создаем пустой контекст и 1 категорию, но в базу её не сохраняем
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var category = new CategoryGenerator().Generate();

            // Запускаем приложение и получаем клиент, но с опцией, что он
            // не будет автоматически выполнять Redirect (чтобы мы могли проверить реальный
            // ответ на наше запрос)
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(), 
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            // Обращаемся к форме создания новой категории, только чтобы получить
            // верификационный токен
            var createForm = await client.GetStringAsync("/categories/create");
            var verificationToken = GetRequestVerificationToken(createForm);

            // Формируем запрс, как если бы отправлялась ранее полученная форма
            var formContent = new FormUrlEncodedContent(
                new Dictionary<string, string> {
                    [nameof(Category.CategoryName)] = category.CategoryName,
                    [nameof(Category.Description)] = category.Description,
                    [AspNetVerificationTokenName] = verificationToken
                });

            // Получаем ответ и достаем из базы только что созданную категорию
            var response = await client.PostAsync("/categories/create", formContent);
            context = NorthwindContextHelpers.GetInMemoryContext();
            var newCategory = context.Categories.First();

            // Проверяем, что в качестве ответа нам пришел редирект на список категорий
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.Should().Be("/Categories");

            // Проверяем, что категория из базы совпадает с тестовой
            newCategory.Should().BeEquivalentTo(category,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));
        }

        [TestMethod]
        public async Task Create_AddNewCategory_WithPicture_AsMultipartForm_And_RedirectToList()
        {
            // Всё делаем аналогично тесту выше, кроме формирования запроса
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var category = new CategoryGenerator().Generate();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var createForm = await client.GetStringAsync("/categories/create");
            var verificationToken = GetRequestVerificationToken(createForm);

            // Чтобы можно было отправить сразу тело файла картинки исползуем
            // multipart/form-data запрос
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(category.CategoryName), nameof(Category.CategoryName));
            multipartContent.Add(new StringContent(category.Description), nameof(Category.Description));
            multipartContent.Add(new StringContent(verificationToken), AspNetVerificationTokenName);
            multipartContent.Add(new ByteArrayContent(category.Picture), "Picture", "picture.jpg");

            // Получаем и счверяем результат как в предыдущем тесте, только сверяем еще и картинку 
            var response = await client.PostAsync("/categories/create", multipartContent);
            context = NorthwindContextHelpers.GetInMemoryContext();
            var newCategory = context.Categories.First();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.Should().Be("/Categories");
            newCategory.Should().BeEquivalentTo(category,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description)
                    .Including(c => c.Picture));
        }

        public async Task Create_AddNewCategory_WithoutName_AsUrlEncodedForm_And_NotRedirectToList()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var category = categoryGenerator.Generate();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var createForm = await client.GetStringAsync("/categories/create");
            var verificationToken = GetRequestVerificationToken(createForm);
            var formContent = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    [nameof(Category.CategoryName)] = "",
                    [AspNetVerificationTokenName] = verificationToken,
                });

            var response = await client.PostAsync("/categories/create", formContent);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Headers.Location.Should().BeNull();
            var error = GetResultError;

            error.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Details_GetDetailsOfCategory_And_RedirectToDetails()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var category = categoryGenerator.Generate();
            context.SaveChanges();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var categoryId = context.Categories.First().CategoryId;
            var response = await client.GetAsync($"/categories/details/{categoryId}");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var responseToHTML = await client
                .GetStringAsync($"/categories/Details/{categoryId}");

            var document = GetDocument(responseToHTML);

            var categoryFromDocument = GetCategory(document);
            var products = GetProducts(document);

            categoryFromDocument.Should().BeEquivalentTo(category,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));

        }
        [TestMethod]
        public async Task Details_GetDetailsOfNotExistingCategory_And_RedirectToNotFoundPage()
        {
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync($"/categories/details/-1");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [TestMethod]
        public async Task Details_GetDetailsOfCategory_WithoutId_And_RedirectToNotFoundPage()
        {
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync($"/categories/details");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [TestMethod]
        public async Task Edit_ChangeExistingCategory_WithValidData_AsMultipartForm_And_RedirectToList()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var category = categoryGenerator.Generate();
            context.SaveChanges();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var editForm = await client.GetStringAsync($"/categories/edit/{category.CategoryId}");
            var verificationToken = GetRequestVerificationToken(editForm);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(category.CategoryId.ToString()), nameof(category.CategoryId));
            multipartContent.Add(new StringContent(category.CategoryName), nameof(Category.CategoryName));
            multipartContent.Add(new StringContent(category.Description), nameof(Category.Description));
            multipartContent.Add(new StringContent(verificationToken), AspNetVerificationTokenName);
            multipartContent.Add(new ByteArrayContent(category.Picture), "Picture", "picture.jpg");

            var response = await client.PostAsync($"/categories/edit/{category.CategoryId}", multipartContent);
            context = NorthwindContextHelpers.GetInMemoryContext();
            var editedCategory = context.Categories.First();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.Should().Be("/Categories");
            editedCategory.Should().BeEquivalentTo(category,
                options => options
                .Including(c => c.CategoryName)
                .Including(c => c.Description)
                .Including(c => c.Picture));
        }

        [TestMethod]
        public async Task Edit_ChangeExistingCategory_WithInvalidName_AsMultipartForm_And_NotRedirectToList()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var category = categoryGenerator.Generate();
            context.SaveChanges();


            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var editForm = await client.GetStringAsync($"/categories/edit/{category.CategoryId}");
            var verificationToken = GetRequestVerificationToken(editForm);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(category.CategoryId.ToString()), nameof(category.CategoryId));
            multipartContent.Add(new StringContent(""), nameof(Category.CategoryName));
            multipartContent.Add(new StringContent(category.Description), nameof(Category.Description));
            multipartContent.Add(new StringContent(verificationToken), AspNetVerificationTokenName);
            multipartContent.Add(new ByteArrayContent(category.Picture), "Picture", "picture.jpg");

            var response = await client.PostAsync($"/categories/edit/{category.CategoryId}", multipartContent);
            var error = GetResultError;

            error.Should().NotBeNull();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            context.Categories.First().Should().BeEquivalentTo(category);
        }
        [TestMethod]
        public async Task Edit_ChangeNotExistingCategory_And_RedirectToNotFoundPage()
        {
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync($"/categories/edit/-1");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [TestMethod]
        public async Task Edit_ChangeCategory_WithoutId_And_RedirectToNotFoundPage()
        {
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync("/categories/edit");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [TestMethod]
        public async Task Delete_RemoveExistingCategory_WithoutProducts_AsUrlEncodedForm_And_RedirectToList()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var category = categoryGenerator.Generate();
            context.SaveChanges();


            var categoriesCount = context.Categories.Count();

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var deleteForm = await client.GetStringAsync($"/categories/delete/{category.CategoryId}");
            var verificationToken = GetRequestVerificationToken(deleteForm);

            var document = GetDocument(deleteForm);

            var categoryFromDocument = GetCategory(document);
            var products = GetProducts(document);

            categoryFromDocument.Should().BeEquivalentTo(category,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));
            var deleteRequest = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    [nameof(Category.CategoryId)] = category.CategoryId.ToString(),
                    [AspNetVerificationTokenName] = verificationToken
                });

            var response = await client.PostAsync($"/categories/delete/{category.CategoryId}", deleteRequest);
            context = NorthwindContextHelpers.GetInMemoryContext();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location.Should().Be("/Categories");
            context.Categories.Should().BeEmpty();
        }
        [TestMethod]
        public async Task Delete_RemoveExistingCategory_WithProducts_AsUrlEncodedForm_And_NotRedirectToList()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context).WithProduct(new ProductGenerator(context), 3);
            var category = categoryGenerator.Generate();
            context.SaveChanges();


            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var deleteForm = await client.GetStringAsync($"/categories/delete/{category.CategoryId}");
            var verificationToken = GetRequestVerificationToken(deleteForm);

            var document = GetDocument(deleteForm);

            var categoryFromDocument = GetCategory(document);
            var products = GetProducts(document);

            categoryFromDocument.Should().BeEquivalentTo(category,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));
            var deleteRequest = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    [nameof(Category.CategoryId)] = category.CategoryId.ToString(),
                    [AspNetVerificationTokenName] = verificationToken
                });

            var response = await client.PostAsync($"/categories/delete/{category.CategoryId}", deleteRequest);
            context = NorthwindContextHelpers.GetInMemoryContext();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            context.Categories.First().Should().BeEquivalentTo(category,
                options => options
                .Excluding(c => c.Products));

            var error = GetResultError;
            error.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Delete_RemoveNotExistingCategory_And_RedirectToNotFoundPage()
        {

            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var categoryId = -1;

            var response = await client.GetAsync($"/categories/delete/{categoryId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Delete_RemoveCategory_WithoutId_And_RedirectToNotFoundPage()
        {
            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync($"/categories/delete");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Image_GetImageOfCategory_ExistingImage_And_RedirectToPicture()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var category = categoryGenerator.Generate();
            context.SaveChanges();


            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync($"/categories/image/{category.CategoryId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var result = await response.Content.ReadAsByteArrayAsync();
            result.Should().BeEquivalentTo(category.Picture);
        }

        [TestMethod]
        public async Task Image_GetImageOfCategory_WithoutImage_And_RedirectToNotFoundPage()
        {
            var context = NorthwindContextHelpers.GetInMemoryContext(true);
            var categoryGenerator = new CategoryGenerator(context);
            var category = categoryGenerator.Generate();
            category.Picture = null;
            context.SaveChanges();


            var client = GetTestHttpClient(() => NorthwindContextHelpers.GetInMemoryContext(),
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var response = await client.GetAsync($"/categories/image/{category.CategoryId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        private static HttpClient GetTestHttpClient(
            Func<NorthwindContext>? context = null,
            WebApplicationFactoryClientOptions? clientOptions = null
            )
        {
            var factory = new WebApplicationFactory<Program>();
            if (context != null)
            {
                factory = factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddScoped<NorthwindContext>(services => context());
                    });
                });
            }

            var client = clientOptions != null
                ? factory.CreateClient(clientOptions)
                : factory.CreateClient();

            return client;
        }
        
        private static string GetRequestVerificationToken(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;
            
            return document?
                .QuerySelector($"input[name='{AspNetVerificationTokenName}']")?
                .GetAttribute("value") ?? "";
        }


        private static IEnumerable<Category> GetResultCategories(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;

            foreach (var categoryRow in document.QuerySelectorAll("tr[data-tid|='category-row']"))
            {
                var id = categoryRow.GetAttribute("data-tid")?.Split("-").Last();
                var name = categoryRow.QuerySelector("td[data-tid='category-name']")?.Text().Trim();
                var description = categoryRow.QuerySelector("td[data-tid='category-description']")?.Text().Trim();

                yield return new Category
                {
                    CategoryId = int.Parse(id ?? "-1"),
                    CategoryName = name ?? "",
                    Description = description,
                    Picture = null,
                };
            }
        }
        private static IDocument GetDocument(string htmlSource)
        {
            var document = BrowsingContext.New(Configuration.Default)
                .OpenAsync(req => req.Content(htmlSource)).Result;
            return document;
        }

        private static IEnumerable<Product> GetProducts(IDocument document)
        {
            foreach (var product in document.QuerySelectorAll("p[data-tid|='product-name']"))
            {
                yield return new Product
                {
                    ProductName = product.Text().Trim() ?? ""
                };
            }
        }

        private static Category GetCategory(IDocument document)
        {
            return new Category
            {
                CategoryName = document.QuerySelector("dd[data-tid='category-name']")?
                    .Text().Trim() ?? "",
                Description = document.QuerySelector("dd[data-tid=\"category-description\"]")?
                    .Text().Trim(),
                Picture = null,
            };
        }
        private static IElement GetResultError(string htmlSource)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlSource)).Result;
            var error = document.QuerySelector("div[class*='validation-summary-errors']");

            return error;
        }
    }
}
