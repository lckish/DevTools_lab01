using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Model;

namespace Northwind.Web.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        HttpClient httpClient = new() { BaseAddress = new Uri("http://localhost:5000") };


        CategoryForTests testCategory = new CategoryForTests();

        [TestMethod]
        public async Task Create_ShouldReturnView_WithEmptyForm()
        {
            var response = await httpClient.GetStringAsync("/categories/create");
            var document = GetDocument(response);
            var category = GetCategoryInput(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href)
                .Where(l => l.EndsWith("/Categories"));
            links.Count().Should().Be(2);
            category.Should().BeEquivalentTo(new Category()
            {
                CategoryName = "",
                Description = "",
            });

        }

        [TestMethod]
        public async Task Delete_ShouldReturnView_WithCategory_And_WithProductList_WhenIdIsValid()
        {

            var response = await httpClient.GetStringAsync($"/categories/delete/1");
            var document = GetDocument(response);

            var category = GetCategory(document);
            var products = GetProducts(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href)
                .Where(l => l.EndsWith("/Categories"));

            links.Count().Should().Be(2);

            category.Should().BeEquivalentTo(testCategory,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));

            products.Should().BeEquivalentTo(testCategory.Products,
                options => options
                    .Including(c => c.ProductName));
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFoundPage_WhenIdIsInvalid()
        {
            var response = await httpClient.GetAsync("/categories/delete/0");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Delete_ShouldReturnNotFoundPage_WhenIdIsNull()
        {
            var response = await httpClient.GetAsync("/categories/delete/");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Details_ShouldReturnView_With_Category_And_Products_WhenIdIsValid()
        {
            var response = await httpClient.GetStringAsync($"/categories/Details/1");
            var document = GetDocument(response);

            var category = GetCategory(document);
            var products = GetProducts(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href).Where(l => l.EndsWith("/Categories"));

            links.Count().Should().Be(2);

            category.Should().BeEquivalentTo(testCategory,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));

            products.Should().BeEquivalentTo(testCategory.Products,
                options => options
                    .Including(c => c.ProductName));
        }

        [TestMethod]
        public async Task Details_ShouldReturnNotFoundPage_WhenIdIsInvalid()
        {
            var response = await httpClient.GetAsync("/categories/Details/0");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Details_ShouldReturnNotFoundPage_WhenIdIsNull()
        {
            var response = await httpClient.GetAsync("/categories/Details/");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Edit_ShouldReturnView_WithFilledFields_WhenIdIsValid()
        {
            var response = await httpClient.GetStringAsync($"/categories/edit/1");
            var document = GetDocument(response);

            var category = GetCategoryInput(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href).Where(l => l.EndsWith("/Categories"));
            links.Count().Should().Be(2);

            category.Should().BeEquivalentTo(testCategory,
                options => options
                    .Including(c => c.CategoryName)
                    .Including(c => c.Description));
        }

        [TestMethod]
        public async Task Edit_ShouldReturnNotFoundPage_WhenIdIsInvalid()
        {
            var response = await httpClient.GetAsync("/categories/Edit/0");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Edit_ShouldReturnNotFoundPage_WhenIdIsNull()
        {
            var response = await httpClient.GetAsync("/categories/Edit/");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Home_ShouldReturnView()
        {
            var response = await httpClient.GetStringAsync("/");
            var document = GetDocument(response);

            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href).ToList();

            links.Where(l => l.EndsWith("/")).Should().NotBeEmpty();
            links.Where(l => l.Contains("Categories")).Should().NotBeEmpty();
        }

        private static IDocument GetDocument(string htmlSource)
        {
            return BrowsingContext.New(Configuration.Default)
                .OpenAsync(req => req.Content(htmlSource)).Result;
        }

        private static IEnumerable<Product> GetProducts(IDocument document)
        {
            foreach (var product in document.QuerySelectorAll("p[data-tid|='product-name']"))
            {
                yield return new Product
                {
                    ProductName = product.Text().Trim() ?? string.Empty
                };
            }
        }

        private static Category GetCategory(IDocument document)
        {
            return new Category
            {
                CategoryName = document.QuerySelector("dd[data-tid='category-name']")?
                    .Text().Trim() ?? string.Empty,
                Description = document.QuerySelector("dd[data-tid=\"category-description\"]")?
                    .Text().Trim(),
                Picture = null,
            };
        }

        private static Category GetCategoryInput(IDocument document)
        {
            return new Category
            {
                CategoryName = document.QuerySelector("input[data-tid='category-name']")?
                    .GetAttribute("value")?.Trim() ?? string.Empty,
                Description = document.QuerySelector("input[data-tid='category-description']")?
                    .GetAttribute("value")?.Trim(),
                Picture = null
            };
        }
    }
}
