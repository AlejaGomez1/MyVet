using Microsoft.AspNetCore.Mvc;
using MyVet.Web.Helpers;
using MyVet.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyVet.Web.Controllers
{
    public class AccountController : Controller /*(Con este controlador vamos a loguearnos y desloguearnos)*/
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper) /*(el userhelper se inyecta con un constructor)*/
        {
            _userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if(result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "User or password not valid.");
            }

            return View(model);
        }
    }
}
