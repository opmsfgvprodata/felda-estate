using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MVC_SYSTEM.Controllers
{
    public class SapXmlController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //private MVC_SYSTEM_HQ_Models db = new MVC_SYSTEM_HQ_Models();
        //MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        errorlog geterror = new errorlog();
        GetConfig GetConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        GetWilayah GetWilayah = new GetWilayah();
        Connection Connection = new Connection();
        //GetLadang GetLadang = new GetLadang();
        GlobalFunction globalFunction = new GlobalFunction();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        // GET: SapXml
        public ActionResult Index(string filter)
        {

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);

            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
            LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;

            ViewBag.YearList = yearlist;
            ViewBag.ClosingTransaction = "class = active";

            return View();
        }

        public JsonResult GetLadang(int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (GetWilayah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
                else
                {
                    ladanglist = new SelectList(db.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                }
            }

            return Json(ladanglist);
        }
        public ActionResult _Index(int? MonthList, int? YearList, int? LadangIDList, int? WilayahIDList , string userName)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //added by kamalia 24/11/21
            MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var message = "";

            //var postingData = new List<vw_SAPPostData>();
            var listData = new List<vw_SAPPostData>();

            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()) 
                && !String.IsNullOrEmpty(LadangIDList.ToString()) && !String.IsNullOrEmpty(WilayahIDList.ToString()) && !String.IsNullOrEmpty(userName))
            {

                //postingData = dbr.vw_SAPPostData
                //    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                //                x.fld_NegaraID == NegaraID &&
                //                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                //                x.fld_LadangID == LadangID).ToList();
               listData = dbr.vw_SAPPostData
                     .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).ToList();
                if (!listData.Any())
                {
                    message = GlobalResEstate.msgErrorSearch;
                }
            }

            else
            {
                message = "Sila masukkan ID SAP";
               // message = GlobalResEstate.msgChooseMonthYear;
            }

            ViewBag.Message = message;
            //added by kamalia 24/11/21
           

            ViewBag.Year = YearList;
            ViewBag.Month = MonthList;
            ViewBag.userName = userName;
            return View("_Index", listData);

        }

        public ActionResult CredentialLogin(string postGLToGL, string postGLToVendor, string postGLToCustomer)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 13;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            CustMod_SAPCredential sapCredential = new CustMod_SAPCredential();

            sapCredential.GLtoGLGuid = postGLToGL;
            sapCredential.GLtoGVendorGuid = postGLToVendor;
            sapCredential.GLtoGCustomerGuid = postGLToCustomer;

            return PartialView("CredentialLogin", sapCredential);
        }

       
        public ActionResult XMLData(string userName, Guid? postGLToGL, Guid? postGLToVendor, Guid? postGLToCustomer)
        {

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            

            var PostingData = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToGL || x.fld_SAPPostRefID == postGLToVendor || x.fld_SAPPostRefID == postGLToCustomer).OrderBy(o => o.fld_ItemNo).OrderBy(r => r.fld_SAPPostRefID.ToString()).Distinct();

            var date = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToGL || x.fld_SAPPostRefID == postGLToVendor || x.fld_SAPPostRefID == postGLToCustomer).OrderBy(o => o.fld_ItemNo).OrderBy(r => r.fld_SAPPostRefID.ToString()).Distinct().FirstOrDefault();

            var CountA2 = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToGL && x.fld_DocType == "A2").OrderBy(o => o.fld_ItemNo).OrderBy(r => r.fld_SAPPostRefID.ToString()).Distinct().Count();

            var CountGLKR = dbr.vw_SAPPostData.Where(x =>  x.fld_SAPPostRefID == postGLToVendor && x.fld_DocType == "KR" && x.fld_GL != null).OrderBy(o => o.fld_ItemNo).OrderBy(r => r.fld_SAPPostRefID.ToString()).Distinct().Count();

            var CountVNKR = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToVendor && x.fld_DocType == "KR" && x.fld_VendorCode != null).OrderBy(o => o.fld_ItemNo).OrderBy(r => r.fld_SAPPostRefID.ToString()).Distinct().Count();


            ViewBag.CountDataA2 = CountA2;
            ViewBag.CountDataGLKR = CountGLKR;
            ViewBag.CountDataVNKR = CountVNKR;
            ViewBag.Docdate = date.fld_DocDate.ToString("yyyyMMdd");
            ViewBag.PstgDate = date.fld_PostingDate.ToString("yyyyMMdd");
            ViewBag.BlineDate = date.fld_DocDate.ToString("yyyy-MM-dd");
            ViewBag.Username = userName;

            return View("XMLData", PostingData);
        }

    }
}