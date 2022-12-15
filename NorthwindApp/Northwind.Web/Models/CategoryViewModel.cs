using Northwind.Model;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Web.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        
        [Display(Name = "Название")]
        [MaxLength(15)]
        public string CategoryName { get; set; } = "";

        [Display(Name = "Описание")]
        [MaxLength(200)]
        public string? Description { get; set; }
        
        [Display(Name = "Изображение")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Продукты")]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    public static class CategoryViewModelExtensions 
    {
        public static CategoryViewModel ToViewModel(this Category category)
        {
            return new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Products = category.Products,
                Picture = null,
            };
        }

        public static Category ToModel(this CategoryViewModel model)
        {
            var category = new Category
            {
                CategoryId = model.CategoryId,
                CategoryName = model.CategoryName,
                Description = model.Description,
                Products = model.Products,
                Picture = null
            };

            if (model.Picture != null && model.Picture.Length != 0)
            {
                using var memoryStream = new MemoryStream();
                model.Picture.CopyTo(memoryStream);
                category.Picture = memoryStream.ToArray();
            }

            return category;
        }

    }
}
