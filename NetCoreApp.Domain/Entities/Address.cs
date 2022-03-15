using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreApp.Domain.Entities
{
    [Table("Addresses")]
    public class Address : EntidadBase
    {
        public string Name { get; set; }
        public string Number { get; set; }
    }
}
