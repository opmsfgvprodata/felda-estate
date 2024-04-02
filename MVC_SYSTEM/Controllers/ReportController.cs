using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MVC_SYSTEM.CustomModels;
using Org.BouncyCastle.Utilities.Collections;
//using Rotativa;
using System.Web.Security;
using System.Web.Script.Serialization;
using MVC_SYSTEM.log;
using Rotativa;
using MVC_SYSTEM.Attributes;
//farahin tambah - 11/1/2021
using MVC_SYSTEM.AuthModels;
using iTextSharp.tool.xml;
using System.Data.SqlClient;
using System.Configuration;
using MVC_SYSTEM.ModelsDapper;
using Dapper;
using Itenso.TimePeriod;
using tbl_Kerjahdr = MVC_SYSTEM.Models.tbl_Kerjahdr;
using tbl_Pkjmast = MVC_SYSTEM.Models.tbl_Pkjmast;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ReportController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //farahin tambah - 11/1/2021
        private MVC_SYSTEM_Models dbModels = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        //end here
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        // GET: Report
        public ActionResult Index()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            ViewBag.ReportList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "report" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fld_Val", "fld_Desc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string ReportList)
        {
            return RedirectToAction(ReportList, "Report");
        }

        public ActionResult WorkerReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            //SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID && x.fld_WilayahID==WilayahID && x.fld_LadangID==LadangID && x.fld_deleted==false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.StatusList = StatusList;
            ViewBag.SelectionList = SelectionList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult WorkerReport(int RadioGroup, string StatusList, string SelectionList, string print)
        //{
        //    ViewBag.Report = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> StatusList2 = new List<SelectListItem>();
        //    List<SelectListItem> SelectionList2 = new List<SelectListItem>();
        //    StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", StatusList).ToList();
        //    StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

        //    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.StatusList = StatusList2;
        //    ViewBag.Print = print;

        //    if (RadioGroup == 0)
        //    {
        //        //Individu Semua
        //        if (StatusList == "0")
        //        {
        //            SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
        //            SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
        //            if (SelectionList == "0")
        //            {
        //                //individu semua pekerja
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //            else
        //            {
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //        }
        //        else
        //        {
        //            SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
        //            SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
        //            if (SelectionList == "0")
        //            {
        //                //individu aktif/xaktif pekerja
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //            else
        //            {
        //                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList);
        //                ViewBag.SelectionList = SelectionList2;
        //                ViewBag.getflag = 2;
        //                return View(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Group
        //        SelectionList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
        //        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
        //        if (SelectionList == "0")
        //        {
        //            //semua kump
        //            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID != null); ViewBag.SelectionList = SelectionList2;
        //            ViewBag.getflag = 2;
        //            return View(result);
        //        }
        //        else
        //        {
        //            //by kump
        //            int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
        //            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == getkump);
        //            ViewBag.SelectionList = SelectionList2;
        //            ViewBag.getflag = 2;
        //            return View(result);
        //        }
        //    }
        //}

        public ActionResult GroupReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.bilangan_ahli >= 0).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.GroupList = GroupList;
            //ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _GroupReport(string GroupList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //List<Models.tbl_Pkjmast> InfoKmpln = new List<Models.tbl_Pkjmast>();
            List<SelectListItem> GroupList2 = new List<SelectListItem>();
            GroupList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.bilangan_ahli >= 0).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            GroupList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.Print = print;

            if (GroupList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseGroup;
                return View();
            }

            if (GroupList == "0")
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID != null && x.fld_Kdaktf == "1").OrderBy(o => o.fld_KumpulanID);
                ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.GroupList = GroupList2;
                //ViewBag.getflag = 2;
                return View(result);
            }
            else
            {
                int groupID = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == GroupList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == groupID && x.fld_Kdaktf == "1");
                ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.GroupList = GroupList2;
                //ViewBag.getflag = 2;
                return View(result);
            }


        }

        public ActionResult AccountReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.StatusList = StatusList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _AccReport(string StatusList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<Models.tbl_Pkjmast> AccPekerja = new List<Models.tbl_Pkjmast>();

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.getflag = 2;
            ViewBag.Print = print;

            if (StatusList == null)
            {
                ViewBag.Message = GlobalResEstate.lblChooseAcc;
                return View(AccPekerja);
            }


            if (StatusList == "0")
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1");
                ViewBag.UserID = getuserid;
                return View(result);
            }
            else
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_StatusAkaun == StatusList);
                ViewBag.UserID = getuserid;
                return View(result);
            }
        }

        public ActionResult KwspSocsoReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.StatusList = StatusList;
            //ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _KwspSocsoReport(string StatusList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<Models.tbl_Pkjmast> KwspSocsoPekerja = new List<Models.tbl_Pkjmast>();

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            //ViewBag.getflag = 2;
            ViewBag.Print = print;

            if (StatusList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseAcc;
                return View(KwspSocsoPekerja);
            }

            else if (StatusList == "0")
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1");
                return View(result);
            }
            else
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_StatusKwspSocso == StatusList);
                return View(result);
            }
        }

        public ActionResult WorkReport()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            // fatin added - 08/03/2023
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetLastGenProcess = db.tbl_SevicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            int? yearcheck = 0;
            int? monthcheck = 0;
            //end

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
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

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);
            //string[] nopkj = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).ToArray();
            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                .OrderBy(o => o.fld_Nama)
                .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama })
                .Distinct(), "Value", "Text").ToList();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            //WorkerList = new SelectList(dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            // fatin added - 08/03/2023
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.SelectionList = SelectionList;

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.bilangan_ahli >= 0).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.YearList = yearlist;
            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", monthcheck);

            var KumpulanList = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).ToList();

            var KumpulanName = KumpulanList.Select(s => s.fld_KodKumpulan).Take(1).FirstOrDefault();

            var GetKerjaInfoDetailsList = dbsp.sp_KerjaInfoDetails(1, KumpulanName, yearcheck, monthcheck, NegaraID, SyarikatID, WilayahID, LadangID).ToList();

            var GetWorkerList = GetKerjaInfoDetailsList.Select(s => s.fld_Nopkj.Trim()).Distinct();

            //fatin modified - 18/09/2023
            var companycode = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID && x.fld_Deleted == false).Select(s => s.fld_CostCentre).FirstOrDefault();
            //end
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            PilihanAktvt = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_compcode == companycode).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc }), "Value", "Text").ToList(); // fatin modified - 20/06/2023
            PilihanAktvt.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.GetWorkerList = GetWorkerList;
            ViewBag.SelectionData = new SelectList(KumpulanList.Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text");
            ViewBag.SelectionDataLabel = "Nama Kumpulan";
            ViewBag.GroupList = GroupList;
            ViewBag.PilihanAktvt = PilihanAktvt;
            // end

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            ViewBag.WorkerList = WorkerList;
            ViewBag.getflag = 1;
            return View();
        }

        // fatin added - 08/03/2023 
        public ActionResult _DailyWorkReport(int? MonthList, int? YearList, string print, string SelectionData, int? SelectionCategory, DateTime? SelectedDate, string PilihanAktvt)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host = "", catalog = "", user = "", pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
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
                if (i == YearList)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", MonthList);

            ViewBag.SelectionData = SelectionCategory == 1 ?
            new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text", SelectionData).ToList()
            :
            new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();

            ViewBag.SelectionDataLabel = SelectionCategory == 1 ?
            "Nama Kumpulan"
            :
            "Nama Individu";

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            ViewBag.NegaraID = NegaraID;
            ViewBag.MonthSelection = MonthList;
            ViewBag.YearSelection = YearList;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.Wilayah = db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_Deleted == false).Select(s => s.fld_WlyhName).FirstOrDefault();
            ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).Select(s => s.fld_LdgName).FirstOrDefault();
            ViewBag.TarikhJanaGaji = db.tbl_SevicesProcess.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).OrderByDescending(o => o.fld_DTProcess).Select(s => s.fld_DTProcess).FirstOrDefault();

            if (MonthList == null && YearList == null && SelectionData == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View();
            }

            if (SelectedDate == null)
            {
                if (PilihanAktvt == "0")
                {
                    var GetKerjaInfoDetailsList = dbsp.sp_KerjaInfoDetails(SelectionCategory, SelectionData, YearList, MonthList, NegaraID, SyarikatID, WilayahID, LadangID).ToList();
                    var GetWorkerList = GetKerjaInfoDetailsList.Select(s => s.fld_Nopkj.Trim()).Distinct();
                    ViewBag.GetWorkerList = GetWorkerList;
                    if (GetKerjaInfoDetailsList.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View("_DailyWorkReport", GetKerjaInfoDetailsList);
                }
                else
                {
                    var GetKerjaInfoDetailsList = dbsp.sp_KerjaInfoDetails(SelectionCategory, SelectionData, YearList, MonthList, NegaraID, SyarikatID, WilayahID, LadangID).Where(x => x.fld_KodAktvt == PilihanAktvt).ToList();
                    var GetWorkerList = GetKerjaInfoDetailsList.Select(s => s.fld_Nopkj.Trim()).Distinct();
                    ViewBag.GetWorkerList = GetWorkerList;
                    if (GetKerjaInfoDetailsList.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View("_DailyWorkReport", GetKerjaInfoDetailsList);
                }
            }
            else
            {
                if (PilihanAktvt == "0")
                {
                    var GetKerjaInfoDetailsList = dbsp.sp_KerjaInfoDetails(SelectionCategory, SelectionData, YearList, MonthList, NegaraID, SyarikatID, WilayahID, LadangID).Where(x => x.fld_Tarikh == SelectedDate).ToList();
                    var GetWorkerList = GetKerjaInfoDetailsList.Select(s => s.fld_Nopkj.Trim()).Distinct();
                    ViewBag.GetWorkerList = GetWorkerList;
                    if (GetKerjaInfoDetailsList.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View("_DailyWorkReport", GetKerjaInfoDetailsList);
                }
                else
                {
                    var GetKerjaInfoDetailsList = dbsp.sp_KerjaInfoDetails(SelectionCategory, SelectionData, YearList, MonthList, NegaraID, SyarikatID, WilayahID, LadangID).Where(x => x.fld_KodAktvt == PilihanAktvt && x.fld_Tarikh == SelectedDate).ToList();
                    var GetWorkerList = GetKerjaInfoDetailsList.Select(s => s.fld_Nopkj.Trim()).Distinct();
                    ViewBag.GetWorkerList = GetWorkerList;
                    if (GetKerjaInfoDetailsList.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View("_DailyWorkReport", GetKerjaInfoDetailsList);
                }
            }

        }

        public JsonResult WorkerData(int SelectionCategory)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> SelectionData = new List<SelectListItem>();
            SelectionData = SelectionCategory == 1 ?
            new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_Keterangan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text").ToList()
            :
            new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();

            SelectionData.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            dbr.Dispose();
            return Json(SelectionData);
        }
        // end

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult _WorkReport(int? MonthList, int? YearList, string WorkerList, string print)
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
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

            var MonthList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            string[] nopkj = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).ToArray();

            List<SelectListItem> WorkerList2 = new List<SelectListItem>();
            WorkerList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj) && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            //WorkerList2 = new SelectList(dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj }).Distinct(), "Value", "Text").ToList();
            WorkerList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            //var result = dbr.tbl_Kerja.Where(x=>x.fld_Tarikh.Value.Month==MonthList && x.fld_Tarikh.Value.Year==YearList)

            //List<Models.tbl_Kerja> WorkList = new List<Models.tbl_Kerja>();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthList = MonthList2;
            ViewBag.YearList = yearlist;
            ViewBag.WorkerList = WorkerList2;
            ViewBag.MonthSelection = MonthList;
            ViewBag.YearSelection = YearList;
            ViewBag.getflag = 2;
            ViewBag.Print = print;

            if (MonthList == null && YearList == null && WorkerList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View();
            }

            if (WorkerList == "0")
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh);
                if (result.ToList().Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoInform;
                }
                return View(result);
            }
            else
            {
                var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_Nopkj == WorkerList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh);
                if (result.ToList().Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoInform;
                }
                return View(result);
            }

        }

        // yana added - 18/04/2023 
        public ActionResult RumusanReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            //SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID && x.fld_WilayahID==WilayahID && x.fld_LadangID==LadangID && x.fld_deleted==false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            //SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.StatusList = StatusList;
            ViewBag.SelectionList = SelectionList;
            ViewBag.getflag = 1;
            return View();
        }
        // end here
        // yana added - 18/04/2023
        public ViewResult _RumusanReport(int? RadioGroup, string StatusList, string SelectionList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            List<SelectListItem> SelectionList2 = new List<SelectListItem>();

            List<Models.tbl_Pkjmast> InfoPekerja = new List<Models.tbl_Pkjmast>();

            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", StatusList).ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            // yana added - 050523
            ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).Select(s => s.fld_LdgName).FirstOrDefault();
            // end here

            ViewBag.Print = print;

            if (StatusList == null && SelectionList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(InfoPekerja);
            }
            else
            {
                if (RadioGroup == 0)
                {
                    //Individu Semua
                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                        if (SelectionList == "0")
                        {
                            //individu semua pekerja
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                        else
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                    }
                    else
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                        if (SelectionList == "0")
                        {
                            //individu aktif/xaktif pekerja
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                        else
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                    }
                }
                else
                {
                    //Group
                    SelectionList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                    SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                    if (SelectionList == "0")
                    {
                        //semua kump
                        var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID != null); ViewBag.SelectionList = SelectionList2;
                        if (result.Count() == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }
                        ViewBag.getflag = 2;
                        return View(result);
                    }
                    else
                    {
                        //by kump
                        int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                        var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == getkump);
                        if (result.Count() == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }
                        ViewBag.SelectionList = SelectionList2;
                        ViewBag.getflag = 2;
                        return View(result);
                    }
                }
            }
        }
        // end here

        public ActionResult ExpiredNotiReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            ViewBag.Report = "class = active";
            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExpiredNotiReport(string MonthList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            if (MonthList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChoosePassportExpired;
                return View();
            }

            //var result = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID);/*, "fldOptConfValue", "fldOptConfDesc", MonthList);*/
            ViewBag.Report = "class = active";
            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "exprdmonthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", MonthList);
            ViewBag.SelectionMonth = MonthList;
            ViewBag.getflag = 2;
            //ViewBag.Print = print;

            return View();
        }

        public ActionResult ExpiredPermit(string MonthList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime todaydate = DateTime.Today;
            DateTime startdate = DateTime.Today.AddMonths(int.Parse(MonthList));
            if (MonthList == "-1")
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2prmt.Value.Month <= todaydate.Month && x.fld_T2prmt.Value.Year <= todaydate.Year);
                ViewBag.DataCount = result.Count();
                ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Print = print;
                return View(result);
            }
            else
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2prmt.Value.Month == startdate.Month && x.fld_T2prmt.Value.Year == startdate.Year);
                ViewBag.DataCount = result.Count();
                ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                ViewBag.Print = print;
                return View(result);
            }
        }

        public ActionResult ExpiredPassport(string MonthList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime todaydate = DateTime.Today;
            DateTime startdate = DateTime.Today.AddMonths(int.Parse(MonthList));
            if (MonthList == "-1")
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2pspt.Value.Month <= todaydate.Month && x.fld_T2pspt.Value.Year <= todaydate.Year);
                ViewBag.DataCount = result.Count();
                ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                //ViewBag.MonthSelection = MonthList;
                ViewBag.Print = print;
                return View(result);
            }
            else
            {
                var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_T2pspt.Value.Month == startdate.Month && x.fld_T2pspt.Value.Year == startdate.Year);
                ViewBag.DataCount = result.Count();
                ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                //ViewBag.MonthSelection = MonthList;
                ViewBag.Print = print;
                return View(result);
            }
        }

        public ActionResult HasilReport()
        {
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
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

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);
            var tblkerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
            string[] nopkj = tblkerja.Select(s => s.fld_Nopkj).ToArray();
            //string[] group = tblkerja.Select(s => s.fld_Kum).Distinct().ToArray();
            //string[] pkt = tblkerja.Select(s => s.fld_KodPkt).Distinct().ToArray();

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj) && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            //List<SelectListItem> GroupList = new List<SelectListItem>();
            //GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted==false && group.Contains(x.fld_KodKumpulan)).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }).Distinct(), "Value", "Text").ToList();
            //GroupList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            //List<SelectListItem> PktList = new List<SelectListItem>();
            //WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            //WorkerList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            ViewBag.SelectionList = WorkerList;
            ViewBag.getflag = 1;
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult HasilReport(int RadioGroup, int MonthList, int YearList, string SelectionList)
        //{
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int year = timezone.gettimezone().Year;
        //    int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = rangeyear; i <= year; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    var MonthList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);

        //    string[] nopkj = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).ToArray();

        //    List<SelectListItem> WorkerList2 = new List<SelectListItem>();
        //    WorkerList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
        //    WorkerList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.MonthList = MonthList2;
        //    ViewBag.YearList = yearlist;
        //    ViewBag.SelectionList = WorkerList2;
        //    ViewBag.MonthSelection = MonthList;
        //    ViewBag.YearSelection = YearList;
        //    ViewBag.getflag = 2;

        //    if (RadioGroup == 0)
        //    {
        //        //Individu
        //        if(SelectionList=="0")
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001").OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //        else
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001" && x.fld_Nopkj==SelectionList).OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }

        //    }
        //    else if (RadioGroup == 0)
        //    {
        //        //Kumpulan
        //        if(SelectionList=="0")
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001").OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //        else
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001" && x.fld_Kum == SelectionList).OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }

        //    }
        //    else
        //    {
        //        //Peringkat/subperingkat
        //        if(SelectionList=="0")
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001").OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //        else
        //        {
        //            var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_KodAktvt == "1001" && x.fld_KodPkt==SelectionList).OrderBy(o => o.fld_Tarikh);
        //            return View(result);
        //        }
        //    }
        //}

        //[HttpPost]
        public ActionResult HasilReportDetail(int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.MonthSelection = MonthList;
            ViewBag.YearSelection = YearList;
            ViewBag.Print = print;

            if (MonthList == null && YearList == null && SelectionList == null)
            {

                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View();

            }

            if (RadioGroup == 0)
            {
                //Individu
                if (SelectionList == "0")
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList &&
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          //x.fld_KodAktvt == "1001" &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList &&
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          //x.fld_KodAktvt == "1001" && 
                                                          x.fld_Nopkj == SelectionList &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View(result);
                }

            }
            else if (RadioGroup == 1)
            {
                //Kumpulan
                if (SelectionList == "0")
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList &&
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          //x.fld_KodAktvt == "1001" &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList &&
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          //x.fld_KodAktvt == "1001" && 
                                                          x.fld_Kum == SelectionList &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View(result);
                }

            }
            else
            {
                //Peringkat/subperingkat
                if (SelectionList == "0")
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList &&
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          //x.fld_KodAktvt == "1001" &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.tbl_Kerja.Where(x => x.fld_Tarikh.Value.Month == MonthList &&
                                                          x.fld_Tarikh.Value.Year == YearList &&
                                                          //x.fld_KodAktvt == "1001" && 
                                                          x.fld_KodPkt == SelectionList &&
                                                          x.fld_NegaraID == NegaraID &&
                                                          x.fld_SyarikatID == SyarikatID &&
                                                          x.fld_WilayahID == WilayahID &&
                                                          x.fld_LadangID == LadangID)
                                              .OrderBy(o => o.fld_Tarikh);
                    ViewBag.DataCount = result.Count();
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    return View(result);
                }
            }
        }

        public ActionResult HasilSearch()
        {
            ViewBag.Report = "class = active";
            return View();
        }

        public ActionResult BankAccReport()
        {
            ViewBag.Report = "class = active";
            //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //int? getuserid = GetIdentity.ID(User.Identity.Name);
            ////string host, catalog, user, pass = "";
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //var result = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID);

            //ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            //ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            //ViewBag.Print = print;
            return View();
        }

        public ActionResult _BankAccReport(string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var result = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Print = print;
            return View(result);
        }

        public ActionResult AIPSReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now.AddMonths(-1);

            var yearlist = new List<SelectListItem>();
            for (var i = startyear; i <= currentyear; i++)
            {
                if (i == selectdate.Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KumpulanID.ToString(), Text = s.fld_KodKumpulan }).Distinct(), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.YearList = yearlist;
            ViewBag.GroupList = GroupList;
            ViewBag.WorkerList = WorkerList;
            return View();
        }

        public ActionResult AIPSReportDetail(int? YearList, string GroupList, string WorkerList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.Print = print;

            if (GroupList == null && YearList == null && WorkerList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseAips;
                return View();
            }

            if (GroupList == "0")
            {
                var result = dbr.vw_RptAIPS.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);
                if (result.Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }
                ViewBag.DataCount = result.Count();
                return PartialView(result);
            }
            else
            {
                int groupID = int.Parse(GroupList);
                if (WorkerList == "0")
                {
                    var result = dbr.vw_RptAIPS.Where(x => x.fld_KumpulanID == groupID && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.DataCount = result.Count();
                    return PartialView(result);
                }
                else
                {
                    var result = dbr.vw_RptAIPS.Where(x => x.fld_KumpulanID == groupID && x.fld_Nopkj == WorkerList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);
                    if (result.Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }
                    ViewBag.DataCount = result.Count();
                    return PartialView(result);
                }
            }

        }

        public JsonResult GetList(int RadioGroup, string StatusList)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            string SelectionLabel = "";

            if (RadioGroup == 0)
            {
                if (String.IsNullOrEmpty(StatusList))
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";

                    SelectionList = new SelectList(
                        dbr.tbl_Pkjmast
                            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                        x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                            .OrderBy(o => o.fld_Nopkj)
                            .Select(
                                s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                        "Value", "Text").ToList();
                    SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                }

                else
                {
                    //Individu Semua
                    SelectionLabel = "Pekerja";
                    if (StatusList == "0")
                    {
                        SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf != null).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                    }
                    else
                    {
                        SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                    }
                }

            }
            else
            {
                //Group
                SelectionLabel = "Kumpulan";
                SelectionList = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            return Json(new { SelectionList = SelectionList, SelectionLabel = SelectionLabel });
        }

        public JsonResult GetList2(int RadioGroup)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SelectionList = new List<SelectListItem>();
            string SelectionLabel = "";

            var tblkerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
            string[] nopkj = tblkerja.Select(s => s.fld_Nopkj).ToArray();
            string[] group = tblkerja.Select(s => s.fld_Kum).Distinct().ToArray();
            string[] pkt = tblkerja.Select(s => s.fld_KodPkt).Distinct().ToArray();

            if (RadioGroup == 0)
            {
                //Individu Semua
                SelectionLabel = "Pekerja";
                SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && nopkj.Contains(x.fld_Nopkj)).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
                SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            else if (RadioGroup == 1)
            {
                //Group
                SelectionLabel = "Kumpulan";
                SelectionList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && group.Contains(x.fld_KodKumpulan)).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }).Distinct(), "Value", "Text").ToList();
                SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            else
            {
                //Pkt
                SelectionLabel = "Peringkat";
                SelectionList = new SelectList(dbr.tbl_Kerja
                                                  .Where(x => x.fld_NegaraID == NegaraID &&
                                                              x.fld_SyarikatID == SyarikatID &&
                                                              x.fld_WilayahID == WilayahID &&
                                                              x.fld_LadangID == LadangID)
                                                  .Select(s => new SelectListItem { Value = s.fld_KodPkt, Text = s.fld_KodPkt })
                                                  .Distinct(), "Value", "Text").ToList();
                SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            }
            return Json(new { SelectionList = SelectionList, SelectionLabel = SelectionLabel });
        }

        public JsonResult GetWorkerList(string groupid)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> WorkerList = new List<SelectListItem>();
            int idgroup = int.Parse(groupid);
            if (idgroup == 0)
            {
                WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
                //WorkerList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
            }
            else
            {
                WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_KumpulanID == idgroup).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
                //WorkerList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
            }
            return Json(WorkerList);
        }
        [HttpPost]
        public ActionResult ConvertPDF(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            MasterModels.tblHtmlReport tblHtmlReport = new MasterModels.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            db.tblHtmlReport.Add(tblHtmlReport);
            db.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        }

        //public ActionResult GetPDF(int id)
        //{
        //    int? NegaraID = 0;
        //    int? SyarikatID = 0;
        //    int? WilayahID = 0;
        //    int? LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string width = "", height = "";
        //    string imagepath = Server.MapPath("~/Asset/Images/");

        //    var gethtml = db.tblHtmlReport.Find(id);
        //    var getsize = db.tblReportLists.Where(x => x.fldReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //    if (getsize != null)
        //    {
        //        width = getsize.fldWidthReport.ToString();
        //        height = getsize.fldHeightReport.ToString();
        //    }
        //    else
        //    {
        //        var getsizesubreport = db.tblSubReportLists.Where(x => x.fldSubReportListAction == gethtml.fldReportName.ToString()).FirstOrDefault();
        //        width = getsizesubreport.fldSubWidthReport.ToString();
        //        height = getsizesubreport.fldSubHeightReport.ToString();
        //    }
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    var logosyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();

        //    //Export HTML String as PDF.
        //    //Image logo = Image.GetInstance(imagepath + logosyarikat);
        //    //Image alignment
        //    //logo.ScaleToFit(50f, 50f);
        //    //logo.Alignment = Image.TEXTWRAP | Image.ALIGN_CENTER;
        //    //StringReader sr = new StringReader(gethtml.fldHtlmCode);
        //    Document pdfDoc = new Document(new Rectangle(int.Parse("1190"), int.Parse("1684")), 50f, 50f, 50f, 50f);
        //    //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
        //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //    pdfDoc.Open();
        //    //pdfDoc.Add(logo);
        //    using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
        //    {
        //        using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
        //        {
        //            htmlWorker.Open();
        //            htmlWorker.Parse(sr);
        //        }
        //    }
        //    pdfDoc.Close();
        //    Response.ContentType = "application/pdf";
        //    Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
        //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //    Response.Write(pdfDoc);
        //    Response.End();

        //    db.Entry(gethtml).State = EntityState.Deleted;
        //    db.SaveChanges();
        //    return View();
        //}

        //public ActionResult testPhoto(HttpPostedFileBase file)
        //{
        //    if (file != null)
        //    {
        //        string pic = System.IO.Path.GetFileName(file.FileName);
        //        string path = System.IO.Path.Combine(
        //                               Server.MapPath("~/Asset/Images"), pic);
        //        // file is uploaded
        //        file.SaveAs(path);

        //        // save the image path path to the database or you can send image 
        //        // directly to database
        //        // in-case if you want to store byte[] ie. for DB
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            file.InputStream.CopyTo(ms);
        //            byte[] array = ms.GetBuffer();
        //            byte[] array2 = new byte[file.ContentLength];

        //            GetIdentity getidentity = new GetIdentity();
        //            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //            int? getuserid = getidentity.ID(User.Identity.Name);
        //            string host, catalog, user, pass = "";
        //            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //            var filesaving = dbr.tbl_Photo.Where(x => x.fld_Photo == array2).FirstOrDefault();
        //            if(filesaving==null)
        //            {
        //                filesaving.fld_Photo = array2;
        //                filesaving.fld_Nopkj = "1112345";
        //                dbr.tbl_Photo.Add(filesaving);
        //                dbr.SaveChanges();
        //            }
        //        }



        //    }
        //    // after successfully uploading redirect the user
        //    //return RedirectToAction("actionname", "controller name");
        //    return View();

        //}

        public ActionResult IncentiveReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc");

            ViewBag.YearList = yearlist;

            return View();
        }

        public ViewResult _WorkerIncentiveRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatInsentifPekerja> MaklumatInsentifPekerja = new List<vw_MaklumatInsentifPekerja>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.Print = print;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();


            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(MaklumatInsentifPekerja);
            }

            else
            {
                if (RadioGroup == 0)

                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            var pendapatan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            var potongan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            MaklumatInsentifPekerja.Add(
                                new vw_MaklumatInsentifPekerja
                                {
                                    Pkjmast = i,
                                    Pendapatan = pendapatan,
                                    Potongan = potongan
                                });
                        }

                        if (MaklumatInsentifPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatInsentifPekerja);

                    }

                    else
                    {
                        var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerDataSingle = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        else
                        {
                            workerDataSingle = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        if (workerDataSingle != null)
                        {
                            var pendapatan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == SelectionList && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            var potongan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == SelectionList && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            MaklumatInsentifPekerja.Add(
                                new vw_MaklumatInsentifPekerja
                                {
                                    Pkjmast = workerDataSingle,
                                    Pendapatan = pendapatan,
                                    Potongan = potongan
                                });
                        }
                    }

                    if (MaklumatInsentifPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatInsentifPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            var pendapatan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            var potongan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            MaklumatInsentifPekerja.Add(
                                new vw_MaklumatInsentifPekerja
                                {
                                    Pkjmast = i,
                                    Pendapatan = pendapatan,
                                    Potongan = potongan
                                });
                        }

                        if (MaklumatInsentifPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatInsentifPekerja);
                    }

                    else
                    {
                        var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            var pendapatan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("P") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            var potongan = dbview.vw_MaklumatInsentif
                                .Where(a => a.fld_Nopkj == (i.fld_Nopkj) && a.fld_Month == MonthList &&
                                            a.fld_Year == YearList && a.fld_KodInsentif.Contains("T") &&
                                            a.fld_NegaraID == NegaraID &&
                                            a.fld_SyarikatID == SyarikatID && a.fld_WilayahID == WilayahID &&
                                            a.fld_LadangID == LadangID && a.fld_Deleted == false)
                                .OrderBy(x => x.fld_KodInsentif)
                                .ToList();

                            MaklumatInsentifPekerja.Add(
                                new vw_MaklumatInsentifPekerja
                                {
                                    Pkjmast = i,
                                    Pendapatan = pendapatan,
                                    Potongan = potongan
                                });
                        }
                    }

                    if (MaklumatInsentifPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatInsentifPekerja);
                }
            }
        }

        public ActionResult _WorkerIncentiveRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ActionResult LeaveReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            return View();
        }

        public ViewResult _WorkerLeaveRptSearch(int? RadioGroup, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatCutiPekerja> MaklumatCutiPekerja = new List<vw_MaklumatCutiPekerja>();

            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Print = print;

            if (YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(MaklumatCutiPekerja);
            }

            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<Int32> CutiTahunByBulanList = new List<Int32>();
                            List<Int32> CutiAmByBulanList = new List<Int32>();
                            List<Int32> CutiSakitByBulanList = new List<Int32>();
                            List<Int32> CutiMingguanList = new List<Int32>();
                            List<Int32> PontengList = new List<Int32>();

                            var leaveAllocation = dbview.tbl_CutiPeruntukan
                                .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                            x.fld_Deleted == false);

                            var cutiTahunan = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C02")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiTahunan.HasValue)
                            {
                                cutiTahunan = 0;
                            }

                            var cutiAm = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C01")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiAm.HasValue)
                            {
                                cutiAm = 0;
                            }

                            var cutiSakit = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C03")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiSakit.HasValue)
                            {
                                cutiSakit = 0;
                            }

                            for (var month = 1; month <= 12; month++)
                            {
                                var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiTahunByBulanList.Add(cutiTahunByBulan);

                                var cutiAmByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiAmByBulanList.Add(cutiAmByBulan);

                                var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiSakitByBulanList.Add(cutiSakitByBulan);

                                var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiMingguanList.Add(cutiMingguanByBulan);

                                var pontengByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                PontengList.Add(pontengByBulan);

                            }

                            MaklumatCutiPekerja.Add(
                                new vw_MaklumatCutiPekerja
                                {
                                    Pkjmast = i,
                                    CutiTahunan = cutiTahunan,
                                    CutiAm = cutiAm,
                                    CutiSakit = cutiSakit,
                                    CutiTahunByBulan = CutiTahunByBulanList,
                                    CutiAmByBulan = CutiAmByBulanList,
                                    CutiSakitByBulan = CutiSakitByBulanList,
                                    CutiMingguanByBulan = CutiMingguanList,
                                    PontengByBulan = PontengList
                                });
                        }

                        if (MaklumatCutiPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatCutiPekerja);
                    }

                    else
                    {
                        var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerDataSingle = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        else
                        {
                            workerDataSingle = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        if (workerDataSingle != null)
                        {
                            List<Int32> CutiTahunByBulanList = new List<Int32>();
                            List<Int32> CutiAmByBulanList = new List<Int32>();
                            //List<Int32> HadirHariMingguList = new List<Int32>();
                            List<Int32> CutiSakitByBulanList = new List<Int32>();
                            List<Int32> CutiMingguanList = new List<Int32>();
                            List<Int32> PontengList = new List<Int32>();

                            var leaveAllocation = dbview.tbl_CutiPeruntukan
                                .Where(x => x.fld_NoPkj == SelectionList && x.fld_Tahun == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                            var cutiTahunan = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C02")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiTahunan.HasValue)
                            {
                                cutiTahunan = 0;
                            }

                            var cutiAm = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C01")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiAm.HasValue)
                            {
                                cutiAm = 0;
                            }

                            var cutiSakit = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C03")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiSakit.HasValue)
                            {
                                cutiSakit = 0;
                            }

                            for (var month = 1; month <= 12; month++)
                            {
                                var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiTahunByBulanList.Add(cutiTahunByBulan);

                                var cutiAmByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiAmByBulanList.Add(cutiAmByBulan);

                                var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiSakitByBulanList.Add(cutiSakitByBulan);

                                var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiMingguanList.Add(cutiMingguanByBulan);

                                var pontengByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                PontengList.Add(pontengByBulan);
                            }

                            MaklumatCutiPekerja.Add(
                                new vw_MaklumatCutiPekerja
                                {
                                    Pkjmast = workerDataSingle,
                                    CutiTahunan = cutiTahunan,
                                    CutiAm = cutiAm,
                                    CutiSakit = cutiSakit,
                                    CutiTahunByBulan = CutiTahunByBulanList,
                                    CutiAmByBulan = CutiAmByBulanList,
                                    CutiSakitByBulan = CutiSakitByBulanList,
                                    CutiMingguanByBulan = CutiMingguanList,
                                    PontengByBulan = PontengList
                                });
                        }
                    }

                    if (MaklumatCutiPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatCutiPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<Int32> CutiTahunByBulanList = new List<Int32>();
                            List<Int32> CutiAmByBulanList = new List<Int32>();
                            List<Int32> CutiSakitByBulanList = new List<Int32>();
                            List<Int32> CutiMingguanList = new List<Int32>();
                            List<Int32> PontengList = new List<Int32>();

                            var leaveAllocation = dbview.tbl_CutiPeruntukan
                                .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                            var cutiTahunan = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C02")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiTahunan.HasValue)
                            {
                                cutiTahunan = 0;
                            }

                            var cutiAm = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C01")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiAm.HasValue)
                            {
                                cutiAm = 0;
                            }

                            var cutiSakit = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C03")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiSakit.HasValue)
                            {
                                cutiSakit = 0;
                            }

                            for (var month = 1; month <= 12; month++)
                            {
                                var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiTahunByBulanList.Add(cutiTahunByBulan);

                                var cutiAmByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiAmByBulanList.Add(cutiAmByBulan);

                                var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiSakitByBulanList.Add(cutiSakitByBulan);

                                var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiMingguanList.Add(cutiMingguanByBulan);

                                var pontengByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                PontengList.Add(pontengByBulan);

                            }

                            MaklumatCutiPekerja.Add(
                                new vw_MaklumatCutiPekerja
                                {
                                    Pkjmast = i,
                                    CutiTahunan = cutiTahunan,
                                    CutiAm = cutiAm,
                                    CutiSakit = cutiSakit,
                                    CutiTahunByBulan = CutiTahunByBulanList,
                                    CutiAmByBulan = CutiAmByBulanList,
                                    CutiSakitByBulan = CutiSakitByBulanList,
                                    CutiMingguanByBulan = CutiMingguanList,
                                    PontengByBulan = PontengList
                                });
                        }

                        if (MaklumatCutiPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatCutiPekerja);
                    }

                    else
                    {
                        var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<Int32> CutiTahunByBulanList = new List<Int32>();
                            List<Int32> CutiAmByBulanList = new List<Int32>();
                            List<Int32> CutiSakitByBulanList = new List<Int32>();
                            List<Int32> CutiMingguanList = new List<Int32>();
                            List<Int32> PontengList = new List<Int32>();

                            var leaveAllocation = dbview.tbl_CutiPeruntukan
                                .Where(x => x.fld_NoPkj == i.fld_Nopkj && x.fld_Tahun == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                            var cutiTahunan = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C02")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiTahunan.HasValue)
                            {
                                cutiTahunan = 0;
                            }

                            var cutiAm = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C01")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiAm.HasValue)
                            {
                                cutiAm = 0;
                            }

                            var cutiSakit = leaveAllocation
                                .Where(x => x.fld_KodCuti == "C03")
                                .Select(s => s.fld_JumlahCuti)
                                .SingleOrDefault();

                            if (!cutiSakit.HasValue)
                            {
                                cutiSakit = 0;
                            }

                            for (var month = 1; month <= 12; month++)
                            {
                                var cutiTahunByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiTahunByBulanList.Add(cutiTahunByBulan);

                                var cutiAmByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiAmByBulanList.Add(cutiAmByBulan);

                                var cutiSakitByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiSakitByBulanList.Add(cutiSakitByBulan);

                                var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                CutiMingguanList.Add(cutiMingguanByBulan);

                                var pontengByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                PontengList.Add(pontengByBulan);

                            }

                            MaklumatCutiPekerja.Add(
                                new vw_MaklumatCutiPekerja
                                {
                                    Pkjmast = i,
                                    CutiTahunan = cutiTahunan,
                                    CutiAm = cutiAm,
                                    CutiSakit = cutiSakit,
                                    CutiTahunByBulan = CutiTahunByBulanList,
                                    CutiAmByBulan = CutiAmByBulanList,
                                    CutiSakitByBulan = CutiSakitByBulanList,
                                    CutiMingguanByBulan = CutiMingguanList,
                                    PontengByBulan = PontengList
                                });
                        }
                    }

                    if (MaklumatCutiPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatCutiPekerja);
                }
            }
        }

        public ActionResult _WorkerLeaveRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ActionResult _WorkerAnnualLeaveByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getAnnualLeave = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "C02" && x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month &&
                            x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(getAnnualLeave);
        }

        public ActionResult _WorkerPublicHolidayByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getPublicHoliday = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "C01" && x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month &&
                            x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(getPublicHoliday);
        }
        public ActionResult _WorkerWeeklyLeaveByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var cutiMingguanByBulan = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "C07" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(cutiMingguanByBulan);
        }

        public ActionResult _WorkerSkipByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var pontengByBulan = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "P01" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(pontengByBulan);
        }

        public ActionResult AttendanceReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            return View();
        }

        public ViewResult _WorkerAttendanceRptSearch(int? RadioGroup, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_MaklumatKehadiranPekerja> MaklumatKehadiranPekerja = new List<vw_MaklumatKehadiranPekerja>();

            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Print = print;

            if (YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(MaklumatKehadiranPekerja);
            }

            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<Int32> hadirHariBiasaList = new List<Int32>();
                            List<Int32> hadirHariMingguList = new List<Int32>();
                            List<Int32> hadirHariCutiUmumList = new List<Int32>();

                            for (var month = 1; month <= 12; month++)
                            {
                                var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariMingguList.Add(hadirHariMingguByBulan);

                                var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                            }

                            MaklumatKehadiranPekerja.Add(
                                new vw_MaklumatKehadiranPekerja
                                {
                                    Pkjmast = i,
                                    HadirHariBiasaByBulan = hadirHariBiasaList,
                                    HadirHariMingguByBulan = hadirHariMingguList,
                                    HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                });
                        }

                        if (MaklumatKehadiranPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatKehadiranPekerja);
                    }

                    else
                    {
                        var workerDataSingle = new ViewingModels.tbl_Pkjmast();

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerDataSingle = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        else
                        {
                            workerDataSingle = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        if (workerDataSingle != null)
                        {
                            List<Int32> hadirHariBiasaList = new List<Int32>();
                            List<Int32> hadirHariMingguList = new List<Int32>();
                            List<Int32> hadirHariCutiUmumList = new List<Int32>();

                            for (var month = 1; month <= 12; month++)
                            {
                                var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariMingguList.Add(hadirHariMingguByBulan);

                                var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == SelectionList &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                            }

                            MaklumatKehadiranPekerja.Add(
                                new vw_MaklumatKehadiranPekerja
                                {
                                    Pkjmast = workerDataSingle,
                                    HadirHariBiasaByBulan = hadirHariBiasaList,
                                    HadirHariMingguByBulan = hadirHariMingguList,
                                    HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                });
                        }

                    }

                    if (MaklumatKehadiranPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(MaklumatKehadiranPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<Int32> hadirHariBiasaList = new List<Int32>();
                            List<Int32> hadirHariMingguList = new List<Int32>();
                            List<Int32> hadirHariCutiUmumList = new List<Int32>();

                            for (var month = 1; month <= 12; month++)
                            {
                                var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariMingguList.Add(hadirHariMingguByBulan);

                                var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                            }

                            MaklumatKehadiranPekerja.Add(
                                new vw_MaklumatKehadiranPekerja
                                {
                                    Pkjmast = i,
                                    HadirHariBiasaByBulan = hadirHariBiasaList,
                                    HadirHariMingguByBulan = hadirHariMingguList,
                                    HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                });
                        }

                        if (MaklumatKehadiranPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatKehadiranPekerja);
                    }

                    else
                    {
                        var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                        IOrderedQueryable<ViewingModels.tbl_Pkjmast> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.tbl_Pkjmast
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<Int32> hadirHariBiasaList = new List<Int32>();
                            List<Int32> hadirHariMingguList = new List<Int32>();
                            List<Int32> hadirHariCutiUmumList = new List<Int32>();

                            for (var month = 1; month <= 12; month++)
                            {
                                var hadirHariBiasaByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariBiasaList.Add(hadirHariBiasaByBulan);

                                var hadirHariMingguByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariMingguList.Add(hadirHariMingguByBulan);

                                var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                                    .Count(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == i.fld_Nopkj &&
                                                x.fld_Tarikh.Value.Month == month &&
                                                x.fld_Tarikh.Value.Year == YearList &&
                                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                                hadirHariCutiUmumList.Add(hadirHariCutiUmumByBulan);
                            }

                            MaklumatKehadiranPekerja.Add(
                                new vw_MaklumatKehadiranPekerja
                                {
                                    Pkjmast = i,
                                    HadirHariBiasaByBulan = hadirHariBiasaList,
                                    HadirHariMingguByBulan = hadirHariMingguList,
                                    HadirHariCutiUmumByBulan = hadirHariCutiUmumList

                                });
                        }

                        if (MaklumatKehadiranPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(MaklumatKehadiranPekerja);
                    }
                }
            }
        }

        public ActionResult _WorkerRegularDayAttendanceByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var getRegularDayAttendance = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "H01" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month &&
                            x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(getRegularDayAttendance);
        }

        public ActionResult _WorkerWeekendAttendanceByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var getWeekendAttendance = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "H02" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(getWeekendAttendance);
        }

        public ActionResult _WorkerPublicHolidayAttendanceByMonth(string nopkj, int? month, int? year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var hadirHariCutiUmumByBulan = dbview.tbl_Kerjahdr
                .Where(x => x.fld_Kdhdct == "H03" && x.fld_Nopkj == nopkj &&
                            x.fld_Tarikh.Value.Month == month &&
                            x.fld_Tarikh.Value.Year == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(o => o.fld_Tarikh);

            return View(hadirHariCutiUmumByBulan);
        }

        public ActionResult _WorkerAttendanceRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ActionResult TransactionListingReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month); //fatin modified - 15/12/2023

            ViewBag.YearList = yearlist;
            ViewBag.UserID = getuserid;

            return View();
        }

        public ViewResult _TransactionListingRptSearch(int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();


            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.NamaPengurus = dbview2.tbl_Ladang
                .Where(x => x.fld_ID == LadangID)
                .Select(s => s.fld_Pengurus).Single();
            ViewBag.NamaPenyelia = dbview2.tblUsers
                .Where(x => x.fldUserID == getuserid)
                .Select(s => s.fldUserFullName).Single();
            ViewBag.IDPenyelia = getuserid;
            ViewBag.Print = print;

            //Added by Shazana 29/8/2023
            var jawatanPenyelia = db.tblUserIDApps.Where(x => x.fldUserid == User.Identity.Name).Select(x => x.fldJawatan).FirstOrDefault();
            string namajawatan = "";
            if (jawatanPenyelia != null)
            {
                namajawatan = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "position" && x.fldOptConfValue == jawatanPenyelia).Select(x => x.fldOptConfDesc).FirstOrDefault();
            }
            else
            { namajawatan = ""; }
            namajawatan = namajawatan == null ? "" : namajawatan;
            ViewBag.namajawatan = namajawatan;

            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseMonthYear;
                return View();
            }

            else
            {
                var TransactionListingList = dbview.vw_RptSctran
                    .Where(x => x.fld_KodAktvt != "3803" && x.fld_KodAktvt != "3800" && x.fld_KodAktvt != "3807" && x.fld_Month == MonthList &&
                                x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID)
                    .OrderBy(o => new { o.fld_Kategori, o.fld_Amt });

                if (!TransactionListingList.Any())
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                    return View();

                }

                ViewBag.UserID = getuserid;
                return View(TransactionListingList);
            }
        }

        public ActionResult PaySlipReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.MonthList = monthList;
            ViewBag.StatusList = statusList;

            return View();
        }

        public ActionResult _WorkerPaySlipRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public string PDFInvalidGen()
        {
            return GlobalResEstate.msgInvalidPDFConvert;
        }

        public bool CheckGenIdentity(int id, string genid, int? userid, string username, out string CookiesValue)
        {
            bool result = false;
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, userid, username);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            CookiesValue = "";

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                Guid genidC = Guid.Parse(genid);
                var CheckIdentity = dbr.tbl_PdfGen.Where(x => x.fld_ID == genidC && x.fld_UserID == id).FirstOrDefault();

                if (CheckIdentity == null)
                {
                    result = false;
                }
                else
                {
                    result = true;
                    CookiesValue = CheckIdentity.fld_CookiesVal;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public void getBackAuth(string getcookiesval)
        {
            string CookiesValue = "";
            try
            {
                CookiesValue = Request.Cookies[FormsAuthentication.FormsCookieName].Value;
                if (CookiesValue != getcookiesval)
                {
                    HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    cookie.Value = getcookiesval;
                    Response.Cookies.Add(cookie);
                    //geterror.testlog("Try if : " + User.Identity.Name, "Try if : " + CookiesValue, "Try if : " + getcookiesval);
                }
                //geterror.testlog("Try no if : " + User.Identity.Name, "Try no if : " + CookiesValue, "Try no if : " + getcookiesval);
            }
            catch
            {
                HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, getcookiesval);
                Response.SetCookie(authoCookies);
                //geterror.testlog("Catch : " + User.Identity.Name, "Catch : " + CookiesValue, "Catch : " + getcookiesval);
            }
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
        }

        public ViewResult _WorkerPaySlipRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //if (print == "Yes" && User.Identity.Name == "")
            //{
            //    geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "Print Mode : " + print);
            //}
            int? getuserid = GetIdentity.ID(User.Identity.Name);

            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);

            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_PaySlipPekerja> PaySlipPekerja = new List<vw_PaySlipPekerja>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(PaySlipPekerja);
            }

            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiPekerja> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == i.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }

                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = i,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }

                        if (PaySlipPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(PaySlipPekerja);
                    }

                    else
                    {
                        var workerDataSingle = new ViewingModels.vw_GajiPekerja();

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerDataSingle = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        else
                        {
                            workerDataSingle = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        if (workerDataSingle != null)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == SelectionList && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == workerDataSingle.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + " " + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }


                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == SelectionList && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = workerDataSingle,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }
                    }

                    if (PaySlipPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(PaySlipPekerja);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiPekerja> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == i.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }

                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = i,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }

                        if (PaySlipPekerja.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(PaySlipPekerja);
                    }

                    else
                    {
                        var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                        IOrderedQueryable<ViewingModels.vw_GajiPekerja> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiPekerja
                                .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                            x.fld_Year == YearList && x.fld_Month == MonthList &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var i in workerData)
                        {
                            List<ViewingModels.vw_MaklumatInsentif> workerIncentiveRecordList = new List<ViewingModels.vw_MaklumatInsentif>();

                            List<FootNoteCustomModel> footNoteCustomModelList = new List<FootNoteCustomModel>();

                            var workerMonthlySalary = dbview.tbl_GajiBulanan
                                .SingleOrDefault(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                                      x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                                      x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                      x.fld_LadangID == LadangID);

                            List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                            var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                .Where(x => x.fld_GajiID == i.fld_ID && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID);

                            foreach (var caruman in workerAdditionalContribution)
                            {
                                CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                            }

                            var workerIncentiveRecord = dbview.vw_MaklumatInsentif
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                            foreach (var insentifRecord in workerIncentiveRecord)
                            {
                                workerIncentiveRecordList.Add(insentifRecord);
                            }

                            List<KerjaPekerjaCustomModel> kerjaPekerjaCustomModelList = new List<KerjaPekerjaCustomModel>();

                            var workerWorkRecordGroupBy = dbview.vw_KerjaPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodAktvt, x.fld_KodPkt, x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_KodAktvt)
                                .ThenBy(t => t.Key.fld_KodPkt)
                                .ThenBy(t2 => t2.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_JumlahHasil = lg.Sum(w => w.fld_JumlahHasil),
                                        fld_Unit = lg.FirstOrDefault().fld_Unit,
                                        fld_KadarByr = lg.FirstOrDefault().fld_KadarByr,
                                        fld_Gandaan = lg.FirstOrDefault().fldOptConfFlag3,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Amount)
                                    });

                            foreach (var work in workerWorkRecordGroupBy)
                            {
                                KerjaPekerjaCustomModel kerjaPekerjaCustomModel = new KerjaPekerjaCustomModel();

                                kerjaPekerjaCustomModel.fld_ID = work.fld_ID;
                                kerjaPekerjaCustomModel.fld_Desc = work.fld_Desc;
                                kerjaPekerjaCustomModel.fld_KodPkt = work.fld_KodPkt;
                                kerjaPekerjaCustomModel.fld_JumlahHasil = work.fld_JumlahHasil;
                                kerjaPekerjaCustomModel.fld_Unit = work.fld_Unit;
                                kerjaPekerjaCustomModel.fld_KadarByr = work.fld_KadarByr;
                                kerjaPekerjaCustomModel.fld_Gandaan = work.fld_Gandaan;
                                kerjaPekerjaCustomModel.fld_TotalAmount = work.fld_TotalAmount;

                                kerjaPekerjaCustomModelList.Add(kerjaPekerjaCustomModel);
                            }

                            List<OTPekerjaCustomModel> otPekerjaCustomModelList = new List<OTPekerjaCustomModel>();

                            var workerOTRecordGroupBy = dbview.vw_OTPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => x.fld_Kdhdct)
                                .OrderBy(o => o.Key)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_JumlahJamOT = lg.Sum(w => w.fld_JamOT),
                                        fld_Desc = lg.FirstOrDefault().fldDesc,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_Gandaan = lg.FirstOrDefault().fldRate,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerOTRecordGroupBy)
                            {
                                OTPekerjaCustomModel otPekerjaCustomModel = new OTPekerjaCustomModel();

                                otPekerjaCustomModel.fld_ID = ot.fld_ID;
                                otPekerjaCustomModel.fld_Desc = GlobalResEstate.lblOvertime + ot.fld_Desc;
                                otPekerjaCustomModel.fld_JumlahJamOT = ot.fld_JumlahJamOT;
                                otPekerjaCustomModel.fld_Unit = GlobalResEstate.lblHour;
                                otPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                otPekerjaCustomModel.fld_Gandaan = ot.fld_Gandaan;
                                otPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                otPekerjaCustomModelList.Add(otPekerjaCustomModel);

                                FootNoteCustomModel otFootNoteCustomModel = new FootNoteCustomModel();

                                otFootNoteCustomModel.fld_Desc = GlobalResEstate.lblOvertime + " " + ot.fld_Desc;
                                otFootNoteCustomModel.fld_Bilangan = ot.fld_JumlahJamOT;

                                footNoteCustomModelList.Add(otFootNoteCustomModel);
                            }

                            List<BonusPekerjaCustomModel> bonusPekerjaCustomModelList = new List<BonusPekerjaCustomModel>();

                            var workerBonusRecordGroupBy = dbview.vw_BonusPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_KodPkt, x.fld_Bonus, x.fld_KodAktvt })
                                .OrderBy(o => o.Key.fld_KodPkt)
                                .ThenBy(t => t.Key.fld_Bonus)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fld_Desc,
                                        fld_KodPkt = lg.FirstOrDefault().fld_KodPkt,
                                        fld_BilanganHari = lg.Count(),
                                        fld_Bonus = lg.FirstOrDefault().fld_Bonus,
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerBonusRecordGroupBy)
                            {
                                BonusPekerjaCustomModel bonusPekerjaCustomModel = new BonusPekerjaCustomModel();

                                bonusPekerjaCustomModel.fld_ID = ot.fld_ID;
                                bonusPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                bonusPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                bonusPekerjaCustomModel.fld_KodPkt = ot.fld_KodPkt;
                                bonusPekerjaCustomModel.fld_Bonus = ot.fld_Bonus;
                                bonusPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                bonusPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                bonusPekerjaCustomModelList.Add(bonusPekerjaCustomModel);
                            }

                            List<CutiPekerjaCustomModel> cutiPekerjaCustomModelList = new List<CutiPekerjaCustomModel>();

                            var workerLeaveRecordGroupBy = dbview.vw_CutiPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_ID = lg.FirstOrDefault().fld_ID,
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_BilanganHari = lg.Count(),
                                        fld_KadarByr = lg.FirstOrDefault().fld_Kadar,
                                        fld_TotalAmount = lg.Sum(w => w.fld_Jumlah)
                                    });

                            foreach (var ot in workerLeaveRecordGroupBy)
                            {
                                CutiPekerjaCustomModel cutiPekerjaCustomModel = new CutiPekerjaCustomModel();

                                cutiPekerjaCustomModel.fld_ID = ot.fld_ID;
                                cutiPekerjaCustomModel.fld_Desc = ot.fld_Desc;
                                cutiPekerjaCustomModel.fld_BilanganHari = ot.fld_BilanganHari;
                                cutiPekerjaCustomModel.fld_KadarByr = ot.fld_KadarByr;
                                cutiPekerjaCustomModel.fld_TotalAmount = ot.fld_TotalAmount;

                                cutiPekerjaCustomModelList.Add(cutiPekerjaCustomModel);
                            }

                            var workerWorkingDay = dbview.vw_KehadiranPekerja
                                .Where(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList)
                                .GroupBy(x => new { x.fld_Kdhdct })
                                .OrderBy(o => o.Key.fld_Kdhdct)
                                .Select(lg =>
                                    new
                                    {
                                        fld_Desc = lg.FirstOrDefault().fldOptConfDesc,
                                        fld_Bilangan = lg.Count(),
                                    });

                            foreach (var workingDay in workerWorkingDay)
                            {
                                FootNoteCustomModel footNoteCustomModel = new FootNoteCustomModel();

                                footNoteCustomModel.fld_Desc = workingDay.fld_Desc;
                                footNoteCustomModel.fld_Bilangan = workingDay.fld_Bilangan;

                                footNoteCustomModelList.Add(footNoteCustomModel);
                            }

                            var workerRainDay = dbview.vw_KehadiranPekerja
                                .Count(x => x.fld_Nopkj == i.fld_Nopkj && x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList && x.fld_Hujan == 1);

                            if (workerRainDay != 0)
                            {
                                FootNoteCustomModel footNoteHariHujanCustomModel = new FootNoteCustomModel();

                                footNoteHariHujanCustomModel.fld_Desc = GlobalResEstate.lblTotalRainDay;
                                footNoteHariHujanCustomModel.fld_Bilangan = workerRainDay;

                                footNoteCustomModelList.Add(footNoteHariHujanCustomModel);
                            }

                            PaySlipPekerja.Add(
                                new vw_PaySlipPekerja()
                                {
                                    Pkjmast = i,
                                    GajiBulanan = workerMonthlySalary,
                                    InsentifPekerja = workerIncentiveRecordList,
                                    KerjaPekerja = kerjaPekerjaCustomModelList,
                                    OTPekerja = otPekerjaCustomModelList,
                                    BonusPekerja = bonusPekerjaCustomModelList,
                                    CutiPekerja = cutiPekerjaCustomModelList,
                                    FootNote = footNoteCustomModelList,
                                    CarumanTambahan = carumanTambahanCustomModelList
                                });
                        }
                    }

                    if (PaySlipPekerja.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(PaySlipPekerja);
                }
            }
        }

        public ActionResult PaySheetReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            var monthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc", month);

            //added by faeza 08.11.2021
            List<SelectListItem> PaymentModeList = new List<SelectListItem>();
            PaymentModeList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PaymentModeList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.MonthList = monthList;
            ViewBag.StatusList = statusList;
            ViewBag.PaymentModeList = PaymentModeList;//added by faeza 08.11.2021

            return View();
        }

        public ActionResult _WorkerPaySheetRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ViewResult _WorkerPaySheetRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string PaymentModeList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<vw_PaySheetPekerjaCustomModel>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Ladang = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.NamaPengurus = dbview2.tbl_Ladang
                .Where(x => x.fld_ID == LadangID)
                .Select(s => s.fld_Pengurus).Single();
            ViewBag.NamaPenyelia = dbview2.tblUsers
                .Where(x => x.fldUserID == getuserid)
                .Select(s => s.fldUserFullName).Single();
            ViewBag.IDPenyelia = getuserid;
            ViewBag.Print = print;

            //added by faeza 08.11.2021
            List<SelectListItem> PaymentModeList2 = new List<SelectListItem>();
            PaymentModeList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PaymentModeList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.PaymentModeSelection = PaymentModeList;
            //modified by faeza 08.11.2021
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(PaySheetPekerjaList);
            }

            else
            {
                if (RadioGroup == 0)
                {
                    if (SelectionList == "0")
                    {
                        if (PaymentModeList == "0")
                        {
                            IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var salary in salaryData)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salary,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);

                        }
                        else
                        {
                            IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var salary in salaryData)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salary,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);

                        }
                    }
                    else
                    {
                        if (PaymentModeList == "0")
                        {
                            var salaryDataSingle = new ViewingModels.vw_PaySheetPekerja();
                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryDataSingle = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                salaryDataSingle = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (salaryDataSingle != null)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salaryDataSingle.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salaryDataSingle,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);

                        }
                        else
                        {
                            var salaryDataSingle = new ViewingModels.vw_PaySheetPekerja();
                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryDataSingle = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            else
                            {
                                salaryDataSingle = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama)
                                    .SingleOrDefault();
                            }

                            if (salaryDataSingle != null)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salaryDataSingle.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salaryDataSingle,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);
                        }
                    }
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        if (PaymentModeList == "0")
                        {
                            IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var salary in salaryData)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salary,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);

                        }
                        else
                        {
                            IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;
                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var salary in salaryData)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salary,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);
                        }
                    }

                    else
                    {
                        if (PaymentModeList == "0")
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                                .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID)
                                .Select(s => s.fld_KumpulanID)
                                .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var salary in salaryData)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salary,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);
                        }
                        else
                        {
                            var groupData = dbview.tbl_KumpulanKerja
                                .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                            x.fld_LadangID == LadangID)
                                .Select(s => s.fld_KumpulanID)
                                .SingleOrDefault();

                            IOrderedQueryable<ViewingModels.vw_PaySheetPekerja> salaryData;

                            if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList &&
                                                x.fld_Ktgpkj == WorkCategoryList &&
                                                x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            else
                            {
                                salaryData = dbview.vw_PaySheetPekerja
                                    .Where(x => x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                                x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_PaymentMode == PaymentModeList &&
                                                x.fld_SyarikatID == SyarikatID &&
                                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                    .OrderBy(x => x.fld_Nama);
                            }

                            foreach (var salary in salaryData)
                            {
                                var workerAdditionalContribution = dbr.tbl_ByrCarumanTambahan
                                    .Where(x => x.fld_GajiID == salary.fld_ID && x.fld_NegaraID == NegaraID &&
                                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                                x.fld_LadangID == LadangID);

                                List<CarumanTambahanCustomModel> carumanTambahanCustomModelList = new List<CarumanTambahanCustomModel>();

                                foreach (var caruman in workerAdditionalContribution)
                                {
                                    CarumanTambahanCustomModel carumanTambahanCustomModel = new CarumanTambahanCustomModel();

                                    carumanTambahanCustomModel.fld_ID = caruman.fld_ID;
                                    carumanTambahanCustomModel.fld_KodCarumanTambahan = caruman.fld_KodSubCaruman;
                                    carumanTambahanCustomModel.fld_CarumanMajikan = caruman.fld_CarumanMajikan;
                                    carumanTambahanCustomModel.fld_CarumanPekerja = caruman.fld_CarumanPekerja;

                                    carumanTambahanCustomModelList.Add(carumanTambahanCustomModel);
                                }

                                PaySheetPekerjaList.Add(
                                    new vw_PaySheetPekerjaCustomModel()
                                    {
                                        PaySheetPekerja = salary,
                                        CarumanTambahan = carumanTambahanCustomModelList
                                    });
                            }

                            if (PaySheetPekerjaList.Count == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }

                            return View(PaySheetPekerjaList);
                        }
                    }
                }
            }// end modified faeza
        }

        public ActionResult AverageMonthlySalaryReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now;

            var yearlist = new List<SelectListItem>();
            for (var i = startyear; i <= currentyear; i++)
            {
                if (i == selectdate.Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KumpulanID.ToString(), Text = s.fld_KodKumpulan }).Distinct(), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.YearList = yearlist;
            ViewBag.GroupList = GroupList;
            ViewBag.WorkerList = WorkerList;
            return View();
        }

        public ViewResult AverageMonthlySalaryReportDetail(int? YearList, string GroupList, string WorkerList)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            //if (GroupList == "0") //Shazana 29/11/2022
            if (GroupList == "0" && WorkerList == "0") //Shazana 29/11/2022
            {
                // var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).ToList(); //Shazana 29/11/2022
                var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).OrderBy(x => x.fld_Nama).ToList();//Shazana 29/11/2022
                ViewBag.DataCount = result.Count();
                return View(result);
            }

            //Shazana 29/11/2022
            if (GroupList == "0" && WorkerList != "0")
            {
                var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).Where(x => x.fld_Nopkj == WorkerList).OrderBy(x => x.fld_Nama).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
            //Close Shazana 29/11/2022
            else
            {
                GroupList = GroupList == null ? "0" : GroupList;
                int groupID = int.Parse(GroupList);
                if (WorkerList == "0")
                {
                    //var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID,YearList).Where(x=>x.fld_GroupID==groupID).ToList(); //Shazana 29/11/2022
                    var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).Where(x => x.fld_GroupID == groupID).OrderBy(x => x.fld_Nama).ToList(); //Shazana 29/11/2022
                    ViewBag.DataCount = result.Count();
                    return View(result);
                }
                else
                {
                    /*var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).Where(x=>x.fld_GroupID==groupID && x.fld_Nopkj==WorkerList).ToList();*/ //Shazana 29/11/2022
                    var result = dbsp.sp_RptPurataGajiBulanan(NegaraID, SyarikatID, WilayahID, LadangID, YearList).Where(x => x.fld_Nopkj == WorkerList).OrderBy(x => x.fld_Nama).ToList(); /*Shazana 29/11/2022*/
                    ViewBag.DataCount = result.Count();
                    return View(result);
                }
            }
        }

        public ActionResult MinimumWageReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false &&
                                                   x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID),
                "fldOptConfValue", "fldOptConfDesc");

            ViewBag.YearList = yearlist;

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            return View();
        }

        public ActionResult _MinimumWageRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.StatusList = statusList;

            var workCategoryList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();

            ViewBag.WorkCategoryList = workCategoryList;

            return View();
        }

        public ViewResult _MinimumWageRptSearch(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();

            List<CustMod_MinimumWage> GajiMinimaList = new List<CustMod_MinimumWage>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.Print = print;

            var minimumWageValue = dbview2.tblOptionConfigsWeb
                .Where(x => x.fldOptConfFlag1 == "gajiMinima" && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                .Select(s => s.fldOptConfValue)
                .Single();

            var minimumWageInt = Convert.ToInt32(minimumWageValue);


            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(GajiMinimaList);
            }

            else
            {
                if (RadioGroup == 0)

                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiMinima> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var worker in workerData)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_Deleted == false)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                            .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = worker.fld_Nopkj;
                            GajiMinima.Nama = worker.fld_Nama;
                            GajiMinima.Warganegara = worker.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = worker.fld_Trshjw;
                            GajiMinima.Nokp = worker.fld_Nokp;
                            GajiMinima.KategoriKerja = worker.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = worker.fld_ByrKerja;
                            GajiMinima.Sebab = worker.fld_Sebab;
                            GajiMinima.PelanTindakan = worker.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }

                        if (GajiMinimaList.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(GajiMinimaList);
                    }

                    else
                    {
                        var workerDataSingle = new ViewingModels.vw_GajiMinima();

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerDataSingle = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Nopkj == SelectionList && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        else
                        {
                            workerDataSingle = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama)
                                .SingleOrDefault();
                        }

                        if (workerDataSingle != null)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == SelectionList && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_Deleted == false)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == SelectionList &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = workerDataSingle.fld_Nopkj;
                            GajiMinima.Nama = workerDataSingle.fld_Nama;
                            GajiMinima.Warganegara = workerDataSingle.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = workerDataSingle.fld_Trshjw;
                            GajiMinima.Nokp = workerDataSingle.fld_Nokp;
                            GajiMinima.KategoriKerja = workerDataSingle.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = workerDataSingle.fld_ByrKerja;
                            GajiMinima.Sebab = workerDataSingle.fld_Sebab;
                            GajiMinima.PelanTindakan = workerDataSingle.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }
                    }

                    if (GajiMinimaList.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(GajiMinimaList);
                }

                else
                {
                    if (SelectionList == "0")
                    {
                        IOrderedQueryable<ViewingModels.vw_GajiMinima> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_Kdaktf == StatusList && x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var worker in workerData)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_Deleted == false)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = worker.fld_Nopkj;
                            GajiMinima.Nama = worker.fld_Nama;
                            GajiMinima.Warganegara = worker.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = worker.fld_Trshjw;
                            GajiMinima.Nokp = worker.fld_Nokp;
                            GajiMinima.KategoriKerja = worker.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = worker.fld_ByrKerja;
                            GajiMinima.Sebab = worker.fld_Sebab;
                            GajiMinima.PelanTindakan = worker.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }

                        if (GajiMinimaList.Count == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }

                        return View(GajiMinimaList);
                    }

                    else
                    {
                        var groupData = dbview.tbl_KumpulanKerja
                            .Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID &&
                                        x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                        x.fld_LadangID == LadangID)
                            .Select(s => s.fld_KumpulanID)
                            .SingleOrDefault();

                        IOrderedQueryable<ViewingModels.vw_GajiMinima> workerData;

                        if (!String.IsNullOrEmpty(WorkCategoryList) && !String.IsNullOrEmpty(StatusList))
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_KumpulanID == groupData && x.fld_Kdaktf == StatusList &&
                                            x.fld_Ktgpkj == WorkCategoryList &&
                                            x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        else
                        {
                            workerData = dbview.vw_GajiMinima
                                .Where(x => x.fld_ByrKerja < minimumWageInt && x.fld_Month == MonthList && x.fld_Year == YearList &&
                                            x.fld_KumpulanID == groupData && x.fld_NegaraID == NegaraID &&
                                            x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .OrderBy(x => x.fld_Nama);
                        }

                        foreach (var worker in workerData)
                        {
                            var getOfferedWorkingDay = dbview.tbl_Produktiviti
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj && x.fld_Month == MonthList &&
                                            x.fld_Year == YearList && x.fld_Deleted == false)
                                .Select(s => s.fld_HadirKerja)
                                .SingleOrDefault();

                            var getActualWorkingDay = dbview.tbl_Kerjahdr
                                .Where(x => x.fld_Nopkj == worker.fld_Nopkj &&
                                            x.fld_Tarikh.Value.Month == MonthList &&
                                            x.fld_Tarikh.Value.Year == YearList &&
                                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                                .Select(s => s.fld_Kdhdct == "H01" || s.fld_Kdhdct == "H02" || s.fld_Kdhdct == "H03").Count();

                            CustMod_MinimumWage GajiMinima = new CustMod_MinimumWage();

                            GajiMinima.NoPkj = worker.fld_Nopkj;
                            GajiMinima.Nama = worker.fld_Nama;
                            GajiMinima.Warganegara = worker.fld_Kdrkyt;
                            GajiMinima.TarikhSahJawatan = worker.fld_Trshjw;
                            GajiMinima.Nokp = worker.fld_Nokp;
                            GajiMinima.KategoriKerja = worker.fld_Ktgpkj;
                            GajiMinima.JumlahHariBekerja = getActualWorkingDay;
                            GajiMinima.JumlahHariTawaranKerja = getOfferedWorkingDay;
                            GajiMinima.GajiBulanan = worker.fld_ByrKerja;
                            GajiMinima.Sebab = worker.fld_Sebab;
                            GajiMinima.PelanTindakan = worker.fld_Tindakan;
                            GajiMinimaList.Add(GajiMinima);
                        }
                    }

                    if (GajiMinimaList.Count == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoRecord;
                    }

                    return View(GajiMinimaList);
                }
            }
        }

        public ActionResult ProductivityReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nopkj)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.SelectionList = SelectionList;

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

            ViewBag.MonthList = new SelectList(
                db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID &&
                                                   x.fld_SyarikatID == SyarikatID && x.fldDeleted == false),
                "fldOptConfValue", "fldOptConfDesc", month);

            var statusList = new List<SelectListItem>();
            statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            statusList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));


            ViewBag.StatusList = statusList;

            var unitList = new List<SelectListItem>();
            unitList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "unit" && x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            unitList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            var allPeringkatList = new List<SelectListItem>();

            var peringkatList = new SelectList(
                dbr.tbl_PktUtama
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_PktUtama)
                    .Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama }),
                "Value", "Text").ToList();

            allPeringkatList.AddRange(peringkatList);

            var subPktList = new SelectList(
                dbr.tbl_SubPkt
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_Pkt)
                    .Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt }),
                "Value", "Text").ToList();

            allPeringkatList.AddRange(subPktList);

            var blokList = new SelectList(
                dbr.tbl_Blok
                    .Where(x => x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(o => o.fld_Blok)
                    .Select(s => new SelectListItem { Value = s.fld_Blok, Text = s.fld_Blok }),
                "Value", "Text").ToList();

            allPeringkatList.AddRange(blokList);

            allPeringkatList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));
            ViewBag.AllPeringkatList = allPeringkatList;
            ViewBag.UnitList = unitList;

            return View();
        }

        public ViewResult _ProductivityRptSearch(int? MonthList, int? YearList,
            string SelectionList, string UnitList, string AllPeringkatList, string StatusList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<sp_RptProduktiviti_Result> RptProduktiviti = new List<sp_RptProduktiviti_Result>();

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = YearList;
            ViewBag.WorkerList = SelectionList;
            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Print = print;

            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(RptProduktiviti);
            }

            else
            {
                RptProduktiviti = dbsp.sp_RptProduktiviti(NegaraID, SyarikatID, WilayahID, LadangID, YearList,
                        MonthList, SelectionList, UnitList, AllPeringkatList, StatusList)
                    .ToList();

                if (RptProduktiviti.Count == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }

                return View(RptProduktiviti);
            }
        }

        public ActionResult _ProductivityRptAdvanceSearch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var statusList = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                    .OrderBy(o => o.fldOptConfDesc)
                    .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }),
                "Value", "Text").ToList();
            statusList.Insert(0, (new SelectListItem { Text = "Semua", Value = "" }));

            ViewBag.StatusList = statusList;

            return View();
        }

        public ActionResult KwspSocsoMonthlyReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int range = int.Parse(GetConfig.GetData("yeardisplay"));
            int startyear = DateTime.Now.AddYears(-range).Year;
            int currentyear = DateTime.Now.Year;
            DateTime selectdate = DateTime.Now;

            var yearlist = new List<SelectListItem>();
            for (var i = startyear; i <= currentyear; i++)
            {
                if (i == selectdate.Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList = new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KumpulanID.ToString(), Text = s.fld_KodKumpulan }).Distinct(), "Value", "Text").ToList();
            GroupList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> WorkerList = new List<SelectListItem>();
            WorkerList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + " - " + s.fld_Nama }).Distinct(), "Value", "Text").ToList();
            WorkerList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.YearList = yearlist;
            ViewBag.GroupList = GroupList;
            ViewBag.WorkerList = WorkerList;
            return View();
        }

        public ActionResult KwspSocsoMonthlyReportDetail(int? YearList, string GroupList, string WorkerList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.Print = print;

            int groupID = Convert.ToInt32(GroupList);
            //int YearID = Convert.ToInt32(YearList);

            if (YearList == null && GroupList == null && WorkerList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseAips;
                return View();
            }

            if (GroupList == "0")
            {
                var result = dbr.vw_rptKwspSocso.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                if (result.ToList().Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoInform;
                }
                return View(result);
            }
            else
            {
                //int groupID = int.Parse(GroupList);

                if (WorkerList == "0")
                {
                    var result = dbr.vw_rptKwspSocso.Where(x => x.fld_KumpulanID == groupID && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                    if (result.ToList().Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoInform;
                    }
                    return View(result);
                }
                else
                {
                    var result = dbr.vw_rptKwspSocso.Where(x => x.fld_KumpulanID == groupID && x.fld_Nopkj == WorkerList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                    if (result.ToList().Count() == 0)
                    {
                        ViewBag.Message = GlobalResEstate.msgNoInform;
                    }
                    return View(result);
                }
            }
        }

        public ActionResult SkbReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
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

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        //public ActionResult SkbReportDetail(int? MonthList, int? YearList, string print)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    var result = dbsp.sp_RptSkb(NegaraID, SyarikatID, WilayahID, LadangID, MonthList, YearList).ToList();
        //    ViewBag.DataCount = result.Count();
        //    ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
        //    ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
        //    ViewBag.Print = print;

        //    if (MonthList == null && YearList == null)
        //    {
        //        ViewBag.Message = GlobalResEstate.msgChooseMonthYear;
        //        return View();
        //    }

        //    if (result.ToList().Count() == 0)
        //    {
        //        ViewBag.Message = GlobalResEstate.lblNoSkb;
        //        return View();
        //    }

        //    return View(result);
        //}

        public ActionResult AccStatusReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
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

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month).ToList();
            MonthList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ActionResult AccStatusReportDetail(int? YearList, int? MonthList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.Print = print;

            if (YearList == null && MonthList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseMonthYear;
                return View();
            }

            if (MonthList == 0)
            {
                var result = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Month).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
            else
            {
                var result = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
        }

        public ActionResult GenSalaryStatusReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Report = "class = active";
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
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

            var MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false & x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month).ToList();
            MonthList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.MonthList = MonthList;
            ViewBag.YearList = yearlist;
            return View();
        }

        public ActionResult GenSalaryStatusReportDetail(int YearList, int MonthList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.MonthSelection = MonthList;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            if (MonthList == 0)
            {
                var result = db.tbl_SevicesProcess.Where(x => x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Month).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
            else
            {
                var result = db.tbl_SevicesProcess.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                ViewBag.DataCount = result.Count();
                return View(result);
            }
        }

        public ActionResult PaySlipRpt()
        {
            ViewBag.Report = "class = active";
            //int month = timezone.gettimezone().AddMonths(-1).Month;
            int month = timezone.gettimezone().Month; //fatin modified - 15/12/2023
            int year = timezone.gettimezone().Year;
            int rangeyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var yearlist = new List<SelectListItem>();
            for (var i = rangeyear; i <= year; i++)
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
            var monthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;
            ViewBag.MonthList = monthList;
            ViewBag.YearList = yearlist;
            ViewBag.StatusList = StatusList;
            return View();
        }


        public ActionResult _PaySlipRptAdvance()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> CategoryList = new List<SelectListItem>();
            CategoryList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            CategoryList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.WorkCategoryList = CategoryList;
            return View();
        }

        public ActionResult _PaySlipRptSearch(int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string StatusList, string WorkCategoryList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string[] flag1 = new string[] { "KesukaranMembaja", "KesukaranMenuai", "KesukaranMemunggah", "designation", "jantina" };
            List<MasterModels.tblOptionConfigsWeb> webConfigList = GetConfig.GetWebConfigList(flag1, NegaraID, SyarikatID);
            var pktHargaKesukaran = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_LadangID == LadangID).ToList();

            ViewBag.WebConfigList = webConfigList;
            ViewBag.PktHargaKesukaran = pktHargaKesukaran;
            ViewBag.SelectedMonth = MonthList;
            ViewBag.SelectedYear = YearList;
            ViewBag.Print = print;
            var pkjList = new List<Models.tbl_Pkjmast>();
            //find pekerja
            if (WorkCategoryList == "0" || WorkCategoryList == null)
            {
                if (RadioGroup == 0)
                {
                    //individu
                    if (StatusList == "0")
                    {
                        // aktif & xaktif
                        if (SelectionList == "0")
                        {
                            //semua individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                        }
                        else
                        {
                            //selected individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                        }

                    }
                    else
                    {
                        // aktif/xaktif
                        if (SelectionList == "0")
                        {
                            //semua individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                        }
                        else
                        {
                            //selected individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                        }
                    }
                }
                else
                {
                    //group
                    if (SelectionList == "0")
                    {
                        //semua group
                        pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                    }
                    else
                    {
                        //selected group
                        var kumpID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                        pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                    }
                }
            }
            else
            {
                //kategori pkj
                pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
            }

            List<Payslip_Result> payslipList = new List<Payslip_Result>();
            List<tbl_Kerja> kerjaList = new List<tbl_Kerja>();
            List<tbl_Kerjahdr> kerjahdrList = new List<tbl_Kerjahdr>();
            List<tbl_KerjaKesukaran> kerjakesukaranList = new List<tbl_KerjaKesukaran>();
            if (MonthList != null && YearList != null)
            {
                var pkjNoList = pkjList.Select(s => s.fld_Nopkj).ToList();
                var workers = new List<Worker>();
                foreach (var pkjNo in pkjNoList)
                {
                    workers.Add(new Worker { WorkerID = pkjNo });
                }
                var workersDT = workers.ToDataTable();

                string constr = Connection.GetConnectionString(WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                var con = new SqlConnection(constr);
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("NegaraID", NegaraID);
                    parameters.Add("SyarikatID", SyarikatID);
                    parameters.Add("WilayahID", WilayahID);
                    parameters.Add("LadangID", LadangID);
                    parameters.Add("Month", MonthList);
                    parameters.Add("Year", YearList);
                    parameters.Add("Workers", workersDT.AsTableValuedParameter("[dbo].[Workers]"));
                    con.Open();
                    SqlMapper.Settings.CommandTimeout = 300;
                    payslipList = SqlMapper.Query<Payslip_Result>(con, "sp_Payslip_V2", parameters).ToList();
                    con.Close();
                }
                catch (Exception)
                {
                    throw;
                }
                kerjaList = dbr.tbl_Kerja.Where(x => pkjNoList.Contains(x.fld_Nopkj) && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_LadangID == LadangID && x.fld_HrgaKwsnSkar > 0.00m && !string.IsNullOrEmpty(x.fld_HrgaKwsnSkar.Value.ToString())).ToList();
                kerjahdrList = dbr.tbl_Kerjahdr.Where(x => pkjNoList.Contains(x.fld_Nopkj) && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_LadangID == LadangID).ToList();
                var hardWorkDataIDs = kerjaList.Select(s => s.fld_ID).ToList();
                kerjakesukaranList = dbr.tbl_KerjaKesukaran.Where(x => hardWorkDataIDs.Contains(x.fld_KerjaID.Value)).ToList();

            }

            ViewBag.Kump = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).ToList();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).Select(s => s.fld_LdgCode + "-" + s.fld_LdgName).FirstOrDefault();

            ViewBag.PayslipList = payslipList;
            ViewBag.KerjaList = kerjaList;
            ViewBag.KerjahdrList = kerjahdrList;
            ViewBag.KerjakesukaranList = kerjakesukaranList;
            return View(pkjList);
        }


        public ActionResult _PaySlipRptDetail(string pkj, List<Payslip_Result> payslip, int month, int year, List<MasterModels.tblOptionConfigsWeb> webConfigList, List<tbl_PktHargaKesukaran> pktHargaKesukaran, List<tbl_Pkjmast> tbl_Pkjmast, List<tbl_Kerja> tbl_Kerja, List<tbl_Kerjahdr> tbl_Kerjahdr, List<tbl_KerjaKesukaran> tbl_KerjaKesukaran, List<tbl_KumpulanKerja> tbl_KumpulanKerja, string NamaSyarikat, string NoSyarikat, string NamaLadang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var getpkjInfo = tbl_Pkjmast.Where(x => x.fld_Nopkj == pkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1);
            var hardWorkDatas = tbl_Kerja.Where(x => x.fld_Nopkj == pkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_LadangID == LadangID && x.fld_HrgaKwsnSkar > 0.00m && !string.IsNullOrEmpty(x.fld_HrgaKwsnSkar.Value.ToString())).ToList();
            var attWorkDatas = tbl_Kerjahdr.Where(x => x.fld_Nopkj == pkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_LadangID == LadangID).ToList();
            var hardWorkDataIDs = hardWorkDatas.Select(s => s.fld_ID).ToList();
            var hardWorkDatasNew = tbl_KerjaKesukaran.Where(x => hardWorkDataIDs.Contains(x.fld_KerjaID.Value)).ToList();

            ViewBag.NamaPkj = getpkjInfo.Select(s => s.fld_Nama).FirstOrDefault();
            ViewBag.NoKwsp = getpkjInfo.Select(s => s.fld_Nokwsp).FirstOrDefault();
            ViewBag.NoSocso = getpkjInfo.Select(s => s.fld_Noperkeso).FirstOrDefault();
            ViewBag.NoKp = getpkjInfo.Select(s => s.fld_Nokp).FirstOrDefault();

            int? kumpID = getpkjInfo.Select(s => s.fld_KumpulanID).FirstOrDefault();//desc
            string ktgrPkj = getpkjInfo.Select(s => s.fld_Ktgpkj).FirstOrDefault();//desc
            string jntnaPkj = getpkjInfo.Select(s => s.fld_Kdjnt).FirstOrDefault();//desc

            ViewBag.Kump = tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
            ViewBag.Kategori = webConfigList.Where(x => x.fldOptConfFlag1 == "designation" && x.fldOptConfValue == ktgrPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
            ViewBag.Jantina = webConfigList.Where(x => x.fldOptConfFlag1 == "jantina" && x.fldOptConfValue == jntnaPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.NamaSyarikat = NamaSyarikat;
            ViewBag.NoSyarikat = NoSyarikat;
            //farahin tambah -  15/09/2021

            ViewBag.NamaLadang = NamaLadang;
            //
            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.Date = System.DateTime.Now.ToShortDateString();
            ViewBag.AttWorkDatas = attWorkDatas;
            ViewBag.HardWorkDatas = hardWorkDatas;
            ViewBag.WebConfigList = webConfigList;
            ViewBag.PktHargaKesukaran = pktHargaKesukaran;
            ViewBag.HardWorkDatasNew = hardWorkDatasNew;
            return View(payslip);
        }


        public ActionResult _PaySlipRptDaycount(string nopkj, int month, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //shah
            var FooterPayslipDetails = new List<FooterPayslipDetails>();
            int id = 1;
            //get Hadir and cuti Count
            var hdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            var hdrhrbs = hdr.Where(x => x.fld_Kdhdct == "H01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrbs", count = hdrhrbs });
            id += 1;
            var hdrhrmg = hdr.Where(x => x.fld_Kdhdct == "H02").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrmg", count = hdrhrmg });
            id += 1;
            var hdrhrcu = hdr.Where(x => x.fld_Kdhdct == "H03").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcu", count = hdrhrcu });
            id += 1;
            var hdrhrpg = hdr.Where(x => x.fld_Kdhdct == "P01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrpg", count = hdrhrpg });
            id += 1;
            var hdrhrct = hdr.Where(x => x.fld_Kdhdct == "C02").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrct", count = hdrhrct });
            id += 1;
            var hdrhrtg = hdr.Where(x => x.fld_Kdhdct == "C05").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrtg", count = hdrhrtg });
            id += 1;
            var hdrhrcs = hdr.Where(x => x.fld_Kdhdct == "C03").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcs", count = hdrhrcs });
            id += 1;
            var hdrhrca = hdr.Where(x => x.fld_Kdhdct == "C01").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrca", count = hdrhrca });
            id += 1;
            var hdrhrcm = hdr.Where(x => x.fld_Kdhdct == "C07").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcm", count = hdrhrcm });
            id += 1;
            var hdrhrcb = hdr.Where(x => x.fld_Kdhdct == "C04").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrcb", count = hdrhrcb });
            id += 1;
            var hdrhrch = hdr.Where(x => x.fld_Kdhdct == "C10").Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrch", count = hdrhrch });
            id += 1;
            var hdrhrhujan = hdr.Where(x => x.fld_Hujan == 1).Count();
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrhrhujan", count = hdrhrhujan });

            //get hdr OT
            var hdrot = dbr.vw_KerjaHdrOT.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            id += 1;
            var hdrothrbs = hdrot.Where(x => x.fld_Kdhdct == "H01").Sum(s => s.fld_JamOT);
            hdrothrbs = hdrothrbs == null ? 0m : hdrothrbs;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrbs", value = hdrothrbs.Value });
            id += 1;
            var hdrothrcm = hdrot.Where(x => x.fld_Kdhdct == "H02").Sum(s => s.fld_JamOT);
            hdrothrcm = hdrothrcm == null ? 0m : hdrothrcm;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrcm", value = hdrothrcm.Value });
            id += 1;
            var hdrothrcu = hdrot.Where(x => x.fld_Kdhdct == "H03").Sum(s => s.fld_JamOT);
            hdrothrcu = hdrothrcu == null ? 0m : hdrothrcu;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hdrothrcu", value = hdrothrcu.Value });

            //get Jumlah Hari Kerja
            int? hrkrja = db.tbl_HariBekerjaLadang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_BilHariBekerja).FirstOrDefault();
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "hrkrja", count = hrkrja.Value });

            id += 1;
            var jmlhctumum = 0;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "jmlhctumum", count = jmlhctumum });

            //get jmlh hari hadir
            var cdct = new string[] { "H01", "H02", "H03" };
            var jmlhhdr = hdr.Where(x => cdct.Contains(x.fld_Kdhdct)).Count();
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "jmlhhdr", count = jmlhhdr });

            //get avg slry
            DateTime cdate = new DateTime(year, month, 15);
            DateTime ldate = cdate.AddMonths(-1);
            var cravgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == cdate.Month && x.fld_Year == cdate.Year && x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_PurataGaji, s.fld_PurataGaji12Bln }).FirstOrDefault();
            var crmnthavgslry = cravgslry == null ? 0m : cravgslry.fld_PurataGaji;
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "crmnthavgslry", value = crmnthavgslry.Value });
            
            var lsmnthavgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == ldate.Month && x.fld_Year == ldate.Year && x.fld_Nopkj == nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_PurataGaji).FirstOrDefault();
            lsmnthavgslry= lsmnthavgslry == null ? 0m : lsmnthavgslry;
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "lsmnthavgslry", value = lsmnthavgslry.Value });

            var yearavgslry = cravgslry == null || cravgslry.fld_PurataGaji12Bln > 200 ? 0m : cravgslry.fld_PurataGaji12Bln;
            id += 1;
            FooterPayslipDetails.Add(new FooterPayslipDetails { id = id, flag = "yearavgslry", value = yearavgslry.Value });
            //shah
            return View(FooterPayslipDetails);
        }

        //public ActionResult htmltopdf(string month)
        //{
        //    return new Rotativa.MVC.RouteAsPdf("ExpiredPassport", new { month = month });
        //}

        //farahin edit whole function - 23/02/2021
        [HttpPost]
        public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            dbr.tblHtmlReports.Add(tblHtmlReport);
            dbr.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        }

        //farahin edit whole function - 23/02/2021
        public ActionResult GetPDF(int id) //untuk cater report pdf
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string width = "1700", height = "1190";
            string imagepath = Server.MapPath("~/Asset/Images/");

            var gethtml = dbModels.tblHtmlReports.Find(id);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();

            //Export HTML String as PDF.

            Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
            {
                using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
                {
                    htmlWorker.Open();
                    htmlWorker.Parse(sr);
                }
            }
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            dbModels.Entry(gethtml).State = EntityState.Deleted;
            dbModels.SaveChanges();
            return View();
        }

        //farahin-23/02/2021 - tambah function convertpdf3 untuk cater download PDF report passport
        public ActionResult ConvertPDF3(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            dbr.tblHtmlReports.Add(tblHtmlReport);
            dbr.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF3", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        }

        //farahin-23/02/2021 - tambah function getpdf3 utk cater download PDF report passport
        public ActionResult GetPDF3(int id)
        {

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string width = "1700", height = "1190";
            string imagepath = Server.MapPath("~/Asset/Images/");

            var gethtml = dbModels.tblHtmlReports.Find(id);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();


            Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            using (TextReader sr = new StringReader(gethtml.fldHtlmCode))
            {
                using (var htmlWorker = new HTMLWorkerExtended(pdfDoc, imagepath + logosyarikat))
                {
                    htmlWorker.Open();
                    htmlWorker.Parse(sr);
                }
            }
            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            dbModels.Entry(gethtml).State = EntityState.Deleted;
            dbModels.SaveChanges();
            return View();
        }

        //farahin - 23/2/2021 - tambah function convertpdf4 untuk vater report kehadiran pekerja
        public ActionResult ConvertPDF4(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tblHtmlReport tblHtmlReport = new Models.tblHtmlReport();

            tblHtmlReport.fldHtlmCode = myHtml;
            tblHtmlReport.fldFileName = filename;
            tblHtmlReport.fldReportName = reportname;

            dbr.tblHtmlReports.Add(tblHtmlReport);
            dbr.SaveChanges();

            success = true;
            status = "success";

            return Json(new { success = success, id = tblHtmlReport.fldID, msg = msg, status = status, link = Url.Action("GetPDF4", "Report", null, "http") + "/" + tblHtmlReport.fldID });
        }
        //farahin-23/2/2021 - tambah function getpdf4 untuk vater report kehadiran pekerja
        public ActionResult GetPDF4(int id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string width = "1700", height = "1190";
            string imagepath = Server.MapPath("~/Asset/Images/");

            var gethtml = dbModels.tblHtmlReports.Find(id);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var logosyarikat = db2.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_LogoName).FirstOrDefault();

            //Export HTML String as PDF.

            StringReader sr = new StringReader(gethtml.fldHtlmCode);
            Document pdfDoc = new Document(new Rectangle(int.Parse(width), int.Parse(height)), 50f, 50f, 50f, 50f);

            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);

            pdfDoc.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + gethtml.fldFileName + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            dbModels.Entry(gethtml).State = EntityState.Deleted;
            dbModels.SaveChanges();
            return View();
        }

        public ActionResult AsasPeringkat()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.pktlist = StatusList2;
            ViewBag.getflag = 1;
            return View();


        }

        [HttpPost]
        public ActionResult AsasPeringkat(string pktlist)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.pktlist = StatusList2;
            ViewBag.getflag = 2;
            ViewBag.getlevel = pktlist;
            return View();

        }

        public ActionResult AsasPeringkatSemua()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            ViewBag.TableInfo = "class = active";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var result = dbr.vw_JnsPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

            return PartialView(result);
        }

        public ActionResult AsasPeringkatUtama()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var result = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(x => x.fld_PktUtama).ToList();
            ViewBag.DataCount = result.Count();
            ViewBag.getflag = 2;
            return View(result);
        }

        public ActionResult AsasPeringkatSubPkt()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            var result = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(x => x.fld_Pkt).ToList();
            ViewBag.DataCount = result.Count();
            ViewBag.getflag = 2;
            return View(result);


        }

        public ActionResult AsasPeringkatBlok()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();


            var result = dbr.tbl_Blok.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(x => x.fld_Blok).ToList();
            ViewBag.DataCount = result.Count();
            ViewBag.getflag = 2;
            return View(result);

        }


        public ActionResult KodMappingAktiviti()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> GLlist = new List<SelectListItem>();
            GLlist = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL }).Distinct(), "Value", "Text").ToList();
            GLlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.GLlist = GLlist;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingAktiviti(string GLlist)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            List<SelectListItem> GLlist2 = new List<SelectListItem>();
            GLlist2 = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_KodGL, Text = s.fld_KodGL }).Distinct(), "Value", "Text").ToList();
            GLlist2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.GLlist = GLlist2;
            ViewBag.getflag = 2;

            if (GLlist == "0")
            {
                var result = db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
            else
            {
                var result = db.tbl_MapGL.Where(x => x.fld_KodGL == GLlist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
        }


        public ActionResult PrintWorkerPdf(int? RadioGroup, string SelectionList, string StatusList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerRptSearch", new { RadioGroup, StatusList, SelectionList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public string PDFInvalid()
        {
            return GlobalResEstate.msgInvalidPDFConvert;
        }


        public ViewResult _WorkerRptSearch(int? RadioGroup, string StatusList, string SelectionList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> StatusList2 = new List<SelectListItem>();
            List<SelectListItem> SelectionList2 = new List<SelectListItem>();

            List<Models.tbl_Pkjmast> InfoPekerja = new List<Models.tbl_Pkjmast>();

            StatusList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text", StatusList).ToList();
            StatusList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.StatusList = StatusList2;
            ViewBag.Print = print;

            if (StatusList == null && SelectionList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseWork;
                return View(InfoPekerja);
            }
            else
            {
                if (RadioGroup == 0)
                {
                    //Individu Semua
                    if (StatusList == "0")
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                        if (SelectionList == "0")
                        {
                            //individu semua pekerja
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                        else
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                    }
                    else
                    {
                        SelectionList2 = new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList).OrderBy(o => o.fld_Nopkj).Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }), "Value", "Text").ToList();
                        SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                        if (SelectionList == "0")
                        {
                            //individu aktif/xaktif pekerja
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                        else
                        {
                            var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList);
                            if (result.Count() == 0)
                            {
                                ViewBag.Message = GlobalResEstate.msgNoRecord;
                            }
                            ViewBag.SelectionList = SelectionList2;
                            ViewBag.getflag = 2;
                            return View(result);
                        }
                    }
                }
                else
                {
                    //Group
                    SelectionList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
                    SelectionList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
                    if (SelectionList == "0")
                    {
                        //semua kump
                        var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID != null); ViewBag.SelectionList = SelectionList2;
                        if (result.Count() == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }
                        ViewBag.getflag = 2;
                        return View(result);
                    }
                    else
                    {
                        //by kump
                        int getkump = dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.fld_KodKumpulan == SelectionList).Select(s => s.fld_KumpulanID).FirstOrDefault();
                        var result = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == getkump);
                        if (result.Count() == 0)
                        {
                            ViewBag.Message = GlobalResEstate.msgNoRecord;
                        }
                        ViewBag.SelectionList = SelectionList2;
                        ViewBag.getflag = 2;
                        return View(result);
                    }
                }
            }
        }

        public ActionResult PrintGrpWorkerPdf(string GroupList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_GroupReport", new { GroupList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintWorkerInsentifPdf(int? RadioGroup, int? MonthList, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerIncentiveRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintLeavePdf(int? RadioGroup, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerLeaveRptSearch", new { RadioGroup, YearList, SelectionList, StatusList, WorkCategoryList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintAttRptPdf(int? RadioGroup, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerAttendanceRptSearch", new { RadioGroup, YearList, SelectionList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalidGen");
            }

            return report;
        }

        public ActionResult PrintAccBankPdf(int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_BankAccReport", new { print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalidGen");
            }

            return report;
        }

        public ActionResult PrintPaySheetPdf(int? RadioGroup, int? MonthList, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerPaySheetRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintMiniWagePdf(int? RadioGroup, int? MonthList, int? YearList,
               string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_MinimumWageRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintAccPdf(string StatusList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_AccReport", new { StatusList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintKwspSocsoPdf(string StatusList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_KwspSocsoReport", new { StatusList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintWorkPdf(string MonthList, string YearList, string WorkerList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkReport", new { MonthList, YearList, WorkerList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintTransactionPdf(string MonthList, string YearList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_TransactionListingRptSearch", new { MonthList, YearList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintAccStatusPdf(string MonthList, string YearList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("AccStatusReportDetail", new { MonthList, YearList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintAIPSPdf(string MonthList, string YearList, string WorkerList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("AccStatusReportDetail", new { MonthList, YearList, WorkerList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintProductPdf(int? MonthList, int? YearList,
            string SelectionList, string UnitList, string AllPeringkatList, string StatusList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_ProductivityRptSearch", new { MonthList, YearList, SelectionList, UnitList, AllPeringkatList, StatusList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintKwspSocsoMonthPdf(int? YearList, string GroupList, string WorkerList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("KwspSocsoMonthlyReportDetail", new { YearList, GroupList, WorkerList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintHasilPdf(int? RadioGroup, int? MonthList, int? YearList, string SelectionList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("HasilReportDetail", new { RadioGroup, MonthList, YearList, SelectionList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintNotiPermitPdf(int? MonthList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("ExpiredPermit", new { MonthList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintNotiPassportPdf(string MonthList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("ExpiredPassport", new { MonthList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult PrintWorkerPaySlipPdf(int? RadioGroup, int? MonthList, int? YearList,
            string SelectionList, string StatusList, string WorkCategoryList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_WorkerPaySlipRptSearch", new { RadioGroup, MonthList, YearList, SelectionList, StatusList, WorkCategoryList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }
        public ActionResult KodMappingAktivitiPaysheet()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> Paysheetlist = new List<SelectListItem>();
            Paysheetlist = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_Paysheet, Text = s.fld_Paysheet }).Distinct(), "Value", "Text").ToList();
            Paysheetlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.Paysheetlist = Paysheetlist;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingAktivitiPaysheet(string Paysheetlist)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> Paysheetlist2 = new List<SelectListItem>();
            Paysheetlist2 = new SelectList(db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_Paysheet, Text = s.fld_Paysheet }).Distinct(), "Value", "Text").ToList();
            Paysheetlist2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.Paysheetlist = Paysheetlist2;
            ViewBag.getflag = 2;

            if (Paysheetlist == "0")
            {
                var result = db.tbl_MapGL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
            else
            {
                var result = db.tbl_MapGL.Where(x => x.fld_Paysheet == Paysheetlist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID & x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_ID);
                return View(result);
            }
        }


        public ActionResult KodMappingAktivitiGMN()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);


            var kodKategorilist = db.tbl_KategoriAktiviti
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            List<SelectListItem> KategoriAktiviti = new List<SelectListItem>();
            KategoriAktiviti = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }).Distinct(), "Value", "Text").ToList();
            KategoriAktiviti.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            List<SelectListItem> Costcnt = new List<SelectListItem>();
            Costcnt = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }).Distinct(), "Value", "Text").ToList();
            Costcnt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();


            ViewBag.KategoriAktiviti = KategoriAktiviti;
            ViewBag.Costcnt = Costcnt;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingAktivitiGMN(string KategoriAktiviti, string Costcnt)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var kodKategorilist = db.tbl_KategoriAktiviti
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();

            List<SelectListItem> KategoriAktiviti2 = new List<SelectListItem>();
            KategoriAktiviti2 = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }).Distinct(), "Value", "Text").ToList();
            KategoriAktiviti2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            List<SelectListItem> Costcnt2 = new List<SelectListItem>();
            Costcnt2 = new SelectList(db.vw_GmnMapping
                .Where(x => x.fld_KodKategori == KategoriAktiviti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }).Distinct(), "Value", "Text").ToList();
            Costcnt2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.KategoriAktiviti = KategoriAktiviti2;
            ViewBag.Costcnt = Costcnt2;
            ViewBag.getflag = 2;

            if (KategoriAktiviti == "0" && Costcnt == "0")
            {
                var result = db.vw_GmnMapping
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.bil);
                return View(result);
            }
            else
            {
                var result = db.vw_GmnMapping
                    .Where(x => x.fld_CostCentre == Costcnt && x.fld_KodKategori == KategoriAktiviti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.bil);
                return View(result);
            }
        }


        public JsonResult GetReportGMN(string KategoriAktiviti)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> PilihAktiviti = new List<SelectListItem>();
            PilihAktiviti = new SelectList(db.vw_GmnMapping.Where(x => x.fld_KodKategori == KategoriAktiviti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }).Distinct(), "Value", "Text").ToList();

            PilihAktiviti.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            //dbr.Dispose();
            return Json(new { PilihAktiviti });
        }


        public ActionResult KodMappingPupYm()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            List<SelectListItem> kodKategorilist = new List<SelectListItem>();
            kodKategorilist = new SelectList(db.tbl_KategoriAktiviti
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }), "Value", "Text").ToList();
            kodKategorilist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));


            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.kodKategorilist = kodKategorilist;
            ViewBag.getflag = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KodMappingPupYm(string kodKategorilist)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            List<SelectListItem> kodKategorilist2 = new List<SelectListItem>();
            kodKategorilist2 = new SelectList(db.tbl_KategoriAktiviti
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_Kategori }), "Value", "Text").ToList();
            kodKategorilist2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            ViewBag.kodKategorilist = kodKategorilist2;
            ViewBag.getflag = 2;

            if (kodKategorilist == "0")
            {
                var result = db.tbl_UpahAktiviti
                     .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodAktvt);
                return View(result);
            }
            else
            {
                var result = db.tbl_UpahAktiviti
                    .Where(x => x.fld_KategoriAktvt == kodKategorilist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodAktvt);
                return View(result);
            }
        }


        public ActionResult CustomerReport()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> EstateList = new List<SelectListItem>();

            EstateList = new SelectList(db.tbl_Ladang.OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = (s.fld_ID).ToString(), Text = s.fld_ID + "-" + s.fld_LdgName }), "Value", "Text").ToList();
            EstateList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.EstateList = EstateList;
            //ViewBag.getflag = 1;
            return View();
        }

        public ActionResult _CustomerReport(string EstateList, string print)
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<CustMod_CustSatisfaction> InfoCustSatisList = new List<CustMod_CustSatisfaction>();

            //List<Models.tbl_Pkjmast> InfoKmpln = new List<Models.tbl_Pkjmast>();
            //List<SelectListItem> GroupList2 = new List<SelectListItem>();
            //GroupList2 = new SelectList(dbr.vw_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false && x.bilangan_ahli >= 0).OrderBy(o => o.fld_KodKumpulan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + "-" + s.fld_Keterangan }), "Value", "Text").ToList();
            //GroupList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));
            ViewBag.Print = print;

            if (EstateList == null)
            {
                ViewBag.Message = GlobalResEstate.msgChooseEstate;
                return View();
            }

            if (EstateList == "0")
            {
                var result = dbr.tbl_Kepuasan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_UserID);

                foreach (var info in result)
                {
                    var getLdgInfo = db.tbl_Ladang
                        .Where(x => x.fld_ID == info.fld_LadangID)
                        .Select(s => s.fld_LdgName).FirstOrDefault();

                    var getUserInfo = db.tblUsers
                        .Where(x => x.fldUserID == info.fld_UserID)
                                    .Select(s => s.fldUserFullName).FirstOrDefault();

                    CustMod_CustSatisfaction CustSatis = new CustMod_CustSatisfaction();

                    CustSatis.UID = info.fld_UserID;
                    CustSatis.UIDNama = getUserInfo;
                    CustSatis.LdgID = info.fld_LadangID;
                    CustSatis.LdgNama = getLdgInfo;
                    CustSatis.Satis = info.fld_Kepuasan;
                    CustSatis.Note = info.fld_Catatan;
                    InfoCustSatisList.Add(CustSatis);
                }

                if (InfoCustSatisList.Count == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }

                return View(InfoCustSatisList);

            }
            else
            {
                var result = dbr.tbl_Kepuasan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == Convert.ToInt32(EstateList));

                foreach (var info in result)
                {
                    var getLdgInfo = db.tbl_Ladang
                        .Where(x => x.fld_ID == info.fld_ID)
                        .Select(s => s.fld_LdgName).FirstOrDefault();

                    var getUserInfo = db.tblUsers
                        .Where(x => x.fldUserID == info.fld_UserID)
                                    .Select(s => s.fldUserFullName).FirstOrDefault();

                    CustMod_CustSatisfaction CustSatis = new CustMod_CustSatisfaction();

                    CustSatis.UID = info.fld_UserID;
                    CustSatis.UIDNama = getUserInfo;
                    CustSatis.LdgID = info.fld_LadangID;
                    CustSatis.LdgNama = getLdgInfo;
                    CustSatis.Satis = info.fld_Kepuasan;
                    CustSatis.Note = info.fld_Catatan;
                    InfoCustSatisList.Add(CustSatis);
                }

                if (InfoCustSatisList.Count == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }

                return View(InfoCustSatisList);
            }


        }

        public ActionResult PrintPaySlipPdf(int? RadioGroup, int? MonthList, int? YearList,
                string StatusList, string SelectionList, int id, string genid)
        {
            int? getuserid = 0;
            string getusername = "";
            string getcookiesval = "";
            bool checkidentity = false;
            //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
            var getuser = db.tblUsers.Where(u => u.fldUserID == id && u.fldDeleted == false).SingleOrDefault();
            if (getuser != null)
            {
                getuserid = GetIdentity.ID(getuser.fldUserName);
                getusername = getuser.fldUserName;
            }

            checkidentity = CheckGenIdentity(id, genid, getuserid, getusername, out getcookiesval);

            ActionAsPdf report = new ActionAsPdf("");

            if (checkidentity)
            {
                getBackAuth(getcookiesval);
                var cookies = Request.Cookies.AllKeys.ToDictionary(k => k, k => Request.Cookies[k].Value);
                //geterror.testlog("UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name, "UserName : " + User.Identity.Name);
                string print = "Yes";
                report = new ActionAsPdf("_PaySlipRptSearch", new { RadioGroup, MonthList, YearList, StatusList, SelectionList, print })
                {
                    FormsAuthenticationCookieName = FormsAuthentication.FormsCookieName,
                    Cookies = cookies
                };
            }
            else
            {
                report = new ActionAsPdf("PDFInvalid");
            }

            return report;
        }

        public ActionResult WorkerContribution()
        {
            ViewBag.Report = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            List<SelectListItem> SelectionList = new List<SelectListItem>();
            SelectionList = new SelectList(
                dbr.tbl_Pkjmast
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1")
                    .OrderBy(o => o.fld_Nama)
                    .Select(s => new SelectListItem { Value = s.fld_Nopkj, Text = s.fld_Nopkj + "-" + s.fld_Nama }),
                "Value", "Text").ToList();
            SelectionList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.SelectionList = SelectionList;

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


            List<SelectListItem> JnsPkjList = new List<SelectListItem>();

            JnsPkjList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkjList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.MonthList = monthList;
            ViewBag.JnsPkjList = JnsPkjList;

            return View();
        }

        public ViewResult _WorkerContribution(int? MonthList, int? YearList, string SelectionList, string JnsPkjList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<Models.tbl_GajiBulanan> tbl_GajiBulananList = new List<Models.tbl_GajiBulanan>();
            List<Models.tbl_Insentif> tbl_InsentifList = new List<Models.tbl_Insentif>();
            List<Models.tbl_Pkjmast> tbl_PkjmastList = new List<Models.tbl_Pkjmast>();
            List<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahanList = new List<tbl_ByrCarumanTambahan>();
            List<ContributionReport> ContributionReportList = new List<ContributionReport>();
            decimal? TotalInsentifEfected = 0;
            decimal? TotalSalaryForKWSP = 0;
            decimal? TotalSalaryForPerkeso = 0;
            decimal? BakiCutiTahunan = 0;
            decimal? KWSPEmplyee = 0;
            decimal? KWSPEmplyer = 0;
            decimal? SocsoEmplyee = 0;
            decimal? SocsoEmplyer = 0;
            decimal? SIPEmplyee = 0;
            decimal? SIPEmplyer = 0;
            decimal? SBKPEmplyee = 0;
            decimal? SBKPEmplyer = 0;
            int ID = 1;
            string WorkerName = "";
            string WorkerIDNo = "";

            var GetInsetifEffectCode = db.tbl_JenisInsentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_JenisInsentif == "P" && x.fld_AdaCaruman == true && x.fld_Deleted == false).Select(s => s.fld_KodInsentif).ToList();
            //var GetAddContributionDetails = db.tbl_SubCarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).ToList();
            if (SelectionList == "0")
            {
                tbl_PkjmastList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").ToList();

                if (JnsPkjList == "0")
                {
                    var GetNoPkjas = tbl_PkjmastList.Select(s => s.fld_Nopkj).ToList();
                    tbl_GajiBulananList = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                    tbl_InsentifList = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                }
                else
                {
                    var GetNoPkjas = tbl_PkjmastList.Where(x => x.fld_Jenispekerja == JnsPkjList).Select(s => s.fld_Nopkj).ToList();
                    tbl_GajiBulananList = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                    tbl_InsentifList = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && GetNoPkjas.Contains(x.fld_Nopkj) && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                }
            }
            else
            {
                tbl_PkjmastList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1" && x.fld_Nopkj == SelectionList).ToList();
                tbl_GajiBulananList = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
                tbl_InsentifList = dbr.tbl_Insentif.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionList && GetInsetifEffectCode.Contains(x.fld_KodInsentif) && x.fld_Deleted == false && x.fld_Month == MonthList && x.fld_Year == YearList).ToList();
            }

            var GetGajiBulananID = tbl_GajiBulananList.Select(s => s.fld_ID).ToList();
            tbl_ByrCarumanTambahanList = dbr.tbl_ByrCarumanTambahan.Where(x => GetGajiBulananID.Contains(x.fld_GajiID.Value)).ToList();

            foreach (var GajiBulananDetail in tbl_GajiBulananList)
            {
                KWSPEmplyee = 0;
                KWSPEmplyer = 0;
                SocsoEmplyee = 0;
                SocsoEmplyer = 0;
                SIPEmplyee = 0;
                SIPEmplyer = 0;
                SBKPEmplyee = 0;
                SBKPEmplyer = 0;

                TotalInsentifEfected = tbl_InsentifList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Sum(s => s.fld_NilaiInsentif);
                TotalInsentifEfected = TotalInsentifEfected == null ? 0 : TotalInsentifEfected;

                //added by faeza 04.01.2024
                BakiCutiTahunan = GajiBulananDetail.fld_BakiCutiTahunan == null ? 0 : GajiBulananDetail.fld_BakiCutiTahunan;
                TotalSalaryForKWSP = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_BonusHarian + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + BakiCutiTahunan/*GajiBulananDetail.fld_BakiCutiTahunan*/; //fatin modified - 26/02/2024
                TotalSalaryForPerkeso = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_BonusHarian + GajiBulananDetail.fld_OT + TotalInsentifEfected + GajiBulananDetail.fld_AIPS + BakiCutiTahunan; //modified faeza 02.04.2024 - add bonus harian
                //commented by faeza 04.01.2024
                //TotalSalaryForKWSP = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_BonusHarian + TotalInsentifEfected + GajiBulananDetail.fld_AIPS;
                //TotalSalaryForPerkeso = GajiBulananDetail.fld_ByrKerja + GajiBulananDetail.fld_ByrCuti + GajiBulananDetail.fld_OT + TotalInsentifEfected + GajiBulananDetail.fld_AIPS;

                KWSPEmplyee = GajiBulananDetail.fld_KWSPPkj;
                KWSPEmplyer = GajiBulananDetail.fld_KWSPMjk;

                SocsoEmplyee = GajiBulananDetail.fld_SocsoPkj;
                SocsoEmplyer = GajiBulananDetail.fld_SocsoMjk;

                var GetAddContribution = tbl_ByrCarumanTambahanList.Where(x => x.fld_GajiID == GajiBulananDetail.fld_ID).FirstOrDefault();

                if (GetAddContribution != null)
                {
                    if (GetAddContribution.fld_KodCaruman == "SIP")
                    {
                        SIPEmplyee = GetAddContribution.fld_CarumanPekerja;
                        SIPEmplyer = GetAddContribution.fld_CarumanMajikan;
                    }
                    else
                    {
                        SBKPEmplyee = GetAddContribution.fld_CarumanPekerja;
                        SBKPEmplyer = GetAddContribution.fld_CarumanMajikan;
                    }
                }

                WorkerName = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Nama).FirstOrDefault();
                WorkerIDNo = tbl_PkjmastList.Where(x => x.fld_Nopkj == GajiBulananDetail.fld_Nopkj).Select(s => s.fld_Nokp).FirstOrDefault();

                if (SBKPEmplyer != 0 || SocsoEmplyer != 0 || KWSPEmplyer != 0)
                {
                    ContributionReportList.Add(new ContributionReport() { ID = ID, WorkerName = WorkerName, TotalSalaryForKwsp = TotalSalaryForKWSP.Value, TotalSalaryForPerkeso = TotalSalaryForPerkeso.Value, KwspContributionEmplyee = KWSPEmplyee.Value, KwspContributionEmplyer = KWSPEmplyer.Value, SipContributionEmplyee = SIPEmplyee.Value, SipContributionEmplyer = SIPEmplyer.Value, SocsoContributionEmplyee = SocsoEmplyee.Value, SocsoContributionEmplyer = SocsoEmplyer.Value, SbkpContributionEmplyee = SBKPEmplyee.Value, SbkpContributionEmplyer = SBKPEmplyer.Value, WorkerNo = GajiBulananDetail.fld_Nopkj, WorkerIDNo = WorkerIDNo });
                    ID++;
                }
            }

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.YearSelection = YearList;
            ViewBag.MonthSelection = MonthList;

            return View(ContributionReportList);
        }

        //public ActionResult TansListingRpt()
        //{
        //    ViewBag.Report = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.YearList = yearlist;
        //    ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);
        //    return View();
        //}

        //public ActionResult TansListingRptDetail(int? MonthList, int? YearList)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (MonthList != null && YearList != null)
        //    {
        //        var result = dbsp.sp_RptTransListing(NegaraID, SyarikatID, WilayahID, LadangID, MonthList, YearList).ToList();
        //        return View(result);
        //    }
        //    else
        //    {
        //        ViewBag.Message = GlobalResEstate.msgNoRecord;
        //        return View();
        //    }
        //}
    }
}