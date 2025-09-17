using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ecommerce.Models
{
    public class Product
    {
        public uint Id { get; set; }
        public required string Name { get; set; }
        public Decimal Price { get; set; }
        public uint CategoryId { get; set; }
        public uint SubcategoryId { get; set; }
        public required string ImagePath { get; set; }

        public override string ToString()
        {
            string str = "Product:\n";
            str += $"Id = {Id}\n";
            str += $"Name = {Name}\n";
            str += $"Price = {Price}\n";
            str += $"CategoryId = {CategoryId}\n";
            str += $"SubcategoryId = {SubcategoryId}\n";
            str += $"ImagePath = {ImagePath}\n";
            return str;
        }
    }
}
