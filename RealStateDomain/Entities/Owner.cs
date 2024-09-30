using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Domain.Entities
{
    public class Owner
    {
        public int IdOwner { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Photo { get; set; }
        public DateTime Birthday { get; set; }


        public ICollection<Property> Properties { get; set; } = new List<Property>();

    }
}
