namespace Ecommerce.Models
{
    public class Subcategory
    {
        public uint Id { get; set; }
        public required string Name { get; set; }
        public uint CategoryId { get; set; }
    }
}
