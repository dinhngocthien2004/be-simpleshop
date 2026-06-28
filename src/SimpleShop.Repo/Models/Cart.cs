namespace SimpleShop.Repo.Models;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AppUser User { get; set; } = default!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
