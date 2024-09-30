using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.DTOs
{
    public class PropertyDTO
    {
        public int IdProperty { get; set; }
        [Required(ErrorMessage = "Name is obligatory")]        
        public string? Name { get; set; }
        public string? Address { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; }
        public string? CodeInternal { get; set; }
        public int Year { get; set; }
        public int IdOwner { get; set; }

        public OwnerDTO Owner { get; set; } = new();
        public List<PropertyImageDTO> PropertyImages { get; set; } = new();
        public List<PropertyTraceDTO> PropertyTraces { get; set; } = new();
    }    
}
