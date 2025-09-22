namespace Ecommerce.DTOs
{
    // Same as Product, but with a Quantity property.
    // It would be better if an instance of Product was stored as a property,
    // but that turns the interaction with the database more difficult.
    public class CartEntryDTO
    {
        public uint Quantity { get; set; }
        public uint ProductId { get; set; }
    }
}
