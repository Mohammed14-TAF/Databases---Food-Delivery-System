namespace YemekSepeti.Models
{
    public class Restaurant
{
    public int RestaurantID { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Address { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public bool IsActive { get; set; }
    public string? Category { get; set; }

}
}