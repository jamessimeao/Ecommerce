namespace Ecommerce.Models
{
    public class Subcategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int CategoryId { get; set; }
    }
}
