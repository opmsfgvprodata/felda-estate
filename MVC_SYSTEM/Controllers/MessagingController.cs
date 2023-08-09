using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class MessagingController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetNSWL GetNSWL = new GetNSWL();
        private GetIdentity GetIdentity = new GetIdentity();
        private ChangeTimeZone GetTime = new ChangeTimeZone();
        // GET: Messaging
        // GET: Messaging/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_Messaging tbl_Messaging = await db.tbl_Messaging.FindAsync(id);
            if (tbl_Messaging == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Messaging);
        }

        public ActionResult _ShowBroadcast()
        {
            return View(db.tbl_Messaging.Where(x => x.fld_Active == true).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}