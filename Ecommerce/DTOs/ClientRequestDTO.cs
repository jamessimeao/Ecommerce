namespace Ecommerce.DTOs
{
    public class ClientRequestDTO
    {
        public uint Quantity { get; set; }
        public uint ProductId { get; set; }
        public bool CreateCheckoutButton { get; set; }
    }
}