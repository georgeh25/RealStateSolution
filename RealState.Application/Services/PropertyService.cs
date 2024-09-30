using AutoMapper;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

using ApplicationFilters = RealEstate.Application.Filters;
using DomainFilters = RealEstate.Domain.Filters;

namespace RealEstate.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public PropertyService(IPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<PropertyDTO> CreatePropertyAsync(PropertyDTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            await _propertyRepository.AddAsync(property);
            return _mapper.Map<PropertyDTO>(property);
        }

        public async Task<bool> ChangePriceAsync(int id, decimal newPrice)
        {
           return await _propertyRepository.ChangePriceAsync(id, newPrice);
        }
                
        public async Task<List<PropertyDTO>> ListPropertiesAsync(ApplicationFilters.PropertyFilter filter)
        {
            // Mapping filters
            var domainFilter = _mapper.Map<DomainFilters.PropertyFilter>(filter);

            var properties = await _propertyRepository.ListAsync(domainFilter);
            return _mapper.Map<List<PropertyDTO>>(properties);
        }
        public async Task<PropertyDTO> GetPropertyByIdAsync(int id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            return _mapper.Map<PropertyDTO>(property);
        }

        public async Task<bool> UpdatePropertyAsync(PropertyDTO propertyDto)
        {
            var property = _mapper.Map<Property>(propertyDto);
            return await _propertyRepository.UpdateAsync(property);
        }
    }
}
