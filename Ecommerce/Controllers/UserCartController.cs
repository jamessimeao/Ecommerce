using Microsoft.AspNetCore.Mvc;
using System.Data;
using Ecommerce.Models;
using Ecommerce.Data.Dapper;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Services;

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
        public async Task<IActionResult> Add([FromBody] CartEntry cartEntry)
        {
            if(cartEntry != null)
            {
                string userId = UserInfo.GetUserId(User);
                await DataManipulation.AddToUserCart(_dbConnection, userId, cartEntry);
                return ViewComponent("Cart",true);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveSingleProduct(ulong id)
        {
            // In this case, id is the id of the product, which should be >= 1
            if (id >= 1)
            {
                string userId = UserInfo.GetUserId(User);
                await DataManipulation.RemoveFromUserCart(_dbConnection, userId, id);
                return ViewComponent("Cart", true);
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
            return ViewComponent("Cart", true);
        }
    }
}
