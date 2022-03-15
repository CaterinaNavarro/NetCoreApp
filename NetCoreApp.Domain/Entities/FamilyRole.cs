using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreApp.Domain.Entities
{
    [Table("FamilyRoles")]
    public class FamilyRole : EntidadBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
