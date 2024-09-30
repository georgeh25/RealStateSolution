using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Filters;
using RealEstate.Domain.Interfaces;
using RealEstate.Infrastructure.Data;

namespace RealEstate.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly RealEstateDbContext _context;

        public PropertyRepository(RealEstateDbContext context)
        {
            _context = context;
        }

        public async Task<Property> AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<bool> ChangePriceAsync(int id, decimal newPrice)
        {            
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return false;

            property.Price = newPrice;           

            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Property>> ListAsync(PropertyFilter filter)
        {
            var query = _context.Properties.Include(p => p.Owner).AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(p => p.Name.Contains(filter.Name));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(filter.Address))
            {
                query = query.Where(p => p.Address.Contains(filter.Address));
            }

            if (filter.Year.HasValue)
            {
                query = query.Where(p => p.Year == filter.Year.Value);
            }
            if (filter.IdOwner.HasValue)
            {
                query = query.Where(p => p.IdOwner == filter.IdOwner.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Property> GetByIdAsync(int id)
        {
            return await _context.Properties
                                 .Include(p => p.Owner)
                                 .Include(p => p.PropertyImages)
                                 .Include(p => p.PropertyTraces)
                                 .FirstOrDefaultAsync(p => p.IdProperty == id);
        }

        public async Task<bool> UpdateAsync(Property property)
        {
            _context.Properties.Update(property);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {                
                return false;
            }
        }
    }
    }
