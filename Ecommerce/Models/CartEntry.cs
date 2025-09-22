namespace Ecommerce.Models
{
    public class CartEntry
    {
        public uint Quantity { get; set; }
        public required Product Product { get; set; }
    }
}
