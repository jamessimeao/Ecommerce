using Ecommerce.Data.Dapper;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace Ecommerce.ViewComponents
{
    [Authorize]
    public class CartViewComponent : ViewComponent
    {
        private readonly IDbConnection _dbConnection;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CartViewComponent(
            IDbConnection dbConnection,
            SignInManager<IdentityUser> signInManager
            )
        {
            _dbConnection = dbConnection;
            _signInManager = signInManager;
        }

        private async Task<IEnumerable<CartEntry>> GetCart(ClaimsPrincipal user)
        {
            IEnumerable<CartEntry> cart = [];
            if (_signInManager.IsSignedIn(user))
            {
                string userId = UserInfo.GetUserId(user);
                bool succededGettingCart;
                (cart, succededGettingCart) = await DataManipulation.GetUserCart(_dbConnection, userId);
            }
            return cart;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool createCheckoutButton, ClaimsPrincipal user)
        {
            IEnumerable<CartEntry> cart = await GetCart(user);
            Tuple<bool, IEnumerable<CartEntry>> model = new(createCheckoutButton, cart);
            return View(model);
        }
    }
}
