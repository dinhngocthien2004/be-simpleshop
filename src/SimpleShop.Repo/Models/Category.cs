namespace SimpleShop.Repo.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }

    public int OwnerId { get; set; }
    public AppUser Owner { get; set; } = default!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
