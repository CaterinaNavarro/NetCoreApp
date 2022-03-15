using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreApp.Domain.Entities
{
    [Table("UsersAddresses")]
    public class UserAddress : EntidadBase
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public long AddressId { get; set; }
        public Address Address { get; set; }
        public bool Main { get; set; }
    }
}
