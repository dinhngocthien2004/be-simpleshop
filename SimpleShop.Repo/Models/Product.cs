using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleShop.Repo.Models;

[Table("Product")]
public class Product
{
    [Key]
    [Column("ProductID")]
    public int ProductID { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("ProductName")]
    public string ProductName { get; set; } = string.Empty;

    [Column("Description")]
    public string? Description { get; set; }

    [Required]
    [Column("Price")]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Column("StockQuantity")]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;

    [MaxLength(500)]
    [Column("ImageUrl")]
    public string? ImageUrl { get; set; }

    [Required]
    [Column("CategoryID")]
    public int CategoryID { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Column("ModifiedDate")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CategoryID")]
    public Category? Category { get; set; }
}
