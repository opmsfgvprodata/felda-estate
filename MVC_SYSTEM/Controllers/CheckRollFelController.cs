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
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Security;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Dynamic;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class CheckRollFelController : Controller
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
        private GetLadang GetLadang = new GetLadang();
        private Connection Connection = new Connection();
        private CheckrollFunction EstateFunction = new CheckrollFunction();
        private GetConfig getConfig = new GetConfig();

        //public ActionResult Index()
        //{
        //    ViewBag.CheckRoll = "class = active";
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    ViewBag.CheckRollMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "dataEntry" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fld_Val", "fld_Desc");
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(string CheckRollMenu)
        //{
        //    return RedirectToAction(CheckRollMenu, "CheckRoll");
        //}

        public ActionResult WorkingDetails()
        {
            DateTime? date = timezone.gettimezone();
            DateTime Today = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            string host, catalog, user, pass = "";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            double ValidDayApp = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string[] FlagDataWebConfig = new string[] { "blokdatakerja", "blokdatakerjavlddt", "blokdatakerjattpursniaga" };

            List<SelectListItem> SelectionData = new List<SelectListItem>();
            SelectionData.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblSelection, Value = "0" }));
            ViewBag.SelectionData = SelectionData;
            ViewBag.CheckRoll = "class = active";
            var tblOptionConfigsWebs = db.tblOptionConfigsWebs.Where(x => FlagDataWebConfig.Contains(x.fldOptConfFlag1) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();
            var CheckBlockKeyInDay = tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "blokdatakerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            var CheckBlockValidDayApp = tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "blokdatakerjavlddt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            var CheckBlockClseBiz = tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "blokdatakerjattpursniaga" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();

            //added by faeza 16.08.2022 - change tbl_Kerjahdr to tbl_Kerja AND add .OrderByDescending(o => o.fld_Tarikh)
            var CheckLastDataKeyIn = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Year == date.Value.Year && x.fld_Tarikh.Value.Month == date.Value.Month).OrderByDescending(o => o.fld_Tarikh).Select(s => s.fld_Tarikh).FirstOrDefault();
            ViewBag.EstateSapCode = GetLadang.GetLadangCostCenter(LadangID);
            ////fitri 06-08-2020
            //var CheckLastDataKeyIn = dbr.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Year == date.Value.Year && x.fld_Tarikh.Value.Month == date.Value.Month).Select(s => s.fld_Tarikh).FirstOrDefault();
            //faeza 29.12.2020
            //var CheckLastDataKeyIn = dbr.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Year == date.Value.Year && x.fld_Tarikh.Value.Month == date.Value.Month).OrderByDescending(o => o.fld_Tarikh).Select(s => s.fld_Tarikh).FirstOrDefault();
            //original
            //var CheckLastDataKeyIn = dbr.tbl_Kerjahdr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderByDescending(o => o.fld_Tarikh).Select(s => s.fld_Tarikh).Take(1).FirstOrDefault();

            ValidDayApp = double.Parse(CheckBlockValidDayApp) - 1;

            if (CheckLastDataKeyIn == null)
            {
                DateTime? LastDate = new DateTime(Today.Year, Today.Month, 1);
                CheckLastDataKeyIn = LastDate;
            }

            double TotalDayLastKeyInDbl = (Today - CheckLastDataKeyIn).Value.TotalDays;
            short TotalDayLastKeyIn = Convert.ToInt16(TotalDayLastKeyInDbl);
            short TotalDayLastNeedKeyIn = short.Parse(CheckBlockKeyInDay);
            double ApprovalDayCount = 0;
            var CheckBlockStatus = db.tbl_BlckKmskknDataKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == date.Value.Year && x.fld_Month == date.Value.Month).FirstOrDefault();
            var GetClosingDataBlock = EstateFunction.GetCloseTransaction(dbr, NegaraID.Value, SyarikatID.Value, WilayahID.Value, LadangID.Value, int.Parse(CheckBlockClseBiz), date.Value);

            var getPaidLeaveCodes = db.tbl_CutiKategori.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => new { leaveCode = s.fld_KodCuti }).ToList();
            var paidLeaveCodes = JsonConvert.SerializeObject(getPaidLeaveCodes);
            ViewBag.PaidLeaveCodes = paidLeaveCodes;
            var getEstateLevels = dbr.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => new { value = s.fld_IOcode, text = s.fld_IOcode }).ToList();
            var estateLevels = JsonConvert.SerializeObject(getEstateLevels);
            ViewBag.EstateLevels = estateLevels;

            if (TotalDayLastKeyIn >= TotalDayLastNeedKeyIn && CheckBlockStatus != null && GetClosingDataBlock)
            {
                if (CheckBlockStatus.fld_BlokStatus == true)
                {
                    dbr.Dispose();
                    return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                }
                else
                {
                    ApprovalDayCount = (Today - CheckBlockStatus.fld_ValidDT).Value.TotalDays;

                    if (ApprovalDayCount > ValidDayApp)
                    {
                        CheckBlockStatus.fld_BlokStatus = true;
                        db.Entry(CheckBlockStatus).State = EntityState.Modified;
                        db.SaveChanges();
                        dbr.Dispose();
                        return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                    }
                    else
                    {
                        dbr.Dispose();
                        return View();
                    }
                }
            }
            else if (TotalDayLastKeyIn >= TotalDayLastNeedKeyIn && CheckBlockStatus == null)
            {
                tbl_BlckKmskknDataKerja tbl_BlckKmskknDataKerja = new tbl_BlckKmskknDataKerja();
                tbl_BlckKmskknDataKerja.fld_BlokStatus = true;
                tbl_BlckKmskknDataKerja.fld_BilHariXKyIn = TotalDayLastKeyIn;
                tbl_BlckKmskknDataKerja.fld_Month = date.Value.Month;
                tbl_BlckKmskknDataKerja.fld_Year = date.Value.Year;
                tbl_BlckKmskknDataKerja.fld_LadangID = LadangID;
                tbl_BlckKmskknDataKerja.fld_WilayahID = WilayahID;
                tbl_BlckKmskknDataKerja.fld_SyarikatID = SyarikatID;
                tbl_BlckKmskknDataKerja.fld_NegaraID = NegaraID;
                tbl_BlckKmskknDataKerja.fld_Reason = "";
                db.tbl_BlckKmskknDataKerja.Add(tbl_BlckKmskknDataKerja);
                db.SaveChanges();
                dbr.Dispose();
                return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
            }
            /*else if (TotalDayLastKeyIn <= TotalDayLastNeedKeyIn && CheckBlockStatus != null) //fitri comment 06-08-2020
            {
                if (CheckBlockStatus.fld_BlokStatus == true)
                {
                    ApprovalDayCount = (Today - CheckBlockStatus.fld_ValidDT).Value.TotalDays;
                    if (ApprovalDayCount > ValidDayApp)
                    {
                        CheckBlockStatus.fld_BlokStatus = true;
                        db.Entry(CheckBlockStatus).State = EntityState.Modified;
                        db.SaveChanges();
                        dbr.Dispose();
                        return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                    }
                    else
                    {
                        dbr.Dispose();
                        return View();
                    }
                }
                else
                {
                    dbr.Dispose();
                    return View();
                }
            }*/
            else if (TotalDayLastKeyIn <= TotalDayLastNeedKeyIn && CheckBlockStatus != null) //fitri add 06-08-2020
            {
                if (CheckBlockStatus.fld_BlokStatus == true)
                {
                    dbr.Dispose();
                    return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                }
                else
                {
                    ApprovalDayCount = (Today - CheckBlockStatus.fld_ValidDT).Value.TotalDays;
                    if (ApprovalDayCount > ValidDayApp)
                    {
                        CheckBlockStatus.fld_BlokStatus = true;
                        db.Entry(CheckBlockStatus).State = EntityState.Modified;
                        db.SaveChanges();
                        dbr.Dispose();
                        return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
                    }
                    else
                    {
                        dbr.Dispose();
                        return View();
                    }
                }
            }
            else if (!GetClosingDataBlock)
            {
                if (CheckBlockStatus == null)
                {
                    tbl_BlckKmskknDataKerja tbl_BlckKmskknDataKerja = new tbl_BlckKmskknDataKerja();
                    tbl_BlckKmskknDataKerja.fld_BlokStatus = true;
                    tbl_BlckKmskknDataKerja.fld_BilHariXKyIn = TotalDayLastKeyIn;
                    tbl_BlckKmskknDataKerja.fld_Month = date.Value.Month;
                    tbl_BlckKmskknDataKerja.fld_Year = date.Value.Year;
                    tbl_BlckKmskknDataKerja.fld_LadangID = LadangID;
                    tbl_BlckKmskknDataKerja.fld_WilayahID = WilayahID;
                    tbl_BlckKmskknDataKerja.fld_SyarikatID = SyarikatID;
                    tbl_BlckKmskknDataKerja.fld_NegaraID = NegaraID;
                    tbl_BlckKmskknDataKerja.fld_Reason = "";
                    db.tbl_BlckKmskknDataKerja.Add(tbl_BlckKmskknDataKerja);
                    db.SaveChanges();
                    dbr.Dispose();
                }
                else
                {
                    CheckBlockStatus.fld_BlokStatus = true;
                    db.Entry(CheckBlockStatus).State = EntityState.Modified;
                    db.SaveChanges();
                    dbr.Dispose();
                }
                return RedirectToAction("CheckRollBlock", "CheckRollFel", new { msg = 1 });
            }
            else
            {
                dbr.Dispose();
                return View();
            }
        }

        public ActionResult CheckRollBlock(int msg)
        {
            string Message = "";
            ViewBag.CheckRoll = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var CheckBlockKeyInDay = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "blokdatakerja" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();
            switch (msg)
            {
                case 1:
                    Message = GlobalResEstate.msgChekrollBlock;
                    break;
                case 2:
                    Message = GlobalResEstate.msgCheckrollBlockDay + CheckBlockKeyInDay + GlobalResEstate.msgCheckrollBlockDay1;
                    break;
            }

            ViewBag.Message = Message;

            return View();
        }

        public ActionResult Attendance()
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WorkCode = new List<SelectListItem>();
            List<SelectListItem> Rainning = new List<SelectListItem>();

            WorkCode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "cuti" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text", "H01").ToList();
            Rainning.Insert(0, (new SelectListItem { Text = "Tidak", Value = "0", Selected = true }));
            Rainning.Insert(0, (new SelectListItem { Text = "Ya", Value = "1" }));


            ViewBag.Rainning = Rainning;
            ViewBag.WorkCode = WorkCode;
            return View();
        }

        [HttpPost]
        public ActionResult Attendance(CustMod_Attandance CustMod_Attandance)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string msg2 = "";
            string statusmsg2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string KumpulanKod = "";
            bool ZeroLeaveBal = false;
            bool InvalidCutiAm = false;
            bool AlertPopup = false;
            bool LeaveSelection = false;
            DateTime date = timezone.gettimezone();
            string bodyview2 = "";
            bool CutOfDateStatus = false;
            string Msg = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<tbl_Kerjahdr> tbl_Kerjahdrs = new List<tbl_Kerjahdr>();
            List<tbl_Kerjahdr> returntbl_Kerjahdr = new List<tbl_Kerjahdr>();
            List<CustMod_WorkIdList> CustMod_WorkIdLists = new List<CustMod_WorkIdList>();
            GlobalFunction GlobalFunction = new GlobalFunction();
            int? LadangNegeriCode = 0;

            try
            {
                LadangNegeriCode = int.Parse(GetLadang.GetLadangNegeriCode(LadangID));
                if (EstateFunction.GetCutiAmMgguMatchDate(NegaraID, SyarikatID, WilayahID, LadangID, CustMod_Attandance.dateseleted, CustMod_Attandance.WorkCode, out Msg))
                {
                    CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, CustMod_Attandance.dateseleted, NegaraID, SyarikatID, WilayahID, LadangID);
                    if (!CutOfDateStatus)
                    {
                        LeaveSelection = EstateFunction.CheckLeaveType(CustMod_Attandance.WorkCode, NegaraID, SyarikatID) ? true : false;
                        if (CustMod_Attandance.SelectionCategory == 1)
                        {
                            KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == CustMod_Attandance.SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                            var pkjids = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj.Trim()).ToList();
                            var datainkrjhdrs = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == CustMod_Attandance.SelectionData && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                            if (datainkrjhdrs.Count() == 0)
                            {
                                foreach (var pkjid in pkjids)
                                {
                                    if (LeaveSelection)
                                    {
                                        if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                        {
                                            tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B", fld_SAPChargeCode = CustMod_Attandance.SAPChargeCode });
                                            EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                        }
                                        else
                                        {
                                            msg2 = msg2 + " ," + pkjid;
                                            statusmsg2 = GlobalResEstate.msgPerhatian;
                                            ZeroLeaveBal = true;
                                        }
                                    }
                                    else
                                    {
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B", fld_SAPChargeCode = CustMod_Attandance.SAPChargeCode });
                                    }
                                }

                                disablesavebtn = true;
                            }
                            else
                            {
                                if (CustMod_Attandance.atteditstatus == 1)
                                {
                                    List<string> needtoadds = pkjids.Except(datainkrjhdrs.Select(s => s.fld_Nopkj.Trim())).ToList();
                                    if (LeaveSelection)
                                    {
                                        foreach (var pkjid in needtoadds)
                                        {
                                            if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                            {
                                                tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B", fld_SAPChargeCode = CustMod_Attandance.SAPChargeCode });
                                                EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, pkjid, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                            }
                                            else
                                            {
                                                msg2 = msg2 + " ," + pkjid;
                                                statusmsg2 = GlobalResEstate.msgPerhatian;
                                                ZeroLeaveBal = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var pkjid in needtoadds)
                                        {
                                            tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_Attandance.SelectionData, fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B", fld_SAPChargeCode = CustMod_Attandance.SAPChargeCode });
                                        }
                                    }
                                    msg = GlobalResEstate.msgUpdate;
                                    statusmsg = "success";
                                    disablesavebtn = true;
                                    if (needtoadds.Count() == 0)
                                    {
                                        msg2 = GlobalResEstate.msgAddAttendance;
                                        statusmsg2 = GlobalResEstate.msgPerhatian;
                                        AlertPopup = true;
                                    }
                                }
                            }
                        }
                        else
                        {

                            var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == CustMod_Attandance.SelectionData && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

                            if (datainkrjhdr == null)
                            {
                                if (LeaveSelection)
                                {
                                    if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, CustMod_Attandance.SelectionData, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                    {
                                        var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == CustMod_Attandance.SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                        KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Kum = KumpulanKod.Trim(), fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B", fld_SAPChargeCode = CustMod_Attandance.SAPChargeCode });
                                        disablesavebtn = true;
                                        EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, CustMod_Attandance.SelectionData, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                    }
                                    else
                                    {
                                        msg2 = msg2 + " ," + CustMod_Attandance.SelectionData;
                                        statusmsg2 = GlobalResEstate.msgPerhatian;
                                        ZeroLeaveBal = true;
                                    }
                                }
                                else
                                {
                                    var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == CustMod_Attandance.SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                    KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                    tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Kum = KumpulanKod.Trim(), fld_Tarikh = CustMod_Attandance.dateseleted, fld_Kdhdct = CustMod_Attandance.WorkCode, fld_Hujan = CustMod_Attandance.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B", fld_SAPChargeCode = CustMod_Attandance.SAPChargeCode });
                                }
                            }
                            else
                            {
                                if (CustMod_Attandance.atteditstatus == 1)
                                {
                                    if (LeaveSelection)
                                    {
                                        if (EstateFunction.IndividuCheckLeaveTake(datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID))
                                        {
                                            EstateFunction.LeaveAdd(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID, WilayahID, LadangID);

                                            if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                            {
                                                EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                            }
                                            else
                                            {
                                                msg2 = msg2 + " ," + datainkrjhdr.fld_Nopkj;
                                                statusmsg2 = GlobalResEstate.msgPerhatian;
                                                ZeroLeaveBal = true;
                                            }
                                        }
                                        else
                                        {
                                            if (EstateFunction.LeaveCalBal(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID))
                                            {
                                                EstateFunction.LeaveDeduct(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, CustMod_Attandance.WorkCode, NegaraID, SyarikatID, WilayahID, LadangID);
                                            }
                                            else
                                            {
                                                msg2 = msg2 + " ," + datainkrjhdr.fld_Nopkj;
                                                statusmsg2 = GlobalResEstate.msgPerhatian;
                                                ZeroLeaveBal = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (EstateFunction.IndividuCheckLeaveTake(datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID))
                                        {
                                            EstateFunction.LeaveAdd(dbr, CustMod_Attandance.dateseleted.Year, datainkrjhdr.fld_Nopkj, datainkrjhdr.fld_Kdhdct, NegaraID, SyarikatID, WilayahID, LadangID);
                                        }
                                    }

                                    if (!ZeroLeaveBal)
                                    {
                                        bool DeleteDataKerja = false;
                                        if (datainkrjhdr.fld_Kdhdct != CustMod_Attandance.WorkCode && datainkrjhdr.fld_Hujan != CustMod_Attandance.Rainning)
                                        {
                                            DeleteDataKerja = true;
                                        }
                                        else if (datainkrjhdr.fld_Kdhdct != CustMod_Attandance.WorkCode && datainkrjhdr.fld_Hujan == CustMod_Attandance.Rainning)
                                        {
                                            DeleteDataKerja = true;
                                        }
                                        else if (datainkrjhdr.fld_Kdhdct == CustMod_Attandance.WorkCode && datainkrjhdr.fld_Hujan != CustMod_Attandance.Rainning)
                                        {
                                            DeleteDataKerja = true;
                                        }

                                        if (DeleteDataKerja)
                                        {
                                            var GetKerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == datainkrjhdr.fld_Tarikh && x.fld_Nopkj == datainkrjhdr.fld_Nopkj).ToList();
                                            if (GetKerja.Count > 0)
                                            {
                                                dbr.tbl_Kerja.RemoveRange(GetKerja);
                                                GlobalFunction.CreateAuditTrail(dbr, "D", "Attendance Data (Update Attendance)", getuserid.Value, GetKerja, GetKerja, NegaraID, SyarikatID, WilayahID, LadangID);
                                            }
                                        }

                                        datainkrjhdr.fld_Hujan = CustMod_Attandance.Rainning;
                                        datainkrjhdr.fld_Kdhdct = CustMod_Attandance.WorkCode;
                                        datainkrjhdr.fld_CreatedBy = getuserid;
                                        datainkrjhdr.fld_CreatedDT = date;
                                        dbr.SaveChanges();

                                        msg = GlobalResEstate.msgUpdate;
                                        statusmsg = "success";
                                        disablesavebtn = true;
                                    }
                                }
                            }
                        }

                        if (tbl_Kerjahdrs.Count() != 0)
                        {
                            msg = GlobalResEstate.msgAdd;
                            statusmsg = "success";
                            dbr.tbl_Kerjahdr.AddRange(tbl_Kerjahdrs);
                            dbr.SaveChanges();
                        }

                        if (ZeroLeaveBal)
                        {
                            msg2 = GlobalResEstate.msgAttendanceLeave;
                            msg = msg2;
                            statusmsg2 = GlobalResEstate.msgPerhatian;
                            statusmsg = "warning";
                        }
                    }
                    else
                    {
                        msg = GlobalResEstate.msgError;
                        statusmsg = "warning";
                        disablesavebtn = true;
                    }
                }
                else
                {
                    msg = Msg;
                    statusmsg2 = GlobalResEstate.msgPerhatian;
                    statusmsg = "warning";
                }

                List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();
                List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
                if (CustMod_Attandance.SelectionCategory == 1)
                {
                    var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj.Trim()).ToList();
                    tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == CustMod_Attandance.SelectionData && datainpkjmast.Contains(x.fld_Nopkj) && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                }
                else
                {
                    tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == CustMod_Attandance.SelectionData && x.fld_Tarikh == CustMod_Attandance.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                }

                foreach (var tbl_KerjaData in tbl_KerjaList)
                {
                    var namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                    //Commented by Shazana 9/10/2023
                    //CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
                    //Added by Shazana 9/10/2023 
                    //Commented by Shazana 23/11/2023
                    //decimal? fld_LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                    //CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount, fld_LsPktUtama = fld_LsPktUtama });

                    //Added by Shazana 23/11/2023 -Kalau kong baru kira keluasan
                    decimal? fld_LsPktUtama = 0;
                    fld_LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                    if (fld_LsPktUtama == null)
                    {
                        fld_LsPktUtama = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                    }
                    if (fld_LsPktUtama == null) { fld_LsPktUtama = 0; }
                    CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount, fld_LsPktUtama = fld_LsPktUtama });

                }

                bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, CutOfDateStatus);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgError;
                statusmsg = "warning";
                disablesavebtn = true;
            }
            dbr.Dispose();
            return Json(new { proceedstatus = disablesavebtn, statusmsg, msg, ZeroLeaveBal, statusmsg2, msg2, AlertPopup, tablelisting2 = bodyview2 });
        }

        public ActionResult _NeglectedDay()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string InitialLadang = ""; //added by faeza 28.02.2021
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> JenisChargeHT = new List<SelectListItem>();
            List<SelectListItem> JnisPktHT = new List<SelectListItem>();
            List<SelectListItem> PilihanPktHT = new List<SelectListItem>();
            List<SelectListItem> PilihanAktvtHT = new List<SelectListItem>();
            List<SelectListItem> PilihanMasaHT = new List<SelectListItem>();

            //added by faeza 28.02.2021
            //LadangSelectionID = GetNSWL.GetEstateSelection(getuserid);
            var GetLadangSelectionName = GetNSWL.GetLadangDetail2(LadangID);
            InitialLadang = GetLadangSelectionName.fld_NamaLadang.Substring(0, 3);
            //end faeza

            var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();

            JnisPktHT = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PilihanPktHT = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            //commented by faeza 28.02.2021
            //PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            //PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //added by faeza 28.02.2021

            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);
            //if (InitialLadang == "FTP")
            //{
            //Modified by Shazana 9/11/2023
            PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            //PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && getJenisActvtDetails.Contains(x.fld_KodJenisAktvt) && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();

            PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //}
            //else
            //{
            //    PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_KategoriAktvt != "00").OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            //    PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //}
            //end faeza

            JenisChargeHT.Add(new SelectListItem { Text = "Kong", Value = "kong", Selected = true });
            JenisChargeHT.Add(new SelectListItem { Text = "Kadaran", Value = "kadaran", Selected = false });

            PilihanMasaHT.Add(new SelectListItem { Text = "Sepenuh Hari", Value = "penuh", Selected = true });
            PilihanMasaHT.Add(new SelectListItem { Text = "Separuh Hari", Value = "separuh", Selected = false });

            ViewBag.JnisPktHT = JnisPktHT;
            ViewBag.PilihanPktHT = PilihanPktHT;
            ViewBag.PilihanAktvtHT = PilihanAktvtHT;
            ViewBag.JenisChargeHT = JenisChargeHT;
            ViewBag.PilihanMasaHT = PilihanMasaHT;
            dbr.Dispose();

            return View();
        }

        [HttpPost]
        public ActionResult _NeglectedDay(CustMod_HariTerabai CustMod_HariTerabai)
        {
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string msg2 = "";
            string statusmsg2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string KumpulanKod = "";
            bool ZeroLeaveBal = false;
            bool AlertPopup = false;
            bool AuthStatus = false;
            DateTime date = timezone.gettimezone();
            string bodyview2 = "";
            bool CutOfDateStatus = false;
            string GLCode = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<tbl_Kerjahdr> tbl_Kerjahdrs = new List<tbl_Kerjahdr>();
            List<tbl_Kerja> tbl_Kerjas = new List<tbl_Kerja>();
            List<CustMod_WorkIdList> CustMod_WorkIdLists = new List<CustMod_WorkIdList>();
            List<tbl_KerjaHariTerabai> tbl_KerjaHariTerabais = new List<tbl_KerjaHariTerabai>();
            tbl_JenisAktiviti GetJenisActvtDetails = new tbl_JenisAktiviti();
            tbl_UpahAktiviti GetActvty = new tbl_UpahAktiviti();

            string idpengurus = CustMod_HariTerabai.ManagerID;
            string passpengurus = CustMod_HariTerabai.ManagerPassword;
            decimal? Amount = 0;
            decimal? Hasil = 0;
            decimal? GetKadarUpah = 0;
            decimal? GetKadarUpah2 = 0;
            string JenisCharge = "";
            string MasaKerja = "";
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);

            if (string.IsNullOrEmpty(idpengurus) == false && string.IsNullOrEmpty(passpengurus) == false)
            {
                if (CustMod_HariTerabai.JenisChargeHT == "kong")
                {
                    GetActvty = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == CustMod_HariTerabai.PilihanAktvtHT && x.fld_compcode == estateCostCenter).FirstOrDefault();
                    //Modified line by kamalia 23/8/2021
                    var getgajiminima = db.tbl_GajiMinimaLdg.Where(x => x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                    GetKadarUpah = getgajiminima != null ? getgajiminima.fld_NilaiGajiMinima : GetActvty.fld_Harga;
                    //
                    GetJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();
                    JenisCharge = CustMod_HariTerabai.JenisChargeHT;
                    MasaKerja = CustMod_HariTerabai.PilihanMasaHT;
                }
                else
                {
                    JenisCharge = CustMod_HariTerabai.JenisChargeHT;
                    MasaKerja = "-";
                }

                passpengurus = Encrypt.Encrypt(passpengurus);
                var pengurus = db.tblUsers.Where(x => x.fldUserName == idpengurus && x.fldUserPassword == passpengurus && x.fldDeleted == false && x.fldRoleID <= 6 && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).SingleOrDefault();
                if (pengurus != null)
                {
                    try
                    {
                        if (estateCostCenter == "1000")
                        {
                            if (EstateFunction.CheckSAPGLMap(dbr, CustMod_HariTerabai.JnisPktHT, CustMod_HariTerabai.PilihanPktHT, CustMod_HariTerabai.PilihanAktvtHT, NegaraID, SyarikatID, WilayahID, LadangID, true, CustMod_HariTerabai.JenisChargeHT, out GLCode, 0))
                            {
                                CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, CustMod_HariTerabai.dateseleted, NegaraID, SyarikatID, WilayahID, LadangID);
                                if (!CutOfDateStatus)
                                {
                                    if (CustMod_HariTerabai.SelectionCategory == 1)
                                    {
                                        KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == CustMod_HariTerabai.SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                        var pkjids = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj).ToList();
                                        var datainkrjhdrs = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                                        if (datainkrjhdrs.Count() == 0)
                                        {
                                            foreach (var pkjid in pkjids)
                                            {
                                                tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_HariTerabai.SelectionData, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Kdhdct = CustMod_HariTerabai.WorkCode, fld_Hujan = CustMod_HariTerabai.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                                if (CustMod_HariTerabai.JenisChargeHT == "kong")
                                                {
                                                    var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == pkjid && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                                                    if (SalaryIncrement != null)
                                                    {
                                                        GetKadarUpah2 = SalaryIncrement;
                                                    }
                                                    else
                                                    {
                                                        GetKadarUpah2 = 0;
                                                    }

                                                    if (CustMod_HariTerabai.PilihanMasaHT == "separuh")
                                                    {
                                                        Amount = (GetKadarUpah + GetKadarUpah2) / 2;
                                                        Hasil = 0.5m;
                                                    }
                                                    else
                                                    {
                                                        Amount = (GetKadarUpah + GetKadarUpah2);
                                                        Hasil = 1;
                                                    }

                                                    tbl_Kerjas.Add(new tbl_Kerja() { fld_Amount = Amount, fld_Bonus = 0, fld_BrtGth = 0, fld_CreatedBy = getuserid, fld_CreatedDT = timezone.gettimezone(), fld_DataSource = "B", fld_JamOT = 0, fld_JnisAktvt = GetActvty.fld_KodJenisAktvt, fld_JnsPkt = CustMod_HariTerabai.JnisPktHT, fld_JumlahHasil = Hasil, fld_KadarByr = GetKadarUpah, fld_KdhMenuai = "-", fld_KodAktvt = CustMod_HariTerabai.PilihanAktvtHT, fld_KodGL = "602", fld_KodPkt = CustMod_HariTerabai.PilihanPktHT, fld_Kong = 0, fld_Kum = CustMod_HariTerabai.SelectionData, fld_LadangID = LadangID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_Nopkj = pkjid, fld_PerBrshGth = 0, fld_Quality = 0, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Unit = GetActvty.fld_Unit, fld_HrgaKwsnSkar = 0, fld_KodKwsnSkar = "-", fld_OverallAmount = Amount });
                                                }
                                                tbl_KerjaHariTerabais.Add(new tbl_KerjaHariTerabai() { fld_Nopkj = pkjid, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_JenisCharge = JenisCharge, fld_MasaKerja = MasaKerja, fld_ApprovedBy = pengurus.fldUserID, fld_ApprovedDT = timezone.gettimezone(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                                            }
                                            disablesavebtn = true;
                                        }
                                        else
                                        {
                                            msg = GlobalResEstate.msgDeleteAttnd;
                                            statusmsg = "warning";
                                            disablesavebtn = true;
                                        }
                                    }
                                    else
                                    {
                                        var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

                                        if (datainkrjhdr == null)
                                        {
                                            var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == CustMod_HariTerabai.SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID }).FirstOrDefault();
                                            KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                            tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Kum = KumpulanKod.Trim(), fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Kdhdct = CustMod_HariTerabai.WorkCode, fld_Hujan = CustMod_HariTerabai.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                            if (CustMod_HariTerabai.JenisChargeHT == "kong")
                                            {
                                                var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == pkjdata.fld_Nopkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                                                if (SalaryIncrement != null)
                                                {
                                                    GetKadarUpah2 = SalaryIncrement;
                                                }
                                                else
                                                {
                                                    GetKadarUpah2 = 0;
                                                }

                                                if (CustMod_HariTerabai.PilihanMasaHT == "separuh")
                                                {
                                                    Amount = (GetKadarUpah + GetKadarUpah2) / 2;
                                                    Hasil = 0.5m;
                                                }
                                                else
                                                {
                                                    Amount = (GetKadarUpah + GetKadarUpah2);
                                                    Hasil = 1;
                                                }

                                                tbl_Kerjas.Add(new tbl_Kerja() { fld_Amount = Amount, fld_Bonus = 0, fld_BrtGth = 0, fld_CreatedBy = getuserid, fld_CreatedDT = timezone.gettimezone(), fld_DataSource = "B", fld_JamOT = 0, fld_JnisAktvt = GetActvty.fld_KodJenisAktvt, fld_JnsPkt = CustMod_HariTerabai.JnisPktHT, fld_JumlahHasil = Hasil, fld_KadarByr = GetKadarUpah, fld_KdhMenuai = "-", fld_KodAktvt = CustMod_HariTerabai.PilihanAktvtHT, fld_KodGL = "602", fld_KodPkt = CustMod_HariTerabai.PilihanPktHT, fld_Kong = 0, fld_Kum = KumpulanKod, fld_LadangID = LadangID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_Nopkj = pkjdata.fld_Nopkj, fld_PerBrshGth = 0, fld_Quality = 0, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Unit = GetActvty.fld_Unit, fld_HrgaKwsnSkar = 0, fld_KodKwsnSkar = "-", fld_OverallAmount = Amount });
                                            }

                                            tbl_KerjaHariTerabais.Add(new tbl_KerjaHariTerabai() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_JenisCharge = JenisCharge, fld_MasaKerja = MasaKerja, fld_ApprovedBy = pengurus.fldUserID, fld_ApprovedDT = timezone.gettimezone(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                                        }
                                        else
                                        {
                                            msg = GlobalResEstate.msgDeleteAttnd;
                                            statusmsg = "warning";
                                            disablesavebtn = true;
                                        }
                                    }

                                    if (tbl_Kerjahdrs.Count() != 0)
                                    {
                                        msg = GlobalResEstate.msgAdd;
                                        statusmsg = "success";
                                        dbr.tbl_Kerjahdr.AddRange(tbl_Kerjahdrs);

                                        if (tbl_Kerjas.Count() > 0)
                                        {
                                            dbr.tbl_Kerja.AddRange(tbl_Kerjas);
                                        }
                                        dbr.tbl_KerjaHariTerabai.AddRange(tbl_KerjaHariTerabais);
                                        dbr.SaveChanges();
                                        EstateFunction.SaveDataKerjaSAP(dbr, dbr, tbl_Kerjas, NegaraID, SyarikatID, WilayahID, LadangID, GLCode, false, "", 0);
                                    }
                                }
                                else
                                {
                                    msg = GlobalResEstate.msgError;
                                    statusmsg = "warning";
                                    disablesavebtn = true;
                                }
                            }
                            else
                            {
                                msg = GlobalResEstate.msgKodGLnotFound;
                                statusmsg = "warning";
                            }
                        }
                        else
                        {
                            CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, CustMod_HariTerabai.dateseleted, NegaraID, SyarikatID, WilayahID, LadangID);
                            if (!CutOfDateStatus)
                            {
                                var tbl_MapGL = db.tbl_MapGL.Where(x => x.fld_SyarikatID == 1 && (x.fld_Paysheet == "PT" || x.fld_Paysheet == "PA") && x.fld_Deleted == false).ToList();
                                List<CustMod_Work> HadirData = new List<CustMod_Work>();
                                if (CustMod_HariTerabai.SelectionCategory == 1)
                                {
                                    KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == CustMod_HariTerabai.SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                                    var tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
                                    var pkjids = tbl_Pkjmast.Select(s => s.fld_Nopkj).ToList();
                                    var datainkrjhdrs = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                                    if (datainkrjhdrs.Count() == 0)
                                    {
                                        foreach (var pkjid in pkjids)
                                        {

                                            GLCode = "";
                                            var paysheetID = "";
                                            var checkatt = tbl_Pkjmast.Where(x => x.fld_Nopkj == pkjid).FirstOrDefault();
                                            if (checkatt.fld_Kdrkyt == "MA")
                                            {
                                                GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == CustMod_HariTerabai.PilihanAktvtHT && x.fld_Paysheet == "PT").Select(s => s.fld_KodGL).FirstOrDefault();
                                                paysheetID = "PT";
                                            }
                                            else
                                            {
                                                GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == CustMod_HariTerabai.PilihanAktvtHT && x.fld_Paysheet == "PA").Select(s => s.fld_KodGL).FirstOrDefault();
                                                paysheetID = "PA";
                                            }

                                            HadirData.Add(new CustMod_Work { nopkj = pkjid, GLCode = GLCode, PaysheetID = paysheetID });

                                            tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjid, fld_Kum = CustMod_HariTerabai.SelectionData, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Kdhdct = CustMod_HariTerabai.WorkCode, fld_Hujan = CustMod_HariTerabai.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                            if (CustMod_HariTerabai.JenisChargeHT == "kong")
                                            {
                                                var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == pkjid && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                                                if (SalaryIncrement != null)
                                                {
                                                    GetKadarUpah2 = SalaryIncrement;
                                                }
                                                else
                                                {
                                                    GetKadarUpah2 = 0;
                                                }

                                                if (CustMod_HariTerabai.PilihanMasaHT == "separuh")
                                                {
                                                    Amount = (GetKadarUpah + GetKadarUpah2) / 2;
                                                    Hasil = 0.5m;
                                                }
                                                else
                                                {
                                                    Amount = (GetKadarUpah + GetKadarUpah2);
                                                    Hasil = 1;
                                                }

                                                tbl_Kerjas.Add(new tbl_Kerja() { fld_Amount = Amount, fld_Bonus = 0, fld_BrtGth = 0, fld_CreatedBy = getuserid, fld_CreatedDT = timezone.gettimezone(), fld_DataSource = "B", fld_JamOT = 0, fld_JnisAktvt = GetActvty.fld_KodJenisAktvt, fld_JnsPkt = CustMod_HariTerabai.JnisPktHT, fld_JumlahHasil = Hasil, fld_KadarByr = GetKadarUpah, fld_KdhMenuai = "-", fld_KodAktvt = CustMod_HariTerabai.PilihanAktvtHT, fld_KodGL = "602", fld_KodPkt = CustMod_HariTerabai.PilihanPktHT, fld_Kong = 0, fld_Kum = CustMod_HariTerabai.SelectionData, fld_LadangID = LadangID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_Nopkj = pkjid, fld_PerBrshGth = 0, fld_Quality = 0, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Unit = GetActvty.fld_Unit, fld_HrgaKwsnSkar = 0, fld_KodKwsnSkar = "-", fld_OverallAmount = Amount });
                                            }
                                            tbl_KerjaHariTerabais.Add(new tbl_KerjaHariTerabai() { fld_Nopkj = pkjid, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_JenisCharge = JenisCharge, fld_MasaKerja = MasaKerja, fld_ApprovedBy = pengurus.fldUserID, fld_ApprovedDT = timezone.gettimezone(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                                        }
                                        disablesavebtn = true;
                                    }
                                    else
                                    {
                                        msg = GlobalResEstate.msgDeleteAttnd;
                                        statusmsg = "warning";
                                        disablesavebtn = true;
                                    }
                                }
                                else
                                {
                                    var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

                                    if (datainkrjhdr == null)
                                    {
                                        var pkjdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == CustMod_HariTerabai.SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Kdaktf == "1").Select(s => new { s.fld_Nopkj, s.fld_KumpulanID, s.fld_Kdrkyt }).FirstOrDefault();
                                        KumpulanKod = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjdata.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KodKumpulan).FirstOrDefault();
                                        tbl_Kerjahdrs.Add(new tbl_Kerjahdr() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Kum = KumpulanKod.Trim(), fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Kdhdct = CustMod_HariTerabai.WorkCode, fld_Hujan = CustMod_HariTerabai.Rainning, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = date, fld_DataSource = "B" });
                                        if (CustMod_HariTerabai.JenisChargeHT == "kong")
                                        {
                                            GLCode = "";
                                            var paysheetID = "";
                                            if (pkjdata.fld_Kdrkyt == "MA")
                                            {
                                                GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == CustMod_HariTerabai.PilihanAktvtHT && x.fld_Paysheet == "PT").Select(s => s.fld_KodGL).FirstOrDefault();
                                                paysheetID = "PT";
                                            }
                                            else
                                            {
                                                GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == CustMod_HariTerabai.PilihanAktvtHT && x.fld_Paysheet == "PA").Select(s => s.fld_KodGL).FirstOrDefault();
                                                paysheetID = "PA";
                                            }

                                            HadirData.Add(new CustMod_Work { nopkj = pkjdata.fld_Nopkj, GLCode = GLCode, PaysheetID = paysheetID });

                                            var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == pkjdata.fld_Nopkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                                            if (SalaryIncrement != null)
                                            {
                                                GetKadarUpah2 = SalaryIncrement;
                                            }
                                            else
                                            {
                                                GetKadarUpah2 = 0;
                                            }

                                            if (CustMod_HariTerabai.PilihanMasaHT == "separuh")
                                            {
                                                Amount = (GetKadarUpah + GetKadarUpah2) / 2;
                                                Hasil = 0.5m;
                                            }
                                            else
                                            {
                                                Amount = (GetKadarUpah + GetKadarUpah2);
                                                Hasil = 1;
                                            }

                                            tbl_Kerjas.Add(new tbl_Kerja() { fld_Amount = Amount, fld_Bonus = 0, fld_BrtGth = 0, fld_CreatedBy = getuserid, fld_CreatedDT = timezone.gettimezone(), fld_DataSource = "B", fld_JamOT = 0, fld_JnisAktvt = GetActvty.fld_KodJenisAktvt, fld_JnsPkt = CustMod_HariTerabai.JnisPktHT, fld_JumlahHasil = Hasil, fld_KadarByr = GetKadarUpah, fld_KdhMenuai = "-", fld_KodAktvt = CustMod_HariTerabai.PilihanAktvtHT, fld_KodGL = "602", fld_KodPkt = CustMod_HariTerabai.PilihanPktHT, fld_Kong = 0, fld_Kum = KumpulanKod, fld_LadangID = LadangID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_Nopkj = pkjdata.fld_Nopkj, fld_PerBrshGth = 0, fld_Quality = 0, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_Unit = GetActvty.fld_Unit, fld_HrgaKwsnSkar = 0, fld_KodKwsnSkar = "-", fld_OverallAmount = Amount });
                                        }

                                        tbl_KerjaHariTerabais.Add(new tbl_KerjaHariTerabai() { fld_Nopkj = pkjdata.fld_Nopkj, fld_Tarikh = CustMod_HariTerabai.dateseleted, fld_JenisCharge = JenisCharge, fld_MasaKerja = MasaKerja, fld_ApprovedBy = pengurus.fldUserID, fld_ApprovedDT = timezone.gettimezone(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                                    }
                                    else
                                    {
                                        msg = GlobalResEstate.msgDeleteAttnd;
                                        statusmsg = "warning";
                                        disablesavebtn = true;
                                    }
                                }

                                if (tbl_Kerjahdrs.Count() != 0)
                                {
                                    msg = GlobalResEstate.msgAdd;
                                    statusmsg = "success";
                                    dbr.tbl_Kerjahdr.AddRange(tbl_Kerjahdrs);

                                    if (tbl_Kerjas.Count() > 0)
                                    {
                                        dbr.tbl_Kerja.AddRange(tbl_Kerjas);
                                    }
                                    dbr.tbl_KerjaHariTerabai.AddRange(tbl_KerjaHariTerabais);
                                    dbr.SaveChanges();
                                    EstateFunction.SaveDataKerjaSAPFPM(dbr, dbr, tbl_Kerjas, NegaraID, SyarikatID, WilayahID, LadangID, HadirData, false, "", 0);
                                }
                            }
                            else
                            {
                                msg = GlobalResEstate.msgError;
                                statusmsg = "warning";
                                disablesavebtn = true;
                            }
                        }

                        List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();
                        List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
                        if (CustMod_HariTerabai.SelectionCategory == 1)
                        {
                            var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj.Trim()).ToList();
                            tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == CustMod_HariTerabai.SelectionData && datainpkjmast.Contains(x.fld_Nopkj) && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                        }
                        else
                        {
                            tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == CustMod_HariTerabai.SelectionData && x.fld_Tarikh == CustMod_HariTerabai.dateseleted && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
                        }
                        foreach (var tbl_KerjaData in tbl_KerjaList)
                        {
                            //Added by Shazana 23/11/2023
                            var JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_KodJnsAktvt == tbl_KerjaData.fld_JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();
                            var namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                            //Modified by Shazana 10/11/2023
                            //CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
                            //Added by Shazana 10/11/2023
                            //Added by Shazana 23/11/2023 -Kalau kong baru kira keluasan
                            decimal? fld_LsPktUtama = 0;
                            if (JenisAktiviti.fld_DisabledFlag == 3)
                            {
                                fld_LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                                //Added by Shazana 29/11/2023
                                if (fld_LsPktUtama == null)
                                {
                                    fld_LsPktUtama = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                                }
                                //Added by Shazana 22/11/2023
                                if (fld_LsPktUtama == null) { fld_LsPktUtama = 0; }
                            }
                            CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount, fld_LsPktUtama = fld_LsPktUtama });

                        }

                        bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, CutOfDateStatus);
                    }
                    catch (Exception ex)
                    {
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                        msg = GlobalResEstate.msgError;
                        statusmsg = "warning";
                        disablesavebtn = true;
                    }
                    dbr.Dispose();
                    AuthStatus = true;
                }
                else
                {
                    AuthStatus = false;
                    msg = GlobalResEstate.msgInvManagID;
                    statusmsg = "warning";
                    disablesavebtn = true;
                }
            }
            else
            {
                AuthStatus = false;
                msg = GlobalResEstate.msgEnterManagID;
                statusmsg = "warning";
                disablesavebtn = true;
            }

            return Json(new { proceedstatus = disablesavebtn, statusmsg, msg, ZeroLeaveBal, statusmsg2, msg2, AlertPopup, tablelisting2 = bodyview2, AuthStatus });
        }

        public ActionResult _OtherDifficulty()
        {
            return View();
        }

        [HttpPost]
        public ActionResult _OtherDifficulty(decimal? OtherDifValue, string ManagerID2, string ManagerPassword2)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            EncryptDecrypt Encrypt = new EncryptDecrypt();
            DateTime? AppDT = timezone.gettimezone();
            string msg = "";
            string statusmsg = "";
            bool AuthStatus = false;
            int AppUsrID = 0;
            string GetValidation = "3";
            if (string.IsNullOrEmpty(ManagerID2) == false && string.IsNullOrEmpty(ManagerPassword2) == false)
            {
                ManagerPassword2 = Encrypt.Encrypt(ManagerPassword2);
                var pengurus = db.tblUsers.Where(x => x.fldUserName == ManagerID2 && x.fldUserPassword == ManagerPassword2 && x.fldDeleted == false && x.fldRoleID <= 6 && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).SingleOrDefault();

                if (pengurus != null)
                {
                    AppUsrID = pengurus.fldUserID;
                    AuthStatus = true;
                    msg = GlobalResEstate.msgValueSuccess;
                    statusmsg = "success";
                }
                else
                {
                    AppDT = null;
                    AuthStatus = false;
                    OtherDifValue = 0;
                    msg = GlobalResEstate.msgInvManagID;
                    statusmsg = "warning";
                }
            }
            else
            {
                AppDT = null;
                AuthStatus = false;
                OtherDifValue = 0;
                msg = GlobalResEstate.msgEnterManagID;
                statusmsg = "warning";
            }
            return Json(new { OtherDifValue, msg, statusmsg, AuthStatus, AppUsrID, AppDT, GetValidation });
        }

        //Kamalia 16/02/2020
        public ActionResult WorkDetail()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            List<SelectListItem> Lejar = new List<SelectListItem>();
            List<SelectListItem> JnisPkt = new List<SelectListItem>();
            List<SelectListItem> PilihanPkt = new List<SelectListItem>();
            List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            List<SelectListItem> Bonus = new List<SelectListItem>();
            List<SelectListItem> TrnsfrLvl = new List<SelectListItem>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Take(1).FirstOrDefault();
            var Pkt = dbr.tbl_PktUtama.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();

            var LejarList = db.tbl_Lejar.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_KodLejar).ToList();
            string LejarSelect = LejarList.Select(s => s.fld_KodLejar).Take(1).FirstOrDefault();
            Lejar = new SelectList(LejarList.Select(s => new SelectListItem { Value = s.fld_KodLejar, Text = s.fld_KodLejar + " - " + s.fld_Desc }), "Value", "Text").ToList();
            JnisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PilihanPkt = new SelectList(Pkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
            var tbl_JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_Lejar == LejarSelect).ToList();
            JnisAktvt = new SelectList(tbl_JenisAktiviti.OrderBy(o => o.fld_KodJnsAktvt).Select(s => new SelectListItem { Value = s.fld_KodJnsAktvt, Text = s.fld_KodJnsAktvt + " - " + s.fld_Desc }), "Value", "Text").ToList();

            TrnsfrLvl.Add(new SelectListItem { Text = "Tidak", Value = "0", Selected = true });
            TrnsfrLvl.Add(new SelectListItem { Text = "Ya", Value = "1", Selected = false });

            //added by kamalia 5/1/2021
            //  var ladangterpilih = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == "0520" && x.fld_LadangID == LadangID && x.fld_Unit == "KONG").Select(s => s.fld_KodAktvt).FirstOrDefault();

            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);
            PilihanAktvt = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            Bonus.Add(new SelectListItem { Text = "0", Value = "0", Selected = true });
            Bonus.Add(new SelectListItem { Text = "50", Value = "50", Selected = false });
            Bonus.Add(new SelectListItem { Text = "100", Value = "100", Selected = false });

            //modified bg faeza 18.08.2021
            string CdKesukaranMenuai = Pkt.Select(s => s.fld_KesukaranMenuaiPktUtama).Take(1).FirstOrDefault();
            string CdKesukaranMembaja = Pkt.Select(s => s.fld_KesukaranMembajaPktUtama).Take(1).FirstOrDefault();
            string CdKesukaranMemunggah = Pkt.Select(s => s.fld_KesukaranMemunggahPktUtama).Take(1).FirstOrDefault();
            string GetKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
            string GetKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
            string GetKesukaranMemunggah = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldOptConfValue == CdKesukaranMemunggah && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();

            ViewBag.JenisAktvt = tbl_JenisAktiviti.Select(s => s.fld_Desc).Take(1).FirstOrDefault();
            ViewBag.GetKesukaranMenuai = GetKesukaranMenuai;
            ViewBag.GetKesukaranMembaja = GetKesukaranMembaja;
            ViewBag.GetKesukaranMemunggah = GetKesukaranMemunggah;
            ViewBag.CdKesukaranMenuai = CdKesukaranMenuai;
            ViewBag.CdKesukaranMembaja = CdKesukaranMembaja;
            ViewBag.CdKesukaranMemunggah = CdKesukaranMemunggah;
            ViewBag.Lejar = Lejar;
            ViewBag.JnisPkt = JnisPkt;
            ViewBag.PilihanPkt = PilihanPkt;
            ViewBag.JnisAktvt = JnisAktvt;
            ViewBag.PilihanAktvt = PilihanAktvt;
            ViewBag.Bonus = Bonus;
            ViewBag.TrnsfrLvl = TrnsfrLvl;
            //added by kamalia 5/1/2021
            //  ViewBag.Ladangterpilih = ladangterpilih;
            dbr.Dispose();
            return View();
            //end modified
        }


        //Kamalia 16/02/2020


        [HttpPost]
        public ActionResult Working(DateTime SelectDate, string SelectionData, int SelectionCategory, string Lejar, byte JnisPkt, string PilihanPkt, string JnisAktvt, string PilihanAktvt, decimal? HrgaKwsnSkr, string KdKwsnSkr, int AppKwnsSkrLainID, DateTime? AppKwnsSkrLainDT, byte TrnsfrLvl, List<Kesukaran> kesukaran, List<CustMod_Work> HadirData)
        {
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            int checksameactvt = 0;
            string kodkumpulan = "";
            string unitcode = "";
            decimal? kong = 0;
            string bodyview = "";
            bool CutOfDateStatus = false;
            int checkkongactvt = 0;
            string GLCode = "";
            decimal? HrgaKwsnSkr2 = HrgaKwsnSkr;
            DateTime DTCreated = timezone.gettimezone();
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            bool TransferPkt = false;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);

            int? NegaraID2 = NegaraID;
            int? SyarikatID2 = SyarikatID;
            int? WilayahID2 = WilayahID;
            int? LadangID2 = LadangID;

            if (TrnsfrLvl == 1)
            {
                TransferPkt = true;
            }
            else
            {
                TransferPkt = false;
            }
            int transferLvlID = 0;
            var transferPktCode = "";
            var sapType = "";
            var PilihanPktID = 0;
            if (TransferPkt)
            {
                PilihanPktID = int.Parse(PilihanPkt);
                var pktTransfer = dbr.tbl_PktPinjam.Where(x => x.fld_ID == PilihanPktID && x.fld_LadangID == LadangID).FirstOrDefault();
                if (pktTransfer != null)
                {
                    JnisPkt = byte.Parse(pktTransfer.fld_JenisPkt.ToString());
                    NegaraID2 = pktTransfer.fld_NegaraIDAsal;
                    SyarikatID2 = pktTransfer.fld_SyarikatIDAsal;
                    WilayahID2 = pktTransfer.fld_WilayahIDAsal;
                    LadangID2 = pktTransfer.fld_LadangIDAsal;
                    transferLvlID = pktTransfer.fld_OriginPktID.Value;
                    transferPktCode = pktTransfer.fld_KodPkt;
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID2.Value, SyarikatID2.Value, NegaraID2.Value);
                    dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                    var tbl_PktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_ID == pktTransfer.fld_OriginPktID).FirstOrDefault();
                    PilihanPkt = tbl_PktUtama.fld_PktUtama;
                    sapType = tbl_PktUtama.fld_SAPType;
                }
            }
            //Added by Shazana 8/1/2024
            string KodUpahAktivitiKeyIn;
            int JumlahKadarHargaBerbeza = 0;
            if (estateCostCenter == "1000")
            {
                if (EstateFunction.CheckSAPGLMap(dbrpkt, JnisPkt, PilihanPkt, PilihanAktvt, NegaraID2, SyarikatID2, WilayahID2, LadangID2, false, "-", out GLCode, transferLvlID))
                {
                    if (HadirData != null)
                    {
                        CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID);
                        if (!CutOfDateStatus)
                        {
                            checksameactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                            :
                            dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();


                            var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();

                            checkkongactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_JnisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                            :
                            dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_JnisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();

                            if (getJenisActvtDetails.fld_KodJnsAktvt == JnisAktvt)
                            {
                                checksameactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                                :
                                dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                            }
                            //Added by Shazana 9/10/2023 
                            decimal? fld_LsPktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_LadangID == LadangID2 && x.fld_WilayahID == WilayahID2 && x.fld_SyarikatID == SyarikatID2).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                            //Added by Shazana 29/11/2023 //sekiranya tidak wujud dalam pkt_utama, code akan check luas dalam subpkt
                            if (fld_LsPktUtama == null)
                            {
                                fld_LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == PilihanPkt && x.fld_LadangID == LadangID2 && x.fld_WilayahID == WilayahID2 && x.fld_SyarikatID == SyarikatID2).Select(x => x.fld_LsPkt).FirstOrDefault();
                            }

                            var repeatingAktivitiPeringkat = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt).ToList();

                            //Added by Shazana 22/12/2023
                            decimal? upahAktivitiSekarangDisable3_ = 0M;
                            decimal? upahAktivitiSekarangDisable3 = 0M;
                            upahAktivitiSekarangDisable3_ = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == PilihanAktvt && x.fld_DisabledFlag == 3).Select(x => x.fld_Harga).FirstOrDefault();
                            if (upahAktivitiSekarangDisable3_ == null)
                            { upahAktivitiSekarangDisable3 = 0M; }
                            else
                            {
                                { upahAktivitiSekarangDisable3 = upahAktivitiSekarangDisable3_; }
                            }

                            //Added by Shazana 9/10/2023 -Tambah validaton untuk tiada nilai luas dalam peringkat dan kodaktiviti dan peringkat yang sama telah wujud
                            //Modified by Shazana 22/11/2023 -Hanya kong yg tiada luas je tak boleh disimpan
                            int? DisabledFlag = db.tbl_JenisAktiviti.Where(x => x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_DisabledFlag).FirstOrDefault();
                            var ListJenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_DisabledFlag == 3).ToList();

                            var ListAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID).Select(x => x.fld_JnisAktvt).ToList();
                            int? AktivitiDisable3 = 0;
                            int? AktivitiDisableBukan3 = 0;
                            int? AktivitiDisableSekarang = 0;
                            //Added by Shazana 22/12/2023
                            decimal? kodaktivitiPrevious = 0;
                            decimal? KadarBayrUpahAktivitiKeyIn = 0;
                            if (SelectionCategory == 1) //Kumpulan
                            {
                                ListAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_JnisAktvt).ToList();
                                //Added by Shazana 22/12/2023
                                KadarBayrUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_KadarByr).FirstOrDefault();
                                AktivitiDisable3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag == 3).ToList().Count();
                                AktivitiDisableBukan3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag != 3).ToList().Count();
                                AktivitiDisableSekarang = db.tbl_JenisAktiviti.Where(x => x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_DisabledFlag).FirstOrDefault();
                                //Added by Shazana 22/12/2023
                                string upahAktivitiPrevious = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate).Select(x => x.fld_KodAktvt).FirstOrDefault();
                                ListAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_JnisAktvt).ToList();
                                if (upahAktivitiPrevious == null)
                                { kodaktivitiPrevious = 0M; }
                                else
                                {
                                    kodaktivitiPrevious = db.tbl_UpahAktiviti.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_KodAktvt == upahAktivitiPrevious && x.fld_DisabledFlag == 3).Select(x => x.fld_Harga).FirstOrDefault();
                                }

                                //Modified by Shazana 8/1/2024
                                //KadarBayrUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionData).Select(x => x.fld_KadarByr).FirstOrDefault();
                                KodUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_KodAktvt).FirstOrDefault();
                                if (KodUpahAktivitiKeyIn == null)
                                { KadarBayrUpahAktivitiKeyIn = 0M; }
                                else
                                {
                                    KadarBayrUpahAktivitiKeyIn = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == KodUpahAktivitiKeyIn).Select(x => x.fld_Harga).FirstOrDefault();
                                }

                            }
                            else
                            {
                                ListAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionData).Select(x => x.fld_JnisAktvt).ToList();
                                AktivitiDisable3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag == 3).ToList().Count();
                                AktivitiDisableBukan3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag != 3).ToList().Count();
                                AktivitiDisableSekarang = db.tbl_JenisAktiviti.Where(x => x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_DisabledFlag).FirstOrDefault();
                                //Added by Shazana 22/12/2023
                                kodaktivitiPrevious = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate).Select(x => x.fld_KadarByr).FirstOrDefault();
                                if (kodaktivitiPrevious == null)
                                { kodaktivitiPrevious = 0M; }

                                //Modified by Shazana 8/1/2024
                                //KadarBayrUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionData).Select(x => x.fld_KadarByr).FirstOrDefault();
                                KodUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionData).Select(x => x.fld_KodAktvt).FirstOrDefault();
                                if (KodUpahAktivitiKeyIn == null)
                                { KadarBayrUpahAktivitiKeyIn = 0M; }
                                else
                                {
                                    KadarBayrUpahAktivitiKeyIn = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == KodUpahAktivitiKeyIn).Select(x => x.fld_Harga).FirstOrDefault();
                                }
                            }

                            if (AktivitiDisableBukan3 > 0 && AktivitiDisableSekarang == 3)
                            {
                                msg = GlobalResEstate.msgExistKadaran;
                                statusmsg = "warning";
                            }
                            else if (AktivitiDisable3 > 0 && AktivitiDisableSekarang != 3)
                            {
                                msg = GlobalResEstate.msgExistKong;
                                statusmsg = "warning";
                            }
                            //Added by Shazana 22/12/2023
                            else if (AktivitiDisable3 > 0 && AktivitiDisableSekarang == 3 && KadarBayrUpahAktivitiKeyIn != upahAktivitiSekarangDisable3)
                            {
                                msg = GlobalResEstate.msgDifferentKong;
                                statusmsg = "warning";
                            }
                            else if ((fld_LsPktUtama == null || fld_LsPktUtama == 0) && DisabledFlag == 3)
                            {
                                msg = GlobalResEstate.msgLuasNull + " (" + PilihanPkt + ") ";
                                statusmsg = "warning";
                            }
                            else if (repeatingAktivitiPeringkat.Count() != 0)
                            {
                                msg = GlobalResEstate.msgSimilarAktivitiPeringkat;
                                statusmsg = "warning";
                            }
                            //commented by faeza 23.11.2022 - original code
                            //if (checksameactvt == 0 && HadirData.Count() != 0 && checkkongactvt == 0)
                            //modified by faeza 23.11.2022 
                            //Modified by Shazana 9/10/2023
                            else if (checksameactvt == 0 && HadirData.Count() != 0)
                            {
                                foreach (var datakerja in HadirData)
                                {
                                    switch (datakerja.checkpurpose)
                                    {
                                        case 1:

                                            break;
                                        case 2:
                                            datakerja.kdhmnuai = "-";
                                            break;
                                        case 3:
                                            datakerja.kdhmnuai = "-";
                                            datakerja.kualiti = 0;
                                            datakerja.hasil = 1;
                                            datakerja.bonus = 0;
                                            break;
                                    }

                                    datakerja.jumlah = datakerja.hasil == null ? datakerja.kadar : datakerja.jumlah;
                                    datakerja.kdhmnuai = datakerja.kdhmnuai == null ? "-" : datakerja.kdhmnuai;
                                    datakerja.kualiti = datakerja.kualiti == null ? 0 : datakerja.kualiti;
                                    datakerja.hasil = datakerja.hasil == null ? 0 : datakerja.hasil;
                                    datakerja.bonus = datakerja.bonus == null ? 0 : datakerja.bonus;
                                    //masukkan looping checking esk
                                    kodkumpulan = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Kum).FirstOrDefault();
                                    unitcode = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_KodAktvt == PilihanAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).Select(s => s.fld_Unit).FirstOrDefault();
                                    HrgaKwsnSkr = HrgaKwsnSkr2 * datakerja.hasil * datakerja.gandaankadar;
                                    var OTHour = datakerja.ot == null ? 0 : datakerja.ot;
                                    if (TransferPkt)
                                    {
                                        PilihanPkt = transferPktCode;
                                    }
                                    tbl_KerjaList.Add(new tbl_Kerja() { fld_Nopkj = datakerja.nopkj, fld_Kum = kodkumpulan, fld_Tarikh = SelectDate, fld_KodPkt = PilihanPkt, fld_Amount = datakerja.jumlah, fld_JnsPkt = JnisPkt, fld_JumlahHasil = datakerja.hasil, fld_KadarByr = datakerja.kadar, fld_KodGL = Lejar, fld_KodAktvt = PilihanAktvt, fld_JamOT = OTHour, fld_DataSource = "B", fld_BrtGth = 0, fld_PerBrshGth = 0, fld_Kong = kong, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = DTCreated, fld_JnisAktvt = JnisAktvt, fld_KdhMenuai = datakerja.kdhmnuai, fld_Bonus = datakerja.bonus, fld_Unit = unitcode, fld_Quality = datakerja.kualiti, fld_HrgaKwsnSkar = HrgaKwsnSkr, fld_KodKwsnSkar = KdKwsnSkr, fld_ApprovalKwsnSkarDT = AppKwnsSkrLainDT, fld_ApprovalKwsnSkarLainBy = AppKwnsSkrLainID, fld_OverallAmount = datakerja.jumlahOA, fld_PinjamStatus = TransferPkt });
                                }
                                dbr.tbl_Kerja.AddRange(tbl_KerjaList);
                                dbr.SaveChanges();
                                EstateFunction.SaveDataKerjaKesukaran(dbr, tbl_KerjaList, kesukaran, NegaraID, SyarikatID);
                                EstateFunction.SaveDataKerjaSAP(dbr, dbrpkt, tbl_KerjaList, NegaraID, SyarikatID, WilayahID, LadangID, GLCode, TransferPkt, transferPktCode, PilihanPktID);
                                msg = GlobalResEstate.msgAdd;
                                statusmsg = "success";
                            }
                            //Added by Shazana 9/10/2023
                            //Jika ulangan aktiviti , ada hadir data dan jenis aktiviti adalah kong  //kumpulan,dah pernah ada data aktiviti sebelum ni
                            else if (checksameactvt != 0 && HadirData.Count() != 0 && JnisAktvt == "05")
                            {
                                foreach (var datakerja in HadirData)
                                {
                                    switch (datakerja.checkpurpose)
                                    {
                                        case 1:

                                            break;
                                        case 2:
                                            datakerja.kdhmnuai = "-";
                                            break;
                                        case 3:
                                            datakerja.kdhmnuai = "-";
                                            datakerja.kualiti = 0;
                                            datakerja.hasil = 1;
                                            datakerja.bonus = 0;
                                            break;
                                    }

                                    datakerja.jumlah = datakerja.hasil == null ? datakerja.kadar : datakerja.jumlah;
                                    datakerja.kdhmnuai = datakerja.kdhmnuai == null ? "-" : datakerja.kdhmnuai;
                                    datakerja.kualiti = datakerja.kualiti == null ? 0 : datakerja.kualiti;
                                    datakerja.hasil = datakerja.hasil == null ? 0 : datakerja.hasil;
                                    datakerja.bonus = datakerja.bonus == null ? 0 : datakerja.bonus;
                                    //masukkan looping checking esk
                                    kodkumpulan = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Kum).FirstOrDefault();
                                    unitcode = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_KodAktvt == PilihanAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).Select(s => s.fld_Unit).FirstOrDefault();
                                    HrgaKwsnSkr = HrgaKwsnSkr2 * datakerja.hasil * datakerja.gandaankadar;
                                    var OTHour = datakerja.ot == null ? 0 : datakerja.ot;
                                    if (TransferPkt)
                                    {
                                        PilihanPkt = transferPktCode;
                                    }

                                    var semakanexist = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == datakerja.nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt).ToList();

                                    //Filter pilihan peringkat dan kodaktiviti yang sama 
                                    if (semakanexist.Count() == 0)
                                    {

                                        //Modified by Shazana 9/10/2023
                                        tbl_KerjaList.Add(new tbl_Kerja() { fld_Nopkj = datakerja.nopkj, fld_Kum = kodkumpulan, fld_Tarikh = SelectDate, fld_KodPkt = PilihanPkt, fld_Amount = datakerja.jumlah, fld_JnsPkt = JnisPkt, fld_JumlahHasil = datakerja.hasil, fld_KadarByr = datakerja.jumlah, fld_KodGL = Lejar, fld_KodAktvt = PilihanAktvt, fld_JamOT = OTHour, fld_DataSource = "B", fld_BrtGth = 0, fld_PerBrshGth = 0, fld_Kong = kong, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = DTCreated, fld_JnisAktvt = JnisAktvt, fld_KdhMenuai = datakerja.kdhmnuai, fld_Bonus = datakerja.bonus, fld_Unit = unitcode, fld_Quality = datakerja.kualiti, fld_HrgaKwsnSkar = HrgaKwsnSkr, fld_KodKwsnSkar = KdKwsnSkr, fld_ApprovalKwsnSkarDT = AppKwnsSkrLainDT, fld_ApprovalKwsnSkarLainBy = AppKwnsSkrLainID, fld_OverallAmount = datakerja.jumlahOA, fld_PinjamStatus = TransferPkt });
                                    }
                                    else
                                    {
                                        msg = GlobalResEstate.msgSimilarAktivitiPeringkat;
                                        statusmsg = "warning";
                                    }
                                }
                                //Filter pilihan peringkat dan kodaktiviti yang sama 
                                if (tbl_KerjaList.Count() != 0)
                                {
                                    dbr.tbl_Kerja.AddRange(tbl_KerjaList);
                                    dbr.SaveChanges();
                                    dbr.SaveChanges();

                                    //Added by Shazana 9/11/2023
                                    EstateFunction.SaveDataKerjaKesukaran(dbr, tbl_KerjaList, kesukaran, NegaraID, SyarikatID);
                                    EstateFunction.SaveDataKerjaSAP(dbr, dbrpkt, tbl_KerjaList, NegaraID, SyarikatID, WilayahID, LadangID, GLCode, TransferPkt, transferPktCode, PilihanPktID);

                                }
                                else
                                {
                                    msg = GlobalResEstate.msgSimilarAktivitiPeringkat;
                                    statusmsg = "warning";
                                }

                                //Added by Shazana 9/10/2023
                                foreach (var datakerja in HadirData)
                                {
                                    var KongList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_JnisAktvt == "05" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                                    var PktList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_JnisAktvt == "05" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_KodPkt).ToList();

                                    List<string> datainpkjmastexldatainkrjhdrs = PktList.ToList();
                                    decimal? LuasKong = 0;

                                    //Dapatkan jumlah keseluruhan luas peringkat utama
                                    foreach (var detailKong in KongList)
                                    {
                                        var LuasList = dbrpkt.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && PktList.Contains(x.fld_PktUtama)).Select(x => x.fld_LsPktUtama).Sum();
                                        decimal? LsPktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == detailKong.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                                        //Added by Shazana 29/11/2023 
                                        if (LsPktUtama == null)
                                        {
                                            LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == detailKong.fld_KodPkt && x.fld_LadangID == LadangID2 && x.fld_WilayahID == WilayahID2 && x.fld_SyarikatID == SyarikatID2).Select(x => x.fld_LsPkt).FirstOrDefault();
                                        }
                                        LuasKong = LuasKong + LsPktUtama;
                                    }

                                    int i = 0;
                                    decimal? valueLs = 0;

                                    decimal? kadarbayarLast = 0M; //Modified by Shazana 22/12/2023 db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID== NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID== LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();
                                    foreach (var detaildatakerja in KongList)
                                    {
                                        //Modified by Shazana 9/11/2023
                                        //decimal? kadarbayarLast = detaildatakerja.fld_KadarByr;
                                        kadarbayarLast = db.tbl_UpahAktiviti.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_KodAktvt == detaildatakerja.fld_KodAktvt && x.fld_DisabledFlag == 3).Select(x => x.fld_Harga).FirstOrDefault(); ;
                                        decimal? LsPktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                                        //Added by Shazana 29/11/2023 
                                        if (LsPktUtama == null)
                                        {
                                            LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID2 && x.fld_WilayahID == WilayahID2 && x.fld_SyarikatID == SyarikatID2).Select(x => x.fld_LsPkt).FirstOrDefault();
                                        }

                                        decimal? ValuePkt = Math.Round((Decimal)(LsPktUtama / LuasKong * kadarbayarLast), 2);
                                        decimal? kadarbayaranKong = ValuePkt == null ? 0 : ValuePkt;
                                        i = i + 1;
                                        int? dd = KongList.Count();
                                        if (dd == i)
                                        {
                                            ValuePkt = kadarbayarLast - valueLs;
                                            detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                            detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                            //Added by Shazaa 9/11/2023
                                            detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                            dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                            dbr.SaveChanges();
                                        }
                                        else
                                        {
                                            valueLs = valueLs + ValuePkt;
                                            detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                            detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                            //Added by Shazaa 9/11/2023
                                            detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                            dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                            dbr.SaveChanges();

                                        }
                                    }
                                }


                                msg = GlobalResEstate.msgAdd;
                                statusmsg = "success";
                            }

                            else
                            {
                                msg = GlobalResEstate.msgDataExist;
                                statusmsg = "warning";
                            }
                        }
                        else
                        {
                            msg = GlobalResEstate.msgError;
                            statusmsg = "warning";
                        }
                    }
                    else
                    {
                        msg = GlobalResEstate.msgNoRecord;
                        statusmsg = "warning";
                    }
                }
                else
                {
                    msg = GlobalResEstate.msgKodGLnotFound;
                    statusmsg = "warning";
                }
            }
            else
            {
                if (HadirData != null)
                {
                    decimal? kadarbayarLast = 0;
                    CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID);
                    if (!CutOfDateStatus)
                    {
                        checksameactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                        :
                        dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();

                        var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();

                        checkkongactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_JnisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                        :
                        dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_JnisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();

                        if (getJenisActvtDetails.fld_KodJnsAktvt == JnisAktvt)
                        {
                            checksameactvt = SelectionCategory == 1 ? dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count()
                            :
                            dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                        }

                        //Added by Shazana 9/10/2023 
                        decimal? fld_LsPktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_LadangID == LadangID2 && x.fld_WilayahID == WilayahID2 && x.fld_SyarikatID == SyarikatID2).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                        //Added by Shazana 29/11/2023 
                        if (fld_LsPktUtama == null)
                        {
                            fld_LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == PilihanPkt && x.fld_LadangID == LadangID2 && x.fld_WilayahID == WilayahID2 && x.fld_SyarikatID == SyarikatID2).Select(x => x.fld_LsPkt).FirstOrDefault();
                        }
                        if (fld_LsPktUtama == null) { fld_LsPktUtama = 0; }

                        var repeatingAktivitiPeringkat = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt).ToList();
                        var namajenisaktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJnsAktvt == JnisAktvt).FirstOrDefault();

                        //Added by Shazana 22/12/2023
                        decimal? upahAktivitiSekarangDisable3_ = 0M;
                        decimal? upahAktivitiSekarangDisable3 = 0M;
                        upahAktivitiSekarangDisable3_ = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == PilihanAktvt && x.fld_DisabledFlag==3).Select(x=>x.fld_Harga).FirstOrDefault();
                        if (upahAktivitiSekarangDisable3_ == null)
                        { upahAktivitiSekarangDisable3 = 0M; }
                        else
                        {
                            { upahAktivitiSekarangDisable3 = upahAktivitiSekarangDisable3_; }
                        }
                        //var upahAktivitiPrevious = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID).ToList();

                        //Modified by Shazana 22/11/2023 -Hanya kong yg tiada luas je tak boleh disimpan
                        int? DisabledFlag = db.tbl_JenisAktiviti.Where(x => x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_DisabledFlag).FirstOrDefault();
                        var ListJenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_DisabledFlag == 3).ToList();

                        var ListAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID).Select(x => x.fld_JnisAktvt).ToList();

                        int? AktivitiDisable3 = 0;
                        int? AktivitiDisableBukan3 = 0;
                        int? AktivitiDisableSekarang = 0;
                        decimal? kodaktivitiPrevious = 0;
                        decimal? KadarBayrUpahAktivitiKeyIn = 0;
                        //Added by Shazana 8/1/2024
                      
                        int? UpahAktivitiDisableBukan3 = 0;
                        int? UpahAktivitiDisableSekarang = 0;

                        if (SelectionCategory == 1) //Kumpulan
                        {
                            ListAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_JnisAktvt).ToList();
                            //Modified by Shazana 8/1/2024
                            //KadarBayrUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_KadarByr).FirstOrDefault();
                            KodUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_KodAktvt).FirstOrDefault();
                            if (KodUpahAktivitiKeyIn == null)
                            { KadarBayrUpahAktivitiKeyIn = 0M; }
                            else
                            {
                                KadarBayrUpahAktivitiKeyIn = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == KodUpahAktivitiKeyIn).Select(x => x.fld_Harga).FirstOrDefault();
                            }
                            AktivitiDisable3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag == 3).ToList().Count();
                            AktivitiDisableBukan3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag != 3).ToList().Count();
                            AktivitiDisableSekarang = db.tbl_JenisAktiviti.Where(x => x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_DisabledFlag).FirstOrDefault();


                            //Added by Shazana on 8/1/2024 -Semakan sekiranya terdapat kadar bayaran berbeza dalam kong
                            var ListUpahAktiviti = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Kum == SelectionData).Select(x => x.fld_KodAktvt).ToList();
                            var ListUpahAktivitiDisable3 = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListUpahAktiviti.Contains(x.fld_KodAktvt) && x.fld_DisabledFlag == 3).ToList();
                            if (ListUpahAktivitiDisable3 != null)
                            {
                                kodaktivitiPrevious = ListUpahAktivitiDisable3.Where(x=>x.fld_Harga != upahAktivitiSekarangDisable3).Select(x=>x.fld_Harga).FirstOrDefault(); 
                            }
                            else
                            { kodaktivitiPrevious = 0; }

                        }
                        else
                        {
                             ListAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionData).Select(x => x.fld_JnisAktvt).ToList();
                            //Modified by Shazana 8/1/2024
                            //KadarBayrUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionData).Select(x => x.fld_KadarByr).FirstOrDefault();
                            KodUpahAktivitiKeyIn = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Nopkj == SelectionData).Select(x => x.fld_KodAktvt).FirstOrDefault();
                            if (KodUpahAktivitiKeyIn == null)
                            { KadarBayrUpahAktivitiKeyIn = 0M; }
                            else
                            {
                                KadarBayrUpahAktivitiKeyIn = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == KodUpahAktivitiKeyIn).Select(x => x.fld_Harga).FirstOrDefault();
                            }
                            AktivitiDisable3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag == 3).ToList().Count();
                            AktivitiDisableBukan3 = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && ListAktivitiKeyIn.Contains(x.fld_KodJnsAktvt) && x.fld_DisabledFlag != 3).ToList().Count();
                            AktivitiDisableSekarang = db.tbl_JenisAktiviti.Where(x => x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(x => x.fld_DisabledFlag).FirstOrDefault();
                            
                            //Modified by Shazana 8/1/2024
                            kodaktivitiPrevious = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == KodUpahAktivitiKeyIn && x.fld_Harga != upahAktivitiSekarangDisable3).Select(x => x.fld_Harga).FirstOrDefault();
                            if (kodaktivitiPrevious == null)
                            { kodaktivitiPrevious = 0M; }
                        }
                      
                        if (AktivitiDisableBukan3 > 0 && AktivitiDisableSekarang == 3)
                        {
                            msg = GlobalResEstate.msgExistKadaran;
                            statusmsg = "warning";
                        }
                        else if (AktivitiDisable3 > 0 && AktivitiDisableSekarang != 3)
                        {
                                msg = GlobalResEstate.msgExistKong;
                                statusmsg = "warning";
                        }
                        else if (AktivitiDisable3 > 0 && AktivitiDisableSekarang == 3 && KadarBayrUpahAktivitiKeyIn != upahAktivitiSekarangDisable3)
                        {
                                msg = GlobalResEstate.msgDifferentKong;
                                statusmsg = "warning";
                        }
                        //Added by Shazana 9/10/2023 -Tambah validaton untuk tiada nilai luas dalam peringkat dan kodaktiviti dan peringkat yang sama telah wujud
                        //Added by Shazana 22/11/2023 - add paparan validation luas untuk kong sahaja
                        else if ((fld_LsPktUtama == null || fld_LsPktUtama == 0) && namajenisaktiviti.fld_DisabledFlag == 3)
                        {
                            msg = GlobalResEstate.msgLuasNull + " (" + PilihanPkt + ") ";
                            statusmsg = "warning";
                        }
                        else if (repeatingAktivitiPeringkat.Count() != 0)
                        {
                            msg = GlobalResEstate.msgSimilarAktivitiPeringkat;
                            statusmsg = "warning";
                        }
                        //Adedd by Shazana 22/12/2023 //Checking different type of kong
                        //Modified by Shazana 8/1/2024
                        else if ((upahAktivitiSekarangDisable3 != 0 )&& kodaktivitiPrevious != 0 && kodaktivitiPrevious != null && kodaktivitiPrevious != 0)
                        {
                                msg = GlobalResEstate.msgDifferentKong;
                                statusmsg = "warning";
                        }
                        //Modified by Shazana 9/10/2023 -modified jadi else if
                        else if (checksameactvt == 0 && HadirData.Count() != 0)
                        {
                            foreach (var datakerja in HadirData)
                            {
                                switch (datakerja.checkpurpose)
                                {
                                    case 1:

                                        break;
                                    case 2:
                                        datakerja.kdhmnuai = "-";
                                        break;
                                    case 3:
                                        datakerja.kdhmnuai = "-";
                                        datakerja.kualiti = 0;
                                        datakerja.hasil = 1;
                                        datakerja.bonus = 0;
                                        break;
                                }
                                datakerja.jumlah = datakerja.hasil == null ? datakerja.kadar : datakerja.jumlah;
                                datakerja.kdhmnuai = datakerja.kdhmnuai == null ? "-" : datakerja.kdhmnuai;
                                datakerja.kualiti = datakerja.kualiti == null ? 0 : datakerja.kualiti;
                                datakerja.hasil = datakerja.hasil == null ? 0 : datakerja.hasil;
                                datakerja.bonus = datakerja.bonus == null ? 0 : datakerja.bonus;
                                //masukkan looping checking esk
                                kodkumpulan = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Kum).FirstOrDefault();
                                unitcode = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_KodAktvt == PilihanAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).Select(s => s.fld_Unit).FirstOrDefault();
                                HrgaKwsnSkr = HrgaKwsnSkr2 * datakerja.hasil * datakerja.gandaankadar;
                                var OTHour = datakerja.ot == null ? 0 : datakerja.ot;
                                if (TransferPkt)
                                {
                                    PilihanPkt = transferPktCode;
                                }
                                tbl_KerjaList.Add(new tbl_Kerja() { fld_Nopkj = datakerja.nopkj, fld_Kum = kodkumpulan, fld_Tarikh = SelectDate, fld_KodPkt = PilihanPkt, fld_Amount = datakerja.jumlah, fld_JnsPkt = JnisPkt, fld_JumlahHasil = datakerja.hasil, fld_KadarByr = datakerja.kadar, fld_KodGL = Lejar, fld_KodAktvt = PilihanAktvt, fld_JamOT = OTHour, fld_DataSource = "B", fld_BrtGth = 0, fld_PerBrshGth = 0, fld_Kong = kong, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = DTCreated, fld_JnisAktvt = JnisAktvt, fld_KdhMenuai = datakerja.kdhmnuai, fld_Bonus = datakerja.bonus, fld_Unit = unitcode, fld_Quality = datakerja.kualiti, fld_HrgaKwsnSkar = HrgaKwsnSkr, fld_KodKwsnSkar = KdKwsnSkr, fld_ApprovalKwsnSkarDT = AppKwnsSkrLainDT, fld_ApprovalKwsnSkarLainBy = AppKwnsSkrLainID, fld_OverallAmount = datakerja.jumlahOA, fld_PinjamStatus = TransferPkt });
                            }
                            dbr.tbl_Kerja.AddRange(tbl_KerjaList);
                            dbr.SaveChanges();
                            EstateFunction.SaveDataKerjaKesukaran(dbr, tbl_KerjaList, kesukaran, NegaraID, SyarikatID);
                            EstateFunction.SaveDataKerjaSAPFPM(dbr, dbrpkt, tbl_KerjaList, NegaraID, SyarikatID, WilayahID, LadangID, HadirData, TransferPkt, transferPktCode, PilihanPktID);

                            //Added by Shazana 9/11/2023
                            var senaraiJenisKong = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3).Select(x => x.fld_KodJnsAktvt).ToList();

                            foreach (var datakerja in HadirData)
                            {
                                var KongList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                                var PktList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_KodPkt).ToList();
                                kadarbayarLast = datakerja.kadar;// Modified by Shazana 22/12/2023 db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();
                                var JenisHadir =dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x=>x.fld_Kdhdct).FirstOrDefault();
                                if (JenisHadir == "H02")
                                {
                                    kadarbayarLast = kadarbayarLast * 2;
                                }
                                else if (JenisHadir == "H03")
                                {
                                    kadarbayarLast = kadarbayarLast * 3;
                                }
                                List<string> datainpkjmastexldatainkrjhdrs = PktList.ToList();
                                decimal? LuasKong = 0;

                                //Dapatkan jumlah keseluruhan luas peringkat utama
                                foreach (var detailKong in KongList)
                                {
                                    var LuasList = dbrpkt.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && PktList.Contains(x.fld_PktUtama)).Select(x => x.fld_LsPktUtama).Sum();
                                    decimal? LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == detailKong.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                                    //Added by Shazana 29/11/2023
                                    if (fld_LsPktUtama == null)
                                    {
                                        fld_LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == detailKong.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                                    }
                                    if (fld_LsPktUtama == null)
                                    {
                                        fld_LsPktUtama = 0;
                                    }
                                    LuasKong = LuasKong + LsPktUtama;
                                }

                                int i = 0;
                                decimal? valueLs = 0;
                                //Added by Shazana 9/11/2023
                                //decimal? kadarbayarLast = db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();

                                foreach (var detaildatakerja in KongList)
                                {
                                    //Modified by Shazana 9/11/2023
                                    //decimal? kadarbayarLast = detaildatakerja.fld_KadarByr;
                                    decimal? LsPktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                                    if (LsPktUtama == null)
                                    {
                                        LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                                    }
                                    if (LsPktUtama == null)
                                    {
                                        LsPktUtama = 0;
                                    }
                                    //Modified by Shazana 9/11/2023
                                    decimal? ValuePkt = Math.Round((Decimal)(LsPktUtama / LuasKong * kadarbayarLast), 2);

                                    i = i + 1;
                                    int? dd = KongList.Count();
                                    if (dd == i)
                                    {
                                        ValuePkt = kadarbayarLast - valueLs;
                                        detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                        detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                        //Added by Shazana 9/10/2023
                                        detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                        dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                        dbr.SaveChanges();
                                    }
                                    else
                                    {
                                        valueLs = valueLs + ValuePkt;
                                        detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                        detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                        //Added by Shazana 9/10/2023
                                        detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                        dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                        dbr.SaveChanges();

                                    }
                                }

                            }

                            msg = GlobalResEstate.msgAdd;
                            statusmsg = "success";
                        }
                        //Added by Shazana 9/10/2023
                        //Jika ulangan aktiviti , ada hadir data dan jenis aktiviti adalah kong  //kumpulan,dah pernah ada data aktiviti sebelum ni
                        else if (checksameactvt != 0 && HadirData.Count() != 0 && namajenisaktiviti.fld_DisabledFlag == 3)
                        {
                            foreach (var datakerja in HadirData)
                            {
                                switch (datakerja.checkpurpose)
                                {
                                    case 1:

                                        break;
                                    case 2:
                                        datakerja.kdhmnuai = "-";
                                        break;
                                    case 3:
                                        datakerja.kdhmnuai = "-";
                                        datakerja.kualiti = 0;
                                        datakerja.hasil = 1;
                                        datakerja.bonus = 0;
                                        break;
                                }

                                datakerja.jumlah = datakerja.hasil == null ? datakerja.kadar : datakerja.jumlah;
                                datakerja.kdhmnuai = datakerja.kdhmnuai == null ? "-" : datakerja.kdhmnuai;
                                datakerja.kualiti = datakerja.kualiti == null ? 0 : datakerja.kualiti;
                                datakerja.hasil = datakerja.hasil == null ? 0 : datakerja.hasil;
                                datakerja.bonus = datakerja.bonus == null ? 0 : datakerja.bonus;
                                //masukkan looping checking esk
                                kodkumpulan = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Kum).FirstOrDefault();
                                unitcode = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_KodAktvt == PilihanAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).Select(s => s.fld_Unit).FirstOrDefault();
                                HrgaKwsnSkr = HrgaKwsnSkr2 * datakerja.hasil * datakerja.gandaankadar;
                                var OTHour = datakerja.ot == null ? 0 : datakerja.ot;
                                if (TransferPkt)
                                {
                                    PilihanPkt = transferPktCode;
                                }

                                var semakanexist = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == datakerja.nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KodPkt == PilihanPkt && x.fld_KodAktvt == PilihanAktvt).ToList();

                                //Filter pilihan peringkat dan kodaktiviti yang sama 
                                if (semakanexist.Count() == 0)
                                {
                                    tbl_KerjaList.Add(new tbl_Kerja() { fld_Nopkj = datakerja.nopkj, fld_Kum = kodkumpulan, fld_Tarikh = SelectDate, fld_KodPkt = PilihanPkt, fld_Amount = datakerja.jumlah, fld_JnsPkt = JnisPkt, fld_JumlahHasil = datakerja.hasil, fld_KadarByr = datakerja.jumlah, fld_KodGL = Lejar, fld_KodAktvt = PilihanAktvt, fld_JamOT = OTHour, fld_DataSource = "B", fld_BrtGth = 0, fld_PerBrshGth = 0, fld_Kong = kong, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_CreatedBy = getuserid, fld_CreatedDT = DTCreated, fld_JnisAktvt = JnisAktvt, fld_KdhMenuai = datakerja.kdhmnuai, fld_Bonus = datakerja.bonus, fld_Unit = unitcode, fld_Quality = datakerja.kualiti, fld_HrgaKwsnSkar = HrgaKwsnSkr, fld_KodKwsnSkar = KdKwsnSkr, fld_ApprovalKwsnSkarDT = AppKwnsSkrLainDT, fld_ApprovalKwsnSkarLainBy = AppKwnsSkrLainID, fld_OverallAmount = datakerja.jumlahOA, fld_PinjamStatus = TransferPkt });
                                }
                                else
                                {
                                    msg = GlobalResEstate.msgSimilarAktivitiPeringkat;
                                    statusmsg = "warning";
                                }
                            }
                            //Filter pilihan peringkat dan kodaktiviti yang sama 
                            if (tbl_KerjaList.Count() != 0)
                            {
                                dbr.tbl_Kerja.AddRange(tbl_KerjaList);
                                dbr.SaveChanges();
                                dbr.SaveChanges();
                                //Added by Shazana 9/11/2023
                                EstateFunction.SaveDataKerjaKesukaran(dbr, tbl_KerjaList, kesukaran, NegaraID, SyarikatID);
                                EstateFunction.SaveDataKerjaSAPFPM(dbr, dbrpkt, tbl_KerjaList, NegaraID, SyarikatID, WilayahID, LadangID, HadirData, TransferPkt, transferPktCode, PilihanPktID);

                            }
                            else
                            {
                                msg = GlobalResEstate.msgSimilarAktivitiPeringkat;
                                statusmsg = "warning";
                            }

                            if (tbl_KerjaList.Count() != 0)
                            {
                                //Added by Shazana 9/10/2023
                                var senaraiJenisKong = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3).Select(x => x.fld_KodJnsAktvt).ToList();
                                foreach (var datakerja in HadirData)
                                {
                                    kadarbayarLast = datakerja.kadar;//Modified by Shazana 22/12/2023 db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();
                                    var JenisHadir = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_Kdhdct).FirstOrDefault();
                                    if (JenisHadir == "H02")
                                    {
                                        kadarbayarLast = kadarbayarLast * 2;
                                    }
                                    else if (JenisHadir == "H03")
                                    {
                                        kadarbayarLast = kadarbayarLast * 3;
                                    }
                                    var KongList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                                    //decimal? kadarbayarLast = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderByDescending(x=>x.fld_CreatedDT).Select(x=>x.fld_KadarByr).FirstOrDefault();
                                    var PktList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_KodPkt).ToList();

                                    List<string> datainpkjmastexldatainkrjhdrs = PktList.ToList();
                                    decimal? LuasKong = 0;

                                    //Dapatkan jumlah keseluruhan luas peringkat utama
                                    foreach (var detailKong in KongList)
                                    {
                                        var LuasList = dbrpkt.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && PktList.Contains(x.fld_PktUtama)).Select(x => x.fld_LsPktUtama).Sum();
                                        decimal? LsPktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == detailKong.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                                        //Added by Shazana 29/11/2023
                                        if (LsPktUtama == null)
                                        {
                                            LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == detailKong.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                                        }
                                        if (LsPktUtama == null)
                                        {
                                            LsPktUtama = 0;
                                        }
                                        LuasKong = LuasKong + LsPktUtama;
                                    }

                                    int i = 0;
                                    decimal? valueLs = 0;
                                    //Added by Shazana 9/11/2023
                                    // decimal? kadarbayarLast = db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();

                                    foreach (var detaildatakerja in KongList)
                                    {
                                        //Modified by Shazana 9/11/2023
                                        //decimal? kadarbayarLast = detaildatakerja.fld_KadarByr;
                                        decimal? LsPktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                                        //Added by Shazana 29/11/2023
                                        if (LsPktUtama == null)
                                        {
                                            LsPktUtama = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                                        }
                                        if (LsPktUtama == null)
                                        {
                                            LsPktUtama = 0;
                                        }

                                        //Modified by Shazana 9/11/2023
                                        decimal? ValuePkt = Math.Round((Decimal)(LsPktUtama / LuasKong * kadarbayarLast), 2);

                                        i = i + 1;
                                        int? dd = KongList.Count();
                                        if (dd == i)
                                        {
                                            ValuePkt = kadarbayarLast - valueLs;
                                            detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                            detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                            //Added by Shazana 9/10/2023
                                            detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                            dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                            dbr.SaveChanges();
                                        }
                                        else
                                        {
                                            valueLs = valueLs + ValuePkt;
                                            detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                            detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                            //Added by Shazana 9/10/2023
                                            detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                            dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                            dbr.SaveChanges();

                                        }
                                    }

                                }

                                msg = GlobalResEstate.msgAdd;
                                statusmsg = "success";
                            }
                        }

                        else
                        {
                            msg = GlobalResEstate.msgDataExist;
                            statusmsg = "warning";
                        }
                    }
                    else
                    {
                        msg = GlobalResEstate.msgError;
                        statusmsg = "warning";
                    }
                }
                else
                {
                    msg = GlobalResEstate.msgNoRecord;
                    statusmsg = "warning";
                }
            }

            bodyview = RenderRazorViewToString("WorkRecordList", EstateFunction.RecordWorkingList(dbr, SelectionCategory, SelectionData, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID), false);
            dbr.Dispose();
            return Json(new { msg, statusmsg, tablelisting = bodyview });
        }

        public JsonResult GetPktTrnsfr(string JnisAktvt, byte JnsPkt, byte TrnsfrLvl)
        {
            string host, catalog, user, pass = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DateTime? CurrentDate = timezone.gettimezone().Date;
            List<SelectListItem> PilihPeringkat = new List<SelectListItem>();
            string[] jenisKesukaran = new string[] { "HargaKesukaran", "HargaTambahan" };
            var kesukaranList = getConfig.GetWebConfigFlag2Filter(jenisKesukaran, NegaraID, SyarikatID);

            MVC_SYSTEM_Models dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var SelectPkt = dbr.tbl_PktPinjam.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_JenisPkt == JnsPkt && x.fld_EndDT >= CurrentDate).ToList();
            PilihPeringkat = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_KodPkt + " - " + s.fld_NamaPkt + " - (" + s.fld_SAPCode + ")" }), "Value", "Text").ToList();

            if (PilihPeringkat.Count == 0)
            {
                PilihPeringkat.Insert(0, (new SelectListItem { Text = "No Data", Value = "-" }));
            }

            dynamic kesukaran = new ExpandoObject();
            var tbl_PktHargaKesukaran = new List<tbl_PktHargaKesukaran>();

            if (SelectPkt.Count() > 0)
            {
                var PilihanPktID = SelectPkt.FirstOrDefault().fld_ID;
                var pktTransfer = dbr.tbl_PktPinjam.Where(x => x.fld_ID == PilihanPktID && x.fld_LadangID == LadangID && x.fld_EndDT >= CurrentDate).FirstOrDefault();
                var PilihanPkt = "";
                if (pktTransfer != null)
                {
                    NegaraID = pktTransfer.fld_NegaraIDAsal;
                    SyarikatID = pktTransfer.fld_SyarikatIDAsal;
                    WilayahID = pktTransfer.fld_WilayahIDAsal;
                    LadangID = pktTransfer.fld_LadangIDAsal;

                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                    PilihanPkt = dbrpkt.tbl_PktUtama.Where(x => x.fld_ID == pktTransfer.fld_OriginPktID).Select(s => s.fld_PktUtama).FirstOrDefault();
                }
                switch (JnsPkt)
                {
                    case 1:
                        var SelectPkt1 = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                        var firstSelectPkt = SelectPkt1.FirstOrDefault();
                        tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt.fld_PktUtama && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                        kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                        break;
                    case 2:
                        var SelectPkt2 = dbrpkt.tbl_SubPkt.Where(x => x.fld_KodPktUtama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                        var firstSelectPkt2 = SelectPkt2.FirstOrDefault();
                        tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt2.fld_Pkt && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                        kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                        break;
                    case 3:
                        var SelectPkt3 = dbrpkt.tbl_Blok.Where(x => x.fld_KodPktutama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                        //PilihPeringkat = new SelectList(SelectPkt3.Select(s => new SelectListItem { Value = s.fld_Blok, Text = s.fld_Blok + " - " + s.fld_NamaBlok }), "Value", "Text").ToList();
                        var firstSelectPkt3 = SelectPkt3.FirstOrDefault();
                        tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt3.fld_Blok && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                        kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                        break;
                }
            }

            dbr.Dispose();

            return Json(new { PilihPeringkat, kesukaran });
        }
        public JsonResult DeleteAttInfo(Guid Data, int SelectionCategory, string SelectionData, DateTime SelectDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            string bodyview2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string namelabel = "";
            string dateformat = GetConfig.GetData("dateformat2");
            string SelectDatePassback = string.Format("{0:" + dateformat + "}", SelectDate);
            bool datakrjaproceed = false;
            bool checkhadir = false;
            bool checkxhadir = false;
            string kodhdr = "";
            string kodhjn = "";
            string namepkj = "";
            decimal? GajiTerkumpul = 0;
            decimal? GajiHariIni = 0;
            List<tbl_Pkjmast> tbl_Pkjmast = new List<tbl_Pkjmast>();
            List<CustMod_Kerjahdr> CustMod_Kerjahdrs = new List<CustMod_Kerjahdr>();
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            tbl_KumpulanKerja tbl_KumpulanKerja = new tbl_KumpulanKerja();
            tbl_Kerjahdr tbl_Kerjahdr = new tbl_Kerjahdr();
            List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();
            GlobalFunction GlobalFunction = new GlobalFunction();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetKerjaHdr = dbr.tbl_Kerjahdr.Find(Data);

            if (EstateFunction.IndividuCheckLeaveTake(GetKerjaHdr.fld_Kdhdct, NegaraID, SyarikatID))
            {
                EstateFunction.LeaveAdd(dbr, GetKerjaHdr.fld_Tarikh.Value.Year, GetKerjaHdr.fld_Nopkj, GetKerjaHdr.fld_Kdhdct, NegaraID, SyarikatID, WilayahID, LadangID);
            }

            if (GetKerjaHdr != null)
            {
                var GetKerja = dbr.tbl_Kerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == GetKerjaHdr.fld_Tarikh && x.fld_Nopkj == GetKerjaHdr.fld_Nopkj).ToList();
                if (GetKerja.Count > 0)
                {
                    dbr.tbl_Kerja.RemoveRange(GetKerja);
                    GlobalFunction.CreateAuditTrail(dbr, "D", "Working Data (Delete Attendance)", getuserid.Value, GetKerja, GetKerja, NegaraID, SyarikatID, WilayahID, LadangID);
                }
                dbr.tbl_Kerjahdr.Remove(GetKerjaHdr);
                GlobalFunction.CreateAuditTrail(dbr, "D", "Attendance Data (Delete Attendance)", getuserid.Value, GetKerjaHdr, GetKerjaHdr, NegaraID, SyarikatID, WilayahID, LadangID);
                dbr.SaveChanges();
                msg = GlobalResEstate.msgDelete2;
                statusmsg = "success";
            }

            if (SelectionCategory == 1)
            {
                //check kehadiran
                KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj).ToList();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj }).Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).Select(s => s.fld_Nopkj).ToList();
                if (datainkrjhdr.Count() > 0)
                {
                    List<string> datainpkjmastexldatainkrjhdrs = datainpkjmast.Except(datainkrjhdr).ToList();

                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmastexldatainkrjhdrs.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    var pkjmasts2 = dbr.tbl_Pkjmast.Where(x => datainkrjhdr.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    var vw_Kerja_Bonus = dbr.vw_Kerja_Bonus.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                    var vw_Kerja_Hdr_Cuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                    var vw_Kerja_OT = dbr.vw_Kerja_OT.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();

                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_GajiHariIni = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        checkxhadir = true;
                    }

                    foreach (var pkjmast2 in pkjmasts2)
                    {
                        tbl_Kerjahdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast2.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).ToList();

                        GajiTerkumpul = dataKerja.Sum(s => s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        GajiTerkumpul = GajiTerkumpul + vw_Kerja_Bonus.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah);

                        GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);
                        GajiHariIni = GajiHariIni + vw_Kerja_Bonus.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah);

                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast2.fld_Nopkj, fld_Nama = pkjmast2.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = tbl_Kerjahdr.fld_Hujan == 0 ? "Tidak" : "Ya", fld_CreatedBy = getidentity.Username2(tbl_Kerjahdr.fld_CreatedBy), fld_CreatedDT = tbl_Kerjahdr.fld_CreatedDT, fld_UniqueID = tbl_Kerjahdr.fld_UniqueID, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_GajiHariIni = GajiHariIni.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        kodhdr = tbl_Kerjahdr.fld_Kdhdct;
                        kodhjn = tbl_Kerjahdr.fld_Hujan.ToString();
                        checkhadir = true;
                    }

                    if (checkxhadir && checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else if (!checkxhadir && checkhadir)
                    {
                        datakrjaproceed = true;
                    }
                    else if (checkxhadir && !checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else
                    {
                        datakrjaproceed = false;
                    }

                    disablesavebtn = true;
                }
                else
                {
                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmast.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();
                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_GajiHariIni = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                    }
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }

                //check kerja
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == SelectionData && datainpkjmast.Contains(x.fld_Nopkj) && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();

            }
            else
            {
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").FirstOrDefault();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj, j.fld_Kdhdct, j.fld_Hujan, j.fld_CreatedBy, j.fld_CreatedDT }).Where(x => x.fld_Nopkj.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).FirstOrDefault();
                //var datainkrjhdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == datainpkjmast.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                if (datainkrjhdr != null)
                {
                    var vw_Kerja_Bonus = dbr.vw_Kerja_Bonus.Where(x => x.fld_Nopkj == datainkrjhdr.fld_Nopkj && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                    var vw_Kerja_Hdr_Cuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == datainkrjhdr.fld_Nopkj && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                    var vw_Kerja_OT = dbr.vw_Kerja_OT.Where(x => x.fld_Nopkj == datainkrjhdr.fld_Nopkj && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();

                    var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datainkrjhdr.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).ToList();
                    GajiTerkumpul = dataKerja.Sum(s => s.fld_OverallAmount);
                    GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;

                    GajiTerkumpul = GajiTerkumpul + vw_Kerja_Bonus.Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Sum(s => s.fld_Jumlah);
                    GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);

                    GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);
                    GajiHariIni = GajiHariIni + vw_Kerja_Bonus.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_Jumlah);

                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(datainkrjhdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = datainkrjhdr.fld_Hujan == 0 ? "Tidak" : "Ya", fld_CreatedBy = getidentity.Username2(datainkrjhdr.fld_CreatedBy), fld_CreatedDT = datainkrjhdr.fld_CreatedDT, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_GajiHariIni = GajiHariIni.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                    kodhdr = datainkrjhdr.fld_Kdhdct;
                    kodhjn = datainkrjhdr.fld_Hujan.ToString();
                    disablesavebtn = true;
                    datakrjaproceed = true;
                    checkhadir = true;
                }
                else
                {
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Tarikh = SelectDate, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_GajiHariIni = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                    namelabel = datainpkjmast.fld_Nama;
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }
                namelabel = datainpkjmast.fld_Nama;

                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }

            foreach (var tbl_KerjaData in tbl_KerjaList)
            {
                namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                //Added by Shazana 29/11/2023
                decimal? fld_LsPktUtama = 0;
                fld_LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                if (fld_LsPktUtama == null)
                {
                    fld_LsPktUtama = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                }
                if (fld_LsPktUtama == null) { fld_LsPktUtama = 0; }

                CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount, fld_LsPktUtama = fld_LsPktUtama });
            }

            bodyview = RenderRazorViewToString("WorkerListDetailsCheck", CustMod_Kerjahdrs, NegaraID, SyarikatID, false);
            bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, false);

            string dayname = "";
            int getday = (int)SelectDate.DayOfWeek;
            dayname = GetTriager.getDayName(getday);
            dbr.Dispose();

            return Json(new { statusmsg, msg, tablelisting = bodyview, tablelisting2 = bodyview2, dayname, proceedstatus = disablesavebtn, namelabel = namelabel + " - " + SelectDatePassback, datakrjaproceed, kodhdr, kodhjn, checkhadir, checkxhadir });
        }

        public ActionResult DeleteWorkInfo(DateTime SelectDate, string SelectionData, int SelectionCategory, string pkt, string kodatvt)
        {
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            GlobalFunction GlobalFunction = new GlobalFunction();
            decimal? kadarbayarLast = 0;
            if (SelectionCategory == 1)
            {

                var deleteworkerinfo = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Kum == SelectionData && x.fld_KodPkt == pkt && x.fld_KodAktvt == kodatvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                dbr.tbl_Kerja.RemoveRange(deleteworkerinfo);
                GlobalFunction.CreateAuditTrail(dbr, "D", "Working Data (Delete Work)", getuserid.Value, deleteworkerinfo, deleteworkerinfo, NegaraID, SyarikatID, WilayahID, LadangID);
                dbr.SaveChanges();


                var senaraiJenisKong = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3).Select(x => x.fld_KodJnsAktvt).ToList();
                var kodaktivitiinfo = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_KodJnsAktvt == kodatvt).Select(x => x.fld_KodJnsAktvt).ToList();
                //var KongList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datakerja.nopkj && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();

                //Added by Shazana 9/10/2023 - Kiraan semula harga jenis aktiviti kong kerana 1 data telah didelete
                if (kodaktivitiinfo != null)
                {
                    foreach (var workerinfo in deleteworkerinfo)
                    {

                        var KongList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == workerinfo.fld_Nopkj && x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                        var PktList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == workerinfo.fld_Nopkj && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_KodPkt).ToList();
                        kadarbayarLast = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == workerinfo.fld_KodAktvt).Select(x => x.fld_Harga).FirstOrDefault(); //Modified by Shazana 22/12/2023  db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == workerinfo.fld_KodAktvt).Select(x => x.fld_Harga).FirstOrDefault(); //Modified by Shazana 22/12/2023 db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();
                        var JenisHadir = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == workerinfo.fld_Nopkj && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_Kdhdct).FirstOrDefault();
                        if (JenisHadir == "H02")
                        {
                            kadarbayarLast = kadarbayarLast * 2;
                        }
                        else if (JenisHadir == "H03")
                        {
                            kadarbayarLast = kadarbayarLast * 3;
                        }
                        List<string> datainpkjmastexldatainkrjhdrs = PktList.ToList();
                        decimal? LuasKong = 0;

                        //Dapatkan jumlah keseluruhan luas peringkat utama
                        foreach (var detailKong in KongList)
                        {
                            var LuasList = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && PktList.Contains(x.fld_PktUtama)).Select(x => x.fld_LsPktUtama).Sum();
                            decimal? LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == detailKong.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();

                            LuasKong = LuasKong + LsPktUtama;
                        }

                        int i = 0;
                        decimal? valueLs = 0;

                        //Added by Shazana 9/11/2023
                        // decimal? kadarbayarLast = db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();

                        foreach (var detaildatakerja in KongList)
                        {
                            //Modified by Shazana 9/11/2023
                            //decimal? kadarbayarLast = detaildatakerja.fld_KadarByr;
                            decimal? LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                            decimal? ValuePkt = 0;
                            if (LsPktUtama != 0)
                            { ValuePkt = Math.Round((Decimal)(LsPktUtama / LuasKong * kadarbayarLast), 2); }

                            i = i + 1;
                            int? dd = KongList.Count();
                            if (dd == i)
                            {
                                ValuePkt = kadarbayarLast - valueLs;
                                detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                //Added by Shazana 9/10/2023
                                detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                dbr.SaveChanges();
                            }
                            else
                            {
                                valueLs = valueLs + ValuePkt;
                                detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                                detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                                //Added by Shazana 9/10/2023
                                detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                                dbr.Entry(detaildatakerja).State = EntityState.Modified;
                                dbr.SaveChanges();

                            }
                        }

                    }
                }

            }
            else
            {
                var deleteworkerinfo = dbr.tbl_Kerja.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == SelectionData && x.fld_KodPkt == pkt && x.fld_KodAktvt == kodatvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                dbr.tbl_Kerja.RemoveRange(deleteworkerinfo);
                GlobalFunction.CreateAuditTrail(dbr, "D", "Working Data (Delete Work)", getuserid.Value, deleteworkerinfo, deleteworkerinfo, NegaraID, SyarikatID, WilayahID, LadangID);
                dbr.SaveChanges();


                //Added by Shazana 9/10/2023 - Kiraan semula harga kong kerana 1 data telah didelete
                ////nana tambah
                ///
                var senaraiJenisKong = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3).Select(x => x.fld_KodJnsAktvt).ToList();
                var KongList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                //Modified by Shazana 9/11/2023
                //var KongList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_JnisAktvt == "05" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                var PktList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && senaraiJenisKong.Contains(x.fld_JnisAktvt) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_KodPkt).ToList();
                kadarbayarLast = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodAktvt == kodatvt).Select(x => x.fld_Harga).FirstOrDefault(); //Modified by Shazana 22/12/2023  db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();
                var JenisHadir = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(x => x.fld_Kdhdct).FirstOrDefault();
                if (JenisHadir == "H02")
                {
                    kadarbayarLast = kadarbayarLast * 2;
                }
                else if (JenisHadir == "H03")
                {
                    kadarbayarLast = kadarbayarLast * 3;
                }
                List<string> datainpkjmastexldatainkrjhdrs = PktList.ToList();
                decimal? LuasKong = 0;

                //Dapatkan jumlah keseluruhan luas peringkat utama
                foreach (var detailKong in KongList)
                {
                    var LuasList = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && PktList.Contains(x.fld_PktUtama)).Select(x => x.fld_LsPktUtama).Sum();
                    decimal? LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == detailKong.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                    LuasKong = LuasKong + LsPktUtama;
                }

                int i = 0;
                decimal? valueLs = 0;

                //Added by Shazana 9/11/2023
                // decimal? kadarbayarLast = db.tbl_GajiMinimaLdg.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(x => x.fld_NilaiGajiMinima).FirstOrDefault();

                foreach (var detaildatakerja in KongList)
                {
                    //Modified by Shazana 9/11/2023
                    //decimal? kadarbayarLast = detaildatakerja.fld_KadarByr;
                    decimal? LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                    if (LsPktUtama == null)
                    {
                        LsPktUtama = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == detaildatakerja.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID).Select(x => x.fld_LsPkt).FirstOrDefault();
                    }
                    if (LsPktUtama == null)
                    {
                        LsPktUtama = 0;
                    }
                    decimal? ValuePkt = Math.Round((Decimal)(LsPktUtama / LuasKong * kadarbayarLast), 2);

                    i = i + 1;
                    int? dd = KongList.Count();
                    if (dd == i)
                    {
                        ValuePkt = kadarbayarLast - valueLs;
                        detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                        detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                        //Added by Shazana 9/10/2023
                        detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                        dbr.Entry(detaildatakerja).State = EntityState.Modified;
                        dbr.SaveChanges();
                    }
                    else
                    {
                        valueLs = valueLs + ValuePkt;
                        detaildatakerja.fld_Amount = ValuePkt == null ? 0 : ValuePkt;
                        detaildatakerja.fld_OverallAmount = ValuePkt == null ? 0 : ValuePkt;
                        //Added by Shazana 9/10/2023
                        detaildatakerja.fld_KadarByr = ValuePkt == null ? 0 : ValuePkt;
                        dbr.Entry(detaildatakerja).State = EntityState.Modified;
                        dbr.SaveChanges();

                    }
                }

            }
            msg = GlobalResEstate.msgDelete2;
            statusmsg = "success";
            bodyview = RenderRazorViewToString("WorkRecordList", EstateFunction.RecordWorkingList(dbr, SelectionCategory, SelectionData, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID), false);
            dbr.Dispose();
            return Json(new { msg, statusmsg, tablelisting = bodyview });
        }

        public ActionResult ActivityCodeDetails(string ActivityType)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);

            var CodeActivt = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == ActivityType && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).ToList();

            ViewBag.ActvtType = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJnsAktvt == ActivityType).Select(s => s.fld_Desc).FirstOrDefault();

            if (ActivityType == "99")
            {
                CodeActivt = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).ToList();

                ViewBag.ActvtType = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJnsAktvt == ActivityType).Select(s => s.fld_Desc).FirstOrDefault();

            }
            return View(CodeActivt);
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
            SelectionData.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            dbr.Dispose();
            return Json(SelectionData);
        }

        public JsonResult WorkerAvlbleChecking(int SelectionCategory, string SelectionData)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            bool disablesavebtn = true;
            bool datedisable = true;
            string namelabel = "";
            List<tbl_Pkjmast> tbl_Pkjmast = new List<tbl_Pkjmast>();
            List<CustMod_Kerjahdr> CustMod_Kerjahdrs = new List<CustMod_Kerjahdr>();
            tbl_KumpulanKerja tbl_KumpulanKerja = new tbl_KumpulanKerja();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (SelectionCategory == 1)
            {
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == tbl_KumpulanKerja.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();
                if (tbl_Pkjmast.Count() == 0)
                {
                    msg = GlobalResEstate.msgNoRecord; //tiada pekerja dalam kumpulan ini
                    statusmsg = "warning";
                    disablesavebtn = true;
                    datedisable = true;
                }
                else
                {
                    msg = GlobalResEstate.msgWorkerFound; //ada pekerja dalam kumpulan ini
                    statusmsg = "success";
                    disablesavebtn = false;
                    datedisable = false;
                }
                namelabel = tbl_KumpulanKerja.fld_Keterangan;
            }
            else
            {
                tbl_Pkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
                msg = GlobalResEstate.msgWorkerFound; //ada pekerja dalam kumpulan ini
                statusmsg = "success";
                disablesavebtn = false;
                datedisable = false;
            }

            foreach (var pkjmast1 in tbl_Pkjmast)
            {
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                if (tbl_KumpulanKerja != null)
                {
                    datedisable = false;
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "-", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_GajiHariIni = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                }
                else
                {
                    msg = GlobalResEstate.msgWorkerGroup; //ada pekerja dalam kumpulan ini
                    statusmsg = "warning";
                    datedisable = true;
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = "Tiada Kumpulan", fld_Status = "-", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = "-", fld_GajiHariIni = "-", fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                }
                if (SelectionCategory != 1)
                {
                    namelabel = pkjmast1.fld_Nama;
                }
            }

            bodyview = RenderRazorViewToString("WorkerListDetailsCheck", CustMod_Kerjahdrs, NegaraID, SyarikatID, false);
            dbr.Dispose();
            return Json(new { statusmsg, msg, tablelisting = bodyview, proceedstatus = disablesavebtn, namelabel, datedisable });
        }

        public JsonResult WorkerDateChecking(int SelectionCategory, string SelectionData, DateTime SelectDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            string bodyview = "";
            string bodyview2 = "";
            bool disablesavebtn = true;
            int KumpulanID = 0;
            string namelabel = "";
            string dateformat = GetConfig.GetData("dateformat2");
            string SelectDatePassback = string.Format("{0:" + dateformat + "}", SelectDate);
            bool datakrjaproceed = false;
            bool checkhadir = false;
            bool checkxhadir = false;
            string kodhdr = "";
            string kodhjn = "";
            string namepkj = "";
            decimal? GajiTerkumpul = 0;
            decimal? GajiHariIni = 0;
            bool CutOfDateStatus = false;
            string HariTerabaiStatus = "";
            List<tbl_Pkjmast> tbl_Pkjmast = new List<tbl_Pkjmast>();
            List<CustMod_Kerjahdr> CustMod_Kerjahdrs = new List<CustMod_Kerjahdr>();
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            tbl_KumpulanKerja tbl_KumpulanKerja = new tbl_KumpulanKerja();
            tbl_Kerjahdr tbl_Kerjahdr = new tbl_Kerjahdr();
            List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (SelectionCategory == 1)
            {
                //check kehadiran
                KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj.Trim()).ToList();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj }).Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).Select(s => s.fld_Nopkj.Trim()).ToList();

                var vw_Kerja_Bonus = dbr.vw_Kerja_Bonus.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                var vw_Kerja_Hdr_Cuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                var vw_Kerja_OT = dbr.vw_Kerja_OT.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                if (datainkrjhdr.Count() > 0)
                {
                    List<string> datainpkjmastexldatainkrjhdrs = datainpkjmast.Except(datainkrjhdr).ToList();

                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmastexldatainkrjhdrs.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    var pkjmasts2 = dbr.tbl_Pkjmast.Where(x => datainkrjhdr.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();

                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).ToList();
                        GajiTerkumpul = dataKerja.Sum(s => s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        GajiTerkumpul = GajiTerkumpul + vw_Kerja_Bonus.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj).Sum(s => s.fld_Jumlah);

                        GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);

                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_GajiHariIni = GajiHariIni.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        checkxhadir = true;
                    }

                    foreach (var pkjmast2 in pkjmasts2)
                    {
                        tbl_Kerjahdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Kum.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast2.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).ToList();
                        GajiTerkumpul = dataKerja.Sum(s => s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        GajiTerkumpul = GajiTerkumpul + vw_Kerja_Bonus.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah);

                        GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);
                        GajiHariIni = GajiHariIni + vw_Kerja_Bonus.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == pkjmast2.fld_Nopkj).Sum(s => s.fld_Jumlah);

                        HariTerabaiStatus = tbl_Kerjahdr.fld_Hujan == 0 ? "Tidak" : "Ya - " + EstateFunction.GetHariTerabaiJnsCharge(dbr, pkjmast2.fld_Nopkj, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID).ToUpper();
                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast2.fld_Nopkj, fld_Nama = pkjmast2.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(tbl_Kerjahdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = HariTerabaiStatus, fld_CreatedBy = getidentity.Username2(tbl_Kerjahdr.fld_CreatedBy), fld_CreatedDT = tbl_Kerjahdr.fld_CreatedDT, fld_UniqueID = tbl_Kerjahdr.fld_UniqueID, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_GajiHariIni = GajiHariIni.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                        kodhdr = tbl_Kerjahdr.fld_Kdhdct;
                        kodhjn = tbl_Kerjahdr.fld_Hujan.ToString();
                        checkhadir = true;
                    }

                    if (checkxhadir && checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else if (!checkxhadir && checkhadir)
                    {
                        datakrjaproceed = true;
                    }
                    else if (checkxhadir && !checkhadir)
                    {
                        datakrjaproceed = false;
                    }
                    else
                    {
                        datakrjaproceed = false;
                    }

                    msg = GlobalResEstate.msgDataExist;
                    statusmsg = "warning";
                    disablesavebtn = true;
                }
                else
                {
                    var pkjmasts1 = dbr.tbl_Pkjmast.Where(x => datainpkjmast.Contains(x.fld_Nopkj) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").OrderBy(o => o.fld_Nopkj).ToList();
                    foreach (var pkjmast1 in pkjmasts1)
                    {
                        tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == pkjmast1.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                        var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).ToList();
                        GajiTerkumpul = dataKerja.Sum(s => s.fld_OverallAmount);
                        GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);
                        GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                        GajiTerkumpul = GajiTerkumpul + vw_Kerja_Bonus.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Nopkj == pkjmast1.fld_Nopkj).Sum(s => s.fld_Jumlah);

                        CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = pkjmast1.fld_Nopkj, fld_Nama = pkjmast1.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_GajiHariIni = GajiHariIni.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                        namelabel = tbl_KumpulanKerja.fld_Keterangan;
                    }
                    msg = GlobalResEstate.msgNoRecord; //ada pekerja dalam kumpulan ini
                    statusmsg = "success";
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }

                //check kerja
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == SelectionData && datainpkjmast.Contains(x.fld_Nopkj) && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();

            }
            else
            {

                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").FirstOrDefault();
                tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == datainpkjmast.fld_KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).FirstOrDefault();
                var datainkrjhdr = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => j.fld_Nopkj, k => k.fld_Nopkj, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_StatusApproved, j.fld_Nopkj, j.fld_Kdhdct, j.fld_Hujan, j.fld_CreatedBy, j.fld_CreatedDT, j.fld_UniqueID }).Where(x => x.fld_Nopkj.Trim() == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan).FirstOrDefault();
                var vw_Kerja_Bonus = dbr.vw_Kerja_Bonus.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                var vw_Kerja_Hdr_Cuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                var vw_Kerja_OT = dbr.vw_Kerja_OT.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                if (datainkrjhdr != null)
                {

                    var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datainpkjmast.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).ToList();
                    GajiTerkumpul = dataKerja.Sum(s => s.fld_OverallAmount);
                    GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);
                    GajiHariIni = GajiHariIni + vw_Kerja_Bonus.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_Jumlah);


                    GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                    GajiTerkumpul = GajiTerkumpul + vw_Kerja_Bonus.Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Sum(s => s.fld_Jumlah);

                    HariTerabaiStatus = datainkrjhdr.fld_Hujan == 0 ? "Tidak" : "Ya - " + EstateFunction.GetHariTerabaiJnsCharge(dbr, datainkrjhdr.fld_Nopkj, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID).ToUpper();
                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Status = "Ada rekod", fld_HdrCt = GetConfig.GetWebConfigDesc(datainkrjhdr.fld_Kdhdct, "cuti", (int)NegaraID, (int)SyarikatID), fld_Hujan = HariTerabaiStatus, fld_CreatedBy = getidentity.Username2(datainkrjhdr.fld_CreatedBy), fld_CreatedDT = datainkrjhdr.fld_CreatedDT, fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_GajiHariIni = GajiHariIni.ToString(), fld_UniqueID = datainkrjhdr.fld_UniqueID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_Tarikh = SelectDate });
                    kodhdr = datainkrjhdr.fld_Kdhdct;
                    kodhjn = datainkrjhdr.fld_Hujan.ToString();
                    msg = GlobalResEstate.msgDataExist;
                    statusmsg = "warning";
                    disablesavebtn = true;
                    datakrjaproceed = true;
                    checkhadir = true;
                }
                else
                {
                    var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == datainpkjmast.fld_Nopkj && x.fld_Kum == tbl_KumpulanKerja.fld_KodKumpulan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == SelectDate.Month && x.fld_Tarikh.Value.Year == SelectDate.Year).ToList();
                    GajiTerkumpul = dataKerja.Sum(s => s.fld_OverallAmount);
                    GajiHariIni = dataKerja.Where(x => x.fld_Tarikh == SelectDate).Sum(s => s.fld_OverallAmount == null ? 0 : s.fld_OverallAmount);
                    GajiTerkumpul = GajiTerkumpul == null ? 0 : GajiTerkumpul;
                    GajiTerkumpul = GajiTerkumpul + vw_Kerja_Bonus.Where(x => x.fld_Nopkj == datainpkjmast.fld_Nopkj).Sum(s => s.fld_Jumlah_B) + vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == datainpkjmast.fld_Nopkj).Sum(s => s.fld_Jumlah) + vw_Kerja_OT.Where(x => x.fld_Nopkj == datainpkjmast.fld_Nopkj).Sum(s => s.fld_Jumlah);

                    CustMod_Kerjahdrs.Add(new CustMod_Kerjahdr() { fld_Nopkj = datainpkjmast.fld_Nopkj, fld_Nama = datainpkjmast.fld_Nama, fld_Kum = tbl_KumpulanKerja.fld_KodKumpulan, fld_Tarikh = SelectDate, fld_Status = "Tiada rekod", fld_HdrCt = "-", fld_Hujan = "-", fld_GajiTerkumpul = GajiTerkumpul.ToString(), fld_GajiHariIni = GajiHariIni.ToString(), fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
                    namelabel = datainpkjmast.fld_Nama;
                    msg = GlobalResEstate.msgNoRecord;
                    statusmsg = "success";
                    disablesavebtn = false;
                    datakrjaproceed = false;
                    checkxhadir = true;
                }
                namelabel = datainpkjmast.fld_Nama;

                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }

            foreach (var tbl_KerjaData in tbl_KerjaList)
            {
                namepkj = EstateFunction.PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                //Modified by Shazana 9/10/2023
                //CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
                //Added by Shazana 9/10/2023
                decimal? fld_LsPktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(x => x.fld_LsPktUtama).FirstOrDefault();
                //Added by Shazana 23/11/2023 -Kalau kong baru kira keluasan
                if (fld_LsPktUtama == null)
                {
                    fld_LsPktUtama = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == tbl_KerjaData.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(x => x.fld_LsPkt).FirstOrDefault();
                }
                //Added by Shazana 22/11/2023
                if (fld_LsPktUtama == null) { fld_LsPktUtama = 0; }
                CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_Unit = tbl_KerjaData.fld_Unit, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount, fld_LsPktUtama = fld_LsPktUtama });
            }

            CutOfDateStatus = EstateFunction.GetStatusCutProcess(dbr, SelectDate, NegaraID, SyarikatID, WilayahID, LadangID);

            bodyview = RenderRazorViewToString("WorkerListDetailsCheck", CustMod_Kerjahdrs, NegaraID, SyarikatID, CutOfDateStatus);
            bodyview2 = RenderRazorViewToString("WorkRecordList", CustMod_WorkerWorks, CutOfDateStatus);

            geterror.testservedoing(bodyview);
            geterror.testlog(SelectionCategory.ToString(), SelectionData, SelectDate.ToString());

            string dayname = "";
            int getday = (int)SelectDate.DayOfWeek;
            dayname = GetTriager.getDayName(getday);
            dbr.Dispose();

            int? LadangNegeriCode = int.Parse(GetLadang.GetLadangNegeriCode(LadangID));
            string AttCodeType = EstateFunction.GetAttType(NegaraID, SyarikatID, WilayahID, LadangID, SelectDate);

            return Json(new { statusmsg, msg, tablelisting = bodyview, tablelisting2 = bodyview2, dayname, proceedstatus = disablesavebtn, namelabel = namelabel + " - " + SelectDatePassback, datakrjaproceed, kodhdr, kodhjn, checkhadir, checkxhadir, CutOfDateStatus, AttCodeType });
        }

        public JsonResult GetAttandanceDetails(int SelectionCategory, string SelectionData)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            var dT = timezone.gettimezone();
            int day = dT.Day;
            string bodyview = "";
            var getDayShowLastMnthData = int.Parse(GetConfig.GetWebConfigValue("DateToShowLastMonth", 1, 1));
            dT = day > getDayShowLastMnthData ? dT : dT.AddMonths(-1);
            int year = dT.Year;
            int month = dT.Month;
            List<vw_Kerjahdr> vw_Kerjahdr = new List<vw_Kerjahdr>();
            List<CustMod_Kerjahdrgroup> CustMod_Kerjahdrgroups = new List<CustMod_Kerjahdrgroup>();

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (SelectionCategory == 1)
            {
                var trkhkjrhdrs = dbr.vw_Kerjahdr.Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_StatusApproved == 1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).Select(s => s.fld_Tarikh).Distinct().ToList();

                foreach (var trkhkjrhdr in trkhkjrhdrs)
                {
                    CustMod_Kerjahdrgroups.Add(new CustMod_Kerjahdrgroup() { fld_KodKumpulan = SelectionData, fld_Tarikh = trkhkjrhdr });
                }

                bodyview = RenderRazorViewToString("GroupAttandanceDetails", CustMod_Kerjahdrgroups, false);
            }
            else
            {
                vw_Kerjahdr = dbr.vw_Kerjahdr.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_StatusApproved == 1 && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                var dataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year).ToList();
                var vw_Kerja_Bonus = dbr.vw_Kerja_Bonus.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                var vw_Kerja_Hdr_Cuti = dbr.vw_Kerja_Hdr_Cuti.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                var vw_Kerja_OT = dbr.vw_Kerja_OT.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Tarikh).ToList();
                bodyview = RenderRazorViewToString("IndividuAttandanceDetails", vw_Kerjahdr, vw_Kerja_Bonus, vw_Kerja_Hdr_Cuti, vw_Kerja_OT, dataKerja, false);
            }
            dbr.Dispose();
            return Json(new { tablelisting2 = bodyview });
        }

        public JsonResult GetWorkCodeStatus(string WorkCode)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var GetWorkCodeStatus = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "cuti" && x.fldOptConfValue == WorkCode && x.fldDeleted == false).FirstOrDefault();

            db.Dispose();

            return Json(new { workstatus = GetWorkCodeStatus.fldOptConfFlag2 });
        }

        public JsonResult GetPkt(int JnisPkt, string JnisAktvt, byte TrnsfrLvl)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string[] jenisKesukaran = new string[] { "HargaKesukaran", "HargaTambahan" };
            var kesukaranList = getConfig.GetWebConfigFlag2Filter(jenisKesukaran, NegaraID, SyarikatID);

            List<SelectListItem> PilihPeringkat = new List<SelectListItem>();
            string hargaKesukaranMembaja = "";
            string hargaKesukaranMenuai = "";
            string hargaKesukaranMemunggah = "";//added by faeza 18.08.2021
            string CdKesukaranMembaja = "";
            string CdKesukaranMenuai = "";
            string CdKesukaranMemunggah = "";//added by faeza 18.08.2021

            dynamic kesukaran = new ExpandoObject();
            var tbl_PktHargaKesukaran = new List<tbl_PktHargaKesukaran>();

            //modified by faeza 18.08.2021
            switch (JnisPkt)
            {
                case 1:
                    var SelectPkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt.Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }), "Value", "Text").ToList();
                    CdKesukaranMembaja = SelectPkt.Select(s => s.fld_KesukaranMembajaPktUtama).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt.Select(s => s.fld_KesukaranMenuaiPktUtama).Take(1).FirstOrDefault();
                    CdKesukaranMemunggah = SelectPkt.Select(s => s.fld_KesukaranMemunggahPktUtama).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    hargaKesukaranMemunggah = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldOptConfValue == CdKesukaranMemunggah && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    var firstSelectPkt = SelectPkt.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt.fld_PktUtama && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
                case 2:
                    var SelectPkt2 = dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt2.Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }), "Value", "Text").ToList();
                    CdKesukaranMembaja = SelectPkt2.Select(s => s.fld_KesukaranMembajaPkt).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt2.Select(s => s.fld_KesukaranMenuaiPkt).Take(1).FirstOrDefault();
                    CdKesukaranMemunggah = SelectPkt2.Select(s => s.fld_KesukaranMemunggahPkt).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    hargaKesukaranMemunggah = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldOptConfValue == CdKesukaranMemunggah && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    var firstSelectPkt2 = SelectPkt2.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt2.fld_Pkt && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
                case 3:
                    var SelectPkt3 = dbr.tbl_Blok.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    PilihPeringkat = new SelectList(SelectPkt3.Select(s => new SelectListItem { Value = s.fld_Blok, Text = s.fld_Blok + " - " + s.fld_NamaBlok }), "Value", "Text").ToList();
                    CdKesukaranMembaja = SelectPkt3.Select(s => s.fld_KesukaranMembajaBlok).Take(1).FirstOrDefault();
                    CdKesukaranMenuai = SelectPkt3.Select(s => s.fld_KesukaranMenuaiBlok).Take(1).FirstOrDefault();
                    CdKesukaranMemunggah = SelectPkt3.Select(s => s.fld_KesukaranMemunggahBlok).Take(1).FirstOrDefault();
                    hargaKesukaranMembaja = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldOptConfValue == CdKesukaranMembaja && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault(); ;
                    hargaKesukaranMenuai = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldOptConfValue == CdKesukaranMenuai && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    hargaKesukaranMemunggah = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldOptConfValue == CdKesukaranMemunggah && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                    var firstSelectPkt3 = SelectPkt3.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt3.fld_KodPkt && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
            }

            dbr.Dispose();
            return Json(new { PilihPeringkat, kesukaran });
        }//end modified

        public JsonResult GetPlhnPkt(string PilihanPkt, int JnisPkt, string JnisAktvt, byte TrnsfrLvl)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string[] jenisKesukaran = new string[] { "HargaKesukaran", "HargaTambahan" };
            var kesukaranList = getConfig.GetWebConfigFlag2Filter(jenisKesukaran, NegaraID, SyarikatID);

            dynamic kesukaran = new ExpandoObject();
            var tbl_PktHargaKesukaran = new List<tbl_PktHargaKesukaran>();

            MVC_SYSTEM_Models dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (TrnsfrLvl == 1)
            {
                var PilihanPktID = int.Parse(PilihanPkt);
                var pktTransfer = dbr.tbl_PktPinjam.Where(x => x.fld_ID == PilihanPktID && x.fld_LadangID == LadangID).FirstOrDefault();
                //var pktHargaKesukaran = PktHargaKesukaran(dbr, JnisAktvt, PilihanPkt, LadangID);
                if (pktTransfer != null)
                {
                    NegaraID = pktTransfer.fld_NegaraIDAsal;
                    SyarikatID = pktTransfer.fld_SyarikatIDAsal;
                    WilayahID = pktTransfer.fld_WilayahIDAsal;
                    LadangID = pktTransfer.fld_LadangIDAsal;
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                    PilihanPkt = dbrpkt.tbl_PktUtama.Where(x => x.fld_ID == pktTransfer.fld_OriginPktID).Select(s => s.fld_PktUtama).FirstOrDefault();
                }
            }

            switch (JnisPkt) //modified by faeza 18.08.2021
            {
                case 1:
                    var SelectPkt = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    var firstSelectPkt = SelectPkt.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt.fld_PktUtama && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
                case 2:
                    var SelectPkt2 = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    var firstSelectPkt2 = SelectPkt2.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt2.fld_Pkt && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
                case 3:
                    var SelectPkt3 = dbrpkt.tbl_Blok.Where(x => x.fld_Blok == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    var firstSelectPkt3 = SelectPkt3.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt3.fld_Blok && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
            }
            dbr.Dispose();
            return Json(new { kesukaran });
        }

        public JsonResult GetActvtType(string Lejar)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> JnisAktvt = new List<SelectListItem>();
            List<SelectListItem> PilihanAktvt = new List<SelectListItem>();
            var JnisAktvtList = db.tbl_JenisAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false && x.fld_Lejar == Lejar).OrderBy(o => o.fld_KodJnsAktvt).ToList();
            string SelectJnisActvt = JnisAktvtList.Select(s => s.fld_KodJnsAktvt).Take(1).FirstOrDefault();
            JnisAktvt = new SelectList(JnisAktvtList.Select(s => new SelectListItem { Value = s.fld_KodJnsAktvt, Text = s.fld_Desc }), "Value", "Text").ToList();
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);
            var tbl_UpahAktiviti = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == SelectJnisActvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).ToList();
            PilihanAktvt = new SelectList(tbl_UpahAktiviti.OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt }), "Value", "Text").ToList();
            PilihanAktvt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            db.Dispose();
            return Json(new { JnisAktvt, PilihanAktvt });
        }

        public JsonResult GetAktvt(string PilihanPkt, int JnisPkt, string JnisAktvt, byte TrnsfrLvl)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string[] jenisKesukaran = new string[] { "HargaKesukaran", "HargaTambahan" };
            var kesukaranList = getConfig.GetWebConfigFlag2Filter(jenisKesukaran, NegaraID, SyarikatID);

            List<SelectListItem> PilihAktiviti = new List<SelectListItem>();
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);
            var tbl_UpahAktiviti = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == JnisAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).ToList();
            PilihAktiviti = new SelectList(tbl_UpahAktiviti.OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc + " (RM" + s.fld_Harga + ")" }), "Value", "Text").ToList();
            PilihAktiviti.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            var AktivitiToolTip = tbl_UpahAktiviti.OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();
            var JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_DisabledFlag != 5 && x.fld_KodJnsAktvt == JnisAktvt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();
            string Lain2JnsAtvt = "";
            MVC_SYSTEM_Models dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //added by kamalia 7/1/2021
            //tmbah pilihan aktiviti utk ladang

            //Kamalia 16/02/2020
            // var ladangterpilih = db.tbl_UpahAktiviti.Where(x => x.fld_KodAktvt == "0520" && x.fld_LadangID == LadangID && x.fld_Unit == "KONG").Select(s => s.fld_KodAktvt).FirstOrDefault();

            if (TrnsfrLvl == 1)
            {
                var PilihanPktID = int.Parse(PilihanPkt);
                var pktTransfer = dbr.tbl_PktPinjam.Where(x => x.fld_ID == PilihanPktID && x.fld_LadangID == LadangID).FirstOrDefault();
                //var pktHargaKesukaran = PktHargaKesukaran(dbr, JnisAktvt, PilihanPkt, LadangID);
                if (pktTransfer != null)
                {
                    NegaraID = pktTransfer.fld_NegaraIDAsal;
                    SyarikatID = pktTransfer.fld_SyarikatIDAsal;
                    WilayahID = pktTransfer.fld_WilayahIDAsal;
                    LadangID = pktTransfer.fld_LadangIDAsal;
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                    PilihanPkt = dbrpkt.tbl_PktUtama.Where(x => x.fld_ID == pktTransfer.fld_OriginPktID).Select(s => s.fld_PktUtama).FirstOrDefault();
                }
            }

            dynamic kesukaran = new ExpandoObject();
            var tbl_PktHargaKesukaran = new List<tbl_PktHargaKesukaran>();

            switch (JnisPkt) //modified by faeza 18.08.2021
            {
                case 1:
                    var SelectPkt = dbrpkt.tbl_PktUtama.Where(x => x.fld_PktUtama == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    var firstSelectPkt = SelectPkt.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt.fld_PktUtama && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
                case 2:
                    var SelectPkt2 = dbrpkt.tbl_SubPkt.Where(x => x.fld_Pkt == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    var firstSelectPkt2 = SelectPkt2.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt2.fld_Pkt && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
                case 3:
                    var SelectPkt3 = dbrpkt.tbl_Blok.Where(x => x.fld_Blok == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    var firstSelectPkt3 = SelectPkt3.FirstOrDefault();
                    tbl_PktHargaKesukaran = dbrpkt.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JnisAktvt && x.fld_PktUtama == firstSelectPkt3.fld_Blok && x.fld_LadangID == LadangID && x.fld_Deleted == false).ToList();
                    kesukaran = tbl_PktHargaKesukaran.Join(kesukaranList, a => a.fld_JenisHargaKesukaran, b => b.fldOptConfFlag1, (a, b) => new { a.fld_KodHargaKesukaran, a.fld_HargaKesukaran, a.fld_JenisHargaKesukaran, a.fld_KeteranganHargaKesukaran, b.fldOptConfFlag2 }).ToList();

                    break;
            }

            if (JenisAktiviti != null)
            {
                Lain2JnsAtvt = JenisAktiviti.fld_Desc;
            }
            else
            {
                Lain2JnsAtvt = "-";
            }
            db.Dispose();

            //Kamalia 16/02/2020
            return Json(new { PilihAktiviti, Lain2JnsAtvt, AktivitiToolTip, kesukaran });
        }


        //public JsonResult GetAktvt2()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> PilihanAktvtHT = new List<SelectListItem>();
        //    var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).FirstOrDefault();
        //    PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc + " (RM" + s.fld_Harga + ")" }), "Value", "Text").ToList();
        //    PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    var AktivitiToolTip = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();

        //    db.Dispose();
        //    return Json(new { PilihanAktvtHT, AktivitiToolTip });
        //}

        public JsonResult GetAktvt2()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string InitialLadang = ""; //added by faeza 28.02.2021
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> PilihanAktvtHT = new List<SelectListItem>();

            //added by faeza 28.02.2021
            var GetLadangSelectionName = GetNSWL.GetLadangDetail2(LadangID);
            InitialLadang = GetLadangSelectionName.fld_NamaLadang.Substring(0, 3);
            //end faeza

            //Modified by Shazana 10/11/2023
            //var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).OrderByDescending(x=>x.fld_KodJnsAktvt).FirstOrDefault();
            var getJenisActvtDetails = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag == 3 && x.fld_Deleted == false).Select(x => x.fld_KodJnsAktvt).ToList();

            //commented by faeza 28.02.2021
            //PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc + " (RM" + s.fld_Harga + ")" }), "Value", "Text").ToList();
            //PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //var AktivitiToolTip = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_Deleted == false).OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();

            //added by faeza 28.02.2021
            //if (InitialLadang == "FTP")
            //{
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);
            //Modified by SHazana 10/11/2023
            PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && getJenisActvtDetails.Contains(x.fld_KodJenisAktvt) && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc + " (RM" + s.fld_Harga + ")" }), "Value", "Text").ToList();
            PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //Modified by SHazana 10/11/2023
            //var AktivitiToolTip = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();
            var AktivitiToolTip = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && getJenisActvtDetails.Contains(x.fld_KodJenisAktvt) && x.fld_compcode == estateCostCenter).OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();

            db.Dispose();
            return Json(new { PilihanAktvtHT, AktivitiToolTip });
            //}
            //else
            //{
            //    PilihanAktvtHT = new SelectList(db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_KategoriAktvt != "00").OrderBy(o => o.fld_KodAktvt).Select(s => new SelectListItem { Value = s.fld_KodAktvt, Text = s.fld_KodAktvt + " - " + s.fld_Desc + " (RM" + s.fld_Harga + ")" }), "Value", "Text").ToList();
            //    PilihanAktvtHT.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //    var AktivitiToolTip = db.tbl_UpahAktiviti.Where(x => x.fld_Deleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJenisAktvt == getJenisActvtDetails.fld_KodJnsAktvt && x.fld_KategoriAktvt != "00").OrderBy(o => o.fld_KodAktvt).Select(s => new { Label = s.fld_KodAktvt + " - " + s.fld_Desc + " - RM" + s.fld_Harga }).ToList();
            //    db.Dispose();
            //    return Json(new { PilihanAktvtHT, AktivitiToolTip });
            //}
            //end faeza            
        }

        public JsonResult GetGLSAP(byte JnisPkt, string PilihanPkt, string PilihanAktvt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            int? getuserid = getidentity.ID(User.Identity.Name);
            string msg = "";
            string statusmsg = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string GLCode = "";
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);

            if (estateCostCenter == "1000")
            {
                if (EstateFunction.CheckSAPGLMap(dbr, JnisPkt, PilihanPkt, PilihanAktvt, NegaraID, SyarikatID, WilayahID, LadangID, false, "-", out GLCode, 0))
                {

                }
                else
                {
                    msg = GlobalResEstate.msgKodGLnotFound;
                    statusmsg = "warning";
                }
            }
            else
            {
                var mapGLs = db.tbl_MapGL.Where(x => x.fld_SyarikatID == 1 && (x.fld_Paysheet == "PT" || x.fld_Paysheet == "PA") && x.fld_KodAktvt == PilihanAktvt && x.fld_Deleted == false).ToList();
                var GL8800 = new List<GL8800>();
                foreach (var mapGL in mapGLs)
                {
                    GL8800.Add(new GL8800 { GL = mapGL.fld_KodGL, Paysheet = mapGL.fld_Paysheet });
                }
                if (GL8800.Count() >= 1)
                {
                    var gLs = GL8800.Select(s => new { s.GL, s.Paysheet }).Distinct().ToList();
                    if (gLs.Count() > 1)
                    {
                        GLCode = JsonConvert.SerializeObject(gLs);
                    }
                    else
                    {
                        msg = GlobalResEstate.msgKodGLnotFound;
                        statusmsg = "warning";
                    }
                }
                else
                {
                    msg = GlobalResEstate.msgKodGLnotFound;
                    statusmsg = "warning";
                }
            }

            dbr.Dispose();
            return Json(new { msg, statusmsg, GLCode });
        }

        public JsonResult GetMenuaiRate(DateTime SelectDate, int JnisPkt, string PilihanPkt, string kdhmnuai, byte TrnsfrLvl)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            decimal? kadarharga = 0;
            string msg = "";
            string statusmsg = "";
            bool closeform = true;
            bool YieldBracketFullMonth = true;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            MVC_SYSTEM_Models dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (TrnsfrLvl == 1)
            {
                var PilihanPktID = int.Parse(PilihanPkt);
                var pktTransfer = dbr.tbl_PktPinjam.Where(x => x.fld_ID == PilihanPktID && x.fld_LadangID == LadangID).FirstOrDefault();
                if (pktTransfer != null)
                {
                    JnisPkt = byte.Parse(pktTransfer.fld_JenisPkt.ToString());
                    NegaraID = pktTransfer.fld_NegaraIDAsal;
                    SyarikatID = pktTransfer.fld_SyarikatIDAsal;
                    WilayahID = pktTransfer.fld_WilayahIDAsal;
                    LadangID = pktTransfer.fld_LadangIDAsal;
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                    PilihanPkt = dbrpkt.tbl_PktUtama.Where(x => x.fld_ID == pktTransfer.fld_OriginPktID).Select(s => s.fld_PktUtama).FirstOrDefault();
                }
            }

            kadarharga = EstateFunction.YieldBracket(SelectDate, JnisPkt, PilihanPkt, kdhmnuai, dbrpkt, NegaraID, SyarikatID, WilayahID, LadangID, out YieldBracketFullMonth);
            if (kadarharga != 0)
            {
                msg = GlobalResEstate.msgWorkInfo;
                statusmsg = "success";
                closeform = false;
            }
            else
            {
                if (!YieldBracketFullMonth)
                {
                    msg = GlobalResEstate.msgYieldBracket;
                }
                else
                {
                    msg = GlobalResEstate.msgErrorData;
                }
                statusmsg = "warning";
                closeform = true;
            }
            return Json(new { kadarharga = string.Format("{0:0.00}", kadarharga), msg = msg, statusmsg = statusmsg, closeform = closeform });

        }

        public JsonResult CheckValidKwsnSkr(string JnisAktvt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string Result = "";

            var GetValidation = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kwsnskr" && x.fldOptConfValue == JnisAktvt && x.fldDeleted == false).FirstOrDefault();

            Result = GetValidation != null ? GetValidation.fldOptConfFlag2 : "3";

            return Json(Result);
        }

        //Kamalia 16/02/2020
        public JsonResult GetForm(int SelectionCategory, string SelectionData, byte JnisPkt, string PilihanPkt, string JnisAktvt, string KodAktvt, DateTime SelectDate, byte TrnsfrLvl)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string keteranganhdr, statushdr;
            string bodyview = "";
            short KadarByrn = 0;
            decimal? kadarharga = 0;
            //Added line by kamalia 30/4/2021
            decimal? kadarharga1 = 0;
            decimal? kadarharga2 = 0;
            string msg = "";
            string statusmsg = "";
            bool closeform = true;
            bool YieldBracketFullMonth = true;
            string GLCode = "";
            bool openRate = false;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<CustMod_AttWork> CustMod_AttWorkList = new List<CustMod_AttWork>();
            MVC_SYSTEM_Models dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);
            var PktUtama = new tbl_PktUtama();
            int? NegaraID2 = NegaraID;
            int? SyarikatID2 = SyarikatID;
            int? WilayahID2 = WilayahID;
            int? LadangID2 = LadangID;
            var IOCC = "";
            int transferLvlID = 0;
            if (TrnsfrLvl == 0)
            {
                switch (JnisPkt)
                {
                    case 1:
                        PktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_PktUtama == PilihanPkt).FirstOrDefault();
                        IOCC = PktUtama.fld_IOcode;
                        break;
                    case 2:
                        var subPkt = dbrpkt.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_Pkt == PilihanPkt).FirstOrDefault();
                        PktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_PktUtama == subPkt.fld_KodPktUtama).FirstOrDefault();
                        IOCC = PktUtama.fld_IOcode;
                        break;
                    case 3:
                        var blokPkt = dbrpkt.tbl_Blok.Where(x => x.fld_LadangID == LadangID && x.fld_Blok == PilihanPkt).FirstOrDefault();
                        PktUtama = dbrpkt.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_PktUtama == blokPkt.fld_KodPktutama).FirstOrDefault();
                        IOCC = PktUtama.fld_IOcode;
                        break;
                }
            }
            else
            {
                var PilihanPktID = int.Parse(PilihanPkt);
                var pktTransfer = dbr.tbl_PktPinjam.Where(x => x.fld_ID == PilihanPktID && x.fld_LadangID == LadangID).FirstOrDefault();
                if (pktTransfer != null)
                {
                    IOCC = pktTransfer.fld_SAPCode;
                    JnisPkt = byte.Parse(pktTransfer.fld_JenisPkt.ToString());
                    NegaraID2 = pktTransfer.fld_NegaraIDAsal;
                    SyarikatID2 = pktTransfer.fld_SyarikatIDAsal;
                    WilayahID2 = pktTransfer.fld_WilayahIDAsal;
                    LadangID2 = pktTransfer.fld_LadangIDAsal;
                    transferLvlID = pktTransfer.fld_OriginPktID.Value;
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID2.Value, SyarikatID2.Value, NegaraID2.Value);
                    dbrpkt = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
                    PilihanPkt = dbrpkt.tbl_PktUtama.Where(x => x.fld_ID == pktTransfer.fld_OriginPktID).Select(s => s.fld_PktUtama).FirstOrDefault();
                }
            }

            if (estateCostCenter == "1000")
            {
                if (EstateFunction.CheckSAPGLMap(dbrpkt, JnisPkt, PilihanPkt, KodAktvt, NegaraID2, SyarikatID2, WilayahID2, LadangID2, false, "-", out GLCode, transferLvlID))
                {
                    var tbl_JenisAktiviti = db.tbl_JenisAktiviti.Join(db.tbl_UpahAktiviti,
                        j => new { j.fld_NegaraID, j.fld_SyarikatID, KodJnsAktvt = j.fld_KodJnsAktvt },
                        k => new { k.fld_NegaraID, k.fld_SyarikatID, KodJnsAktvt = k.fld_KodJenisAktvt },
                        (j, k) => new
                        {
                            k.fld_NegaraID,
                            k.fld_SyarikatID,
                            j.fld_KodJnsAktvt,
                            k.fld_DisabledFlag,
                            k.fld_Harga,
                            k.fld_KodAktvt,
                            k.fld_KdhByr,
                            k.fld_Unit,
                            j.fld_Deleted,
                            k.fld_MaxProduktiviti,
                            k.fld_compcode
                        })
                            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJnsAktvt == JnisAktvt && x.fld_KodAktvt == KodAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).FirstOrDefault();
                    //tbl_JenisAktiviti.fld_DisabledFlag - 1 = kong box & kualiti box xde, 2 = kong je xde, 3 = kong je kluar

                    if (tbl_JenisAktiviti.fld_DisabledFlag != 3)
                    {
                        if (tbl_JenisAktiviti.fld_KdhByr == "B" || tbl_JenisAktiviti.fld_KdhByr == "A")
                            kadarharga = tbl_JenisAktiviti.fld_KdhByr == "B" ? tbl_JenisAktiviti.fld_Harga : EstateFunction.YieldBracket(SelectDate, JnisPkt, PilihanPkt, "A", dbrpkt, NegaraID2, SyarikatID2, WilayahID2, LadangID2, out YieldBracketFullMonth);
                        else
                        {
                            kadarharga = tbl_JenisAktiviti.fld_Harga;
                            openRate = true;
                        }
                    }
                    else
                    {
                        if (tbl_JenisAktiviti.fld_KdhByr == "B" || tbl_JenisAktiviti.fld_KdhByr == "A")
                        {
                            //Modified line by kamalia 30/4/2021
                            kadarharga1 = tbl_JenisAktiviti.fld_KdhByr == "B" ? tbl_JenisAktiviti.fld_Harga : EstateFunction.YieldBracket(SelectDate, JnisPkt, PilihanPkt, "A", dbrpkt, NegaraID2, SyarikatID2, WilayahID2, LadangID2, out YieldBracketFullMonth);
                            //Modified line by kamalia 23/8/2021
                            //Modified by Shazana 22/12/2023
                            //var getgajiminima = db.tbl_GajiMinimaLdg.Where(x => x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                            //kadarharga = getgajiminima != null ? getgajiminima.fld_NilaiGajiMinima : kadarharga1;
                            var getgajiminima = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID2 && x.fld_SyarikatID == SyarikatID2 && x.fld_KodAktvt == KodAktvt && x.fld_Deleted == false).FirstOrDefault();
                            kadarharga = getgajiminima != null ? getgajiminima.fld_Harga : kadarharga1;
                        }
                        else
                        {
                            kadarharga = tbl_JenisAktiviti.fld_Harga;
                            openRate = true;
                        }
                    }

                    kadarharga2 = kadarharga;
                    if (kadarharga != 0)
                    {
                        if (SelectionCategory == 1)
                        {
                            var checkatts = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nama, j.fld_Nopkj, j.fld_Kdhdct, k.fld_StatusApproved, k.fld_Kdaktf }).Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
                            foreach (var checkatt in checkatts)
                            {
                                GetConfig.GetCutiDesc(checkatt.fld_Kdhdct, "cuti", out keteranganhdr, out statushdr, out KadarByrn, NegaraID, SyarikatID);
                                if (tbl_JenisAktiviti.fld_DisabledFlag == 3)
                                {
                                    var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == checkatt.fld_Nopkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                                    if (SalaryIncrement != null)
                                    {
                                        kadarharga = SalaryIncrement + kadarharga2;
                                    }
                                    else
                                    {
                                        kadarharga = kadarharga2;
                                    }
                                }
                                CustMod_AttWorkList.Add(new CustMod_AttWork() { Nopkj = checkatt.fld_Nopkj, Namapkj = checkatt.fld_Nama, Keteranganhdr = keteranganhdr, statushdr = statushdr, disabletextbox = tbl_JenisAktiviti.fld_DisabledFlag, Kadar = kadarharga, KadarByrn = KadarByrn, KdhByr = tbl_JenisAktiviti.fld_KdhByr, Unit = tbl_JenisAktiviti.fld_Unit, MaximumHsl = tbl_JenisAktiviti.fld_MaxProduktiviti, EstateCostCenter = estateCostCenter, GLCode = GLCode, PaysheetID = "", OpenRate = openRate });
                            }
                        }
                        else
                        {
                            var checkatt = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nama, j.fld_Nopkj, j.fld_Kdhdct, k.fld_StatusApproved, k.fld_Kdaktf }).Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").FirstOrDefault();
                            GetConfig.GetCutiDesc(checkatt.fld_Kdhdct, "cuti", out keteranganhdr, out statushdr, out KadarByrn, NegaraID, SyarikatID);
                            if (tbl_JenisAktiviti.fld_DisabledFlag == 3)
                            {
                                var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == checkatt.fld_Nopkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                                if (SalaryIncrement != null)
                                {
                                    kadarharga = SalaryIncrement + kadarharga2;
                                }
                                else
                                {
                                    kadarharga = kadarharga2;
                                }
                            }
                            CustMod_AttWorkList.Add(new CustMod_AttWork() { Nopkj = checkatt.fld_Nopkj, Namapkj = checkatt.fld_Nama, Keteranganhdr = keteranganhdr, statushdr = statushdr, disabletextbox = tbl_JenisAktiviti.fld_DisabledFlag, Kadar = kadarharga, KadarByrn = KadarByrn, KdhByr = tbl_JenisAktiviti.fld_KdhByr, Unit = tbl_JenisAktiviti.fld_Unit, MaximumHsl = tbl_JenisAktiviti.fld_MaxProduktiviti, EstateCostCenter = estateCostCenter, GLCode = GLCode, PaysheetID = "", OpenRate = openRate });
                        }
                        msg = GlobalResEstate.msgWorkInfo;
                        statusmsg = "success";
                        closeform = false;
                    }
                    else
                    {
                        if (!YieldBracketFullMonth)
                        {
                            msg = GlobalResEstate.msgYieldBracket;
                        }
                        else
                        {
                            msg = GlobalResEstate.msgErrorData;
                        }
                        statusmsg = "warning";
                        closeform = true;
                    }
                    bodyview = RenderRazorViewToString("WorkingDetailsForm", CustMod_AttWorkList, false);
                }
                else
                {
                    msg = GlobalResEstate.msgKodGLnotFound;
                    statusmsg = "warning";
                    closeform = true;
                }
            }
            else
            {
                var tbl_MapGL = db.tbl_MapGL.Where(x => x.fld_SyarikatID == 1 && (x.fld_Paysheet == "PA" || x.fld_Paysheet == "PT") && x.fld_Deleted == false).ToList();
                var tbl_JenisAktiviti = db.tbl_JenisAktiviti.Join(db.tbl_UpahAktiviti,
                        j => new { j.fld_NegaraID, j.fld_SyarikatID, KodJnsAktvt = j.fld_KodJnsAktvt },
                        k => new { k.fld_NegaraID, k.fld_SyarikatID, KodJnsAktvt = k.fld_KodJenisAktvt },
                        (j, k) => new
                        {
                            k.fld_NegaraID,
                            k.fld_SyarikatID,
                            j.fld_KodJnsAktvt,
                            k.fld_DisabledFlag,
                            k.fld_Harga,
                            k.fld_KodAktvt,
                            k.fld_KdhByr,
                            k.fld_Unit,
                            j.fld_Deleted,
                            k.fld_MaxProduktiviti,
                            k.fld_compcode
                        })
                            .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodJnsAktvt == JnisAktvt && x.fld_KodAktvt == KodAktvt && x.fld_Deleted == false && x.fld_compcode == estateCostCenter).FirstOrDefault();
                //tbl_JenisAktiviti.fld_DisabledFlag - 1 = kong box & kualiti box xde, 2 = kong je xde, 3 = kong je kluar

                if (tbl_JenisAktiviti.fld_DisabledFlag != 3)
                {
                    if (tbl_JenisAktiviti.fld_KdhByr == "B" || tbl_JenisAktiviti.fld_KdhByr == "A")
                        kadarharga = tbl_JenisAktiviti.fld_KdhByr == "B" ? tbl_JenisAktiviti.fld_Harga : EstateFunction.YieldBracket(SelectDate, JnisPkt, PilihanPkt, "A", dbrpkt, NegaraID2, SyarikatID2, WilayahID2, LadangID2, out YieldBracketFullMonth);
                    else
                    {
                        kadarharga = tbl_JenisAktiviti.fld_Harga;
                        openRate = true;
                    }
                }
                else
                {
                    if (tbl_JenisAktiviti.fld_KdhByr == "B" || tbl_JenisAktiviti.fld_KdhByr == "A")
                    {
                        //Modified line by kamalia 30/4/2021
                        kadarharga1 = tbl_JenisAktiviti.fld_KdhByr == "B" ? tbl_JenisAktiviti.fld_Harga : EstateFunction.YieldBracket(SelectDate, JnisPkt, PilihanPkt, "A", dbrpkt, NegaraID2, SyarikatID2, WilayahID2, LadangID2, out YieldBracketFullMonth);
                        //Modified line by kamalia 23/8/2021
                        //Modified by Shazana 22/12/2023
                        //var getgajiminima = db.tbl_GajiMinimaLdg.Where(x => x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        //kadarharga = getgajiminima != null ? getgajiminima.fld_NilaiGajiMinima : kadarharga1;
                        var getgajiminima = db.tbl_UpahAktiviti.Where(x => x.fld_NegaraID == NegaraID2 && x.fld_SyarikatID== SyarikatID2 && x.fld_KodAktvt == KodAktvt && x.fld_Deleted == false).FirstOrDefault();
                        kadarharga = getgajiminima != null ? getgajiminima.fld_Harga : kadarharga1;

                    }
                    else
                    {
                        kadarharga = tbl_JenisAktiviti.fld_Harga;
                        openRate = true;
                    }
                }

                kadarharga2 = kadarharga;
                if (kadarharga != 0)
                {
                    var paysheetID = "";
                    if (SelectionCategory == 1)
                    {
                        var checkatts = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nama, j.fld_Nopkj, j.fld_Kdhdct, k.fld_StatusApproved, k.fld_Kdaktf, k.fld_Kdrkyt }).Where(x => x.fld_Kum == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").ToList();
                        foreach (var checkatt in checkatts)
                        {
                            GLCode = "";
                            if (checkatt.fld_Kdrkyt == "MA")
                            {
                                GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == KodAktvt && x.fld_Paysheet == "PT").Select(s => s.fld_KodGL).FirstOrDefault();
                                paysheetID = "PT";
                            }
                            else
                            {
                                GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == KodAktvt && x.fld_Paysheet == "PA").Select(s => s.fld_KodGL).FirstOrDefault();
                                paysheetID = "PA";
                            }
                            GetConfig.GetCutiDesc(checkatt.fld_Kdhdct, "cuti", out keteranganhdr, out statushdr, out KadarByrn, NegaraID, SyarikatID);
                            if (tbl_JenisAktiviti.fld_DisabledFlag == 3)
                            {
                                var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == checkatt.fld_Nopkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                                if (SalaryIncrement != null)
                                {
                                    kadarharga = SalaryIncrement + kadarharga2;
                                }
                                else
                                {
                                    kadarharga = kadarharga2;
                                }
                            }
                            CustMod_AttWorkList.Add(new CustMod_AttWork() { Nopkj = checkatt.fld_Nopkj, Namapkj = checkatt.fld_Nama, Keteranganhdr = keteranganhdr, statushdr = statushdr, disabletextbox = tbl_JenisAktiviti.fld_DisabledFlag, Kadar = kadarharga, KadarByrn = KadarByrn, KdhByr = tbl_JenisAktiviti.fld_KdhByr, Unit = tbl_JenisAktiviti.fld_Unit, MaximumHsl = tbl_JenisAktiviti.fld_MaxProduktiviti, EstateCostCenter = estateCostCenter, GLCode = GLCode, PaysheetID = paysheetID, OpenRate = openRate });
                        }
                    }
                    else
                    {
                        var checkatt = dbr.tbl_Kerjahdr.Join(dbr.tbl_Pkjmast, j => new { j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }, k => new { k.fld_Nopkj, k.fld_NegaraID, k.fld_SyarikatID, k.fld_WilayahID, k.fld_LadangID }, (j, k) => new { j.fld_Kum, j.fld_Tarikh, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID, k.fld_Nama, j.fld_Nopkj, j.fld_Kdhdct, k.fld_StatusApproved, k.fld_Kdaktf, k.fld_Kdrkyt }).Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").FirstOrDefault();
                        GLCode = "";
                        if (checkatt.fld_Kdrkyt == "MA")
                        {
                            GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == KodAktvt && x.fld_Paysheet == "PT").Select(s => s.fld_KodGL).FirstOrDefault();
                            paysheetID = "PT";
                        }
                        else
                        {
                            GLCode = tbl_MapGL.Where(x => x.fld_KodAktvt == KodAktvt && x.fld_Paysheet == "PA").Select(s => s.fld_KodGL).FirstOrDefault();
                            paysheetID = "PA";
                        }
                        GetConfig.GetCutiDesc(checkatt.fld_Kdhdct, "cuti", out keteranganhdr, out statushdr, out KadarByrn, NegaraID, SyarikatID);
                        if (tbl_JenisAktiviti.fld_DisabledFlag == 3)
                        {
                            var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == checkatt.fld_Nopkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                            if (SalaryIncrement != null)
                            {
                                kadarharga = SalaryIncrement + kadarharga2;
                            }
                            else
                            {
                                kadarharga = kadarharga2;
                            }
                        }
                        CustMod_AttWorkList.Add(new CustMod_AttWork() { Nopkj = checkatt.fld_Nopkj, Namapkj = checkatt.fld_Nama, Keteranganhdr = keteranganhdr, statushdr = statushdr, disabletextbox = tbl_JenisAktiviti.fld_DisabledFlag, Kadar = kadarharga, KadarByrn = KadarByrn, KdhByr = tbl_JenisAktiviti.fld_KdhByr, Unit = tbl_JenisAktiviti.fld_Unit, MaximumHsl = tbl_JenisAktiviti.fld_MaxProduktiviti, EstateCostCenter = estateCostCenter, GLCode = GLCode, PaysheetID = paysheetID, OpenRate = openRate });
                    }
                    msg = GlobalResEstate.msgWorkInfo;
                    statusmsg = "success";
                    closeform = false;
                }
                else
                {
                    if (!YieldBracketFullMonth)
                    {
                        msg = GlobalResEstate.msgYieldBracket;
                    }
                    else
                    {
                        msg = GlobalResEstate.msgErrorData;
                    }
                    statusmsg = "warning";
                    closeform = true;
                }
                bodyview = RenderRazorViewToString("WorkingDetailsForm", CustMod_AttWorkList, false);
            }
            dbr.Dispose();
            return Json(new { tablelisting = bodyview, msg, statusmsg, closeform, GLCode, IOCC });
        }
        public string RenderRazorViewToString(string viewname, object dataview, int? NegaraID, int? SyarikatID, bool CutOfDateStatus)
        {
            ViewData.Model = dataview;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.CutOfDateStatus = CutOfDateStatus;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public string RenderRazorViewToString(string viewname, object dataview, bool CutOfDateStatus)
        {
            ViewData.Model = dataview;
            ViewBag.CutOfDateStatus = CutOfDateStatus;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        public string RenderRazorViewToString(string viewname, object dataview, object dataview2, object dataview3, object dataview4, object param, bool CutOfDateStatus)
        {
            ViewData.Model = dataview;
            ViewBag.Param = param;
            ViewBag.Bonus = dataview2;
            ViewBag.Cuti = dataview3;
            ViewBag.Ot = dataview4;
            ViewBag.CutOfDateStatus = CutOfDateStatus;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
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
        public ActionResult Work()
        {
            return View();
        }
    }
}