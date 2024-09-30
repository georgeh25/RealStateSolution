using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Interfaces
{
    public interface IPropertyImageRepository
    {
        Task<PropertyImage> GetByIdAsync(int id);
        Task<List<PropertyImage>> GetByPropertyIdAsync(int propertyId);
        Task<int> AddAsync(PropertyImage propertyImage);
        Task UpdateAsync(PropertyImage propertyImage);
        Task DeleteAsync(int id);
    }
}
