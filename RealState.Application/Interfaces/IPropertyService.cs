using RealEstate.Application.DTOs;
using RealEstate.Application.Filters;

namespace RealEstate.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<PropertyDTO> CreatePropertyAsync(PropertyDTO propertyDto);
        Task<PropertyDTO> GetPropertyByIdAsync(int id);
        Task<List<PropertyDTO>> ListPropertiesAsync(PropertyFilter filter);
        Task<bool> UpdatePropertyAsync(PropertyDTO propertyDto);
        Task<bool> ChangePriceAsync(int id, decimal newPrice);
    }
}
