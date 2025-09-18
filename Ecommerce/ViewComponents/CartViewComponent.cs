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

        private async Task<IEnumerable<Tuple<uint, Product>>> GetDetailedCart(ClaimsPrincipal user)
        {
            IEnumerable<Tuple<uint, Product>> detailedCart = [];
            if (_signInManager.IsSignedIn(user))
            {
                string userId = UserInfo.GetUserId(user);
                bool succededGettingCart;
                (detailedCart, succededGettingCart) = await DataManipulation.GetUserDetailedCart(_dbConnection, userId);
            }
            return detailedCart;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool createCheckoutButton, ClaimsPrincipal user)
        {
            IEnumerable<Tuple<uint, Product>> detailedCart = await GetDetailedCart(user);
            Tuple<bool, IEnumerable<Tuple<uint, Product>>> model = new(createCheckoutButton, detailedCart);
            return View(model);
        }
    }
}
