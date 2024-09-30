using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Filters
{
    public class PropertyFilter
    {
        public string? Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Address { get; set; }
        public int? Year { get; set; }
        public int? IdOwner { get; set; }
    }
}
