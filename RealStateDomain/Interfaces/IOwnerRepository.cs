using RealEstate.Domain.Entities;

namespace RealEstate.Domain.Interfaces
{
    public interface IOwnerRepository
    {
        Task<Owner> GetByIdAsync(int id);        
        Task AddAsync(Owner owner);
        Task UpdateAsync(Owner owner);
        Task DeleteAsync(int id);
    }
}
