using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        GetIdentity getidentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DashPermitExpiry()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            List<sp_DashPermitExpired_Result> dashpermitresult = new List<sp_DashPermitExpired_Result>();

            dashpermitresult = dbsp.sp_DashPermitExpired(SyarikatID, LadangID).ToList();

            return Json(dashpermitresult);
        }
        public ActionResult DashStatusAkaun()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            int month = DateTime.Now.Month - 1;
            int year = DateTime.Now.Year;

            List<sp_DashStatusAkaun_Result> dashStatusAkaun = new List<sp_DashStatusAkaun_Result>();

            dashStatusAkaun = dbsp.sp_DashStatusAkaun(SyarikatID, year, month, WilayahID, LadangID).ToList();

            return Json(dashStatusAkaun);
        }
        public ActionResult DatatablePermit(string permit)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID, type = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            if (permit == "3 Bulan")
                type = 1;
            else if (permit == "2 Bulan")
                type = 2;
            else if (permit == "1 Bulan")
                type = 3;
            else if (permit == "Semasa")
                type = 4;
            else
                type = 5;

            List<sp_DatatablePermitExpired_Result> datatablepermit = new List<sp_DatatablePermitExpired_Result>();

            datatablepermit = dbsp.sp_DatatablePermitExpired(SyarikatID, WilayahID, type, LadangID).ToList();

            return Json(datatablepermit);
        }
        public ActionResult ChartKerakyatan()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            List<sp_DashAllKerakyatan_Result> dashkerakyatan = new List<sp_DashAllKerakyatan_Result>();

            //aini add control jumlah != 0 20062023
            dashkerakyatan = dbsp.sp_DashAllKerakyatan(SyarikatID, LadangID).Where(x => x.fld_Jumlah != 0).ToList();

            return Json(dashkerakyatan);
        }

        public ActionResult DatatableKerakyatan(string krytn)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            List<sp_DatatableKerakyatan_Result> kerakyatan = new List<sp_DatatableKerakyatan_Result>();

            kerakyatan = dbsp.sp_DatatableKerakyatan(SyarikatID, LadangID, krytn).ToList();

            return Json(kerakyatan);
        }

        //aini add transactiong listing 05072023
        public ActionResult DashTrans()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            int year = DateTime.Now.Year;

            List<sp_DashTransactionListing_Result> dashTrans = new List<sp_DashTransactionListing_Result>();

            dashTrans = dbsp.sp_DashTransactionListing(SyarikatID, LadangID, year).OrderBy(o => o.fld_Month2).ToList();

            return Json(dashTrans);
        }

    }
}