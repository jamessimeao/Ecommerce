using Dapper;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using System.Data;

namespace Ecommerce.Data.Dapper
{
    public static class DataManipulation
    {
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

        public static async Task<IEnumerable<Product>> SearchForProduct(IDbConnection dbConnection, string query)
        {
            // The input query may lead to sql injection or other attacks!

            if (query != null && query.Length > 0)
            {
                // Do a simple search, just for testing
                // A query like "abc def" will be replaced by the pattern "%abc% %def%"
                string[] substrings = query.Split(' ');
                string pattern = "";
                foreach (string substring in substrings)
                {
                    pattern += "%" + substring + "% ";
                }
                // Remove last space character
                pattern = pattern[..^1];

                string sql = $"SELECT * FROM products WHERE name LIKE '{pattern}'";
                IEnumerable<Product> products = await dbConnection.QueryAsync<Product>(sql);
                return products;
            }
            else
            {
                return [];
            }
        }

        [Authorize]
        public static async Task<IEnumerable<CartEntry>> GetUserCart(IDbConnection dbConnection, string userId)
        {
            // Should be careful with userId, which is susceptible to sql injection.

            string sql = $"SELECT quantity AS \"Quantity\",productId AS \"ProductId\" FROM cartentries WHERE userId='{userId}' ORDER BY productId";
            IEnumerable<CartEntry> cart = await dbConnection.QueryAsync<CartEntry>(sql);
            return cart;
        }

        [Authorize]
        public static async Task AddToUserCart(IDbConnection dbConnection, string userId, CartEntry cartEntry)
        {
            // Should be careful with userId, which is susceptible to sql injection.

            // If the user don't have the product in the cart,
            // add a new row to the cartentries table with que new product and quantity.
            // Else just add the new quantity to the old quantity.
            string sql = $"INSERT INTO cartentries (userId,productId,quantity) VALUES('{userId}',{cartEntry.ProductId},{cartEntry.Quantity}) ON CONFLICT (userId,productId) DO UPDATE SET quantity=cartentries.quantity+EXCLUDED.quantity";
            await dbConnection.ExecuteAsync(sql);
        }

        [Authorize]
        public static async Task RemoveFromUserCart(IDbConnection dbConnection, string userId, uint productId)
        {
            // Should be careful with userId, which is susceptible to sql injection.

            // Delete the row from the cartentries table with the specified product for the user.
            string sql = $"DELETE FROM cartentries WHERE userId='{userId}' AND productId={productId}";
            await dbConnection.ExecuteAsync(sql);
        }

        [Authorize]
        public static async Task RemoveUserCart(IDbConnection dbConnection, string userId)
        {
            // Should be careful with userId, which is susceptible to sql injection.

            // Remove every row of the cartentries table for the specified user.
            string sql = $"DELETE FROM cartentries WHERE userId='{userId}'";
            await dbConnection.ExecuteAsync(sql);
        }
    }
}



            