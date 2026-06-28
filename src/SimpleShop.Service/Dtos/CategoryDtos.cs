using System.ComponentModel.DataAnnotations;

namespace SimpleShop.Service.Dtos;

public record CategoryDto(int Id, string Name, string Description, bool IsActive, int OwnerId, string OwnerName, DateTime CreatedAt, int ProductCount);

// public record CreateCategoryRequest(
//     [property: Required, StringLength(100, MinimumLength = 2)] string Name,
//     [property: Required, StringLength(250)] string Description);

public class CreateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    [StringLength(250)]
    public string Description { get; set; }
}
public class UpdateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    [StringLength(250)]
    public string Description { get; set; }

    public bool IsActive { get; set; }
}
