namespace Ecommerce.DTOs
{
    public class CartEntryRequest
    {
        public uint Quantity { get; set; }
        public uint ProductId { get; set; }
        public bool CreateCheckoutButton { get; set; }
    }
}
