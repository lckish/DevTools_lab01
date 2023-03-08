using Bogus;
using Northwind.Model;

namespace Northwind.Web.Tests.TestDataGenerators
{
    public class ProductGenerator : ITestDataGenerator<Product>
    {
        private readonly NorthwindContext? northwindContext;
        private Faker<Product> faker;
        public Product Generate()
        {
            var product = faker.Generate();
            northwindContext?.Products.Add(product);
            return product;
        }

        public IEnumerable<Product> Generate(int count)
        {
            var products = faker.Generate(count);
            northwindContext?.Products.AddRange(products);
            return products;
        }

        public ProductGenerator(NorthwindContext? northwindContext)
        {
            this.northwindContext = northwindContext;
            faker = new Faker<Product>().
                StrictMode(false)
                .RuleFor(p => p.ProductName, f => f.Commerce.ProductName())
                .RuleFor(p => p.SupplierId, f => f.Random.Int(1, 2147483647))
                .RuleFor(p => p.CategoryId, f => f.Random.Int(1, 2147483647))
                .RuleFor(p => p.QuantityPerUnit, f => f.Lorem.Sentence())
                .RuleFor(p => p.UnitPrice, f => Convert.ToDecimal(f.Commerce.Price()))
                .RuleFor(p => p.UnitsInStock, f => f.Random.Short(1, 32767))
                .RuleFor(p => p.UnitsOnOrder, f => f.Random.Short(1, 32767))
                .RuleFor(p => p.ReorderLevel, f => f.Random.Short(1, 32767))
                .RuleFor(p => p.Discontinued, f => f.Random.Bool());
        }

        public ProductGenerator WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(nameof(name),
                    "Явно указанное имя продукта не должно быть пустым");

            faker.RuleFor(p => p.ProductName, () => name);
            return this;
        }

        public ProductGenerator WithSupplierId(int supplierId)
        {
            faker.RuleFor(p => p.SupplierId, () => supplierId);
            return this;
        }

        public ProductGenerator WithCategoryId(int categoryId)
        {
            faker.RuleFor(p => p.CategoryId, () => categoryId);
            return this;
        }

        public ProductGenerator WithQuantityPerUnit(string quantityPerUnit)
        {
            faker.RuleFor(p => p.QuantityPerUnit, () => quantityPerUnit);
            return this;
        }

        public ProductGenerator WithUnitPrice(decimal unitPrice)
        {
            faker.RuleFor(p => p.UnitPrice, () => unitPrice);
            return this;
        }

        public ProductGenerator UnitsInStock(short unitsInStock)
        {
            faker.RuleFor(p => p.UnitsInStock, () => unitsInStock);
            return this;
        }

        public ProductGenerator WithUnitsOnOrder(short unitsOnOrder)
        {
            faker.RuleFor(p => p.UnitsOnOrder, () => unitsOnOrder);
            return this;
        }

        public ProductGenerator WithReorderLevel(short reorderLevel)
        {
            faker.RuleFor(p => p.ReorderLevel, () => reorderLevel);
            return this;
        }

        public ProductGenerator WithDiscontinued(bool discontinued)
        {
            faker.RuleFor(p => p.Discontinued, () => discontinued);
            return this;
        }
    }
}