using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool createCheckoutButton)
        {
            return View(createCheckoutButton);
        }
    }
}
