using System.Collections.Generic;

namespace NetCoreApp.API.Dtos
{
    public class UserGetByIdResponseDto
    {
        public string Name { get; set; }
        public decimal HeightCm { get; set; }
        public IEnumerable<UserGetByIdAddressResponseDto> Adresses { get; set; }
    }
}
