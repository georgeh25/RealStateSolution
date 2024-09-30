using RealEstate.Domain.Entities;
using RealEstate.Domain.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Interfaces
{    public interface IPropertyRepository
    {
        Task<Property> GetByIdAsync(int id);
        Task<List<Property>> ListAsync(PropertyFilter filter);
        Task<Property> AddAsync(Property property);
        Task<bool> UpdateAsync(Property property);
        Task<bool> ChangePriceAsync(int id, decimal newPrice);
    }

    
}
