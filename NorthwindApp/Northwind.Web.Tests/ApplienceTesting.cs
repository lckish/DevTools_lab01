using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Northwind.Model;
using Microsoft.Extensions.DependencyInjection;
using AngleSharp;
using AngleSharp.Dom;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using AngleSharp.Html.Parser;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AngleSharp.Html.Dom;

namespace Northwind.Web.Tests
{
    [TestClass]
    public class ApplienceTesting
    {
        [TestMethod]
        public async Task Home_ReturnViewResult_With_Links()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetStringAsync("/");
            var document = GetDocument(response);

            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href);

            links.Any(l=>l.EndsWith("/")).Should().BeTrue();
            links.Any(l=>l.Contains("Categories")).Should().BeTrue();
        }

        [TestMethod]
        public async Task Create_ReturnViewResult_With_EmptyForm_And_Links()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetStringAsync("/categories/create");
            var document = GetDocument(response);

            var category = GetCategoryFromForm(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href).Where(l => l.EndsWith("/Categories"));

            category.Should().BeEquivalentTo(new Category()
            {
                CategoryName = string.Empty,
                Description = string.Empty,
            });

            links.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task Delete_ReturnViewResult_With_CategoryInfo_And_ProductList_And_Links_WhenIdIsValid()
        {
            var context = new NorthwindContext(
                new DbContextOptionsBuilder<NorthwindContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Northwind;" +
                "Trusted_Connection=True;MultipleActiveResultSets=true").Options);
            var categoryReference = context.Categories.Include(c => c.Products).First();

            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetStringAsync($"/categories/delete/{categoryReference.CategoryId}");
            var document = GetDocument(response);

            var category = GetCategory(document);
            var products = GetProducts(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href).Where(l => l.EndsWith("/Categories"));

            category.Should().BeEquivalentTo(categoryReference,
                options => options
                    .Excluding(c => c.CategoryId)
                    .Excluding(c => c.Products)
                    .Excluding(c => c.Picture));

            products.Should().BeEquivalentTo(categoryReference.Products,
                options => options
                    .Including(c => c.ProductName));

            links.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task Delete_ReturnNotFoundPage_WhenIdIsInvalid()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetAsync("/categories/delete/-1");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Delete_ReturnNotFoundPage_WhenIdIsNull()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetAsync("/categories/delete/");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Details_ReturnViewResult_With_CategoryInfo_And_ProductList_And_Links_WhenIdIsValid()
        {
            var context = new NorthwindContext(
                new DbContextOptionsBuilder<NorthwindContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Northwind;" +
                "Trusted_Connection=True;MultipleActiveResultSets=true").Options);
            var categoryReference = context.Categories.Include(c => c.Products).First();

            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetStringAsync($"/categories/Details/{categoryReference.CategoryId}");
            var document = GetDocument(response);

            var category = GetCategory(document);
            var products = GetProducts(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href).Where(l => l.EndsWith("/Categories"));

            category.Should().BeEquivalentTo(categoryReference,
                options => options
                    .Excluding(c => c.CategoryId)
                    .Excluding(c => c.Products)
                    .Excluding(c => c.Picture));

            products.Should().BeEquivalentTo(categoryReference.Products,
                options => options
                    .Including(c => c.ProductName));

            links.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task Details_ReturnNotFoundPage_WhenIdIsInvalid()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetAsync("/categories/Details/-1");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Details_ReturnNotFoundPage_WhenIdIsNull()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetAsync("/categories/Details/");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Edit_ReturnViewResult_With_PrfilledForm_And_Links_WhenIdIsValid()
        {
            var context = new NorthwindContext(
                new DbContextOptionsBuilder<NorthwindContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Northwind;" +
                "Trusted_Connection=True;MultipleActiveResultSets=true").Options);
            var categoryReference = context.Categories.First();

            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetStringAsync($"/categories/edit/{categoryReference.CategoryId}");
            var document = GetDocument(response);

            var category = GetCategoryFromForm(document);
            var links = document.Links.OfType<IHtmlAnchorElement>()
                .Select(l => l.Href).Where(l => l.EndsWith("/Categories"));

            category.Should().BeEquivalentTo(categoryReference,
                options => options
                    .Excluding(c => c.CategoryId)
                    .Excluding(c => c.Products)
                    .Excluding(c => c.Picture));

            links.Count().Should().Be(2);
        }

        [TestMethod]
        public async Task Edit_ReturnNotFoundPage_WhenIdIsInvalid()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetAsync("/categories/Edit/-1");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Edit_ReturnNotFoundPage_WhenIdIsNull()
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetAsync("/categories/Edit/");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task Index_ReturnViewResultWithFirstThreeCategories()
        {
            var context = new NorthwindContext(
                new DbContextOptionsBuilder<NorthwindContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Northwind;" +
                "Trusted_Connection=True;MultipleActiveResultSets=true").Options);
            var categoriesRef = context.Categories.Include(c => c.Products.Take(3));

            var linksRef = new Dictionary<string, string>()
            {
                { "Редактировать", $"/Edit/{categoriesRef.First().CategoryId}" },
                { "Детали", $"/Details/{categoriesRef.First().CategoryId}" },
                { "Удалить", $"/Delete/{categoriesRef.First().CategoryId}" }
            };

            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5129") };

            var response = await client.GetStringAsync("/categories");
            var document = GetDocument(response);

            var categories = GetCategories(document).ToList();
            var products = GetProducts(document);
            var links = document.Links.OfType<IHtmlAnchorElement>().Select(l => l.Href);

            categories.Should().BeEquivalentTo(categoriesRef,
                options => options
                    .Excluding(c => c.Products)
                    .Excluding(c => c.Picture));

            products.Take(3).Should().BeEquivalentTo(categoriesRef.First().Products,
                options => options
                .Including(p => p.ProductName));

            links.Where(l=>l.EndsWith("/Create")).Count().Should().Be(1);
            links.Where(l=>l.Contains("/Edit/")).Count().Should().Be(categories.Count);
            links.Where(l=>l.Contains("/Details/")).Count().Should().Be(categories.Count);
            links.Where(l=>l.Contains("/Delete/")).Count().Should().Be(categories.Count);
        }

        private static IDocument GetDocument(string htmlSource)
        {
            return BrowsingContext.New(Configuration.Default)
                .OpenAsync(req => req.Content(htmlSource)).Result;
        }

        private static IEnumerable<Category> GetCategories(IDocument document)
        {
            foreach (var categoryRow in document.QuerySelectorAll("tr[data-tid|='category-row']"))
            {
                var id = categoryRow.GetAttribute("data-tid")?.Split("-").Last();
                var name = categoryRow.QuerySelector("td[data-tid='category-name']")?.Text().Trim();
                var description = categoryRow.QuerySelector("td[data-tid='category-description']")?.Text().Trim();

                yield return new Category
                {
                    CategoryId = int.Parse(id ?? "-1"),
                    CategoryName = name ?? string.Empty,
                    Description = description,
                    Picture = null
                };
            }
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
                Description = document.QuerySelector("dd[data-tid='category-description']")?
                    .Text().Trim(),
                Picture = null,
            };
        }

        private static Category GetCategoryFromForm(IDocument document)
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
