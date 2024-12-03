using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cafee_Prototype.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Yanlış veya boş isim girdiniz.")]
        [DisplayName("İsim")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage ="Yanlış veya boş fiyat girdiniz.")]
        [DisplayName("Fiyat")]
        public decimal ProductPrice { get; set; }

        public string? ProductImage { get; set; }

        [Required(ErrorMessage ="Yanlış veya boş açıklama girdiniz.")]
        [DisplayName("Açıklama")]
        public string? ProductDescription { get; set; }

        public bool IsActive { get; set; }
        
        [Required]
        [DisplayName("Kategori")]
        public int ProductCategoryId { get; set; }

        [ForeignKey("ProductCategoryId")]
        public ProductCategory? ProductCategory { get; set; }
    }
}