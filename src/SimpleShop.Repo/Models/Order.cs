namespace SimpleShop.Repo.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUser User { get; set; } = default!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Placed";
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
