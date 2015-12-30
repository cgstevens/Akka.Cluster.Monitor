using System.Web.Mvc;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        
        public ActionResult Main()
        {
            ViewBag.Title = "Main Page";

            return View();
        }

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult ReportApi()
        {
            return View();
        }

        public ActionResult ServiceStatus()
        {
            return View();
        }
    }
    
}
