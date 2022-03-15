using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreApp.Domain.Entities
{
    [Table("Families")]
    public class Family : EntidadBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<UserRelationship> Members { get; set; }
    }
}
