namespace NetCoreApp.API.Dtos
{
    public class UserGetByIdAddressResponseDto
    {
        public long AddressId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public bool Main { get; set; }
    }
}
