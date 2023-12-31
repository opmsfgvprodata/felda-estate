﻿using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using System.Web.Mvc;
using static MVC_SYSTEM.Class.GlobalFunction;
using System.Data.SqlClient;
using Dapper;
using MVC_SYSTEM.ModelsDapper;

namespace MVC_SYSTEM.Controllers
{
    //Check_Balik
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class SalaryGeneratorController : Controller
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
        private CheckrollFunction EstateFunction = new CheckrollFunction();
        // GET: SalaryGenerator
        public ActionResult Index()
        {
            ViewBag.GenSalary = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ViewBag.GenList = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "genSalary" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fld_Val", "fld_Desc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string GenList)
        {
            return RedirectToAction(GenList, "SalaryGenerator");
        }

        public ActionResult SalaryGeneratorIndex()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            //DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            DateTime Minus1month = timezone.gettimezone(); //fatin modified - 15/12/2023
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;

            ViewBag.GenSalary = "class = active";

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
            ViewBag.LadangID = LadangID; //Ashahri - 27/02/2022

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", month);
            ViewBag.ProcessList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "gensalary" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc");

            dbr.Dispose();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalaryGeneratorIndex(string ProcessList, int MonthList, int YearList)
        {
            try
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                DateTime SelectedDate = new DateTime(YearList, MonthList, 1);

                bool success = false;
                string status = "";
                string msg = "";
                bool CutOfDateStatus = false;
                bool LastCloseBizStatus = false;

                CutOfDateStatus = EstateFunction.GetStatusCutGenProcess(dbr, SelectedDate, NegaraID, SyarikatID, WilayahID, LadangID, out LastCloseBizStatus);
                if (LastCloseBizStatus && !CutOfDateStatus)
                {
                    var servicesname = db.tbl_ServicesList.Where(x => x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).Select(s => s.fld_ServicesName).FirstOrDefault();

                    var deleteprocess = db.tbl_SevicesProcess.Where(x => x.fld_ServicesName == servicesname && x.fld_Flag == 0).FirstOrDefault();
                    if (deleteprocess != null)
                    {
                        db.tbl_SevicesProcess.Remove(deleteprocess);
                        db.SaveChanges();
                    }

                    var getexistingprocess = db.tbl_SevicesProcess.Where(x => x.fld_ServicesName == servicesname).FirstOrDefault();

                    MasterModels.tbl_SevicesProcessHistory SalaryHistoryModel = new MasterModels.tbl_SevicesProcessHistory();

                    MasterModels.tbl_SevicesProcessHistory salaryhistory = new MasterModels.tbl_SevicesProcessHistory();

                    if (getexistingprocess == null)
                    {

                        MasterModels.tbl_SevicesProcess tbl_SevicesProcess = new MasterModels.tbl_SevicesProcess();

                        tbl_SevicesProcess.fld_ProcessName = ProcessList;
                        tbl_SevicesProcess.fld_ServicesName = servicesname;
                        tbl_SevicesProcess.fld_Flag = 1;
                        tbl_SevicesProcess.fld_UserID = getidentity.ID(User.Identity.Name);
                        tbl_SevicesProcess.fld_DTProcess = timezone.gettimezone();
                        tbl_SevicesProcess.fld_Year = YearList;
                        tbl_SevicesProcess.fld_Month = MonthList;
                        tbl_SevicesProcess.fld_UplSelCat = 0;
                        tbl_SevicesProcess.fld_SelCatVal = 0;
                        tbl_SevicesProcess.fld_ClientID = 99;
                        tbl_SevicesProcess.fld_NegaraID = NegaraID;
                        tbl_SevicesProcess.fld_SyarikatID = SyarikatID;
                        tbl_SevicesProcess.fld_WilayahID = WilayahID;
                        tbl_SevicesProcess.fld_LadangID = LadangID;
                        tbl_SevicesProcess.fld_ProcessPercentage = 0;

                        PropertyCopy.Copy(SalaryHistoryModel, tbl_SevicesProcess);


                        PropertyCopy.Copy(salaryhistory, tbl_SevicesProcess);

                        db.tbl_SevicesProcess.Add(tbl_SevicesProcess);
                        db.tbl_SevicesProcessHistory.Add(SalaryHistoryModel);
                        db.tbl_SevicesProcessHistory.Add(salaryhistory);
                        db.SaveChanges();

                        StartService(servicesname, 100);

                        success = true;
                        status = "success";
                        msg = GlobalResEstate.msgWait;
                    }
                    else
                    {
                        msg = GlobalResEstate.msgWait;
                        status = "warning";
                        success = true;
                    }
                }
                else if (LastCloseBizStatus && CutOfDateStatus)
                {
                    success = true;
                    status = "warning";
                    msg = GlobalResEstate.msgGenerateError1;
                }
                else if (!LastCloseBizStatus && CutOfDateStatus)
                {
                    success = true;
                    status = "warning";
                    msg = GlobalResEstate.msgGenerateError2;
                }

                return Json(new { success, msg, status });
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message + " This from controller", ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger" });
            }

        }

        public ActionResult _StopProcess()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var GetServiceRunningData = db.tbl_SevicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Flag == 1).ToList();
            return View(GetServiceRunningData);
        }
        
        public JsonResult _StopProcessPost()
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            var GetServiceRunningData = db.tbl_SevicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Flag == 1).ToList();
            if (GetServiceRunningData.Count > 0)
            {
                GetServiceRunningData.ForEach(u => u.fld_Flag = 0);
                db.SaveChanges();
                msg = "Proses jana berjaya dihentikan";
                statusmsg = "success";
            }
            else
            {
                msg = "Proses jana telah dihentikan";
                statusmsg = "warning";
            }

            return Json(new { msg, statusmsg });
        }

        public ActionResult CheckrollReport()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host = "", catalog = "", user = "", pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            //List<SelectListItem> SelectionData = new List<SelectListItem>();

            var GetLastGenProcess = db.tbl_SevicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            int? yearcheck = 0;
            int? monthcheck = 0;

            yearcheck = GetLastGenProcess == null ? timezone.gettimezone().Year : GetLastGenProcess.fld_Year;
            monthcheck = GetLastGenProcess == null ? timezone.gettimezone().Month : GetLastGenProcess.fld_Month;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drpyear; i <= drprangeyear; i++)
            {
                if (i == yearcheck)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            ViewBag.MonthList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", monthcheck);

            var KumpulanList = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_KodKumpulan).ToList();

            var KumpulanName = KumpulanList.Select(s => s.fld_KodKumpulan).Take(1).FirstOrDefault();

            var GetKerjaInfoDetailsList = new List<KerjaInfoDetails_Result>();

            string constr = Connection.GetConnectionString(WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("KategoriPilih", 1);
                parameters.Add("PilihanCari", KumpulanName);
                parameters.Add("year", yearcheck);
                parameters.Add("Month", monthcheck);
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("WilayahID", WilayahID);
                parameters.Add("LadangID", LadangID);
                con.Open();
                SqlMapper.Settings.CommandTimeout = 300;
                GetKerjaInfoDetailsList = SqlMapper.Query<KerjaInfoDetails_Result>(con, "sp_KerjaInfoDetails_V2", parameters).ToList();
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }

            //var GetKerjaInfoDetailsList = dbsp.sp_KerjaInfoDetails(1, KumpulanName, yearcheck, monthcheck, NegaraID, SyarikatID, WilayahID, LadangID).ToList();

            var GetWorkerList = GetKerjaInfoDetailsList.Select(s => s.fld_Nopkj.Trim()).Distinct();

            ViewBag.GetWorkerList = GetWorkerList;
            ViewBag.SelectionData = new SelectList(KumpulanList.Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text");

            ViewBag.SelectionDataLabel = "Nama Kumpulan";

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            ViewBag.BulanTahun = GetTriager.GetMonthName(int.Parse(monthcheck.ToString())) + ", " + yearcheck;

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;

            dbsp.Dispose();
            dbr.Dispose();
            return View("CheckrollReport", GetKerjaInfoDetailsList);
        }

        [HttpPost]
        public ActionResult CheckrollReport(int MonthList, int YearList, string SelectionData, int SelectionCategory)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host = "", catalog = "", user = "", pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);
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

            var GetKerjaInfoDetailsList = new List<KerjaInfoDetails_Result>();

            string constr = Connection.GetConnectionString(WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            var con = new SqlConnection(constr);
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("KategoriPilih", SelectionCategory);
                parameters.Add("PilihanCari", SelectionData);
                parameters.Add("year", YearList);
                parameters.Add("Month", MonthList);
                parameters.Add("NegaraID", NegaraID);
                parameters.Add("SyarikatID", SyarikatID);
                parameters.Add("WilayahID", WilayahID);
                parameters.Add("LadangID", LadangID);
                con.Open();
                SqlMapper.Settings.CommandTimeout = 300;
                GetKerjaInfoDetailsList = SqlMapper.Query<KerjaInfoDetails_Result>(con, "sp_KerjaInfoDetails_V2", parameters).ToList();
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }

           // var GetKerjaInfoDetailsList = dbsp.sp_KerjaInfoDetails(SelectionCategory, SelectionData, YearList, MonthList, NegaraID, SyarikatID, WilayahID, LadangID).ToList();

            var GetWorkerList = GetKerjaInfoDetailsList.Select(s => s.fld_Nopkj.Trim()).Distinct();

            ViewBag.GetWorkerList = GetWorkerList;
            ViewBag.SelectionData = SelectionCategory == 1 ?
            new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text", SelectionData).ToList()
            :
            new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text", SelectionData).ToList();

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

            ViewBag.BulanTahun = GetTriager.GetMonthName(MonthList) + ", " + YearList;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            dbsp.Dispose();
            dbr.Dispose();
            return View("CheckrollReport", GetKerjaInfoDetailsList);
        }

        public JsonResult WorkerData(int SelectionCategory)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> SelectionData = new List<SelectListItem>();
            SelectionData = SelectionCategory == 1 ?
            new SelectList(dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).OrderBy(o => o.fld_Keterangan).Select(s => new SelectListItem { Value = s.fld_KodKumpulan, Text = s.fld_KodKumpulan + " - " + s.fld_Keterangan }), "Value", "Text").ToList()
            :
            new SelectList(dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1" && x.fld_KumpulanID != null).OrderBy(o => o.fld_Nama).Select(s => new SelectListItem { Value = s.fld_Nopkj.ToString(), Text = s.fld_Nopkj + " - " + s.fld_Nama }), "Value", "Text").ToList();

            SelectionData.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblAll, Value = "0" }));

            dbr.Dispose();
            return Json(SelectionData);
        }

        public void StartService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                ServiceController sc = new ServiceController(serviceName);
                if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message + " This from class", ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                //db2.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}