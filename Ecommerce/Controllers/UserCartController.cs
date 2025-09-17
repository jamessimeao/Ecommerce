using Microsoft.AspNetCore.Mvc;
using System.Data;
using Ecommerce.Models;
using Ecommerce.Data.Dapper;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Services;
using Humanizer;
using Ecommerce.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecommerce.Controllers
{
    public class UserCartController : Controller
    {
        private readonly IDbConnection _dbConnection;

        public UserCartController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CartEntryRequest dto)
        {
            if (dto.Quantity >= 1 && dto.ProductId >= 1)
            {
                CartEntry cartEntry = new CartEntry { Quantity = dto.Quantity, ProductId = dto.ProductId };
                string userId = UserInfo.GetUserId(User);
                await DataManipulation.AddToUserCart(_dbConnection, userId, cartEntry);
                return ViewComponent("Cart", dto.CreateCheckoutButton);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveSingleProduct([FromBody] CartEntryRequest dto)
        {
            // In this case, id is the id of the product, which should be >= 1
            if (dto.ProductId >= 1)
            {
                string userId = UserInfo.GetUserId(User);
                await DataManipulation.RemoveFromUserCart(_dbConnection, userId, dto.ProductId);
                return ViewComponent("Cart", dto.CreateCheckoutButton);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveAllProducts()
        {
            string userId = UserInfo.GetUserId(User);
            await DataManipulation.RemoveUserCart(_dbConnection, userId);
            return ViewComponent("Cart", false);
        }
    }
}
