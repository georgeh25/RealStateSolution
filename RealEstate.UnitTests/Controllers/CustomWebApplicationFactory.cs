using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;


namespace RealEstate.UnitTests.Controllers
{
    public class CustomWebApplicationFactory<TStartup>: WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                // Remove the descriptor from the original DbContext.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<RealEstateDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory DbContext for testing
                services.AddDbContext<RealEstateDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    //options.EnableSensitiveDataLogging();
                });

                var serviceProvider = services.BuildServiceProvider();

                // Create a scope to obtain the RealEstateDbContext
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<RealEstateDbContext>();

                    db.Database.EnsureCreated();

                    if (!db.Owners.Any())
                    {
                        db.Owners.Add(new Owner
                        {
                            IdOwner = 1,
                            Name = "Default Owner",
                            Address = "123 Main St",
                            Photo = "default.png",
                            Birthday = new DateTime(1980, 1, 1)
                        });
                        db.SaveChanges();
                    }
                }
            });
        }
    }
}
