using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreMVC.Models
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Название продукта")]
        public string? ProductName { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Название производителя")]
        public string? ManufacturerName { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }
        [Required]
        [Display(Name = "Цена")]
        public double Price { get; set; }
        [Display(Name = "Фото продукта")]
        public string? Image { get; set; }
        [Required]
        [Display(Name = "ID категории")]
        public int CategoryId { get; set; }
        
        public Category? Category { get; set; }
        public List<OrderDetail>? OrderDetail { get; set; }
        public List<CartDetail>? CartDetail { get; set; }

        [NotMapped]
        public string? CategoryName { get; set; }
    }
}
