using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.DTOs
{
    public class PropertyImageDTO
    {
        public int IdPropertyImage { get; set; }
        public int IdProperty { get; set; }
        public string? File { get; set; }
        public bool Enabled { get; set; }
    }
}
