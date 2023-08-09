using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ClosingTransactionController : Controller
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
        // GET: ClosingTransaction
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

            ViewBag.ClosingTransaction = "class = active";

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

            List<SelectListItem> CloseOpen = new List<SelectListItem>();
            CloseOpen.Insert(0, (new SelectListItem { Text = "Tutup Urus Niaga", Value = "true" }));
            if (getidentity.HQAuth(User.Identity.Name))
            {
                CloseOpen.Insert(1, (new SelectListItem { Text = "Buka Urus Niaga", Value = "false" }));
            }
            
            ViewBag.CloseOpen = CloseOpen;

            //ViewBag.ProcessList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "gensalary" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc");

            dbr.Dispose();
            return View();
        }

        [HttpPost]
        public ActionResult Index(int Month, int Year, bool CloseOpen)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            int? AuditTrailStatus = 0;

            ViewBag.ClosingTransaction = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string monthstring = Month.ToString();
            if (monthstring.Length == 1)
            {
                monthstring = "0" + monthstring;
            }
            var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year).FirstOrDefault();
            var CheckScTransSalary = dbr.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodAktvt == "4000").Select(s => s.fld_Amt).FirstOrDefault();
            var CheckSkbReg = dbr.tbl_Skb.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == monthstring && x.fld_Tahun == Year).FirstOrDefault();
            if (ClosingTransaction != null)
            {
                if (CheckSkbReg.fld_NoSkb != null)
                {
                    if (CheckSkbReg.fld_GajiBersih == CheckScTransSalary)
                    {
                        if (ClosingTransaction.fld_Credit == ClosingTransaction.fld_Debit)
                        {
                            if (CloseOpen == true && ClosingTransaction.fld_StsTtpUrsNiaga == true)
                            {
                                msg = GlobalResEstate.msgUrusNiagaClose;
                                statusmsg = "warning";
                            }
                            else
                            {
                                AuditTrailStatus = CloseOpen == true ? 1 : 0;
                                ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
                                ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
                                ClosingTransaction.fld_ModifiedBy = getuserid;
                                dbr.Entry(ClosingTransaction).State = EntityState.Modified;
                                dbr.SaveChanges();
                                UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, AuditTrailStatus);

                             //  FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
                                msg = GlobalResEstate.msgUpdate;
                                statusmsg = "success";
                            }
                            
                        }
                        else
                        {
                            msg = GlobalResEstate.msgBalance;
                            statusmsg = "warning";
                        }
                    }
                    else
                    {
                        msg = GlobalResEstate.msgNoSKBClose;
                        statusmsg = "warning";
                    }
                    
                }
                else
                {
                    msg = GlobalResEstate.msgNoSKBReg;
                    statusmsg = "warning";
                }
            }
            else
            {
                msg = GlobalResEstate.msgGenSalary;
                statusmsg = "warning";
            }

            dbr.Dispose();
            return Json(new { msg , statusmsg });
        }

        [HttpPost]
        public ActionResult PostingSapNCloseTransaction(int Month, int Year, bool CloseOpen)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            int? AuditTrailStatus = 0;

            ViewBag.ClosingTransaction = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string monthstring = Month.ToString();
            if (monthstring.Length == 1)
            {
                monthstring = "0" + monthstring;
            }
            var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year).FirstOrDefault();
            var CheckScTransSalary = dbr.tbl_Sctran.Where(x => x.fld_Month == Month && x.fld_Year == Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodAktvt == "4000").Select(s => s.fld_Amt).FirstOrDefault();
            var CheckSkbReg = dbr.tbl_Skb.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == monthstring && x.fld_Tahun == Year).FirstOrDefault();
            if (ClosingTransaction != null)
            {
                if (CheckSkbReg.fld_NoSkb != null)
                {
                    if (CheckSkbReg.fld_GajiBersih == CheckScTransSalary)
                    {
                        if (ClosingTransaction.fld_Credit == ClosingTransaction.fld_Debit)
                        {
                            if (CloseOpen == true && ClosingTransaction.fld_StsTtpUrsNiaga == true)
                            {
                                msg = "Urus niaga telah ditutup";
                                statusmsg = "warning";
                            }
                            else
                            {
                                AuditTrailStatus = CloseOpen == true ? 1 : 0;
                                ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
                                ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
                                ClosingTransaction.fld_ModifiedBy = getuserid;
                                dbr.Entry(ClosingTransaction).State = EntityState.Modified;
                                dbr.SaveChanges();
                                UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, AuditTrailStatus);
                               
                            //    FinanceApplication(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, CloseOpen, CheckSkbReg.fld_GajiBersih, CheckSkbReg.fld_NoSkb, getuserid);
                                msg = GlobalResEstate.msgUpdate;
                                statusmsg = "success";
                            }

                        }
                        else
                        {
                            msg = GlobalResEstate.msgBalance;
                            statusmsg = "warning";
                        }
                    }
                    else
                    {
                        msg = "Sila pastikan nilai pemohonan sama seperti didaftar di No SKB sebelum urusniaga ditutup";
                        statusmsg = "warning";
                    }

                }
                else
                {
                    msg = "Sila daftar No SKB sebelum urusniaga ditutup";
                    statusmsg = "warning";
                }
            }
            else
            {
                msg = GlobalResEstate.msgGenSalary;
                statusmsg = "warning";
            }

            dbr.Dispose();
            return Json(new { msg, statusmsg });
        }

        public ActionResult AuditTrail()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            //List<SelectListItem> SelectionData = new List<SelectListItem>();
            
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

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            var GetAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Thn == year).FirstOrDefault();

            ViewBag.YearList = yearlist;
            ViewBag.Tahun = year;
            return View("AuditTrail", GetAuditTrail);
        }

        [HttpPost]
        public ActionResult AuditTrail(int YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            DateTime Minus1month = timezone.gettimezone().AddMonths(-1);
            int year = Minus1month.Year;
            int month = Minus1month.Month;
            int drpyear = 0;
            int drprangeyear = 0;
            //List<SelectListItem> SelectionData = new List<SelectListItem>();

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

            ViewBag.NamaSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NamaSyarikat)
                .FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();

            var GetAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Thn == YearList).FirstOrDefault();

            ViewBag.YearList = yearlist;
            ViewBag.Tahun = YearList;
            return View("AuditTrail", GetAuditTrail);
        }

        public void UpdateAuditTrail(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Year, int? Month, int? UpdateData)
        {
            var checkAuditTrail = db.tbl_AuditTrail.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Thn == Year).FirstOrDefault();
            switch (Month)
            {
                case 1:
                    checkAuditTrail.fld_Bln1 = UpdateData;
                    break;
                case 2:
                    checkAuditTrail.fld_Bln2 = UpdateData;
                    break;
                case 3:
                    checkAuditTrail.fld_Bln3 = UpdateData;
                    break;
                case 4:
                    checkAuditTrail.fld_Bln4 = UpdateData;
                    break;
                case 5:
                    checkAuditTrail.fld_Bln5 = UpdateData;
                    break;
                case 6:
                    checkAuditTrail.fld_Bln6 = UpdateData;
                    break;
                case 7:
                    checkAuditTrail.fld_Bln7 = UpdateData;
                    break;
                case 8:
                    checkAuditTrail.fld_Bln8 = UpdateData;
                    break;
                case 9:
                    checkAuditTrail.fld_Bln9 = UpdateData;
                    break;
                case 10:
                    checkAuditTrail.fld_Bln10 = UpdateData;
                    break;
                case 11:
                    checkAuditTrail.fld_Bln11 = UpdateData;
                    break;
                case 12:
                    checkAuditTrail.fld_Bln12 = UpdateData;
                    break;
            }

            db.Entry(checkAuditTrail).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void FinanceApplication(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, int? Year, int? Month, bool? UrusniagaStatus, decimal? Amount, string SkbNo, int? UserID)
        {
            var CheckPermohonanWang = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).FirstOrDefault();
            var GetLadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
            if (CheckPermohonanWang == null)
            {
                tbl_SokPermhnWang tbl_SokPermhnWang = new tbl_SokPermhnWang();
                tbl_SokPermhnWang.fld_SemakWil_Status = 0;
                tbl_SokPermhnWang.fld_SokongWilGM_Status = 0;
                tbl_SokPermhnWang.fld_TerimaHQ_Status = 0;
                tbl_SokPermhnWang.fld_TolakWil_Status = 0;
                tbl_SokPermhnWang.fld_TolakWilGM_Status = 0;
                tbl_SokPermhnWang.fld_TolakHQ_Status = 0;
                tbl_SokPermhnWang.fld_NoCIT = GetLadangDetail.fld_NoCIT;
                tbl_SokPermhnWang.fld_NoAcc = GetLadangDetail.fld_NoAcc;
                tbl_SokPermhnWang.fld_NoGL = GetLadangDetail.fld_NoGL;
                tbl_SokPermhnWang.fld_JumlahPermohonan = Amount;
                tbl_SokPermhnWang.fld_SkbNo = SkbNo;
                tbl_SokPermhnWang.fld_StsTtpUrsNiaga = true;
                tbl_SokPermhnWang.fld_NegaraID = NegaraID;
                tbl_SokPermhnWang.fld_SyarikatID = SyarikatID;
                tbl_SokPermhnWang.fld_WilayahID = WilayahID;
                tbl_SokPermhnWang.fld_LadangID = LadangID;
                tbl_SokPermhnWang.fld_Year = Year;
                tbl_SokPermhnWang.fld_Month = Month;
                db.tbl_SokPermhnWang.Add(tbl_SokPermhnWang);
                db.SaveChanges();
            }
            else
            {
                CheckPermohonanWang.fld_SemakWil_Status = 0;
                CheckPermohonanWang.fld_SokongWilGM_Status = 0;
                CheckPermohonanWang.fld_TerimaHQ_Status = 0;
                CheckPermohonanWang.fld_TolakWil_Status = 0;
                CheckPermohonanWang.fld_TolakWilGM_Status = 0;
                CheckPermohonanWang.fld_TolakHQ_Status = 0;
                CheckPermohonanWang.fld_NoCIT = GetLadangDetail.fld_NoCIT;
                CheckPermohonanWang.fld_NoAcc = GetLadangDetail.fld_NoAcc;
                CheckPermohonanWang.fld_NoGL = GetLadangDetail.fld_NoGL;
                CheckPermohonanWang.fld_JumlahPermohonan = Amount;
                CheckPermohonanWang.fld_SkbNo = SkbNo;
                CheckPermohonanWang.fld_StsTtpUrsNiaga = UrusniagaStatus;
                db.Entry(CheckPermohonanWang).State = EntityState.Modified;
                db.SaveChanges();

                if (UrusniagaStatus == false)
                {
                    tblSokPermhnWangHisAction tblSokPermhnWangHisAction = new tblSokPermhnWangHisAction();
                    tblSokPermhnWangHisAction.fldHisSPWID = CheckPermohonanWang.fld_ID;
                    tblSokPermhnWangHisAction.fldHisDesc = "Urus Niaga Dibuka Semula";
                    tblSokPermhnWangHisAction.fldHisUserID = UserID;
                    tblSokPermhnWangHisAction.fldHisAppLevel = 2;
                    tblSokPermhnWangHisAction.fldHisDT = timezone.gettimezone();
                    db.tblSokPermhnWangHisActions.Add(tblSokPermhnWangHisAction);
                    db.SaveChanges();
                }
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