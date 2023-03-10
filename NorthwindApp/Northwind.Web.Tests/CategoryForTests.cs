using Northwind.Model;

namespace Northwind.Web.Tests
{
    internal class CategoryForTests
    {
        public string CategoryName = "Beverages";
        public string Description = "Soft drinks, coffees, teas, beers, and ales";
        public List<Product>? Products = new List<Product>();
        public CategoryForTests()
        {
            string[] productNames =
            {
                "Chai",
                "Chang",
                "Guaraná Fantástica",
                "Sasquatch Ale",
                "Steeleye Stout",
                "Côte de Blaye",
                "Chartreuse verte",
                "Ipoh Coffee",
                "Laughing Lumberjack Lager",
                "Outback Lager",
                "Rhönbräu Klosterbier",
                "Lakkalikööri"
            };
            for(int i = 0; i < productNames.Length; i++)
            {
                Product product = new Product();
                product.ProductName = productNames[i];
                Products.Add(product);
            }
        }
    }
}
