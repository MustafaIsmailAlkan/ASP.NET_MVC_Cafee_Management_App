using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Cafee_Prototype.Models
{
    public class ProductCategory
    {
        public int ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Yanlış veya boş kategori ismi girdiniz.")]
        [DisplayName("Kategori İsmi")]
        public string? CategoryName { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}

