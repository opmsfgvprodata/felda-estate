using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.ViewingModels;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class MaybankFileGenController : Controller
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private GetIdentity getidentity = new GetIdentity();
        private GetTriager GetTriager = new GetTriager();
        private GetNSWL GetNSWL = new GetNSWL();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetConfig GetConfig = new GetConfig();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetWilayah GetWilayah = new GetWilayah();
        private Connection Connection = new Connection();
        private GetGenerateFile GetGenerateFile = new GetGenerateFile();
        // GET: MaybankFileGen
        public ActionResult Index()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);
            
            dbr.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult Index(int Month, int Year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string filePath = "";
            string filename = "";

            string stringyear = "";
            string stringmonth = "";
            string link = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);

            ViewBag.MaybankFileGen = "class = active";

            try
            {
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                //modified by faeza 28.03.2022
                var GetGaji = dbr.vw_MaybankFile.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && x.fld_PaymentMode == "4" && (x.fld_NoAkaun != null || x.fld_NoAkaun.Length == 12)).ToList();

                var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

                filePath = GetGenerateFile.GenFileMaybank(GetGaji, LadangDetail, stringmonth, stringyear, NegaraID, SyarikatID, WilayahID, LadangID, out filename);

                link = Url.Action("Download", "MaybankFileGen", new { filePath, filename });

                dbr.Dispose();

                msg = GlobalResEstate.msgGenerateSuccess;
                statusmsg = "success";
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgGenerateFailed;
                statusmsg = "warning";
            }
            
            return Json(new { msg, statusmsg, link });
        }

        public JsonResult CheckGenDataDetail(int Month, int Year)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            string stringyear = "";
            string stringmonth = "";
            stringyear = Year.ToString();
            stringmonth = Month.ToString();
            stringmonth = (stringmonth.Length == 1 ? "0" + stringmonth : stringmonth);
            decimal? TotalGaji = 0;
            int CountData = 0;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //mofified by faeza 28.03.2022
            var GetGaji = dbr.vw_MaybankFile.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month && x.fld_PaymentMode == "4" && (x.fld_NoAkaun != null || x.fld_NoAkaun.Length == 12)).ToList();

            var LadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();

            string filename = "MBBOPMS" + LadangDetail.fld_LdgCode + stringmonth + stringyear + ".txt";

            

            if(GetGaji.Count() != 0)
            {
                TotalGaji = GetGaji.Sum(s => s.fld_GajiBersih);
                CountData = GetGaji.Count();
                msg = GlobalResEstate.msgDataFound;
                statusmsg = "success";
            }
            else
            {
                msg = GlobalResEstate.msgDataNotFound;
                statusmsg = "warning";
            }
            

            dbr.Dispose();
            return Json(new { msg, statusmsg, file = filename, salary = TotalGaji, totaldata = CountData });
        }

        public FileResult Download(string filePath, string filename)
        {
            string path = HttpContext.Server.MapPath(filePath);

            DownloadFiles.FileDownloads objs = new DownloadFiles.FileDownloads();
            
            var filesCol = objs.GetFiles(path);
            var CurrentFileName = filesCol.Where(x => x.FileName == filename).FirstOrDefault();

            string contentType = string.Empty;
            contentType = "application/txt";
            return File(CurrentFileName.FilePath, contentType, CurrentFileName.FileName);

        }

        //added by faeza 28.03.2022
        public ActionResult CdmasGen()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.MaybankFileGen = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc", month);

            dbr.Dispose();
            return View();
        }

        public ViewResult _cdmas(int? MonthList, int? YearList, string print)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string LdgName = "";
            string LdgCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbview2 = new MVC_SYSTEM_Viewing();
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<Models.vw_MaybankFile> CdmasList = new List<Models.vw_MaybankFile>();

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
            var LdgDetail = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .FirstOrDefault();
            LdgName = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgName)
                .FirstOrDefault();
            LdgCode = db.tbl_Ladang
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_ID == LadangID)
                .Select(s => s.fld_LdgCode)
                .FirstOrDefault();
            ViewBag.Ladang = LdgName.Trim();
            ViewBag.LadangCode = LdgCode.Trim();
            ViewBag.OriginatorId = LdgDetail.fld_OriginatorID;
            ViewBag.OriginatorName = LdgDetail.fld_OriginatorName;
            ViewBag.AccNo = LdgDetail.fld_NoAcc;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();
            ViewBag.Print = print;
            ViewBag.Description = LdgCode + " - Salary payment for " + MonthList + "/" + YearList;
            if (MonthList == null && YearList == null)
            {
                ViewBag.Message = "Sila pilih bulan dan tahun";
                //ViewBag.Message = GlobalResEstate.msgChooseWork;
                //return View();
                return View(CdmasList);
            }
            else
            {
                var CdmasData = dbr.vw_MaybankFile
                    .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID &&
                                x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_PaymentMode == "4" &&
                                (x.fld_NoAkaun != null || x.fld_NoAkaun.Length == 12)).ToList();

                var BankList = db.tbl_Bank
                    .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                    .ToList();

                foreach (var cdmas in CdmasData)
                {
                    CdmasList.Add(
                        new Models.vw_MaybankFile()
                        {
                            fld_Nama = cdmas.fld_Nama,
                            fld_Nokp = cdmas.fld_Nokp,
                            fld_PaymentMode = cdmas.fld_PaymentMode,
                            fld_Kdbank = BankList.Where(x => x.fld_KodBank == cdmas.fld_Kdbank).Select(s => s.fld_NamaBank).FirstOrDefault(),
                            fld_NoAkaun = cdmas.fld_NoAkaun,
                            fld_GajiBersih = cdmas.fld_GajiBersih,
                            fld_Nopkj = cdmas.fld_Nopkj,
                            fld_Kdrkyt = cdmas.fld_Kdrkyt
                        });

                }

                ViewBag.RecordNo = CdmasList.Count();

                if (CdmasList.Count() == 0)
                {
                    ViewBag.Message = GlobalResEstate.msgNoRecord;
                }
                return View(CdmasList);
            }
        }
    }
}