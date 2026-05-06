using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Doctor"))
                    return RedirectToAction("Doctor", "Dashboard");
                if (User.IsInRole("Patient"))
                    return RedirectToAction("MyProfile", "Patient");
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public IActionResult Error404() => View();
        public IActionResult Error500() => View();
    }
}
