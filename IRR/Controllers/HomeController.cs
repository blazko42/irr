using System.Web.Mvc;

namespace IRR.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult LandingPage()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Login()
		{
			return View(new Models.User());
		}
		[HttpPost]
		public ActionResult Login(Models.User user)
		{
			if (user.Login())
				ViewBag.LoginError = user.LoginError;

			return View("LandingPage");
		}

		[HttpGet]
		public ActionResult CreateAccount()
		{
			return View(new Models.User());
		}
		[HttpPost]
		public ActionResult CreateAccount(Models.User user)
		{
			if (user.CreateAccount())

				ViewBag.LoginError = user.LoginError;

			return View("AccountCreated");
		}

		[HttpGet]
		public ActionResult Logout()
		{
			Models.User.Logout();
			Response.Redirect("/Home/LandingPage");
			return View();
		}
	}
}