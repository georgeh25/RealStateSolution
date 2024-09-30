using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.Filters;
using RealEstate.Application.Interfaces;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        /// <summary>
        /// Creates a new property.
        /// </summary>
        /// <param name="dto">Property data to create.</param>
        /// <returns>Created property.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] PropertyDTO dto)
        {
            var result = await _propertyService.CreatePropertyAsync(dto);
            return CreatedAtAction(nameof(GetPropertyById), new { id = result.IdProperty }, result);
        }

        /// <summary>
        /// Gets a property by its ID.
        /// </summary>
        /// <param name="id">Property ID.</param>
        /// <returns>Found property.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
                return NotFound();
            return Ok(property);
        }

        /// <summary>
        /// Changes the price of a property.
        /// </summary>
        /// <param name="id">Property ID.</param>
        /// <param name="newPrice">New price.</param>
        /// <returns>Operation result.</returns>
        [HttpPut("{id}/price")]
        public async Task<IActionResult> ChangePrice(int id, [FromBody] decimal newPrice)
        {
            await _propertyService.ChangePriceAsync(id, newPrice);
            return NoContent();
        }

        /// <summary>
        /// Lists properties with optional filters.
        /// </summary>
        /// <param name="filter">Search filters.</param>
        /// <returns>List of properties.</returns>
        [HttpGet]
        public async Task<IActionResult> ListProperties([FromQuery] PropertyFilter filter)
        {
            var properties = await _propertyService.ListPropertiesAsync(filter);
            return Ok(properties);
        }

        /// <summary>
        /// Updates an existing property.
        /// </summary>
        /// <param name="id">ID of the property to update.</param>
        /// <param name="dto">Updated property data.</param>
        /// <returns>Operation result.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(int id, [FromBody] PropertyDTO dto)
        {
            if (id != dto.IdProperty)
                return BadRequest("The provided ID does not match the property ID.");
            await _propertyService.UpdatePropertyAsync(dto);
            return NoContent();
        }
    }
}