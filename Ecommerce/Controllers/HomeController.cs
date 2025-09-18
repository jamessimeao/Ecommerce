using Ecommerce.Data.Dapper;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDbConnection _dbConnection;

        public HomeController(
            ILogger<HomeController> logger,
            IDbConnection dbConnection
            )
        {
            _logger = logger;
            _dbConnection = dbConnection;
        }

        public async Task<IActionResult> Index([FromQuery] string query)
        {
            IEnumerable<Product> products;
            bool succededGettingProducts = true;
            if (query == null)
            {
                products = await DataManipulation.GetAllProducts(_dbConnection);
            }
            else
            {
                (products,succededGettingProducts) = await DataManipulation.SearchForProduct(_dbConnection,query);
            }

            if (succededGettingProducts)
            {
                return View(products);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}