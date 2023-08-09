using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.Entity;
//farahin tambah - 08032021
using MVC_SYSTEM.AuthModels;
using iTextSharp.tool.xml;
//end here - farahin_08032021

namespace MVC_SYSTEM.Controllers
{
    public class TableInfoController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetConfig GetConfig = new GetConfig();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private ChangeTimeZone timezone = new ChangeTimeZone();

        //farahin tambah - 08032021
        private MVC_SYSTEM_Models dbModels = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        //end here  farahin_08032021   

        // GET: TableInfo
        public ActionResult Index()
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            int?[] reportid = new int?[] { };

            List<SelectListItem> sublist = new List<SelectListItem>();
            ViewBag.TableInfoSubList = sublist;
            ViewBag.TableInfo = "class = active";

            ViewBag.TableInfoList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "jadualUpah" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_Desc }), "Value", "Text").ToList();

            return View();
        }

        public JsonResult GetSubList(int ListID)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var findsub = db.tblMenuLists.Where(x => x.fld_ID == ListID).Select(s => s.fld_Sub).FirstOrDefault();
            List<SelectListItem> sublist = new List<SelectListItem>();
            if (findsub != null)
            {
                sublist = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == findsub && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }
            db.Dispose();
            return Json(sublist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string TableInfoList, string TableInfoSubList)
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            if (TableInfoSubList != null)
            {
                return RedirectToAction(TableInfoSubList, "TableInfo");
            }
            else
            {
                int maintenancelist = int.Parse(TableInfoList);
                var action = db.tblMenuLists.Where(x => x.fld_ID == maintenancelist && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Val).FirstOrDefault();
                db.Dispose();
                return RedirectToAction(action, "TableInfo");
            }
        }

        public ActionResult Payrate()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult PayrateDetail()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.tbl_UpahMenuai.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_HasilLower);
            return View(resultreport);
        }

        public ActionResult PayrateFull()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult PayrateFullDetails()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.vw_JadualUpah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_KodAktvt);
            return View(resultreport);
        }

        public ActionResult YieldBracket()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> JnsPkt = new List<SelectListItem>();
            JnsPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            ViewBag.JnsPktList = JnsPkt;
            return View();
        }

        // mas amend pd 4/6/2020
        // tukar parameter string JnsPktList --> int? JnsPktList
        // tambah parameter string print
        public ActionResult YieldBracketDetails(int? JnsPktList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            ViewBag.TableInfo = "class = active";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime blnstrt = DateTime.Now.AddMonths(-12);
            DateTime blnlst = DateTime.Now.AddMonths(-1);

            DateTime Today = DateTime.Now;
            int[] blockYield = new int[] { 2, 4, 6, 8, 10, 12 };
            var isBlockYield = blockYield.Contains(Today.Month);

            if (isBlockYield)
            {
                blnstrt = DateTime.Now.AddMonths(-13);
                blnlst = DateTime.Now.AddMonths(-2);
            }

            //* mas tambah ViewBag.Print=print - 4/6/2020;
            ViewBag.Print = print;
            //*
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            //* mas comment - 4/6/2020;
            //var result = dbsp.sp_YieldBracketTable(NegaraID, SyarikatID, WilayahID, LadangID, int.Parse(JnsPktList)).ToList();
            //* mas amend - 4/6/2020;
            var result = dbsp.sp_YieldBracketTable(NegaraID, SyarikatID, WilayahID, LadangID, JnsPktList).ToList();
            //*
            result = result.Where(x => x.fldBulan >= blnstrt.Month && x.fldTahun == blnstrt.Year).Union(result.Where(x => x.fldBulan <= blnlst.Month && x.fldTahun == blnlst.Year)).ToList();


            decimal purataHasil = 0;
            decimal jumlahPurata = 0;
            foreach (var hasil in result)
            {
                purataHasil = (hasil.fldHasilTan.Value / hasil.fldLuasHektar.Value);
                jumlahPurata = jumlahPurata + Math.Round(purataHasil, 2);
            }
            ViewBag.JumlahPurata = Math.Round(jumlahPurata, 2);
            return PartialView(result);
        }

        //public ActionResult HasilSawit()
        //{
        //    ChangeTimeZone timezone = new ChangeTimeZone();
        //    int month = timezone.gettimezone().Month;
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    ViewBag.TableInfo = "class = active";

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> BulanList = new List<SelectListItem>();
        //    //List<SelectListItem> TahunList = new List<SelectListItem>();
        //    List<SelectListItem> KumpList = new List<SelectListItem>();

        //    BulanList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1== "monthlist" && x.fldDeleted==false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc}), "Value", "Text",month).ToList();
        //    BulanList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    //TahunList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist").OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text").ToList();
        //    //TahunList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    int drpyear = 0;
        //    int drprangeyear = 0;
        //    //ChangeTimeZone timezone = new ChangeTimeZone();
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

        //    KumpList = new SelectList(dbr.tbl_HasilSawit.Where(x => x.fld_bulan == 9).OrderBy(o => o.fld_kum).Select(s => new SelectListItem { Value = s.fld_kum, Text = s.fld_kum }), "Value", "Text").Distinct().ToList();
        //    KumpList.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
        //    //int blnselected = BulanList.FirstOrDefault();
        //    var resultreport = dbr.tbl_HasilSawit.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_kdldg == "399" && x.fld_bulan== month && x.fld_tahun== drprangeyear);
        //    //KumpList = new SelectList(dbr.tbl_YieldBracket.Where(x => x.fld_bulan == 9).GroupBy(g => g.fld_kum), "fld_kum", "fld_kum").ToList();
        //    //KumpList.Insert(0, (new SelectListItem { Text = "Semua", Value = "ALL" }));
        //    ViewBag.BulanList = BulanList;
        //    ViewBag.TahunList = yearlist;
        //    ViewBag.KumpulanList = KumpList;
        //    return View("HasilSawit", resultreport);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult HasilSawit(int BulanList, int yearlist, string KumpList)
        //{
        //    ChangeTimeZone timezone = new ChangeTimeZone();
        //    int month = timezone.gettimezone().AddMonths(-1).Month;
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    List<SelectListItem> BulanList2 = new List<SelectListItem>();
        //    List<SelectListItem> KumpList2 = new List<SelectListItem>();

        //    BulanList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", month).ToList();
        //    BulanList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

        //    drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
        //    drprangeyear = timezone.gettimezone().Year;
        //    var yearlist2 = new List<SelectListItem>();
        //    for (var i = drpyear; i <= drprangeyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {

        //            yearlist2.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    KumpList2 = new SelectList(dbr.tbl_HasilSawit.Where(x => x.fld_bulan == 9).OrderBy(o => o.fld_kum).Select(s => new SelectListItem { Value = s.fld_kum, Text = s.fld_kum }), "Value", "Text").Distinct().ToList();
        //    KumpList2.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
        //    var resultreport = dbr.tbl_HasilSawit.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_kdldg == "399" && x.fld_bulan == BulanList && x.fld_tahun == yearlist && x.fld_kum== KumpList);
        //    ViewBag.BulanList = BulanList2;
        //    ViewBag.TahunList = yearlist2;
        //    ViewBag.KumpulanList = KumpList2;
        //    return View("HasilSawit", resultreport);
        //}

        public ActionResult WorkerAktvt(string Search)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            ViewBag.TableInfo = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            if (Search != "" && Search != null)
            {
                var resultreport = dbr.tbl_AktvtKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_Lejar.Contains(Search) || x.fld_Aktvt.Contains(Search)));
                return View("WorkerAktvt", resultreport);
            }
            else
            {
                var resultreport = dbr.tbl_AktvtKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID);
                return View("WorkerAktvt", resultreport);
            }
        }

        public ActionResult OilPrice()
        {
            ViewBag.TableInfo = "class = active";
            ChangeTimeZone timezone = new ChangeTimeZone();
            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;
            int year = timezone.gettimezone().Year;
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> BulanList = new List<SelectListItem>();
            BulanList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", month).ToList();
            BulanList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

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
            ViewBag.Bulan = month;
            ViewBag.Tahun = year;
            ViewBag.BulanList = BulanList;
            ViewBag.TahunList = yearlist;
            return View();
        }

        public ActionResult OilPriceDetail(string Bulan, int Tahun)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            int bulan = int.Parse(Bulan);
            var resultreport = db.tbl_HargaSawitSemasa.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Bulan == bulan && x.fld_Tahun == Tahun && x.fld_Deleted == false).ToList();
            ViewBag.Bulan = bulan;
            ViewBag.Tahun = Tahun;
            return View(resultreport);
        }

        public ActionResult KwspSocso()
        {
            return View();
        }

        public ActionResult Socso()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult SocsoDetail()
        {
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            var resultreport = db.vw_Socso.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KdrLower);
            return View(resultreport);
        }

        public ActionResult Kwsp()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> KwspList2 = new List<SelectListItem>();
            KwspList2 = new SelectList(db.tbl_JenisCaruman.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fld_JenisCaruman == "KWSP" && x.fld_Deleted == false).OrderBy(o => o.fld_KodCaruman).Select(s => new SelectListItem { Value = s.fld_KodCaruman.ToString(), Text = s.fld_Keterangan }), "Value", "Text").ToList();

            ViewBag.KwspList = KwspList2;
            return View();
        }
        public ActionResult KwspDetail(string KwspList = "K01")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var resultreport = db.tbl_Kwsp.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_KodCaruman == KwspList);
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            ViewBag.JnsKwsp = db.tbl_JenisCaruman.Where(x => x.fld_KodCaruman == KwspList && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
            return View(resultreport);
        }

        public ActionResult Productivity(string Search)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.TableInfo = "class = active";


            if (Search != "" && Search != null)
            {
                var resultreport = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && (x.fld_Desc.Contains(Search) || x.fld_KodAktvt.Contains(Search))).OrderBy(o => o.fld_Kategori);
                return View("Productivity", resultreport);
            }
            else
            {
                var resultreport = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_Kategori);
                return View("Productivity", resultreport);
            }
        }

        public ActionResult AllocatedPublicHolidayTable()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

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
            return View();
        }

        public ActionResult _AllocatedPublicHolidayTable(short? YearList, int page = 1, string sort = "fld_TarikhCuti", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);


            var message = "";
            if (String.IsNullOrEmpty(YearList.ToString()))
            {
                message = GlobalResEstate.msgChooseAllocatedPublicHoliday;
                ViewBag.Message = message;
            }

            else
            {
                message = GlobalResEstate.msgErrorSearch;
                ViewBag.Message = message;
            }
            MVC_SYSTEM_Viewing dbview = new MVC_SYSTEM_Viewing();

            var kodNegeriLadang = Convert.ToInt32(GetConfig.GetLadangNegeriFromID(LadangID));
            var publiHolidayData = dbview.vw_CutiUmumLdgDetails.Where(x =>
                x.fld_Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false &&
                x.fld_CutiUmumDeleted == false);
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            return View(publiHolidayData);
        }

        public ActionResult PayrateGMN()
        {
            ViewBag.TableInfo = "class = active";
            GetIdentity GetIdentity = new GetIdentity();

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            List<SelectListItem> KategoriList = new List<SelectListItem>();
            KategoriList = new SelectList(db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .OrderBy(o => o.fld_ID)
                .Select(s => new SelectListItem { Value = s.fld_KodKategori.ToString(), Text = s.fld_KodKategori + " - " + s.fld_Kategori }), "Value", "Text").ToList();
            KategoriList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "" }));

            ViewBag.SelectionList = KategoriList;
            //ViewBag.Select = "";
            return View();
        }

        public ActionResult PayrateGMNDetails(string Search, string Kategori)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            if (Kategori != "" && Kategori != null)
            {
                if (Search != "" && Search != null)
                {
                    var resultreport = db.vw_UpahGMN.Where(x => x.fld_KategoriAktvt == Kategori && (x.fld_JnsAktvt.Contains(Search) && x.fld_Desc.Contains(Search) || x.fld_KodAktvt.Contains(Search)) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
                else
                {
                    var resultreport = db.vw_UpahGMN.Where(x => x.fld_KategoriAktvt == Kategori && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
            }
            else
            {
                if (Search != "" && Search != null)
                {
                    var resultreport = db.vw_UpahGMN.Where(x => (x.fld_JnsAktvt.Contains(Search) && x.fld_Desc.Contains(Search) || x.fld_KodAktvt.Contains(Search)) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
                else
                {
                    var resultreport = db.vw_UpahGMN.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Distinct();
                    return View(resultreport);
                }
            }
        }

        public ActionResult AddedContribution()
        {
            ViewBag.TableInfo = "class = active";
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> ContributionList = new List<SelectListItem>();
            List<SelectListItem> SubContributionList = new List<SelectListItem>();
            ContributionList = new SelectList(db.tbl_CarumanTambahan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_JenisCarumanID).Select(s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_NamaCaruman }), "Value", "Text").ToList();
            ViewBag.ContList = ContributionList;
            ViewBag.SubContList = SubContributionList;
            return View();
        }

        public ActionResult AddedContributionDetail(string subcont)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var resultreport = db.tbl_JadualCarumanTambahan.Where(x => x.fld_KodSubCaruman == subcont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);
            ViewBag.NamaCaruman = db.tbl_SubCarumanTambahan.Where(x => x.fld_KodSubCaruman == subcont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KeteranganSubCaruman).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();
            return View(resultreport);



        }

        public JsonResult GetSubContribution(string cont)
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> SubContList = new List<SelectListItem>();
            SubContList = new SelectList(db.tbl_SubCarumanTambahan.Where(x => x.fld_KodCaruman == cont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_JenisSubCarumanID).Select(s => new SelectListItem { Value = s.fld_KodSubCaruman, Text = s.fld_KeteranganSubCaruman }), "Value", "Text").ToList();

            string subcont = db.tbl_SubCarumanTambahan.Where(x => x.fld_KodCaruman == cont && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodSubCaruman).FirstOrDefault();
            return Json(new { SubContList = SubContList, subcont = subcont });
        }

        public ActionResult AllocatedYearlyHolidayTable()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            return View();
        }

        public ActionResult _AllocatedYearlyHolidayTable(int page = 1, string sort = "fld_TarikhCuti", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var annualLeaveData = db.tbl_CutiMaintenance.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
                x.fld_JenisCuti == "C02");

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat).FirstOrDefault();

            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            return View(annualLeaveData);
        }

        public ActionResult AllocatedSickHolidayTable()
        {
            ViewBag.TableInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            return View();
        }

        public ActionResult _AllocatedSickHolidayTable(int page = 1, string sort = "fld_TarikhCuti", string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var sickLeaveData = db.tbl_CutiMaintenance.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false &&
                x.fld_JenisCuti == "C03");

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat).FirstOrDefault();

            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            return View(sickLeaveData);
        }

        //farahin tambah function convertpdf2 - 08032021
        [HttpPost]
        public ActionResult ConvertPDF2(string myHtml, string filename, string reportname)
        {
            bool success = false;
            string msg = "";
            string status = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
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

        //farahin tambah 08032021
        public ActionResult GetPDF(int id)
        {
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
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

        //fatin added - 29/05/2023
        public ActionResult PayrateRubber()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult PayrateRubberDetail()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.tbl_UpahGetah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_HasilLower);
            return View(resultreport);
        }

        public ActionResult HrgGetahSMR()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult HrgGetahSMRDetail()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.tbl_HargaGetahSMR.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_HargaGetah_Lower);
            return View(resultreport);
        }

        public ActionResult ProduktivitiGetah()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult ProduktivitiGetahDetail()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.tbl_ProduktivitiGetah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_PrdktvtCuplumpLower);
            return View(resultreport);
        }

        public ActionResult InsentifKGK()
        {
            ViewBag.TableInfo = "class = active";
            return View();
        }

        public ActionResult InsentifKGKDetail()
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            var resultreport = db.tbl_InsentifKGK.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KGKLower);
            return View(resultreport);
        }
        //end
    }
}