using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.CustomModels;
using System.ServiceModel;
using System.Net;
using MVC_SYSTEM.SAPPostIntegration;
using Microsoft.Ajax.Utilities;
//Add Shazana 17/11/2022
using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.ViewingModels;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class BizTransacController : Controller
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
        private GlobalFunction GlobalFunction = new GlobalFunction();

        //Shazana 17/11/2022
        private GetWilayah getwilyah = new GetWilayah();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_MasterModels dbCorp = new MVC_SYSTEM_MasterModels();
        private DatabaseAction DatabaseAction = new DatabaseAction();
        private SendEmailNotification SendEmailNotification = new SendEmailNotification();
        private MVC_SYSTEM_Models dbest2 = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Viewing dbest3 = new MVC_SYSTEM_Viewing();

        // GET: BizTransac
        public ActionResult Index()
        {
            ViewBag.ClosingTransaction = "class = active";
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            ////Shazana 17/11/2022
            //ViewBag.BizTransacMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "BizTransac" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fld_Val", "fld_Desc");
            if (GetIdentity.SuperPowerUser(User.Identity.Name).ToString() == "0")
            {
                //Modify by Shazana 15/2/2023
                //ViewBag.BizTransacMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "BizTransac" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(x => x.fld_ID).Take(2), "fld_Val", "fld_Desc");
                ViewBag.BizTransacMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "BizTransac" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(x => x.fld_Desc).Take(2), "fld_Val", "fld_Desc");
            }
            else
            {
                //Modify by Shazana 15/2/2023
                //ViewBag.BizTransacMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "BizTransac" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fld_Val", "fld_Desc");
                ViewBag.BizTransacMenu = new SelectList(db.tblMenuLists.Where(x => x.fld_Flag == "BizTransac" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(x => x.fld_Desc), "fld_Val", "fld_Desc");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string BizTransacMenu)
        {
            return RedirectToAction(BizTransacMenu, "BizTransac");
        }

        public ActionResult SAPPosting()
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

            return View();
        }

        public ViewResult _SAPPostingSearch(int MonthList, int YearList)
        {
            if (MonthList != 0 && YearList != 0)
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = getidentity.ID(User.Identity.Name);
                string host, catalog, user, pass = "";
                CustMod_SAPPostingData CustMod_SAPPostingData = new CustMod_SAPPostingData();

                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                var GetSapPostData = dbr.tbl_SAPPostRef.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_Purpose == 1 && x.fld_StatusProceed == false).FirstOrDefault();

                if (GetSapPostData != null)
                {
                    CustMod_SAPPostingData.GetSAPPostRef = GetSapPostData;
                    var GetSapPostVendor = dbr.tbl_SAPPostVendorDataDetails.Where(x => x.fld_SAPPostRefID == GetSapPostData.fld_ID).FirstOrDefault();
                    CustMod_SAPPostingData.GetSAPPostVendorDataDetails = GetSapPostVendor;
                    var GetSapPostGL = dbr.tbl_SAPPostGLIODataDetails.Where(x => x.fld_SAPPostRefID == GetSapPostData.fld_ID).ToList();
                    CustMod_SAPPostingData.SAPPostGLIODataDetails = GetSapPostGL;
                    return View(CustMod_SAPPostingData);
                }
                else
                {
                    ViewBag.Message = "Tiada data";
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "Sila pilih bulan dan tahun";
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult _SAPSaveData(CustMod_SAPPostingSave CustMod_SAPPostingSave)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string  host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";

            if (ModelState.IsValid)
            {
                try
                {
                    int? getuserid = getidentity.ID(User.Identity.Name);
                    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
                    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

                    var GetSAPPostRefDetail = dbr.tbl_SAPPostRef.Find(CustMod_SAPPostingSave.PostingID);
                    GetSAPPostRefDetail.fld_CpdName = CustMod_SAPPostingSave.Name;
                    GetSAPPostRefDetail.fld_CpdName2 = CustMod_SAPPostingSave.Name2;
                    GetSAPPostRefDetail.fld_PostingDate = CustMod_SAPPostingSave.PostingDate;
                    GetSAPPostRefDetail.fld_InvoiceDate = CustMod_SAPPostingSave.InvoiceDate;
                    GetSAPPostRefDetail.fld_RefNo = CustMod_SAPPostingSave.RefNo;
                    GetSAPPostRefDetail.fld_ModifiedBy = getuserid;
                    GetSAPPostRefDetail.fld_ModifiedDT = timezone.gettimezone();
                    dbr.Entry(GetSAPPostRefDetail).State = EntityState.Modified;
                    dbr.SaveChanges();

                    var GetSAPPsitVendorDetails = dbr.tbl_SAPPostVendorDataDetails.Where(x => x.fld_SAPPostRefID == CustMod_SAPPostingSave.PostingID).FirstOrDefault();
                    GetSAPPsitVendorDetails.fld_VendorNo = CustMod_SAPPostingSave.VendorNo;
                    GetSAPPsitVendorDetails.fld_Desc = CustMod_SAPPostingSave.DescVendor;
                    dbr.Entry(GetSAPPsitVendorDetails).State = EntityState.Modified;
                    dbr.SaveChanges();

                    msg = "Berjaya disimpan.";
                    statusmsg = "success";
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    msg = "Gagal disimpan.";
                    statusmsg = "warning";
                }
            }
            
            return Json(new { msg, statusmsg });
        }
        
        [HttpPost]
        public ActionResult _PostToSAP(Guid PostingID, string SAPUsername, string SAPPassword, int Month, int Year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";

            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string MonthString = Month.ToString();
            if (MonthString.Length == 1)
            {
                MonthString = "0" + MonthString;
            }

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            NetworkCredential Cred = new NetworkCredential();
            BAPIACHE09 InputDataDocHeader = new BAPIACHE09();
            BAPIACPA09 InputDataCustPD = new BAPIACPA09();
            BAPIACGL09 InputDataAccGL_ = new BAPIACGL09();
            BAPIACAP09 InputDataAccPay_ = new BAPIACAP09();
            BAPIACTX09 InputDataAccTax_ = new BAPIACTX09();
            BAPIACCR09 InputDataCurAmt_ = new BAPIACCR09();
            BAPIACCR09 InputDataCurAmt2_ = new BAPIACCR09();
            BAPIRET2 OutputReturn_ = new BAPIRET2();

            EndpointAddress endpoint = new EndpointAddress("http://ciFLQ.felhqr.myfelda:8001/sap/bc/srt/rfc/sap/zwsopmsfiar01/300/zwsopmsfiar01/zwsopmsfiar01");
            ZFMOPMSFIAR01Response SAPPostingResponse = new ZFMOPMSFIAR01Response();
            zwsopmsfiar01Client SAPPosting = new zwsopmsfiar01Client(binding, endpoint);
            ZFMOPMSFIAR01 SAPPostingCollectionData = new ZFMOPMSFIAR01();
            int i = 0;
            try
            {
                var GetSAPPostRefDetail = dbr.tbl_SAPPostRef.Find(PostingID);
                var GetSAPPostVendorDetails = dbr.tbl_SAPPostVendorDataDetails.Where(x => x.fld_SAPPostRefID == PostingID).FirstOrDefault();
                var GetSAPPostGLIODetails = dbr.tbl_SAPPostGLIODataDetails.Where(x => x.fld_SAPPostRefID == PostingID).OrderBy(o => o.fld_ItemNo).ToList();

                Cred.UserName = SAPUsername;
                Cred.Password = SAPPassword;
                SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
                SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
                SAPPosting.Open();

                InputDataDocHeader.USERNAME = SAPUsername;
                InputDataDocHeader.HEADER_TXT = "OPMS";
                InputDataDocHeader.COMP_CODE = GetSAPPostRefDetail.fld_CompCode;
                InputDataDocHeader.DOC_DATE = GetSAPPostRefDetail.fld_InvoiceDate.Value.ToString("yyyy-MM-dd");
                InputDataDocHeader.PSTNG_DATE = GetSAPPostRefDetail.fld_PostingDate.Value.ToString("yyyy-MM-dd");
                InputDataDocHeader.DOC_TYPE = "KR";
                InputDataDocHeader.REF_DOC_NO = GetSAPPostRefDetail.fld_RefNo;

                InputDataCustPD.NAME = GetSAPPostRefDetail.fld_CpdName;
                InputDataCustPD.NAME_2 = GetSAPPostRefDetail.fld_CpdName2;
                InputDataCustPD.POSTL_CODE = GetSAPPostRefDetail.fld_PostCode;
                InputDataCustPD.CITY = GetSAPPostRefDetail.fld_City;
                InputDataCustPD.COUNTRY = GetSAPPostRefDetail.fld_Country;
                InputDataCustPD.STREET = GetSAPPostRefDetail.fld_City;

                //InputDataAccGL_.ITEMNO_ACC = "0000000002";
                //InputDataAccGL_.GL_ACCOUNT = "0076510010";
                //InputDataAccGL_.ITEM_TEXT = "GL 1";
                //InputDataAccGL_.TAX_CODE = "TZ";
                //InputDataAccGL_.COSTCENTER = "0113005000";
                //InputDataAccGL_.ORDERID = "C113005203";
                List<BAPIACGL09> InputDataAccGL = new List<BAPIACGL09>();
                var CC = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).Select(s => s.fld_CostCentre).FirstOrDefault();
                foreach (var GetSAPPostGLIODetail in GetSAPPostGLIODetails)
                {
                    InputDataAccGL.Add(new BAPIACGL09() { ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostGLIODetail.fld_ItemNo), GL_ACCOUNT = GetSAPPostGLIODetail.fld_GL, ITEM_TEXT = GetSAPPostGLIODetail.fld_Desc, TAX_CODE = "Q4", COSTCENTER = CC, ORDERID = GetSAPPostGLIODetail.fld_IO });
                }

                //
                InputDataAccPay_.ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostVendorDetails.fld_ItemNo);
                InputDataAccPay_.VENDOR_NO = GetSAPPostVendorDetails.fld_VendorNo;
                InputDataAccPay_.PMNTTRMS = "Z030";
                //InputDataAccPay_.BLINE_DATE = GetSAPPostVendorDetails.fld_BaseDate.Value.ToString("yyyy-MM-dd");
                InputDataAccPay_.ITEM_TEXT = GetSAPPostVendorDetails.fld_Desc;
                InputDataAccPay_.ALLOC_NMBR = "OPMS POSTING";

                BAPIACAP09[] InputDataAccPay = new BAPIACAP09[] { InputDataAccPay_ };

                InputDataCurAmt_.ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostVendorDetails.fld_ItemNo);
                InputDataCurAmt_.CURRENCY = GetSAPPostVendorDetails.fld_Currency;
                InputDataCurAmt_.AMT_DOCCUR = GetSAPPostVendorDetails.fld_Amount.Value;
                InputDataCurAmt_.AMT_BASE = 0;

                //InputDataCurAmt2_.ITEMNO_ACC = "0000000002";
                //InputDataCurAmt2_.CURRENCY = "RM";
                //InputDataCurAmt2_.AMT_DOCCUR = 2000;
                //InputDataCurAmt2_.AMT_BASE = 0;

                List<BAPIACCR09> InputDataCurAmt = new List<BAPIACCR09>();

                InputDataCurAmt.Add(new BAPIACCR09() { ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostVendorDetails.fld_ItemNo), CURRENCY = GetSAPPostVendorDetails.fld_Currency, AMT_DOCCUR = GetSAPPostVendorDetails.fld_Amount.Value, AMT_BASE = 0 });

                foreach (var GetSAPPostGLIODetail in GetSAPPostGLIODetails)
                {
                    InputDataCurAmt.Add(new BAPIACCR09() { ITEMNO_ACC = GetTriager.GetSAPItemNo(GetSAPPostGLIODetail.fld_ItemNo), CURRENCY = GetSAPPostGLIODetail.fld_Currency, AMT_DOCCUR = GetSAPPostGLIODetail.fld_Amount.Value, AMT_BASE = 0 });
                }

                OutputReturn_.FIELD = null;
                OutputReturn_.ID = null;
                OutputReturn_.LOG_MSG_NO = null;
                OutputReturn_.LOG_NO = null;
                OutputReturn_.MESSAGE = null;
                OutputReturn_.MESSAGE_V1 = null;
                OutputReturn_.MESSAGE_V2 = null;
                OutputReturn_.MESSAGE_V3 = null;
                OutputReturn_.MESSAGE_V4 = null;
                OutputReturn_.NUMBER = null;
                OutputReturn_.PARAMETER = null;
                OutputReturn_.ROW = 0;
                OutputReturn_.SYSTEM = null;
                OutputReturn_.TYPE = null;

                BAPIRET2[] OutputReturn = new BAPIRET2[] { OutputReturn_ };

                SAPPostingCollectionData.DOCUMENTHEADER = InputDataDocHeader;
                SAPPostingCollectionData.CUSTOMERCPD = InputDataCustPD;
                SAPPostingCollectionData.ACCOUNTGL = InputDataAccGL.ToArray();
                SAPPostingCollectionData.ACCOUNTPAYABLE = InputDataAccPay;
                //SAPPostingCollectionData.ACCOUNTTAX = InputDataAccTax;
                SAPPostingCollectionData.CURRENCYAMOUNT = InputDataCurAmt.ToArray();
                SAPPostingCollectionData.RETURN = OutputReturn;
                SAPPostingResponse = SAPPosting.ZFMOPMSFIAR01(SAPPostingCollectionData);
                
                List<tbl_SAPPostReturn> SAPReturnList = new List<tbl_SAPPostReturn>();

                if (SAPPostingResponse.RETURN.Count() > 1)
                {
                    var CheckSuccess = SAPPostingResponse.RETURN.Where(x => x.TYPE == "S").Count();
                    EstateFunction.DeleteReturnSAPPost(PostingID, dbr);
                    int NoSort = 1;
                    foreach (var SAPReturn in SAPPostingResponse.RETURN)
                    {
                        if (SAPReturn.TYPE == "S")
                        {
                            var GetSkbToPost = dbr.tbl_Skb.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Bulan == MonthString && x.fld_Tahun == Year).FirstOrDefault();
                            if (GetSkbToPost != null)
                            {
                                GetSkbToPost.fld_NoSkb = SAPReturn.MESSAGE_V2;
                                dbr.Entry(GetSkbToPost).State = EntityState.Modified;
                                dbr.SaveChanges();
                            }
                        }
                        SAPReturnList.Add(new tbl_SAPPostReturn() { fld_SortNo = NoSort, fld_Msg1 = SAPReturn.MESSAGE, fld_Msg2 = SAPReturn.MESSAGE_V1, fld_Msg3 = SAPReturn.MESSAGE_V2, fld_Msg4 = SAPReturn.MESSAGE_V3,  fld_SAPPostRefID = PostingID });
                        NoSort++;
                    }
                    EstateFunction.AddReturnSAPPost(dbr, SAPReturnList);
                    if (CheckSuccess > 0)
                    {
                        msg = "Berjaya dihantar.";
                        statusmsg = "success";
                    }
                    else
                    {
                        msg = "Tidak berjaya dihantar. Sila semak data yang dihantar.";
                        statusmsg = "warning";
                    }
                }
                else
                {
                    EstateFunction.DeleteReturnSAPPost(PostingID, dbr);
                    int NoSort = 1;
                    foreach (var SAPReturn in SAPPostingResponse.RETURN)
                    {
                        SAPReturnList.Add(new tbl_SAPPostReturn() { fld_SortNo = NoSort, fld_Msg1 = SAPReturn.MESSAGE, fld_Msg2 = SAPReturn.MESSAGE_V1, fld_Msg3 = SAPReturn.MESSAGE_V2, fld_Msg4 = SAPReturn.MESSAGE_V3,  fld_SAPPostRefID = PostingID });
                        NoSort++;
                    }
                    EstateFunction.AddReturnSAPPost(dbr, SAPReturnList);
                    msg = "Berjaya dihantar.";
                    statusmsg = "success";
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = "Tidak berjaya dihantar. Sila semak data yang dihantar.";
                statusmsg = "warning";
            }
            return Json(new { msg, statusmsg });
        }

        public ActionResult SAPReturnReport(Guid PostingID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";

            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetSAPReportList = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == PostingID).OrderBy(o => o.fld_SortNo).ToList();

            return View("SAPReturnReport", GetSAPReportList);
        }

        //farahin - testing SAP 
        public ActionResult PostingSAP(string filter)
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

            ViewBag.YearList = yearlist;
            ViewBag.ClosingTransaction = "class = active";

            return View();
        }

        public ActionResult _PostingSAP(int? MonthList, int? YearList)
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

            var postingData = new List<vw_SAPPostData>();

            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()))
            {
                postingData = dbr.vw_SAPPostData
                    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).ToList(); //modified by kamalia 21/3/2022 reverted back

                var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList).FirstOrDefault();
                ViewBag.ClosingStatus = ClosingTransaction.fld_StsTtpUrsNiaga;

                //farahin tambah - 30/12/2021
                var statusProceedA2 = dbr.tbl_SAPPostRef.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_DocType == "A2").FirstOrDefault();

                if (statusProceedA2 != null)
                {
                    ViewBag.statusProceedA2 = statusProceedA2.fld_StatusProceed;
                }
                else
                {
                    ViewBag.statusProceedA2 = null;
                }

                var statusProceedKR = dbr.tbl_SAPPostRef.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_DocType == "KR").FirstOrDefault();

                if (statusProceedKR != null)
                {
                    ViewBag.statusProceedKR = statusProceedKR.fld_StatusProceed;
                }
                else
                {
                    ViewBag.statusProceedKR = null;
                }
                //end by farahin

                //added by sarah 23/11/2022
                if (statusProceedA2 != null)
                {
                    ViewBag.RefNoA2 = statusProceedA2.fld_RefNo;
                }
                else
                {
                    ViewBag.RefNoA2 = null;
                }


                if (statusProceedKR != null)
                {
                    ViewBag.RefNoKR = statusProceedKR.fld_RefNo;
                }
                else
                {
                    ViewBag.RefNoKR = null;
                }
                //ended by sarah

                if (!postingData.Any())
                {
                    message = GlobalResEstate.msgErrorSearch;
                }
            }

            else
            {
                message = GlobalResEstate.msgChooseMonthYear;
            }

            ViewBag.Message = message;
            //added by kamalia 24/11/21
            ViewBag.Existing = db.tbl_SokPermhnWang.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_LadangID == LadangID).Any();
            //modified by kamalia 17/12/21
            ViewBag.GetSokongWil = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_SokongWilGM_Status == 1).Any();
            //  ViewBag.GetTerimaHQ = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TerimaHQ_Status == 1).Any();
            //end
            ViewBag.GetTolakHQ = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakHQ_Status == 1).Any();
            ViewBag.GetTolakWilGM = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWilGM_Status == 1).Any();
            ViewBag.GetTolakWil = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWil_Status == 1).Any();
            ViewBag.GetJumPermohonan = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(s => s.fld_JumlahPermohonan).FirstOrDefault();
            //end 
            //added sarah 23/11/2022
            ViewBag.ReferenceNo = dbr.tbl_SAPPostRef.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(s => s.fld_RefNo).FirstOrDefault();
            //ended by sarah 

            //Added by Shazana 21/12/2022
            var verifystatus = dbr.tbl_SAPPostRef.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_DocType == "KR").Select(s => s.fld_RefNo).FirstOrDefault();
            if (verifystatus == null || verifystatus == "")
            { ViewBag.berjayaverify = "0"; }
            else
            { ViewBag.berjayaverify = "1"; }

            var Ladang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(x => x.fld_LdgCode).FirstOrDefault();
            ViewBag.Ladang = Ladang;

            ViewBag.Year = YearList;
            ViewBag.Month = MonthList;

            //Added by Shazana 13/2/2023
            var monthclose = false;
            if (YearList != null)
            {
                int Year = Convert.ToInt32(YearList);
                int Month = Convert.ToInt32(MonthList);
                var date = new DateTime(Year, Month, 1);
                var audittrail = db.tbl_AuditTrail.Where(x => x.fld_LadangID == LadangID && x.fld_Thn == YearList).FirstOrDefault();
                switch (MonthList)
                {
                    case 1:
                        if (audittrail.fld_Bln1 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 2:
                        if (audittrail.fld_Bln2 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 3:
                        if (audittrail.fld_Bln3 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 4:
                        if (audittrail.fld_Bln4 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 5:
                        if (audittrail.fld_Bln5 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 6:
                        if (audittrail.fld_Bln6 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 7:
                        if (audittrail.fld_Bln7 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 8:
                        if (audittrail.fld_Bln8 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 9:
                        if (audittrail.fld_Bln9 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 10:
                        if (audittrail.fld_Bln10 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 11:
                        if (audittrail.fld_Bln11 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                    case 12:
                        if (audittrail.fld_Bln12 == 1)
                        {
                            monthclose = true;
                        }
                        break;
                }
                ViewBag.audittrail = monthclose;
            }
            else
            {
                ViewBag.audittrail = false;
            }
            return View(postingData);

           
        }

        public JsonResult GenerateRefNo(string docType, Guid SAPPostRefNoID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //int? DivisionID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var SAPPostRefData = dbr.tbl_SAPPostRef.SingleOrDefault(x => x.fld_ID == SAPPostRefNoID);

                var postingMonthYear = SAPPostRefData.fld_PostingDate.Value.ToString("MMyy");

                                var checkrollPostingCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                    x.fldOptConfFlag1 == "sapPostingRefNo" &&
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

                var checkrollDocumentPostingCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                    x.fldOptConfFlag1 == docType &&
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

               var checkrollRefNo = "";

                checkrollRefNo = GlobalFunction.BatchNoSAPPostFunc(LadangID, checkrollPostingCode.fldOptConfValue + postingMonthYear + "-" + checkrollDocumentPostingCode.fldOptConfValue, "sapPostingRefNo", SAPPostRefData.fld_DocType, SAPPostRefData.fld_Month.Value, SAPPostRefData.fld_Year.Value);

                //farahin - 25/02/2022
                int? kodLadang = SAPPostRefData.fld_LadangID.Value;

                var ldgCode = db.tbl_Ladang.Where(x => x.fld_ID == kodLadang).Select(s => s.fld_LdgCode).FirstOrDefault();

                if (ldgCode.Length != 3)
                {
                    ldgCode = ldgCode.PadLeft(3, '0');
                }

                SAPPostRefData.fld_RefNo = checkrollRefNo;
                SAPPostRefData.fld_HeaderText = "3" + ldgCode + checkrollRefNo;

                //farahin comment 25/2/2022
                //SAPPostRefData.fld_RefNo = checkrollRefNo;
                //SAPPostRefData.fld_HeaderText = checkrollRefNo;
                dbr.SaveChanges();

                return Json(checkrollRefNo);
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json("Error");
            }

            finally
            {
                db.Dispose();
            }
        }

        public ActionResult _SAPReturnMsg(Guid? postRefID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getSAPReturnMsgData = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == postRefID).OrderBy(o => o.fld_SortNo);

            return PartialView("_SAPReturnMsg", getSAPReturnMsgData);


        }

        public ActionResult _SAPCredentialLogin(string postGLToGL, string postGLToVendor, string postGLToCustomer)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 13;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            CustMod_SAPCredential sapCredential = new CustMod_SAPCredential();

            sapCredential.GLtoGLGuid = postGLToGL;
            sapCredential.GLtoGVendorGuid = postGLToVendor;
            sapCredential.GLtoGCustomerGuid = postGLToCustomer;

            return PartialView("_SAPCredentialLogin", sapCredential);
        }

        public JsonResult SapPostData(string userName, string password, Guid? postGLToGL, Guid? postGLToVendor, Guid? postGLToCustomer)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                CustMod_ReturnJsonToView returnJsonToView = new CustMod_ReturnJsonToView();
                List<CustMod_ReturnJson> returnJsonList = new List<CustMod_ReturnJson>();


                var ireq = new SAPPosting_FLP.ZfmDocpostOpmsRequest();
                var iresponse = new SAPPosting_FLP.ZfmDocpostOpmsResponse();
                var ICred = new SAPPosting_FLP.ZWS_OPMS_DOCPOSTClient();

                ICred.ClientCredentials.UserName.UserName = userName;
                ICred.ClientCredentials.UserName.Password = password;
                

                var docPost = new SAPPosting_FLP.ZfmDocpostOpms();

                

                SAPPosting_FLP.Bapiacap09[] bapiacap09 = null;
                SAPPosting_FLP.Bapiacap09 bapiacap09_details = new SAPPosting_FLP.Bapiacap09();

                
                //SAPPosting_FLP.Bapiacar09[] bapiacar09 = null;
                //SAPPosting_FLP.Bapiacar09 bapiacar09_details = new SAPPosting_FLP.Bapiacar09();

                SAPPosting_FLP.Bapiacgl09[] bapiacgl09 = null;
                SAPPosting_FLP.Bapiacgl09 bapiacgl09_details = new SAPPosting_FLP.Bapiacgl09();

                SAPPosting_FLP.Bapiaccr09[] bapiaccr09 = null;
                SAPPosting_FLP.Bapiaccr09 bapiaccr09_details = new SAPPosting_FLP.Bapiaccr09();

                

                SAPPosting_FLP.Bapiacpa09 bapiacpa09 = new SAPPosting_FLP.Bapiacpa09();
                SAPPosting_FLP.Bapiache09 bapiache09 = new SAPPosting_FLP.Bapiache09();
                SAPPosting_FLP.Bapiret2[] BAPIRET2 = new SAPPosting_FLP.Bapiret2[1];

                List<tbl_SAPPostReturn> sapPostReturnList = new List<tbl_SAPPostReturn>();

                var month = 0;
                var year = 0;
                int i = 0;



                try
                {
                    ICred.Open();

                    Guid sapPostRefID = new Guid();

                    //GL To GL Process
                    try
                    {
                        var sapDocNo = "";
                        var sortCount = 0;

                        if (!String.IsNullOrEmpty(postGLToGL.ToString()))
                        {

                            var GLToGLPostingData = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToGL)
                                  .OrderBy(o => o.fld_ItemNo).Distinct();

                            if (GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_StatusProceed).SingleOrDefault() == false)
                            {
                                if (GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_NoDocSAP).SingleOrDefault() == null)
                                {
                                    //GL to GL Header Data
                                    foreach (var headerData in GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID))
                                    {
                                        //var posting_date = headerData.fld_PostingDate;

                                        bapiache09.Username = userName;
                                        bapiache09.CompCode = headerData.fld_CompCode;
                                        bapiache09.DocType = headerData.fld_DocType;
                                        bapiache09.HeaderTxt = headerData.fld_HeaderText;
                                        bapiache09.DocDate = headerData.fld_DocDate.ToString("yyyy-MM-dd");
                                        bapiache09.PstngDate = headerData.fld_PostingDate.ToString("yyyy-MM-dd");
                                        bapiache09.RefDocNo = headerData.fld_RefNo;


                                        year = (int)headerData.fld_Year;
                                        month = (int)headerData.fld_Month;
                                    }

                                    //GL to GL - ACCOUNTGL

                                    bapiacgl09 = new SAPPosting_FLP.Bapiacgl09[GLToGLPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                    bapiaccr09 = new SAPPosting_FLP.Bapiaccr09[GLToGLPostingData.DistinctBy(x => x.fld_ItemNo).Count()];

                                    foreach (var GLtoGLItem in GLToGLPostingData.DistinctBy(x => x.fld_ItemNo))
                                    {
                                        bapiacgl09_details = new SAPPosting_FLP.Bapiacgl09();
                                        bapiaccr09_details = new SAPPosting_FLP.Bapiaccr09();
                                        bapiacap09_details = new SAPPosting_FLP.Bapiacap09();

                                        if (!String.IsNullOrEmpty(GLtoGLItem.fld_GL))
                                        {
                                            //GL Account
                                            bapiacgl09_details.ItemnoAcc = GLtoGLItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                            bapiacgl09_details.GlAccount = GLtoGLItem.fld_GL.ToString().Trim().PadLeft(10, '0');
                                            bapiacgl09_details.ItemText = GLtoGLItem.fld_Desc;



                                            if (GLtoGLItem.fld_WilayahID == 12 || GLtoGLItem.fld_WilayahID == 11)
                                            {
                                                bapiacgl09_details.Costcenter = GLtoGLItem.fld_IO;
                                            }
                                            else
                                            {
                                                if (GLtoGLItem.fld_IO != null)
                                                {
                                                    bapiacgl09_details.Orderid = GLtoGLItem.fld_IO;
                                                }
                                                else
                                                {
                                                    bapiacgl09_details.Orderid = null;
                                                }
                                            }

                                            //Currency Amount
                                            bapiaccr09_details.ItemnoAcc = GLtoGLItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                            bapiaccr09_details.Currency = GLtoGLItem.fld_Currency;
                                            //modified by kamalia 24/11/21
                                            bapiaccr09_details.AmtDoccur = (decimal)GLtoGLItem.fld_Amount;


                                        }

                                        bapiacgl09[i] = bapiacgl09_details;
                                        bapiaccr09[i] = bapiaccr09_details;

                                        i = i + 1;
                                    }

                                    BAPIRET2 Return = new BAPIRET2
                                    {
                                        TYPE = null,
                                        ID = null,
                                        MESSAGE = null,
                                        NUMBER = null,
                                        LOG_NO = null,
                                        LOG_MSG_NO = null,
                                        MESSAGE_V1 = null,
                                        MESSAGE_V2 = null,
                                        MESSAGE_V3 = null,
                                        MESSAGE_V4 = null,
                                        PARAMETER = null,
                                        ROW = 0,
                                        FIELD = null,
                                        SYSTEM = null
                                    };


                                    docPost = new SAPPosting_FLP.ZfmDocpostOpms
                                    {
                                        Accountgl = bapiacgl09,
                                        Documentheader = bapiache09,
                                        Currencyamount = bapiaccr09,
                                        Return = BAPIRET2

                                    };


                                    iresponse = ICred.ZfmDocpostOpms(docPost);
                                    BAPIRET2 = iresponse.Return;

                                    foreach (var returnMsg in iresponse.Return)
                                    {
                                        var returnMsgData = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == postGLToGL);

                                        dbr.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                                        dbr.SaveChanges();

                                        sortCount++;

                                        if (returnMsg.Type == "S")
                                        {
                                            sapDocNo = returnMsg.MessageV2;
                                        }

                                        tbl_SAPPostReturn sapPostReturn = new tbl_SAPPostReturn();

                                        sapPostReturn.fld_SortNo = sortCount;
                                        sapPostReturn.fld_Type = returnMsg.Type;
                                        sapPostReturn.fld_ReturnID = returnMsg.Id;
                                        sapPostReturn.fld_Number = returnMsg.Number;
                                        sapPostReturn.fld_LogNo = returnMsg.LogNo;
                                        sapPostReturn.fld_Msg = returnMsg.Message;
                                        sapPostReturn.fld_Msg1 = returnMsg.MessageV1;
                                        sapPostReturn.fld_Msg2 = returnMsg.MessageV2;
                                        sapPostReturn.fld_Msg3 = returnMsg.MessageV3;
                                        sapPostReturn.fld_Msg4 = getuserid + "-" + User.Identity.Name + "(" + DateTime.Today + ")";
                                        sapPostReturn.fld_Param = returnMsg.Parameter;
                                        sapPostReturn.fld_Row = returnMsg.Row.ToString();
                                        sapPostReturn.fld_Field = returnMsg.Field;
                                        sapPostReturn.fld_System = returnMsg.System;
                                        sapPostReturn.fld_SAPPostRefID = postGLToGL;

                                        sapPostReturnList.Add(sapPostReturn);

                                        //dbr.tbl_SAPPostReturn.Add(sapPostReturn);
                                        //dbr.SaveChanges();

                                    }

                                    if (sapPostReturnList.Any())
                                    {
                                        dbr.tbl_SAPPostReturn.AddRange(sapPostReturnList);
                                        dbr.SaveChanges();
                                    }

                                    if (sapPostReturnList.Select(s => s.fld_Type).Contains("E"))
                                    {
                                        CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                        returnJson.Message = "Posting error for GL to GL, kindly check posting report for more information.";
                                        returnJson.Status = "danger";
                                        returnJson.Success = "false";
                                        returnJson.TransactionType = "GL to GL";

                                        returnJsonList.Add(returnJson);
                                    }

                                    else if (sapPostReturnList.Select(s => s.fld_Type).Contains("S"))
                                    {
                                        CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                        returnJson.Message = "Succesfully post GL to GL document.";
                                        returnJson.Status = "success";
                                        returnJson.Success = "false";
                                        returnJson.TransactionType = "GL to GL";

                                        returnJsonList.Add(returnJson);

                                        var getGLPostingData =
                                            dbr.tbl_SAPPostRef.SingleOrDefault(x => x.fld_ID == postGLToGL);

                                        dbr.Entry<tbl_SAPPostRef>(getGLPostingData).State = EntityState.Modified;

                                        getGLPostingData.fld_NoDocSAP = sapDocNo;
                                        getGLPostingData.fld_StatusProceed = true;


                                        dbr.SaveChanges();
                                    }

                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                        CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                        returnJson.Message = ex.Message;
                        returnJson.Status = "danger";
                        returnJson.Success = "false";
                        returnJson.TransactionType = "GL to GL";

                        returnJsonList.Add(returnJson);

                    }
                    /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

                    //Gl to Vendor Process

                    //farahin ubah whole function - 10/2/2022

                    try
                    {
                        var sapDocNo = "";
                        var sortCount = 0;


                        if (!String.IsNullOrEmpty(postGLToVendor.ToString()))
                        {
                            int flagCount = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToVendor).DistinctBy(d => d.fld_flag).Count();

                            flagCount = flagCount + 1;

                            for (int flag = 1; flag < flagCount; flag++)
                            {
                                var GLToVendorPostingData = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToVendor && x.fld_flag == flag)
                                   .OrderBy(o => o.fld_ItemNo).Distinct();

                                if (GLToVendorPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_StatusProceed).SingleOrDefault() == false)
                                {
                                    if (GLToVendorPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_DocNoSAP).SingleOrDefault() == null)
                                    {
                                        foreach (var headerData in GLToVendorPostingData.DistinctBy(x => x.fld_SAPPostRefID))
                                        {
                                            string referenceNo = headerData.fld_RefNo + "-" + flag;

                                            bapiache09.Username = userName;
                                            bapiache09.CompCode = headerData.fld_CompCode;
                                            bapiache09.DocType = headerData.fld_DocType;
                                            bapiache09.HeaderTxt = headerData.fld_HeaderText;
                                            bapiache09.DocDate = headerData.fld_DocDate.ToString("yyyy-MM-dd");
                                            bapiache09.PstngDate = headerData.fld_PostingDate.ToString("yyyy-MM-dd");
                                            bapiache09.RefDocNo = referenceNo.ToString();



                                            //bapiacpa09.Name = headerData.fld_CustCPD;
                                            //bapiacpa09.PostlCode = headerData.fld_Poskod;
                                            //bapiacpa09.City = headerData.fld_DistrictArea;
                                            //bapiacpa09.Country = "MY";

                                            year = (int)headerData.fld_Year;
                                            month = (int)headerData.fld_Month;
                                        }


                                        //GL to Vendor Line Item Details
                                        bapiacgl09 = new SAPPosting_FLP.Bapiacgl09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                        bapiacap09 = new SAPPosting_FLP.Bapiacap09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                        bapiaccr09 = new SAPPosting_FLP.Bapiaccr09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];


                                        i = 0;
                                        foreach (var GLtoVendorItem in GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo))
                                        {
                                            //GLAccount
                                            bapiacgl09_details = new SAPPosting_FLP.Bapiacgl09();
                                            if (GLtoVendorItem.fld_GL != null)
                                            {
                                                bapiacgl09_details.ItemnoAcc = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                bapiacgl09_details.ItemText = GLtoVendorItem.fld_Desc;
                                                bapiacgl09_details.GlAccount = GLtoVendorItem.fld_GL.ToString().PadLeft(10, '0');

                                                if (GLtoVendorItem.fld_WilayahID == 12 || GLtoVendorItem.fld_WilayahID == 11)
                                                {
                                                    bapiacgl09_details.Costcenter = GLtoVendorItem.fld_IO;
                                                }
                                                else
                                                {
                                                    if (GLtoVendorItem.fld_IO != null)
                                                    {
                                                        bapiacgl09_details.Orderid = GLtoVendorItem.fld_IO;
                                                    }
                                                    else
                                                    {
                                                        bapiacgl09_details.Orderid = null;
                                                    }
                                                }
                                                bapiacgl09[i] = bapiacgl09_details;
                                            }


                                            if (GLtoVendorItem.fld_VendorCode != null)
                                            {
                                                //Acc Payable
                                                bapiacap09_details = new SAPPosting_FLP.Bapiacap09();
                                                bapiacap09_details.ItemnoAcc = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                bapiacap09_details.VendorNo = GLtoVendorItem.fld_VendorCode.ToString().PadLeft(10, '0');
                                                bapiacap09_details.ItemText = GLtoVendorItem.fld_Desc.ToString();
                                                bapiacap09_details.BlineDate = GLtoVendorItem.fld_DocDate.ToString("yyyy-MM-dd");

                                                bapiacap09[i] = bapiacap09_details;

                                            }

                                            //Currency Amt
                                            bapiaccr09_details = new SAPPosting_FLP.Bapiaccr09();
                                            bapiaccr09_details.ItemnoAcc = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                            bapiaccr09_details.Currency = GLtoVendorItem.fld_Currency;
                                            //modified by kamalia 24/11/21
                                            bapiaccr09_details.AmtDoccur = (decimal)GLtoVendorItem.fld_Amount;

                                            bapiaccr09[i] = bapiaccr09_details;


                                            i = i + 1;
                                        }

                                        BAPIRET2 Return = new BAPIRET2
                                        {
                                            TYPE = null,
                                            ID = null,
                                            MESSAGE = null,
                                            NUMBER = null,
                                            LOG_NO = null,
                                            LOG_MSG_NO = null,
                                            MESSAGE_V1 = null,
                                            MESSAGE_V2 = null,
                                            MESSAGE_V3 = null,
                                            MESSAGE_V4 = null,
                                            PARAMETER = null,
                                            ROW = 0,
                                            FIELD = null,
                                            SYSTEM = null
                                        };


                                        docPost = new SAPPosting_FLP.ZfmDocpostOpms
                                        {
                                            Accountgl = bapiacgl09,
                                            Documentheader = bapiache09,
                                            Currencyamount = bapiaccr09,
                                            Accountpayable = bapiacap09,
                                            Return = BAPIRET2

                                        };


                                        iresponse = ICred.ZfmDocpostOpms(docPost);
                                        BAPIRET2 = iresponse.Return;


                                        foreach (var returnMsg in iresponse.Return)
                                        {
                                            if (returnMsg.MessageV2 != "")
                                            {
                                                var returnMsgData = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == postGLToVendor && x.fld_flag == flag);

                                                dbr.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                                                dbr.SaveChanges();

                                                sortCount++;

                                                if (returnMsg.Type == "S")
                                                {
                                                    sapDocNo = returnMsg.MessageV2;

                                                    var getGLPostingData =
                                                        dbr.tbl_SAPPostDataDetails.OrderBy(o => o.fld_ItemNo).FirstOrDefault(x => x.fld_SAPPostRefID == postGLToVendor && x.fld_flag == flag);

                                                    dbr.Entry<tbl_SAPPostDataDetails>(getGLPostingData).State = EntityState.Modified;

                                                    getGLPostingData.fld_DocNoSAP = sapDocNo;

                                                    dbr.SaveChanges();
                                                }


                                                tbl_SAPPostReturn sapPostReturn = new tbl_SAPPostReturn();

                                                sapPostReturn.fld_SortNo = sortCount;
                                                sapPostReturn.fld_Type = returnMsg.Type;
                                                sapPostReturn.fld_ReturnID = returnMsg.Id;
                                                sapPostReturn.fld_Number = returnMsg.Number;
                                                sapPostReturn.fld_LogNo = returnMsg.LogNo;
                                                sapPostReturn.fld_Msg = returnMsg.Message;
                                                sapPostReturn.fld_Msg1 = returnMsg.MessageV1;
                                                sapPostReturn.fld_Msg2 = returnMsg.MessageV2;
                                                sapPostReturn.fld_Msg3 = returnMsg.MessageV3;
                                                sapPostReturn.fld_Msg4 = returnMsg.MessageV4;
                                                sapPostReturn.fld_Param = returnMsg.Parameter;
                                                sapPostReturn.fld_Row = returnMsg.Row.ToString();
                                                sapPostReturn.fld_Field = returnMsg.Field;
                                                sapPostReturn.fld_System = returnMsg.System;
                                                sapPostReturn.fld_SAPPostRefID = postGLToVendor;
                                                sapPostReturn.fld_flag = flag;

                                                sapPostReturnList.Add(sapPostReturn);

                                                //dbr.tbl_SAPPostReturn.Add(sapPostReturn);
                                                //dbr.SaveChanges();
                                            }

                                            if (sapPostReturnList.Any())
                                            {
                                                dbr.tbl_SAPPostReturn.AddRange(sapPostReturnList);
                                                dbr.SaveChanges();
                                            }
                                        }

                                        if (sapPostReturnList.Select(s => s.fld_Type).Contains("E"))
                                        {
                                            CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                            returnJson.Message = "Posting error for GL to Vendor, kindly check posting report for more information.";
                                            returnJson.Status = "danger";
                                            returnJson.Success = "false";
                                            returnJson.TransactionType = "GL to Vendor";

                                            returnJsonList.Add(returnJson);
                                        }

                                        else if (sapPostReturnList.Select(s => s.fld_Type).Contains("S"))
                                        {
                                            CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                                            returnJson.Message = "Succesfully post GL to Vendor document.";
                                            returnJson.Status = "success";
                                            returnJson.Success = "false";
                                            returnJson.TransactionType = "GL to Vendor";

                                            returnJsonList.Add(returnJson);
                                        }

                                    }
                                }

                            }

                            int docNo =
                                dbr.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == postGLToVendor && x.fld_DocNoSAP != null).DistinctBy(e => e.fld_flag).Count();


                            flagCount = flagCount - 1;

                            if (docNo == flagCount)
                            {
                                var getGLPostingDataRef =
                                              dbr.tbl_SAPPostRef.SingleOrDefault(x => x.fld_ID == postGLToVendor);

                                dbr.Entry<tbl_SAPPostRef>(getGLPostingDataRef).State = EntityState.Modified;

                                getGLPostingDataRef.fld_StatusProceed = true;

                                dbr.SaveChanges();
                            }

                        }
                    }


                    catch (Exception ex)
                    {
                        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                        CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                        returnJson.Message = ex.Message;
                        returnJson.Status = "danger";
                        returnJson.Success = "false";
                        returnJson.TransactionType = "GL to Vendor";

                        returnJsonList.Add(returnJson);
                    }

                    ICred.Close();
                }

                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                    returnJson.Message = ex.Message;
                    returnJson.Status = "danger";
                    returnJson.Success = "false";
                    returnJson.TransactionType = "GL to Customer";

                    returnJsonList.Add(returnJson);
                }


                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

                returnJsonToView.ReturnJsonList = returnJsonList;
                returnJsonToView.RootUrl = domain;
                returnJsonToView.Action = "_PostingSAP";
                returnJsonToView.Controller = "BizTransac";
                returnJsonToView.Div = "closeTransactionDetails";
                returnJsonToView.ParamName1 = "MonthList";
                returnJsonToView.ParamValue1 = month.ToString();
                returnJsonToView.ParamName2 = "YearList";
                returnJsonToView.ParamValue2 = year.ToString();

                return Json(returnJsonToView);
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }

            finally
            {

                db.Dispose();
            }


        }

        public ActionResult CloseTransaction()
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
        public ActionResult CloseTransaction(int Month, int Year, bool CloseOpen)
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
                //sepul comment filter no SKB 29/11/2021
                //if (CheckSkbReg.fld_NoSkb != null)
                //{
                    //if (CheckSkbReg.fld_GajiBersih == ClosingTransaction.fld_Credit)
                    //{
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
                //    }
                //    else
                //    {
                //        msg = "Sila pastikan nilai pemohonan sama seperti didaftar di No SKB sebelum urusniaga ditutup";
                //        statusmsg = "warning";
                //    }

                //}
                //else
                //{
                //    msg = "Sila daftar No SKB sebelum urusniaga ditutup";
                //    statusmsg = "warning";
                //}
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

        //Shazana 17/11/2022
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User")]
        public ActionResult ManagerApproval()
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int year = timezone.gettimezone().Year;

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User")]
        public ActionResult ManagerApproval(int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone();

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();



            if (WilayahID == 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID(SyarikatID);
                //mywlyid = String.Join("", wlyhid); ;
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                //mywlyid = String.Join("", WilayahID); ;
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            }

            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, getdate.Month, getdate.Year);
            }
            else
            {
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport2(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year);
            }
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            return View();
        }

        public ActionResult ApplicationSupportRegionDetail(List<long> eachid)
        {
            var getdata = dbCorp.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true).ToList();
            ViewBag.getgmstatus = getdata.Where(x => x.fld_SokongWilGM_Status == 1).Count();
            return View(getdata);
        }

        public JsonResult GetLadang(int WilayahID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID2 = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID2, out LadangID, getuserid, User.Identity.Name);

            if (getwilyah.GetAvailableWilayah(SyarikatID))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted_L == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
                else
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
            }

            return Json(ladanglist);
        }

        public JsonResult UpdateData(long DataID, string UpdateFlag, int NegaraId, int SyarikatId, int WilayahId, decimal JumlahWang, int Month, int Year, string NoAcc, string NoCIT, string SebabTolak)
        {
            string DescStatus = "";
            int getuserid = getidentity.ID(User.Identity.Name);
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string NamaWilayah = getwilyah.GetWilayahName(WilayahId);
            string subject = "";
            string msg = "";
            string DepartmentHR = "";
            string DepartmentAM = "";
            string DepartmentCL = "";
            string DepartmentMGR = "";
            string DepartmentRMGR2 = ""; //Added by Shazana 20/2/2023
            string[] to = new string[] { };
            List<string> tolist = new List<string>();
            string[] cc = new string[] { };
            List<string> cclist = new List<string>();
            string[] bcc = new string[] { };
            List<string> bcclist = new List<string>();
            DateTime getdatetime = timezone.gettimezone();
            //bool matchtotal = false;

            var GetEstate = db.tbl_SokPermhnWang.Where(x => x.fld_ID == DataID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == GetEstate.fld_LadangID && x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId).FirstOrDefault();

            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FELDA";
                DepartmentAM = "AM_FINANCE_APPROVAL_FELDA";
                DepartmentCL = "CL_FINANCE_APPROVAL_FELDA";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FELDA";

                //Added by Shazana 20/2/2023
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FELDA";
            }
            else
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FPM";
                DepartmentAM = "AM_FINANCE_APPROVAL_FPM";
                DepartmentCL = "CL_FINANCE_APPROVAL_FPM";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FPM";

                //Added by Shazana 20/2/2023
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FPM";
            }

            switch (UpdateFlag)
            {
                case "SemakWil":
                    DescStatus = "Telah Disemak";
                    //DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, PDP, CIT, NoAcc, NoGL, NoCIT, Manual);
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, NoAcc, NoCIT);
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1, "");

                    subject = "Sokongan Permohonan Gaji";

                    //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    //msg += "<p>Tn/Pn " + ToEmail.fldName + ",</p>";
                    msg += "<p>Tuan/Puan, </p>";

                    //Modify by Shazana 15/2/2023
                    //msg += "<p>Sokongan permohonan gaji (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tuan/Puan (RGM). Keterangan seperti dibawah:-</p>";
                    msg += "<p>Sokongan permohonan gaji (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tuan/Puan (Wilayah (Kewangan/RC)). Keterangan seperti dibawah:-</p>";

                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan (RM)</th><th>Disahkan Oleh</th><th>Disemak Oleh</th><th>Waktu Disemak</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    //Modified by Shazana 21/12/2022
                    //msg += "<td>" + GetEstateDetail.fld_LdgCode + "</td><td>" + GetEstateDetail.fld_LdgName + "</td><td>" + JumlahWang + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionGm", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    msg += "<td>" + GetEstateDetail.fld_LdgCode + "</td><td>" + GetEstateDetail.fld_LdgName + "</td><td>" + JumlahWang + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionFirst", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik ke pautan sokongan</a></td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    //Modified by Shazana 20/2/2023
                    //var emailtolist = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    var emailtolist = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && ((x.fldDepartment == DepartmentHR && x.fldCategory == "TO") || (x.fldDepartment == DepartmentRMGR2 && x.fldCategory == "TO" && x.fldLadangID == GetEstateDetail.fld_ID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    if (emailtolist != null)
                    {
                        foreach (var toemail in emailtolist)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var emailcclist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailcclist != null)
                    {
                        foreach (var ccemail in emailcclist)
                        {
                            cclist.Add(ccemail.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var emailbcclist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (emailbcclist != null)
                    {
                        foreach (var bccemail in emailbcclist)
                        {
                            bcclist.Add(bccemail.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, ToEmail.fldEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, cc, bcc);

                    break;
                case "TolakWil":
                    DescStatus = "Telah Ditolak";
                    //DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 0, 1, 0, 0, 0, 0, "TolakWil", getuserid, getdatetime, 0, 0, "", "", "", 0);
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 0, 1, 0, 0, 0, 0, "TolakWil", getuserid, getdatetime, "", "");
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1, SebabTolak);

                    subject = "Penolakkan Permohonan Gaji";

                    //var GetLdgID = db.tbl_SokPermhnWang.Where(x => x.fld_ID == DataID &&  x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WilayahID == WilayahId).FirstOrDefault();
                    //var GetLdgDetail = db.tbl_Ladang.Where(x => x.fld_ID == GetLdgID.fld_LadangID && x.fld_WlyhID == WilayahId).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    msg += "<p>Kepada Ladang " + GetEstateDetail.fld_LdgName + ",</p>";
                    msg += "<p>Dukacita dimaklumkan, permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Pengurus. Mohon pihak ladang buat semakkan kembali. Keterangan seperti dibawah :-</p>";
                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan(RM)</th><th>Sebab</th><th>Tindakan Oleh</th><th>Waktu Tindakan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";
                    msg += "<td align=\"center\">" + GetEstateDetail.fld_LdgCode + "</td><td align=\"center\">" + GetEstateDetail.fld_LdgName + "</td><td align=\"center\">" + JumlahWang + "</td><td align=\"center\">" + SebabTolak + "</td><td align=\"center\">" + ActionBy + "</td><td align=\"center\">" + getdatetime + "</td>";
                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemailt in ToEmailT)
                        {
                            tolist.Add(toemailt.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (CcEmailT != null)
                    {
                        foreach (var ccemailt in CcEmailT)
                        {
                            cclist.Add(ccemailt.fldEmail);
                        }
                        cc = cclist.ToArray();
                    }

                    var BccEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (BccEmailT != null)
                    {
                        foreach (var bccemailt in BccEmailT)
                        {
                            bcclist.Add(bccemailt.fldEmail);
                        }
                        bcc = bcclist.ToArray();
                    }

                    //SendEmailNotification.SendEmail(subject, msg, GetEstateDetail.fld_LdgEmail, cc, bcc);
                    SendEmailNotification.SendEmail2(subject, msg, to, null, bcc);

                    break;
            }
            return Json(new { DescStatus = DescStatus, ActionBy = ActionBy, getdatetime = getdatetime, SebabTolak = SebabTolak });
        }


        public ActionResult WorkerPaySheetRptSearch(int NegaraID, int SyarikatID, int WilayahID, int LadangID, string PaymentModeList, int Month, int Year)
        {
            //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;

            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Viewing dbest = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbest2 = new MVC_SYSTEM_Models();
            List<ViewingModels.vw_PaySheetPekerjaCustomModel> PaySheetPekerjaList = new List<ViewingModels.vw_PaySheetPekerjaCustomModel>();

            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;
            //ViewBag.WorkerList = SelectionList;
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
                  .Select(s => s.fld_LdgName.Substring(0, 1).ToUpper() + s.fld_LdgName.Substring(1).ToLower())
                  .FirstOrDefault();//modified by kamalia 24/12/21
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            List<SelectListItem> PaymentModeList2 = new List<SelectListItem>();
            PaymentModeList2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" &&
            x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fldOptConfValue)
            .Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            PaymentModeList2.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.PaymentModeSelection = PaymentModeList;

            MVC_SYSTEM_Viewing dbest3 = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            IOrderedQueryable<vw_PaySheetPekerja> salaryData;
            salaryData = dbest.vw_PaySheetPekerja
                .Where(x => x.fld_NegaraID == NegaraID &&
                            x.fld_Year == Year && x.fld_Month == Month &&
                            x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID)
                .OrderBy(x => x.fld_Nama);

            foreach (var salary in salaryData)
            {
                var workerAdditionalContribution = dbest2.tbl_ByrCarumanTambahan
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
                ViewBag.Message = "Tiada Rekod";
            }
            return View(PaySheetPekerjaList);

        }
        public ActionResult TransactionListingRptSearch(int NegaraID, int SyarikatID, int WilayahID, int LadangID, int Month, int Year)
        {
            //int? NegaraID = NegaraId;
            //int? SyarikatID = SyarikatId;
            //int? WilayahID = WilayahId;

            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Viewing dbest = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing dbest3 = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            ViewBag.MonthList = Month;
            ViewBag.YearList = Year;

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
               .Select(s => s.fld_LdgName.Substring(0, 1).ToUpper() + s.fld_LdgName.Substring(1).ToLower())
               .FirstOrDefault();//modified by kamalia 24/12/21
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.Date = DateTime.Now.ToShortDateString();

            var GetCotribution = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag3 == "Employee" && x.fldDeleted == false).Select(s => s.fldOptConfValue).ToList();


            var TransactionListingList = dbest3.vw_RptSctran
                .Where(x => !GetCotribution.Contains(x.fld_KodAktvt) && x.fld_Month == Month &&
                            x.fld_Year == Year && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID)
                .OrderBy(o => new { o.fld_Kategori, o.fld_Amt }).ToList();

            if (!TransactionListingList.Any())
            {
                ViewBag.Message = "Tiada Rekod";
                return View();
            }


            ViewBag.UserID = getuserid;
            return View(TransactionListingList);
            //}
        }

        public string ApplicationSupportHistoryDetail(long SPWID)
        {
            string returndetail = "";
            string fontcolor = "";
            var getdata = db.tblSokPermhnWangHisActions.Where(x => x.fldHisSPWID == SPWID).OrderBy(o => o.fldHisDT).ToArray();
            if (getdata != null)
            {
                foreach (var data in getdata)
                {
                    if (data.fldHisDesc == "Telah Ditolak" || data.fldHisDesc == "Urus Niaga Dibuka Semula")
                    {
                        fontcolor = "red";
                    }
                    else
                    {
                        fontcolor = "green";
                    }
                    returndetail = returndetail + "<font color=\"" + fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + " pada " + data.fldHisDT + "</p>";
                }
            }
            return returndetail;
        }

        //Close Shazana 17/11/2022
    }
}