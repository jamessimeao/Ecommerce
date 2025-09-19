using Dapper;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Ecommerce.Data.Dapper
{
    public static partial class DataManipulation
    {
        private const uint MAX_STRING_SIZE = 100;

        // Any string with alphanumeric characters or -
        [GeneratedRegex(@"^[\w-\s]*$")] 
        private static partial Regex MyRegex();

        // A string is considered safe if it can't lead to sql injection when inserted in sql as 'str'.
        // That is, if PostgreSQL will treat str as a string, not as code.
        public static bool IsSafe(string str)
        {
            return (str != null && str.Length <= MAX_STRING_SIZE && MyRegex().IsMatch(str));
        }

        public static async Task<IEnumerable<Product>> GetAllProducts(IDbConnection dbConnection)
        {
            string sql = "SELECT id, name, price, categoryId, subcategoryId, imagePath FROM products ORDER BY id";
            IEnumerable<Product> products = await dbConnection.QueryAsync<Product>(sql);
            return products;
        }

        public static async Task<IEnumerable<Product>> GetFirstProducts(IDbConnection dbConnection, uint quantity)
        {
            string sql = $"SELECT id, name, price, categoryId, subcategoryId, imagePath FROM products ORDER BY id LIMIT {quantity}";
            IEnumerable<Product> products = await dbConnection.QueryAsync<Product>(sql);
            return products;
        }

        public static async Task<IEnumerable<Product>> GetProductsFromIds(IDbConnection dbConnection, IEnumerable<uint> productIds)
        {
            string idsSequence = "";
            foreach (uint id in productIds)
            {
                idsSequence += id + ",";
            }
            IEnumerable<Product> products = [];
            if (idsSequence.Length > 0)
            {
                // eliminate the last comma
                idsSequence = idsSequence.Substring(0, idsSequence.Length - 1);
                string sql = $"SELECT id, name, price, categoryId, subcategoryId, imagePath FROM products WHERE id IN ({idsSequence}) ORDER BY id";
                products = await dbConnection.QueryAsync<Product>(sql);
            }
            return products;
        }

        public static async Task<(IEnumerable<Product>,bool)> SearchForProduct(IDbConnection dbConnection, string query)
        {
            if (query != null && query.Length > 0 && IsSafe(query))
            {
                // Do a simple search, just for testing.
                // A query like "abc def" will be replaced by the pattern "%abc% %def%".
                string[] substrings = query.Split(' ');
                string pattern = "";
                foreach (string substring in substrings)
                {
                    pattern += "%" + substring + "% ";
                }
                // Remove last space character
                pattern = pattern[..^1];

                string sql = $"SELECT * FROM products WHERE name ILIKE '{pattern}'";
                IEnumerable<Product> products = await dbConnection.QueryAsync<Product>(sql);
                return (products, true);
            }
            else
            {
                return ([], false);
            }
        }

        [Authorize]
        public static async Task<(IEnumerable<CartEntry>,bool)> GetUserCart(IDbConnection dbConnection, string userId)
        {
            IEnumerable<CartEntry> cart = [];
            if (userId != null && IsSafe(userId))
            {
                string sql = $"SELECT quantity AS \"Quantity\",productId AS \"ProductId\" FROM cartentries WHERE userId='{userId}' ORDER BY productId";
                cart = await dbConnection.QueryAsync<CartEntry>(sql);
            }
            return (cart, true);
        }

        [Authorize]
        public static async Task<(IEnumerable<Tuple<uint,Product>>, bool)> GetUserDetailedCart(IDbConnection dbConnection, string userId)
        {
            // Get cart, which only has quantities and product ids
            bool succededGettingCart;
            IEnumerable<CartEntry> cart;
            (cart, succededGettingCart) = await DataManipulation.GetUserCart(dbConnection, userId);
            if (succededGettingCart)
            {
                // Get the products
                List<uint> productIds = [];
                List<uint> quantities = [];
                uint numberOfProducts = 0;
                foreach (CartEntry cartEntry in cart)
                {
                    productIds.Add(cartEntry.ProductId);
                    quantities.Add(cartEntry.Quantity);
                    numberOfProducts++;
                }
                IEnumerable<Product> cartProducts = await DataManipulation.GetProductsFromIds(dbConnection, productIds);

                // Make the detaileCart, which stores all information of a product, not only its id
                List<Tuple<uint, Product>> detailedCart = [];
                int i = 0;
                foreach(Product product in cartProducts)
                {
                    detailedCart.Add(new Tuple<uint,Product>(quantities[i],product));
                    i++;
                }

                return (detailedCart, true);
            }

            return ([], false);
        }

        [Authorize]
        public static async Task<bool> AddToUserCart(IDbConnection dbConnection, string userId, CartEntry cartEntry)
        {

            if (userId != null && IsSafe(userId))
            {
                // If the user don't have the product in the cart,
                // add a new row to the cartentries table with que new product and quantity.
                // Else just add the new quantity to the old quantity.
                string sql = $"INSERT INTO cartentries (userId,productId,quantity) VALUES('{userId}',{cartEntry.ProductId},{cartEntry.Quantity}) ON CONFLICT (userId,productId) DO UPDATE SET quantity=cartentries.quantity+EXCLUDED.quantity";
                await dbConnection.ExecuteAsync(sql);
                return true;
            }
            else
            {
                return false;
            }
        }

        [Authorize]
        public static async Task<bool> RemoveFromUserCart(IDbConnection dbConnection, string userId, uint productId)
        {

            if (userId != null && IsSafe(userId))
            {
                // Delete the row from the cartentries table with the specified product for the user.
                string sql = $"DELETE FROM cartentries WHERE userId='{userId}' AND productId={productId}";
                await dbConnection.ExecuteAsync(sql);
                return true;
            }
            else
            {
                return false;
            }
        }

        [Authorize]
        public static async Task<bool> RemoveUserCart(IDbConnection dbConnection, string userId)
        {

            if (userId != null && IsSafe(userId))
            {
                // Remove every row of the cartentries table for the specified user.
                string sql = $"DELETE FROM cartentries WHERE userId='{userId}'";
                await dbConnection.ExecuteAsync(sql);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}



            