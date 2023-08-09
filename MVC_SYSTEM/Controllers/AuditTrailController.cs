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
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class AuditTrailController : Controller
    {
        GetIdentity Getidentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //aini add timezone & getconfig 31052023
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        // GET: AuditTrail

        public ActionResult Index()
        {
            ViewBag.AuditTrail = "class = active";

            //aini add filter by month and year 31052023
            int drpyear = 0;
            int drprangeyear = 0;
            int month = DateTime.Now.Month - 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = Getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.MonthList = monthList;

            return View();
        }

        public ActionResult UserAuditTrail()
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int? getuserid = Getidentity.ID(User.Identity.Name);
            var getdata = db.tblUserAuditTrails.Where(x => x.fld_CreatedBy == getuserid && x.fld_CreatedDT.Value.Year == currentYear && x.fld_CreatedDT.Value.Month == currentMonth).Join(db.tblUsers,
                c => c.fld_CreatedBy,
                cm => cm.fldUserID,
                (c, cm) => new
                {
                    username = cm.fldUserFullName,
                    date = c.fld_CreatedDT.ToString(),
                    createdby = c.fld_CreatedBy,
                    activity = c.fld_UserActivity
                }).OrderByDescending(o => o.date).Distinct().ToList();
            return Json(getdata);
        }

        //aini add datatable audit trail 31052023
        public ActionResult DatatableAuditTrail(int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = Getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            if(month == 0 && year == 0)
            {
                int cmonth = DateTime.Now.Month - 1;
                int cyear = DateTime.Now.Year;

                List<sp_DashStatusAkaun_Result> dashStatusAkaun = new List<sp_DashStatusAkaun_Result>();

                dashStatusAkaun = dbsp.sp_DashStatusAkaun(SyarikatID, cyear, cmonth, WilayahID, LadangID).ToList();

                return Json(dashStatusAkaun);
            }
            else
            {
                List<sp_DashStatusAkaun_Result> dashStatusAkaun = new List<sp_DashStatusAkaun_Result>();

                dashStatusAkaun = dbsp.sp_DashStatusAkaun(SyarikatID, year, month, WilayahID, LadangID).ToList();

                return Json(dashStatusAkaun);
            }
        }
    }
}