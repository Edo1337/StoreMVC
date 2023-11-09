using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreMVC.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        public int Id { get; set; }
        [Required]
        public int ShoppingCartId { get; set; }
        [Required]
        public int FoodId { get; set; }
        [Required]
        public int Quantity { get; set; }

        public Food Food { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
