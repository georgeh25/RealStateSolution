using AutoMapper;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;

using ApplicationFilters = RealEstate.Application.Filters;
using DomainFilters = RealEstate.Domain.Filters;



namespace RealEstate.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, PropertyDTO>().ReverseMap();
            CreateMap<Owner, OwnerDTO>().ReverseMap();
            CreateMap<PropertyImage, PropertyImageDTO>().ReverseMap();
            CreateMap<PropertyTrace, PropertyTraceDTO>().ReverseMap();

            CreateMap<ApplicationFilters.PropertyFilter, DomainFilters.PropertyFilter>();
        }
    }
}
