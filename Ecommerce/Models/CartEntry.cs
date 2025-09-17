namespace Ecommerce.Models
{
    // Same as Product, but with a Quantity property.
    // It would be better if an instance of Product was stored as a property,
    // but that turns the interaction with the database more difficult.
    public class CartEntry
    {
        public int Quantity { get; set; }
        public int ProductId { get; set; }
 
        public override string ToString()
        {
            string str = "CartEntry:\n";
            str += $"Quantity = {Quantity}\n";
            str += $"ProductId = {ProductId}\n";
            return str;
        }
    }
}
