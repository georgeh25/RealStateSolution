using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Interfaces;
using RealEstate.Application.Mapping;
using RealEstate.Application.Services;

namespace RealEstate.Application
{
    public static class ServiceRegistration
    {
        // Extension method for IServiceCollection to add application-specific services
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper and its mapping profiles            
            services.AddAutoMapper(typeof(MappingProfile));
            // Register the PropertyService as a scoped service
            services.AddScoped<IPropertyService, PropertyService>();
        }
    }
}
