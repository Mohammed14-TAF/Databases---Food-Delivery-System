namespace YemekSepeti.Models
{
    public class CartItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int RestaurantID { get; set; } 
        public decimal TotalPrice => Price * Quantity;
    }
}
