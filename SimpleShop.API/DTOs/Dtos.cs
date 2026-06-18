using System.ComponentModel.DataAnnotations;

namespace SimpleShop.API.DTOs;

// ─── Category DTOs ───────────────────────────────────────────
public class CategoryCreateDto
{
    [Required(ErrorMessage = "CategoryName is required.")]
    [MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "CategoryDescription is required.")]
    [MaxLength(250)]
    public string CategoryDescription { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class CategoryUpdateDto
{
    [Required(ErrorMessage = "CategoryName is required.")]
    [MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "CategoryDescription is required.")]
    [MaxLength(250)]
    public string CategoryDescription { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class CategoryResponseDto
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDescription { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

// ─── Product DTOs ────────────────────────────────────────────
public class ProductCreateDto
{
    [Required(ErrorMessage = "ProductName is required.")]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "StockQuantity must be >= 0.")]
    public int StockQuantity { get; set; } = 0;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryID { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ProductUpdateDto
{
    [Required(ErrorMessage = "ProductName is required.")]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "StockQuantity must be >= 0.")]
    public int StockQuantity { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryID { get; set; }

    public bool IsActive { get; set; } = true;
}

public class ProductResponseDto
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryID { get; set; }
    public string? CategoryName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

// ─── Auth DTOs ───────────────────────────────────────────────
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
