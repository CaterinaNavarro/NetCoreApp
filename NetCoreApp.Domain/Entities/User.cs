using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreApp.Domain.Entities
{
    [Table("Users")]
    public class User : EntidadBase
    {
        public string Name { get; set; }
        public decimal HeightCm { get; set; }
        public IEnumerable<UserAddress> Addresses { get; set; }
        public IEnumerable<UserRelationship> Relationships { get; set; }
    }
}
