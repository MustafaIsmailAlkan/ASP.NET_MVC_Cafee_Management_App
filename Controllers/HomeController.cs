using Microsoft.AspNetCore.Mvc;

namespace Cafee_Prototype.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Main()
        {
            return View();
        }
    }
}