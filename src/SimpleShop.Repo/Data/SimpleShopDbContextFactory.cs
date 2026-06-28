using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SimpleShop.Repo.Data;

public class SimpleShopDbContextFactory : IDesignTimeDbContextFactory<SimpleShopDbContext>
{
    public SimpleShopDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=simpleshopdb;Username=postgres;Password=12345";

        var optionsBuilder = new DbContextOptionsBuilder<SimpleShopDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new SimpleShopDbContext(optionsBuilder.Options);
    }
}