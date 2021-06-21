using System.Web.Mvc;

namespace IRR.Controllers
{
    public class FindLandmarkController : Controller
    {
        // GET: FindLandmark
        public ActionResult FindLandmark()
        {
			if (Models.User.GetSessionUser() == null)
				Response.Redirect("/Home/Login");

			return View();
        }
    }
}