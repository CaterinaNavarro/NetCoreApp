using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreApp.API.Dtos
{
    public class UserAddressAddRequestDto
    {
        public long AddressId { get; set; }
        public bool Main { get; set; }
    }
}
