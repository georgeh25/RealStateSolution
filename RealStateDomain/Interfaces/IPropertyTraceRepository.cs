using RealEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Interfaces
{
    public interface IPropertyTraceRepository
    {
        Task<PropertyTrace> GetByIdAsync(int id);
        Task<List<PropertyTrace>> GetByPropertyIdAsync(int propertyId);
        Task<int> AddAsync(PropertyTrace propertyTrace);
    }
}
