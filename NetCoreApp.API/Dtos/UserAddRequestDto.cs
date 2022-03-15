using System.Collections.Generic;
using System.ComponentModel;

namespace NetCoreApp.API.Dtos
{
    public class UserAddRequestDto
    {
        public string Name { get; set; }
        public decimal HeightCm { get; set; }
        public long? FamilyId { get; set; }
        public long? FamilyRoleId { get; set; }
        public IEnumerable<UserAddressAddRequestDto> Addresses { get; set; }
    }
}
