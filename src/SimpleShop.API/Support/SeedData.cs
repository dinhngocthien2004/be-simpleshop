using Microsoft.EntityFrameworkCore;
using SimpleShop.Repo.Data;
using SimpleShop.Repo.Models;

namespace SimpleShop.API.Support;

public static class SeedData
{
    public static async Task EnsureAdminAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SimpleShopDbContext>();

        var email = Environment.GetEnvironmentVariable("SEED_ADMIN_EMAIL")
            ?? configuration["Seed:AdminEmail"]
            ?? "admin@simpleshop.com";
        var password = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD")
            ?? configuration["Seed:AdminPassword"]
            ?? "@@Admin123@@";

        email = email.Trim().ToLower();

        // INSERT Code First: thêm admin bằng DbContext, không dùng file .sql.
        var admin = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (admin == null)
        {
            admin = new AppUser
            {
                Name = "System Admin",
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }

        // INSERT Code First: mỗi user cần có một giỏ hàng.
        if (!await db.Carts.AnyAsync(c => c.UserId == admin.Id))
        {
            db.Carts.Add(new Cart { UserId = admin.Id });
            await db.SaveChangesAsync();
        }

        await EnsureSampleCategoriesAndProductsAsync(db, admin.Id);
    }

    private static async Task EnsureSampleCategoriesAndProductsAsync(SimpleShopDbContext db, int adminId)
    {
        var now = DateTime.UtcNow;

        // INSERT Code First: seed danh mục mẫu bằng AddRangeAsync + SaveChangesAsync.
        var sampleCategories = new[]
        {
            new Category
            {
                Name = "Electronics",
                Description = "Thiết bị điện tử và phụ kiện công nghệ",
                OwnerId = adminId,
                IsActive = true,
                CreatedAt = now
            },
            new Category
            {
                Name = "Fashion",
                Description = "Quần áo và phụ kiện thời trang",
                OwnerId = adminId,
                IsActive = true,
                CreatedAt = now
            },
            new Category
            {
                Name = "Books",
                Description = "Sách và tài liệu học tập",
                OwnerId = adminId,
                IsActive = true,
                CreatedAt = now
            }
        };

        var existingCategoryNames = await db.Categories
            .Where(c => c.OwnerId == adminId)
            .Select(c => c.Name)
            .ToListAsync();

        var categoriesToInsert = sampleCategories
            .Where(c => !existingCategoryNames.Contains(c.Name))
            .ToList();

        if (categoriesToInsert.Count > 0)
        {
            await db.Categories.AddRangeAsync(categoriesToInsert);
            await db.SaveChangesAsync();
        }

        var categories = await db.Categories
            .Where(c => c.OwnerId == adminId)
            .ToDictionaryAsync(c => c.Name, c => c.Id);

        // INSERT Code First: seed sản phẩm mẫu, khóa ngoại lấy từ category vừa insert.
        var sampleProducts = new[]
        {
           new Product
        {
            Name = "Laptop Dell Inspiron 15",
            Description = "Laptop văn phòng, học tập, màn hình 15 inch",
            Price = 15500000m,
            StockQuantity = 10,
            CategoryId = categories["Electronics"],
            OwnerId = adminId,
            IsActive = true,
            ImageUrl = "/images/products/laptop-dell-inspiron-15.jpg",
            CreatedAt = now
        },

        new Product
        {
            Name = "Tai nghe Bluetooth",
            Description = "Tai nghe không dây pin lâu, âm thanh rõ",
            Price = 750000m,
            StockQuantity = 30,
            CategoryId = categories["Electronics"],
            OwnerId = adminId,
            IsActive = true,
            ImageUrl = "/images/products/tai-nghe-bluetooth.jpg",
            CreatedAt = now
        },

        new Product
        {
            Name = "Tai nghe Sony WH-1000XM5",
            Description = "Tai nghe chống ồn cao cấp",
            Price = 7500000m,
            StockQuantity = 20,
            CategoryId = categories["Electronics"],
            OwnerId = adminId,
            IsActive = true,
            ImageUrl = "/images/products/tai-nghe-sony-wh-1000xm5.jpg",
            CreatedAt = now
        },

        new Product
        {
            Name = "Laptop Dell Inspiron 15",
            Description = "Laptop văn phòng, học tập, màn hình 15 inch",
            Price = 15500000m,
            StockQuantity = 10,
            CategoryId = categories["Electronics"],
            OwnerId = adminId,
            IsActive = true,
            ImageUrl = "/images/products/laptop-dell-inspiron-15.jpg",
            CreatedAt = now
        },

        new Product
        {
            Name = "Tai nghe Bluetooth",
            Description = "Tai nghe không dây pin lâu, âm thanh rõ",
            Price = 750000m,
            StockQuantity = 30,
            CategoryId = categories["Electronics"],
            OwnerId = adminId,
            IsActive = true,
            ImageUrl = "/images/products/tai-nghe-bluetooth.jpg",
            CreatedAt = now
        },
    new Product
    {
        Name = "Tai nghe Sony WH-1000XM5",
        Description = "Tai nghe chống ồn cao cấp",
        Price = 7500000m,
        StockQuantity = 20,
        CategoryId = categories["Electronics"],
        OwnerId = adminId,
        IsActive = true,
        ImageUrl = "https://images.unsplash.com/photo-1518441902117-f0a22d3e2c88", // premium headphones
        CreatedAt = now
    },
    new Product
    {
        Name = "Lập trình ASP.NET Core",
        Description = "Sách hướng dẫn ASP.NET Core Web API và EF Core Code First",
        Price = 250000m,
        StockQuantity = 20,
        CategoryId = categories["Books"],
        OwnerId = adminId,
        IsActive = true,
        ImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794", // programming book
        CreatedAt = now
    }
        };

        var existingProductNames = await db.Products
            .Where(p => p.OwnerId == adminId)
            .Select(p => p.Name)
            .ToListAsync();

        var productsToInsert = sampleProducts
            .Where(p => !existingProductNames.Contains(p.Name))
            .ToList();

        if (productsToInsert.Count > 0)
        {
            await db.Products.AddRangeAsync(productsToInsert);
            await db.SaveChangesAsync();
        }
    }
}
