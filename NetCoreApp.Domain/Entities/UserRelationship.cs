using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreApp.Domain.Entities
{
    [Table("UsersRelationships")]
    public class UserRelationship : EntidadBase
    {
        public long FamilyId { get; set; }
        public Family Family { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        public long? FamilyRoleId { get; set; }
        public FamilyRole FamilyRole { get; set; }
    }
}
