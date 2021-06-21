using System.Web.Mvc;

namespace IRR.Controllers
{
    public class AdminController : Controller
    {
		public ActionResult Index()
		{
			if (Models.User.GetSessionUser() == null || Models.User.GetSessionUser().IsAdmin() == false)
				Response.Redirect("/Home/Login");

			return View();
		}

		public ActionResult Datasets()
		{
			if (Models.User.GetSessionUser() == null || Models.User.GetSessionUser().IsAdmin() == false)
				Response.Redirect("/Home/Login");

			return View();
		}

		public ActionResult SetActiveFeatureExtractor(int? feId)
		{
			if (Models.User.GetSessionUser() == null || Models.User.GetSessionUser().IsAdmin() == false)
				Response.Redirect("/Home/Login");

			Models.FeatureExtractor.SetActiveFeatureExtractor(feId);
			return Redirect("/Admin/Datasets");

		}


		#region Manage Landmarks

		[HttpGet]
		public ActionResult Landmarks()
		{
			if (Models.User.GetSessionUser() == null || Models.User.GetSessionUser().IsAdmin() == false)
				Response.Redirect("/Home/Login");

			return View(Models.Landmark.GetLandmarks());
		}

		[HttpGet]
		public ActionResult NewLandmark(string landmarkId)
		{
			if (Models.User.GetSessionUser() == null || Models.User.GetSessionUser().IsAdmin() == false)
				Response.Redirect("/Home/Login");

			if (landmarkId == null)
				return View(new Models.Landmark());

			return View(Models.Landmark.GetLandmarkEntry(landmarkId));
		}

		[HttpPost]
		public ActionResult NewLandmark(Models.Landmark landmark)
		{
			if (Models.User.GetSessionUser() == null || Models.User.GetSessionUser().IsAdmin() == false)
				Response.Redirect("/Home/Login");

			if (landmark.SaveLandmark())
				return Redirect("Landmarks");

			return Redirect("Landmarks");
		}

		[HttpGet]
		public ActionResult DeleteLandmark(int? ldId)
		{
			if (Models.User.GetSessionUser() == null || Models.User.GetSessionUser().IsAdmin() == false)
				Response.Redirect("/Home/Login");

			Models.Landmark.DeleteLandmark(ldId);

			return Redirect(Request.UrlReferrer.PathAndQuery);
		}

		#endregion
	}
}