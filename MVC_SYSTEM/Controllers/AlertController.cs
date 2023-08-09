using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Controllers
{
    public class AlertController : Controller
    {
        // GET: Users
        public ActionResult Denied()
        {
            //ViewBag.Message = "You cannot access this page.";
            ViewBag.message = GlobalResEstate.msgCannotAkses;
            return View();
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult AlreadySignIn()
        {
            GetIdentity name = new GetIdentity();
            ViewBag.Message = "System has been login by " + name.MyName(User.Identity.Name) + ". Please logout this user to login another user.";
            return View();
        }

        public ActionResult error404()
        {
            //ViewBag.Message = "Sorry page is not exist. TQ";
            ViewBag.message = GlobalResEstate.msgNotExits;
            return View();
        }

        public ActionResult error400()
        {
            //ViewBag.Message = "Sorry you cannot go this page. TQ";
            ViewBag.message = GlobalResEstate.msgCannotGo;
            return View();
        }
    }
}