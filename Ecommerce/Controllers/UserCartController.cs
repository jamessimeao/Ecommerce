using Microsoft.AspNetCore.Mvc;
using System.Data;
using Ecommerce.Data.Dapper;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Services;
using Ecommerce.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecommerce.Controllers
{
    [Authorize]
    [Route("[controller]/[action]/")]
    public class UserCartController : Controller
    {
        private readonly IDbConnection _dbConnection;

        public UserCartController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpPost("productId/{productId}/quantity/{quantity}/createCheckoutButton/{createCheckoutButton}")]
        public async Task<IActionResult> Add(
            [FromRoute] uint productId,
            [FromRoute] uint quantity,
            [FromRoute] bool createCheckoutButton)
        {
            if (productId >= 1 && quantity >= 1)
            {
                CartEntryDTO cartEntry = new CartEntryDTO { Quantity = quantity, ProductId = productId };
                string userId = UserInfo.GetUserId(User);
                bool succeded = await DataManipulation.AddToUserCart(_dbConnection, userId, cartEntry);
                if (succeded)
                {
                    return ViewComponent("Cart", new { createCheckoutButton = createCheckoutButton, user = User });
                }
            }

            return BadRequest();
        }

        [HttpDelete("productId/{productId}/createCheckoutButton/{createCheckoutButton}")]
        public async Task<IActionResult> RemoveSingleProduct([FromRoute] uint productId, [FromRoute] bool createCheckoutButton)
        {
            // In this case, id is the id of the product, which should be >= 1
            if (productId >= 1)
            {
                string userId = UserInfo.GetUserId(User);
                bool succeded = await DataManipulation.RemoveFromUserCart(_dbConnection, userId, productId);
                if (succeded)
                {
                    return ViewComponent("Cart", new { createCheckoutButton = createCheckoutButton, user = User });
                }
            }
                
            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveAllProducts()
        {
            string userId = UserInfo.GetUserId(User);
            bool succeded  = await DataManipulation.RemoveUserCart(_dbConnection, userId);
            if (succeded)
            { 
                return ViewComponent("Cart", new { createCheckoutButton = false, user = User });
            }

            return BadRequest();
        }
    }
}
