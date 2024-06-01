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

using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.Data;  //Added by Shazana 19/6/2023
using System.Web.UI.WebControls;//Added by Shazana 19/6/2023
using System.Web.UI;//Added by Shazana 19/6/2023
using Dapper;
using MVC_SYSTEM.ModelsDapper;
using System.Data.SqlClient;

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

        //Added by Shazana 13/2/2023
        private GeneralClass GeneralClass = new GeneralClass();
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
            string host, catalog, user, pass = "";
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
                        SAPReturnList.Add(new tbl_SAPPostReturn() { fld_SortNo = NoSort, fld_Msg1 = SAPReturn.MESSAGE, fld_Msg2 = SAPReturn.MESSAGE_V1, fld_Msg3 = SAPReturn.MESSAGE_V2, fld_Msg4 = SAPReturn.MESSAGE_V3, fld_SAPPostRefID = PostingID });
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
                        SAPReturnList.Add(new tbl_SAPPostReturn() { fld_SortNo = NoSort, fld_Msg1 = SAPReturn.MESSAGE, fld_Msg2 = SAPReturn.MESSAGE_V1, fld_Msg3 = SAPReturn.MESSAGE_V2, fld_Msg4 = SAPReturn.MESSAGE_V3, fld_SAPPostRefID = PostingID });
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
            //Added by Shazana 23/6/2023
            int? roleuser = getidentity.getRoleID(getuserid);
            ViewBag.RoleUser = roleuser;

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
            //Modified by Shazana 6/3/2023
            ViewBag.GetTerimaHQ = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TerimaHQ_Status == 1).Any();

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


            //Added by Shazana 6/3/2023
            var statusPermohonan = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).FirstOrDefault();
            if (statusPermohonan == null)
            { ViewBag.statusPermohonan = "Permohonan tidak wujud "; }
            else if (statusPermohonan.fld_SemakWil_Status == 1 && statusPermohonan.fld_SokongWilGM_Status == 1 && statusPermohonan.fld_TerimaHQ_Status == 1)
            { ViewBag.statusPermohonan = "Lulus"; }
            else if (statusPermohonan.fld_SemakWil_Status == 0 || statusPermohonan.fld_SokongWilGM_Status == 0 || statusPermohonan.fld_TerimaHQ_Status == 0)
            { ViewBag.statusPermohonan = "Permohonan masih dalam proses "; }
            else
            { ViewBag.statusPermohonan = "Permohonan masih dalam proses "; }


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

            //kalau nak test untuk client FLQ840 tukar SAPPosting_FL'x' ke SAPPosting_FLQ
            //kalau nk deploy live, tukar SAPPosting_FL'x' ke SAPPosting_FLQ
            //farahin update whole function..cater FPM..

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                string compCode = db.tbl_Ladang.Where(w => w.fld_ID == LadangID).Select(s => s.fld_CostCentre).FirstOrDefault();


                CustMod_ReturnJsonToView returnJsonToView = new CustMod_ReturnJsonToView();
                List<CustMod_ReturnJson> returnJsonList = new List<CustMod_ReturnJson>();
                List<tbl_SAPPostReturn> sapPostReturnList = new List<tbl_SAPPostReturn>();

                var month = 0;
                var year = 0;
                int i = 0;



                try
                {
                    if (compCode == "1000")
                    {
                        var ireq = new SAPPosting_FLQ.ZfmDocpostOpmsRequest();
                        var iresponse = new SAPPosting_FLQ.ZfmDocpostOpmsResponse();
                        var ICred = new SAPPosting_FLQ.ZWS_OPMS_DOCPOSTClient();

                        ICred.ClientCredentials.UserName.UserName = userName;
                        ICred.ClientCredentials.UserName.Password = password;

                        var docPost = new SAPPosting_FLQ.ZfmDocpostOpms();

                        //account payable
                        SAPPosting_FLQ.Bapiacap09[] bapiacap09 = null;
                        SAPPosting_FLQ.Bapiacap09 bapiacap09_details = new SAPPosting_FLQ.Bapiacap09();

                        //accountGL
                        SAPPosting_FLQ.Bapiacgl09[] bapiacgl09 = null;
                        SAPPosting_FLQ.Bapiacgl09 bapiacgl09_details = new SAPPosting_FLQ.Bapiacgl09();

                        //currency
                        SAPPosting_FLQ.Bapiaccr09[] bapiaccr09 = null;
                        SAPPosting_FLQ.Bapiaccr09 bapiaccr09_details = new SAPPosting_FLQ.Bapiaccr09();

                        SAPPosting_FLQ.Bapiacpa09 bapiacpa09 = new SAPPosting_FLQ.Bapiacpa09(); //customer cpd
                        SAPPosting_FLQ.Bapiache09 bapiache09 = new SAPPosting_FLQ.Bapiache09(); //documentHeader
                        SAPPosting_FLQ.Bapiret2[] BAPIRET2 = new SAPPosting_FLQ.Bapiret2[1]; //return structure

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

                                        bapiacgl09 = new SAPPosting_FLQ.Bapiacgl09[GLToGLPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                        bapiaccr09 = new SAPPosting_FLQ.Bapiaccr09[GLToGLPostingData.DistinctBy(x => x.fld_ItemNo).Count()];

                                        foreach (var GLtoGLItem in GLToGLPostingData.DistinctBy(x => x.fld_ItemNo))
                                        {
                                            bapiacgl09_details = new SAPPosting_FLQ.Bapiacgl09();
                                            bapiaccr09_details = new SAPPosting_FLQ.Bapiaccr09();
                                            bapiacap09_details = new SAPPosting_FLQ.Bapiacap09();

                                            if (!String.IsNullOrEmpty(GLtoGLItem.fld_GL))
                                            {
                                                //GL Account
                                                bapiacgl09_details.ItemnoAcc = GLtoGLItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                bapiacgl09_details.GlAccount = GLtoGLItem.fld_GL.ToString().Trim().PadLeft(10, '0');
                                                bapiacgl09_details.ItemText = GLtoGLItem.fld_Desc;

                                                if (GLtoGLItem.fld_IO != null)
                                                {
                                                    if (GLtoGLItem.fld_SAPType == "CC")
                                                    {
                                                        bapiacgl09_details.Costcenter = GLtoGLItem.fld_IO;
                                                    }
                                                    else if (GLtoGLItem.fld_SAPType == "IO")
                                                    {
                                                        bapiacgl09_details.Orderid = GLtoGLItem.fld_IO;
                                                    }
                                                    //farahin tambah 22/9/2023 - RISE
                                                    else if (GLtoGLItem.fld_SAPType == "WBS")
                                                    {
                                                        bapiacgl09_details.WbsElement = GLtoGLItem.fld_IO + ".B";
                                                    }
                                                }
                                                else
                                                {
                                                    bapiacgl09_details.Orderid = "";
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


                                        docPost = new SAPPosting_FLQ.ZfmDocpostOpms
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
                                            bapiacgl09 = new SAPPosting_FLQ.Bapiacgl09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                            bapiacap09 = new SAPPosting_FLQ.Bapiacap09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                            bapiaccr09 = new SAPPosting_FLQ.Bapiaccr09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];

                                            i = 0;
                                            foreach (var GLtoVendorItem in GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo))
                                            {
                                                //GLAccount
                                                bapiacgl09_details = new SAPPosting_FLQ.Bapiacgl09();
                                                if (GLtoVendorItem.fld_GL != null)
                                                {
                                                    bapiacgl09_details.ItemnoAcc = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                    bapiacgl09_details.ItemText = GLtoVendorItem.fld_Desc;
                                                    bapiacgl09_details.GlAccount = GLtoVendorItem.fld_GL.ToString().PadLeft(10, '0');

                                                    if (GLtoVendorItem.fld_IO != null)
                                                    {
                                                        if (GLtoVendorItem.fld_SAPType == "CC")
                                                        {
                                                            bapiacgl09_details.Costcenter = GLtoVendorItem.fld_IO;
                                                        }
                                                        else if (GLtoVendorItem.fld_SAPType == "IO")
                                                        {
                                                            bapiacgl09_details.Orderid = GLtoVendorItem.fld_IO;
                                                        }
                                                        //farahin tambah 22/9/2023 - RISE
                                                        else if (GLtoVendorItem.fld_SAPType == "WBS")
                                                        {
                                                            bapiacgl09_details.WbsElement = GLtoVendorItem.fld_IO + ".B";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        bapiacgl09_details.Orderid = "";
                                                    }
                                                    bapiacgl09[i] = bapiacgl09_details;
                                                }

                                                if (GLtoVendorItem.fld_VendorCode != null)
                                                {
                                                    //Acc Payable
                                                    bapiacap09_details = new SAPPosting_FLQ.Bapiacap09();
                                                    bapiacap09_details.ItemnoAcc = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                    bapiacap09_details.VendorNo = GLtoVendorItem.fld_VendorCode.ToString().PadLeft(10, '0');
                                                    bapiacap09_details.ItemText = GLtoVendorItem.fld_Desc.ToString();
                                                    bapiacap09_details.BlineDate = GLtoVendorItem.fld_DocDate.ToString("yyyy-MM-dd");

                                                    bapiacap09[i] = bapiacap09_details;
                                                }

                                                //Currency Amt
                                                bapiaccr09_details = new SAPPosting_FLQ.Bapiaccr09();
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

                                            docPost = new SAPPosting_FLQ.ZfmDocpostOpms
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
                    else if (compCode == "8800")
                    {
                        var req = new FPM_FTQ.ZFM_DOCPOST_OPMSRequest();
                        var res = new FPM_FTQ.ZFM_DOCPOST_OPMSResponse();
                        var cred = new FPM_FTQ.ZWS_OPMS_DOCPOSTClient();

                        cred.ClientCredentials.UserName.UserName = userName;
                        cred.ClientCredentials.UserName.Password = password;

                        var post = new FPM_FTQ.ZFM_DOCPOST_OPMS();

                        FPM_FTQ.BAPIACHE09 BAPIACHE09 = new FPM_FTQ.BAPIACHE09(); //document header
                        FPM_FTQ.BAPIACPA09 BAPIACPA09 = new FPM_FTQ.BAPIACPA09(); //customer cpd
                        FPM_FTQ.BAPIRET2[] BAPIRET = new FPM_FTQ.BAPIRET2[1]; //return structure

                        //account payable
                        FPM_FTQ.BAPIACAP09[] BAPIACAP09 = null;
                        FPM_FTQ.BAPIACAP09 BAPIACAP09_details = new FPM_FTQ.BAPIACAP09();

                        //accountGL
                        FPM_FTQ.BAPIACGL09[] BAPIACGL09 = null;
                        FPM_FTQ.BAPIACGL09 BAPIACGL09_details = new FPM_FTQ.BAPIACGL09();

                        //currency
                        FPM_FTQ.BAPIACCR09[] BAPIACCR09 = null;
                        FPM_FTQ.BAPIACCR09 BAPIACCR09_details = new FPM_FTQ.BAPIACCR09();




                        cred.Open();

                        try
                        {
                            var sapDocNo = "";
                            var sortCount = 0;
                            if (!String.IsNullOrEmpty(postGLToGL.ToString()))
                            {

//GL TO GL
                                var GLToGLPostingData = dbr.vw_SAPPostData.Where(x => x.fld_SAPPostRefID == postGLToGL)
                                      .OrderBy(o => o.fld_ItemNo).Distinct();

                                if (GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_StatusProceed).SingleOrDefault() == false)
                                {
                                    if (GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID).Select(s => s.fld_NoDocSAP).SingleOrDefault() == null)
                                    {
                                        //GL to GL Header Data
                                        foreach (var headerData in GLToGLPostingData.DistinctBy(x => x.fld_SAPPostRefID))
                                        {

                                            BAPIACHE09.USERNAME = userName;
                                            BAPIACHE09.COMP_CODE = headerData.fld_CompCode;
                                            BAPIACHE09.DOC_TYPE = headerData.fld_DocType;
                                            BAPIACHE09.HEADER_TXT = headerData.fld_HeaderText;
                                            BAPIACHE09.DOC_DATE = headerData.fld_DocDate.ToString("yyyy-MM-dd");
                                            BAPIACHE09.PSTNG_DATE = headerData.fld_PostingDate.ToString("yyyy-MM-dd");
                                            BAPIACHE09.REF_DOC_NO = headerData.fld_RefNo;

                                            year = (int)headerData.fld_Year;
                                            month = (int)headerData.fld_Month;
                                        }

                                        //ACCOUNTGL

                                        BAPIACGL09 = new FPM_FTQ.BAPIACGL09[GLToGLPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                        BAPIACCR09 = new FPM_FTQ.BAPIACCR09[GLToGLPostingData.DistinctBy(x => x.fld_ItemNo).Count()];

                                        foreach (var GLtoGLItem in GLToGLPostingData.DistinctBy(x => x.fld_ItemNo))
                                        {
                                            BAPIACGL09_details = new FPM_FTQ.BAPIACGL09();
                                            BAPIACCR09_details = new FPM_FTQ.BAPIACCR09();
                                            BAPIACAP09_details = new FPM_FTQ.BAPIACAP09();

                                            if (!String.IsNullOrEmpty(GLtoGLItem.fld_GL))
                                            {
                                                BAPIACGL09_details.ITEMNO_ACC = GLtoGLItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                BAPIACGL09_details.GL_ACCOUNT = GLtoGLItem.fld_GL.ToString().Trim().PadLeft(10, '0');
                                                BAPIACGL09_details.ITEM_TEXT = GLtoGLItem.fld_Desc;

                                                if (GLtoGLItem.fld_IO != null)
                                                {
                                                    if (GLtoGLItem.fld_SAPType == "CC")
                                                    {
                                                        BAPIACGL09_details.COSTCENTER = GLtoGLItem.fld_IO;
                                                    }
                                                    else if (GLtoGLItem.fld_SAPType == "IO")
                                                    {
                                                        BAPIACGL09_details.ORDERID = GLtoGLItem.fld_IO;
                                                    }
                                                }
                                                else
                                                {
                                                    BAPIACGL09_details.ORDERID = "";
                                                }
                                                //farahin tambah 06/09/2023
                                                if (BAPIACGL09_details.GL_ACCOUNT.StartsWith("002"))
                                                {
                                                    BAPIACGL09_details.ALLOC_NMBR = "OPMSPosting";
                                                }

                                                //farahin tambah 06/09/2023
                                                if (BAPIACGL09_details.GL_ACCOUNT.StartsWith("002"))
                                                {
                                                    BAPIACGL09_details.ALLOC_NMBR = "OPMSPosting";
                                                }

                                                //Currency Amount
                                                BAPIACCR09_details.ITEMNO_ACC = GLtoGLItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                BAPIACCR09_details.CURRENCY = GLtoGLItem.fld_Currency;
                                                BAPIACCR09_details.AMT_DOCCUR = (decimal)GLtoGLItem.fld_Amount;

                                            }

                                            BAPIACGL09[i] = BAPIACGL09_details;
                                            BAPIACCR09[i] = BAPIACCR09_details;

                                            i = i + 1;
                                        }



                                        FPM_FTQ.BAPIRET2 RET = new FPM_FTQ.BAPIRET2
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


                                        post = new FPM_FTQ.ZFM_DOCPOST_OPMS
                                        {
                                            ACCOUNTGL = BAPIACGL09,
                                            DOCUMENTHEADER = BAPIACHE09,
                                            CURRENCYAMOUNT = BAPIACCR09,
                                            RETURN = BAPIRET

                                        };

                                        res = cred.ZFM_DOCPOST_OPMS(post);
                                        BAPIRET = res.RETURN;

                                        foreach (var returnMsg in res.RETURN)
                                        {
                                            var returnMsgData = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == postGLToGL);

                                            dbr.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                                            dbr.SaveChanges();

                                            sortCount++;

                                            if (returnMsg.TYPE == "S")
                                            {
                                                sapDocNo = returnMsg.MESSAGE_V2;
                                            }

                                            tbl_SAPPostReturn sapPostReturn = new tbl_SAPPostReturn();

                                            sapPostReturn.fld_SortNo = sortCount;
                                            sapPostReturn.fld_Type = returnMsg.TYPE;
                                            sapPostReturn.fld_ReturnID = returnMsg.ID;
                                            sapPostReturn.fld_Number = returnMsg.NUMBER;
                                            sapPostReturn.fld_LogNo = returnMsg.LOG_NO;
                                            sapPostReturn.fld_Msg = returnMsg.MESSAGE;
                                            sapPostReturn.fld_Msg1 = returnMsg.MESSAGE_V1;
                                            sapPostReturn.fld_Msg2 = returnMsg.MESSAGE_V2;
                                            sapPostReturn.fld_Msg3 = returnMsg.MESSAGE_V3;
                                            sapPostReturn.fld_Msg4 = getuserid + "-" + User.Identity.Name + "(" + DateTime.Today + ")";
                                            sapPostReturn.fld_Param = returnMsg.PARAMETER;
                                            sapPostReturn.fld_Row = returnMsg.ROW.ToString();
                                            sapPostReturn.fld_Field = returnMsg.FIELD;
                                            sapPostReturn.fld_System = returnMsg.SYSTEM;
                                            sapPostReturn.fld_SAPPostRefID = postGLToGL;

                                            sapPostReturnList.Add(sapPostReturn);
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

//GL TO VENDOR

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

                                                BAPIACHE09.USERNAME = userName;
                                                BAPIACHE09.COMP_CODE = headerData.fld_CompCode;
                                                BAPIACHE09.DOC_TYPE = headerData.fld_DocType;
                                                BAPIACHE09.HEADER_TXT = headerData.fld_HeaderText;
                                                BAPIACHE09.DOC_DATE = headerData.fld_DocDate.ToString("yyyy-MM-dd");
                                                BAPIACHE09.PSTNG_DATE = headerData.fld_PostingDate.ToString("yyyy-MM-dd");
                                                BAPIACHE09.REF_DOC_NO = referenceNo.ToString();

                                                //bapiacpa09.Name = headerData.fld_CustCPD;
                                                //bapiacpa09.PostlCode = headerData.fld_Poskod;
                                                //bapiacpa09.City = headerData.fld_DistrictArea;
                                                //bapiacpa09.Country = "MY";

                                                year = (int)headerData.fld_Year;
                                                month = (int)headerData.fld_Month;
                                            }

                                            //GL to Vendor Line Item Details
                                            BAPIACGL09 = new FPM_FTQ.BAPIACGL09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                            BAPIACAP09 = new FPM_FTQ.BAPIACAP09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];
                                            BAPIACCR09 = new FPM_FTQ.BAPIACCR09[GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo).Count()];

                                            i = 0;
                                            foreach (var GLtoVendorItem in GLToVendorPostingData.DistinctBy(x => x.fld_ItemNo))
                                            {
                                                //GLAccount
                                                BAPIACGL09_details = new FPM_FTQ.BAPIACGL09();
                                                if (GLtoVendorItem.fld_GL != null)
                                                {
                                                    BAPIACGL09_details.ITEMNO_ACC = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                    BAPIACGL09_details.ITEM_TEXT = GLtoVendorItem.fld_Desc;
                                                    BAPIACGL09_details.GL_ACCOUNT = GLtoVendorItem.fld_GL.ToString().PadLeft(10, '0');

                                                    if (GLtoVendorItem.fld_IO != null)
                                                    {
                                                        if (GLtoVendorItem.fld_SAPType == "CC")
                                                        {
                                                            BAPIACGL09_details.COSTCENTER = GLtoVendorItem.fld_IO;
                                                        }
                                                        else if (GLtoVendorItem.fld_SAPType == "IO")
                                                        {
                                                            BAPIACGL09_details.ORDERID = GLtoVendorItem.fld_IO;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        BAPIACGL09_details.ORDERID = "";
                                                    }

                                                    //farahin tambah 06/09/2023
                                                    if (BAPIACGL09_details.GL_ACCOUNT.StartsWith("002"))
                                                    {
                                                        BAPIACGL09_details.ALLOC_NMBR = "OPMSPosting";
                                                    }

                                                    BAPIACGL09[i] = BAPIACGL09_details;
                                                }

                                                if (GLtoVendorItem.fld_VendorCode != null)
                                                {
                                                    //Acc Payable
                                                    BAPIACAP09_details = new FPM_FTQ.BAPIACAP09();
                                                    BAPIACAP09_details.ITEMNO_ACC = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                    BAPIACAP09_details.VENDOR_NO = GLtoVendorItem.fld_VendorCode.ToString().PadLeft(10, '0');
                                                    BAPIACAP09_details.ITEM_TEXT = GLtoVendorItem.fld_Desc.ToString();
                                                    BAPIACAP09_details.BLINE_DATE = GLtoVendorItem.fld_DocDate.ToString("yyyy-MM-dd");

                                                    BAPIACAP09[i] = BAPIACAP09_details;
                                                }

                                                //Currency Amt
                                                BAPIACCR09_details = new FPM_FTQ.BAPIACCR09();
                                                BAPIACCR09_details.ITEMNO_ACC = GLtoVendorItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                                BAPIACCR09_details.CURRENCY = GLtoVendorItem.fld_Currency;
                                                BAPIACCR09_details.AMT_DOCCUR = (decimal)GLtoVendorItem.fld_Amount;

                                                BAPIACCR09[i] = BAPIACCR09_details;

                                                i = i + 1;
                                            }

                                            FPM_FTQ.BAPIRET2 RET = new FPM_FTQ.BAPIRET2
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

                                            post = new FPM_FTQ.ZFM_DOCPOST_OPMS
                                            {
                                                ACCOUNTGL = BAPIACGL09,
                                                DOCUMENTHEADER = BAPIACHE09,
                                                CURRENCYAMOUNT = BAPIACCR09,
                                                ACCOUNTPAYABLE = BAPIACAP09,
                                                RETURN = BAPIRET
                                            };

                                            res = cred.ZFM_DOCPOST_OPMS(post);
                                            BAPIRET = res.RETURN;

                                            foreach (var returnMsg in res.RETURN)
                                            {
                                                if (returnMsg.MESSAGE_V2 != "")
                                                {
                                                    var returnMsgData = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == postGLToVendor && x.fld_flag == flag);

                                                    dbr.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                                                    dbr.SaveChanges();

                                                    sortCount++;

                                                    if (returnMsg.TYPE == "S")
                                                    {
                                                        sapDocNo = returnMsg.MESSAGE_V2;

                                                        var getGLPostingData =
                                                            dbr.tbl_SAPPostDataDetails.OrderBy(o => o.fld_ItemNo).FirstOrDefault(x => x.fld_SAPPostRefID == postGLToVendor && x.fld_flag == flag);

                                                        dbr.Entry<tbl_SAPPostDataDetails>(getGLPostingData).State = EntityState.Modified;

                                                        getGLPostingData.fld_DocNoSAP = sapDocNo;

                                                        dbr.SaveChanges();
                                                    }

                                                    tbl_SAPPostReturn sapPostReturn = new tbl_SAPPostReturn();

                                                    sapPostReturn.fld_SortNo = sortCount;
                                                    sapPostReturn.fld_Type = returnMsg.TYPE;
                                                    sapPostReturn.fld_ReturnID = returnMsg.ID;
                                                    sapPostReturn.fld_Number = returnMsg.NUMBER;
                                                    sapPostReturn.fld_LogNo = returnMsg.LOG_NO;
                                                    sapPostReturn.fld_Msg = returnMsg.MESSAGE;
                                                    sapPostReturn.fld_Msg1 = returnMsg.MESSAGE_V1;
                                                    sapPostReturn.fld_Msg2 = returnMsg.MESSAGE_V2;
                                                    sapPostReturn.fld_Msg3 = returnMsg.MESSAGE_V3;
                                                    sapPostReturn.fld_Msg4 = returnMsg.MESSAGE_V4;
                                                    sapPostReturn.fld_Param = returnMsg.PARAMETER;
                                                    sapPostReturn.fld_Row = returnMsg.ROW.ToString();
                                                    sapPostReturn.fld_Field = returnMsg.FIELD;
                                                    sapPostReturn.fld_System = returnMsg.SYSTEM;
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

                        cred.Close();
                    }
                }

                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    CustMod_ReturnJson returnJson = new CustMod_ReturnJson();

                    returnJson.Message = ex.Message;
                    returnJson.Status = "danger";
                    returnJson.Success = "false";
                    returnJson.TransactionType = "Post To SAP";

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
            // Added by Shazana 27/4/2023
            var CheckPermohonanWang = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == Year && x.fld_Month == Month).FirstOrDefault();

            //Added by Shazana 6/6/2023
            string nama = "";
            MVC_SYSTEM_Viewing dbest = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            var PaySlipNegative = dbest.vw_PaySheetPekerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == Month && x.fld_Year == Year).Where(x => x.fld_GajiBersih < 0).ToList();
            if (PaySlipNegative.Count() > 0)
            {
                foreach (var personDetails in PaySlipNegative)
                {
                    nama = Environment.NewLine + nama + personDetails.fld_Nopkj + " - " + personDetails.fld_Nama + " (" + personDetails.fld_GajiBersih + ")  |    ";
                }
            }

            if (ClosingTransaction != null)
            {
                //sepul comment filter no SKB 29/11/2021
                //if (CheckSkbReg.fld_NoSkb != null)
                //{
                //if (CheckSkbReg.fld_GajiBersih == ClosingTransaction.fld_Credit)
                //{
                var estateClosingChecking_Result = new List<EstateClosingChecking_Result>();
                decimal? jumgl2gl = 0m;
                decimal? jumgl2vdDt = 0m;
                decimal? jumgl2vdCt = 0m;
                decimal? TL_Credit = 0m;
                decimal? TL_Debit = 0m;
                if (CloseOpen)
                {
                    string constr = Connection.GetConnectionString(WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    var con = new SqlConnection(constr);
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("LadangID", LadangID);
                        parameters.Add("Month", Month);
                        parameters.Add("Year", Year);
                        con.Open();
                        estateClosingChecking_Result = SqlMapper.Query<EstateClosingChecking_Result>(con, "sp_EstateClosingChecking", parameters).ToList();
                        con.Close();
                        jumgl2gl = estateClosingChecking_Result.Where(x => x.fld_purpose == 1).Select(s => s.jumgl2gl).FirstOrDefault();
                        jumgl2vdDt = estateClosingChecking_Result.Where(x => x.fld_purpose == 2).Select(s => s.jumgl2vdDt).FirstOrDefault();
                        jumgl2vdCt = estateClosingChecking_Result.Where(x => x.fld_purpose == 2).Select(s => s.jumgl2vdCt).FirstOrDefault();
                        TL_Credit = estateClosingChecking_Result.Where(x => x.fld_purpose == 2).Select(s => s.TL_Credit).FirstOrDefault();
                        TL_Debit = estateClosingChecking_Result.Where(x => x.fld_purpose == 2).Select(s => s.TL_Debit).FirstOrDefault();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                if (jumgl2gl == 0 && jumgl2vdDt + jumgl2vdCt == 0 && TL_Credit - TL_Debit == 0)
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
                            //Added by Shazana 27/4/2023
                            if (CloseOpen == false)
                            {
                                //Added by Shazana 5/5/2023
                                if (CheckPermohonanWang == null)
                                {
                                    //Close Added by Shazana 27/4/2023
                                    AuditTrailStatus = CloseOpen == true ? 1 : 0;
                                    ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
                                    ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
                                    ClosingTransaction.fld_ModifiedBy = getuserid;
                                    dbr.Entry(ClosingTransaction).State = EntityState.Modified;
                                    dbr.SaveChanges();
                                    UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, AuditTrailStatus);

                                    msg = "Urus niaga telah dibuka";
                                    statusmsg = "success";
                                }

                                //Modified by Shazana 5/5/2023
                                else if ((CheckPermohonanWang.fld_SemakWil_Status == 1 && CheckPermohonanWang.fld_SokongWilGM_Status == 1 && CheckPermohonanWang.fld_TerimaHQ_Status == 1) && (CheckPermohonanWang.fld_TolakWil_Status == 0 || CheckPermohonanWang.fld_TolakWilGM_Status == 0 || CheckPermohonanWang.fld_TolakHQ_Status == 0))
                                {
                                    msg = "Urus niaga tidak boleh dibuka. Maklumkan kepada HQ untuk membatalkan permohonan wang.";
                                    statusmsg = "warning";
                                }
                                else
                                {
                                    //Added by Shazana 6/6/2023
                                    if (PaySlipNegative.Count() == 0)
                                    {
                                        //Close Added by Shazana 27/4/2023
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

                                        //Added by Shazana 6/6/2023
                                    }
                                    else
                                    {
                                        msg = "Urusniaga tidak boleh ditutup. " + nama;
                                        statusmsg = "warning";
                                    }
                                    //Close Added by Shazana 6/6/2023
                                }
                            }//Added by Shazana 27/4/2023
                             //Added by Shazana 3/5/2023
                            else
                            {
                                //Added by Shazana 6/6/2023
                                if (PaySlipNegative.Count() == 0)
                                {
                                    AuditTrailStatus = CloseOpen == true ? 1 : 0;
                                    ClosingTransaction.fld_StsTtpUrsNiaga = CloseOpen;
                                    ClosingTransaction.fld_ModifiedDT = timezone.gettimezone();
                                    ClosingTransaction.fld_ModifiedBy = getuserid;
                                    dbr.Entry(ClosingTransaction).State = EntityState.Modified;
                                    dbr.SaveChanges();
                                    UpdateAuditTrail(NegaraID, SyarikatID, WilayahID, LadangID, Year, Month, AuditTrailStatus);
                                    msg = "Urus niaga telah ditutup";
                                    statusmsg = "success";

                                    //Added by Shazana 6/6/2023
                                }
                                else
                                {
                                    msg = "Urusniaga tidak boleh ditutup. " + nama;
                                    statusmsg = "warning";
                                }
                                //Close Added by Shazana 6/6/2023
                            }
                        }//Added by Shazana 27/4/2023
                    }
                    else
                    {
                        msg = GlobalResEstate.msgBalance;
                        statusmsg = "warning";
                    }
                }
                else
                {
                    msg = "PERHATIAN!!!<br/>Urusniaga tidak boleh ditutup!<br/>Transaction Listing (Debit) = RM" + TL_Debit + "<br/>Transaction Listing (Kredit)= RM" + TL_Credit + "<br/>Posting to SAP, GL to GL = RM" + jumgl2gl + "<br/>Posing to SAP, GL to Vendor (Debit) = RM" + jumgl2vdDt + "<br/>Posing to SAP, GL to Vendor (Kredit) = RM" + jumgl2vdCt;
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

            List<SelectListItem> SyarikatIDList = new List<SelectListItem>(); //Added by Shazana 1/8/2023
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> LadangIDList = new List<SelectListItem>();

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    //mywlyid = String.Join("", wlyhid); ;
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName").ToList();
            //    LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => wlyhid.Contains((int)x.fld_WlyhID) && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //}

            //Added by Shazana 1/8/2023
            if (WilayahID == 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == syarikatInfo.fldOptConfValue.ToString() && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                var listwilayah = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                WilayahIDList = new SelectList(listwilayah.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                var syarikatInfo = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).FirstOrDefault();
                int SyarikatCode = Convert.ToInt16(syarikatInfo.fld_SyarikatID);
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID) && x.fld_Deleted == false), "fld_ID", "fld_WlyhName").ToList();

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_CostCentre == syarikatInfo.fldOptConfValue).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();

            }
            else if (WilayahID != 0 && LadangID != 0)
            {

                LadangIDList = new SelectList(db2.tbl_Ladang.Where(x => x.fld_Deleted == false && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID) && x.fld_Deleted == false), "fld_ID", "fld_WlyhName").ToList();

                var ladangInfo = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                SyarikatIDList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == ladangInfo.fld_CostCentre).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            }

            ViewBag.SyarikatIDList = SyarikatIDList; //Added by Shazana 1/8/2023
            ViewBag.WilayahIDList = WilayahIDList;
            ViewBag.LadangIDList = LadangIDList;
            ViewBag.GetView = 1;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //role id authorization ( adding super power user)  - modified by farahin - 17/06/2022
        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3, Super Power User")]
        public ActionResult ManagerApproval(int SyarikatIDList, int WilayahIDList, int LadangIDList)
        {
            int[] wlyhid = new int[] { };
            //string mywlyid = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int role = GetIdentity.RoleID(getuserid).Value; //Added by Shazana 1/8/2023
            DateTime getdate = timezone.gettimezone().AddMonths(-1);
            //DateTime getdate = timezone.gettimezone(); //temporary for display currenth month

            ViewBag.ApplicationSupport = "class = active";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> SyarikatIDList2 = new List<SelectListItem>(); //Added by Shazana 1/8/2023
            List<SelectListItem> WilayahIDList2 = new List<SelectListItem>();
            List<SelectListItem> LadangIDList2 = new List<SelectListItem>();

            //Added by Shazana 1/8/2023
            var SyarikatIDCode = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == SyarikatIDList.ToString()).FirstOrDefault();

            //if (WilayahID == 0 && LadangID == 0)
            //{
            //    wlyhid = getwilyah.GetWilayahID(SyarikatID);
            //    //mywlyid = String.Join("", wlyhid); ;
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //    WilayahIDList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));
            //}
            //else if (WilayahID != 0 && LadangID == 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
            //    LadangIDList2.Insert(0, (new SelectListItem { Text = GlobalResEstate.sltAll, Value = "0" }));

            //}
            //else if (WilayahID != 0 && LadangID != 0)
            //{
            //    //mywlyid = String.Join("", WilayahID); ;
            //    wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
            //    WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
            //    LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_ID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text", LadangIDList).ToList();
            //}

            //Added by Shazana 1/8/2023
            if (WilayahID == 0 && LadangID == 0)
            {
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();

                var ladangInfo = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatIDList.ToString() && x.fld_Deleted == false && x.fld_SyarikatID == SyarikatIDCode.fld_SyarikatID).Select(x => x.fld_WlyhID).ToList();

                WilayahIDList2 = new SelectList(db.tbl_Wilayah.Where(x => x.fld_Deleted == false && ladangInfo.Contains(x.fld_ID)).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text", WilayahIDList).ToList();

                if (LadangIDList != 0 && WilayahIDList != 0)
                {
                    LadangIDList2 = new SelectList(db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatIDList.ToString() && x.fld_Deleted == false && x.fld_SyarikatID == SyarikatIDCode.fld_SyarikatID && x.fld_NegaraID == NegaraID && x.fld_WlyhID == WilayahIDList).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_ID + "-" + s.fld_LdgName }), "Value", "Text", WilayahIDList).ToList();
                }
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahIDList && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList.ToString()).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();

            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                wlyhid = getwilyah.GetWilayahID2(SyarikatID, WilayahID);
                WilayahIDList2 = new SelectList(db2.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID)), "fld_ID", "fld_WlyhName", WilayahIDList).ToList();
                LadangIDList2 = new SelectList(db2.tbl_Ladang.Where(x => x.fld_WlyhID == WilayahID && x.fld_Deleted == false && x.fld_CostCentre == SyarikatIDList.ToString() && x.fld_ID == LadangID).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_LdgName }), "Value", "Text").ToList();
                var listsyarikat = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fldOptConfValue == SyarikatIDList.ToString()).OrderBy(x => x.fldOptConfDesc).ToList();
                SyarikatIDList2 = new SelectList(listsyarikat.OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue.ToString(), Text = s.fldOptConfDesc }), "Value", "Text", SyarikatIDList).ToList();
            }
            //Added by Shazana 1/8/2023
            ViewBag.SyarikatIDList = SyarikatIDList2;
            ViewBag.role = role;
            ViewBag.WilayahIDList = WilayahIDList2;
            ViewBag.LadangIDList = LadangIDList2;
            if (WilayahIDList == 0)
            {
                //Modified by Shazana 1/8/2023
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport3(NegaraID, SyarikatID, getdate.Month, getdate.Year, SyarikatIDCode.fldOptConfValue);
            }
            else
            {
                //Modified by Shazana 1/8/2023
                ViewBag.WilayahSelection = getwilyah.GetWilayahIDForApplicationSupport3(NegaraID, SyarikatID, WilayahIDList, getdate.Month, getdate.Year, SyarikatIDCode.fldOptConfValue);
            }
            ViewBag.CostCentre = SyarikatIDCode.fldOptConfValue;
            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.LadangID = LadangIDList;
            ViewBag.Month = getdate.Month;
            ViewBag.Year = getdate.Year;
            ViewBag.GetView = 0;
            return View();
        }

        //Modified by Shazana 1/8/2023
        public ActionResult ApplicationSupportRegionDetail(List<long> eachid, String Year, String Month, string CostCentre)
        {
            //Modified by Shazana 1/8/2023
            var getdata = dbCorp.vw_PermohonanKewangan.Where(x => eachid.Contains(x.fld_ID) && x.fld_StsTtpUrsNiaga == true && x.fld_CostCentre == CostCentre).ToList();
            ViewBag.getgmstatus = getdata.Where(x => x.fld_SokongWilGM_Status == 1).Count();

            //Added by Shazana 30/3/2023
            ViewBag.Year = Year;
            ViewBag.Month = Month;
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
            string DepartmentRMGR = ""; //Added by Shazana 6/3/2023
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

            //Added by Shazana 3/4/2023
            string CompanyShortName = GetIdentity.getCompanyShortName(SyarikatId);

            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FELDA";
                DepartmentAM = "AM_FINANCE_APPROVAL_FELDA";
                DepartmentCL = "CL_FINANCE_APPROVAL_FELDA";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FELDA";

                //Added by Shazana 20/2/2023
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FELDA";
                //Added by Shazana 6/3/2023
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FELDA";
            }
            else
            {
                DepartmentHR = "HR_FINANCE_APPROVAL_FPM";
                DepartmentAM = "AM_FINANCE_APPROVAL_FPM";
                DepartmentCL = "CL_FINANCE_APPROVAL_FPM";
                DepartmentMGR = "MGR_FINANCE_APPROVAL_FPM";

                //Added by Shazana 20/2/2023
                DepartmentRMGR2 = "RMGR2_FINANCE_APPROVAL_FPM";
                //Added by Shazana 6/3/2023
                DepartmentRMGR = "RMGR_FINANCE_APPROVAL_FPM";
            }

            //Added by Shazana 10/7/2023
            string SemakanNama = "";
            string SokonganNama = "";
            string LulusNama = "";
            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                SemakanNama = " (Pengurus FELDA) ";
                SokonganNama = " (Pegawai Kewangan) ";
                LulusNama = " (Pegawai Wilayah FELDA) ";
            }
            else
            {
                SemakanNama = " (Pengurus FPM) ";
                SokonganNama = " (Pegawai Wilayah FPM) ";
                LulusNama = " (Pegawai Wilayah FPM) ";
            }

            switch (UpdateFlag)
            {
                case "SemakWil":
                    DescStatus = "Telah Disemak";
                    //DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, PDP, CIT, NoAcc, NoGL, NoCIT, Manual);
                    DatabaseAction.UpdateDataTotblSokPermhnWang(DataID, 1, 0, 0, 0, 0, 0, "SemakWil", getuserid, getdatetime, NoAcc, NoCIT);
                    DatabaseAction.InsertDataTotblSokPermhnWangHisAction(DescStatus, getuserid, getdatetime, DataID, 1, "");

                    //Modify by Shazana 3/4/2023
                    subject = CompanyShortName + " -Sokongan Permohonan Gaji(" + GetEstateDetail.fld_LdgName + ")";

                    //var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                    msg = "<html>";
                    msg += "<body>";
                    msg += "<p>Assalamualaikum WBT & Salam sejahtera,</p>";
                    //msg += "<p>Tn/Pn " + ToEmail.fldName + ",</p>";
                    msg += "<p>Tuan/Puan, </p>";

                    //Modify by Shazana 15/2/2023
                    //Modified by Shazana 3/4/2023
                    //msg += "<p>Sokongan permohonan gaji (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tuan/Puan (RGM). Keterangan seperti dibawah:-</p>";
                    msg += "<p>Sokongan permohonan gaji (Gaji Pekerja Buruh) untuk kelulusan diperlukan dari pihak Tuan/Puan (Wilayah (Kewangan/RC)).</p>";
                    msg += "<p>Keterangan seperti dibawah:-</p>";

                    msg += "<table border=\"1\">";
                    msg += "<thead>";
                    msg += "<tr>";
                    //Modified by Shazana 3/4/2023
                    //Modified by Shazana 20/4/2023
                    msg += "<th>Kod Ladang</th><th>Nama Ladang</th><th>Jumlah Permohonan (RM)</th><th>Disahkan Oleh</th><th>Waktu Disahkan</th><th>Pautan</th>";
                    msg += "</tr>";
                    msg += "</thead>";
                    msg += "<tbody>";
                    msg += "<tr>";

                    //Modified by Shazana 10/7/2023 //untuk live, comment dia bahagian testing dan uncomment live
                    //Added by Shazana 6/3/2023 LIVE
                    //Modified by Shazana 1/8/2023
                    msg += "<td>" + GetEstateDetail.fld_LdgCode + "</td><td>" + GetEstateDetail.fld_LdgName + "</td><td>" + JumlahWang + "</td><td align=\"center\">" + ActionBy + "( " + GlobalFunction.getJawatanName(getuserid, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + "http://opms.felda.net.my/ms/ApplicationSupport/ApplicationSupportRegionFirst" + "\">Klik ke pautan sokongan</a></td>";
                    //Added by Shazana 6/3/2023 TESTING
                    //Modified by Shazana 1/8/2023
                    //msg += "<td>" + GetEstateDetail.fld_LdgCode + "</td><td>" + GetEstateDetail.fld_LdgName + "</td><td>" + JumlahWang + "</td><td align=\"center\">" + ActionBy + "( " + GlobalFunction.getJawatanName(getuserid, NegaraId, SyarikatId) + " )" + "</td><td align=\"center\">" + getdatetime + "</td><td><a href=\"" + "http://172.16.25.170/FPM_CORP_STG/ms/ApplicationSupport/ApplicationSupportRegionFirst" + "\">Klik ke pautan sokongan</a></td>";

                    msg += "</tr>";
                    msg += "</tbody>";
                    msg += "</table>";
                    msg += "<p>Terima Kasih.</p>";
                    msg += "</body>";
                    msg += "</html>";

                    //Modified by Shazana 20/2/2023
                    //var emailtolist = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == DepartmentHR && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();

                    //Modified by Shazana 6/3/2023
                    var emailtolist = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && ((x.fldDepartment == DepartmentRMGR && x.fldCategory == "TO" && x.fldWilayahID == WilayahId)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();


                    if (emailtolist != null)
                    {
                        foreach (var toemail in emailtolist)
                        {
                            tolist.Add(toemail.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    //Modified by Shazana 20/2/2023
                    //var emailcclist = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentAM && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentCL && x.fldCategory == "CC" && x.fldLadangID == GetEstateDetail.fld_ID) || (x.fldDepartment == DepartmentHR && x.fldCategory == "CC")) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    //Modified by Shazana 6/3/2023
                    var emailcclist = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId && x.fld_ID == GetEstate.fld_LadangID).Select(x => x.fld_LdgEmail).FirstOrDefault();
                    if (emailcclist != null)
                    {
                        //Modified by Shazana 3/4/2022
                        //foreach (var ccemail in emailcclist)
                        //{
                        //    //Commented by Shazana 6/3/2023
                        //    //cclist.Add(ccemail.fldEmail);
                        //    //Added by Shazana 6/3/2023
                        //    cclist.Add(emailcclist);
                        //}
                        cclist.Add(emailcclist);
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
                    //Modified by Shazana 20/4/2023
                    msg += "<p>Dukacita dimaklumkan, sokongan permohonan gaji (Gaji Pekerja Buruh) telah ditolak oleh Kewangan / RC. Mohon pihak ladang buat semakan kembali. Keterangan seperti dibawah :- :-</p>";
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

                    //Modified by Shazana 25/2/2023
                    //var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID && ((x.fldDepartment == DepartmentCL && x.fldCategory == "TO") || x.fldDepartment == DepartmentAM && x.fldCategory == "TO") && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    var ToEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && ((x.fldDepartment == DepartmentRMGR && x.fldCategory == "TO" && x.fldWilayahID == WilayahId)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    if (ToEmailT != null)
                    {
                        foreach (var toemailt in ToEmailT)
                        {
                            tolist.Add(toemailt.fldEmail);
                        }
                        to = tolist.ToArray();
                    }

                    //Modified by Shazana 25/2/2023
                    //var CcEmailT = db.tblEmailLists.Where(x => x.fldNegaraID == NegaraId && x.fldSyarikatID == SyarikatId && ((x.fldDepartment == DepartmentHR && x.fldCategory == "CC") || (x.fldDepartment == DepartmentMGR && x.fldCategory == "CC" && x.fldWilayahID == WilayahId && x.fldLadangID == GetEstateDetail.fld_ID)) && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                    var CcEmailT = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraId && x.fld_SyarikatID == SyarikatId && x.fld_WlyhID == WilayahId && x.fld_ID == GetEstate.fld_LadangID).Select(x => x.fld_LdgEmail).FirstOrDefault();
                    if (CcEmailT != null)
                    {
                        //Modified by Shazana 3/4/2023
                        //foreach (var ccemailt in CcEmailT)
                        //{
                        //Commented by Shazana 6/3/2023
                        //cclist.Add(ccemailt.fldEmail);
                        //Added by Shazana 6/3/2023
                        cclist.Add(CcEmailT);
                        //}

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
            //Commented by Shazana 24/5/2023
            //if (getdata != null)
            //{
            //    foreach (var data in getdata)
            //    {
            //        if (data.fldHisDesc == "Telah Ditolak" || data.fldHisDesc == "Urus Niaga Dibuka Semula")
            //        {
            //            fontcolor = "red";
            //        }
            //        else
            //        {
            //            fontcolor = "green";
            //        }
            //        returndetail = returndetail + "<font color=\"" + fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + " pada " + data.fldHisDT + "</p>";
            //   

            //Added by Shazana 24/5/2023
            var getdatatblSokPermhnWang = db.tbl_SokPermhnWang.Where(x => x.fld_ID == SPWID).FirstOrDefault();
            var GetEstateDetail = db.tbl_Ladang.Where(x => x.fld_ID == getdatatblSokPermhnWang.fld_LadangID && x.fld_NegaraID == getdatatblSokPermhnWang.fld_NegaraID && x.fld_SyarikatID == getdatatblSokPermhnWang.fld_SyarikatID && x.fld_WlyhID == getdatatblSokPermhnWang.fld_WilayahID).FirstOrDefault();
            string SemakanNama = "";
            string SokonganNama = "";
            string LulusNama = "";
            if (GetEstateDetail.fld_CostCentre == "1000")
            {
                SemakanNama = " (Pengurus FELDA) ";
                SokonganNama = " (Pegawai Kewangan) ";
                LulusNama = " (Pegawai Wilayah FELDA) ";
            }
            else
            {
                SemakanNama = " (Pengurus FELDA) "; //Added by Shazana 6/6/2023
                SokonganNama = " (Pegawai Wilayah FPM) ";//Added by Shazana 6/6/2023
                LulusNama = " (Pegawai Wilayah FELDA) ";//Added by Shazana 6/6/2023
            }

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

                    string roleNama = "";

                    if (data.fldHisDesc.Contains("Telah Disemak"))
                    { roleNama = SemakanNama; }
                    else if (data.fldHisDesc.Contains("Telah Disokong"))
                    {
                        roleNama = SokonganNama;
                    }
                    else if (data.fldHisDesc.Contains("Telah Diluluskan"))
                    {
                        roleNama = LulusNama;
                    }


                    //Modified by Shazana 1/8/2023
                    returndetail = returndetail + "<font color=\"" + fontcolor + "\"><p class=\"specialClass\">" + data.fldHisDesc + " oleh " + getidentity.MyNameFullName(data.fldHisUserID) + "( " + GlobalFunction.getJawatanName(data.fldHisUserID, GetEstateDetail.fld_NegaraID, GetEstateDetail.fld_SyarikatID) + " )" + " pada " + data.fldHisDT + "</p>";

                    //Close Modified by Shazana 27/4/2023
                }
            }
            return returndetail;
        }

        //Close Shazana 17/11/2022

        //Added by Shazana 25/2/2023
        public FileStreamResult exportPDFGLtoGL(int? MonthList, int? YearList)
        {
            Document pdfDoc = new Document(PageSize.A4, 30, 30, 40, 40);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var message = "";

            var postingData = new List<vw_SAPPostData>();
            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()))
            {
                postingData = dbr.vw_SAPPostData
                    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).ToList();

                var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList).FirstOrDefault();
                ViewBag.ClosingStatus = ClosingTransaction.fld_StsTtpUrsNiaga;

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

            }
            if (postingData.Count() > 0)
            {


                foreach (var GLtoGL in postingData.DistinctBy(s => s.fld_SAPPostRefID).Where(x => x.fld_DocType == "A2"))
                {
                    var postRefGuid = postingData.Where(x => x.fld_DocType == "A2").Select(s => s.fld_SAPPostRefID).Distinct().FirstOrDefault();

                    pdfDoc.NewPage();
                    //Header
                    //Modify by Shazana 19/6/2023
                    pdfDoc = GeneralClass.Header(pdfDoc, Syarikat.fld_NamaSyarikat, "(" + GlobalResEstate.lblCompanyNo + " : " + Syarikat.fld_NoSyarikat + ")", "GL to GL");

                    PdfPTable tableHeader = new PdfPTable(4);
                    tableHeader.WidthPercentage = 100;
                    tableHeader.SpacingBefore = 10f;
                    float[] widthsHeader = new float[] { 1, 2.5f, 1, 1 };
                    tableHeader.SetWidths(widthsHeader);
                    PdfPCell cellHeader = new PdfPCell();



                    tableHeader = new PdfPTable(4);
                    tableHeader.HeaderRows = 1; //repeat header in each page
                    tableHeader.WidthPercentage = 100;
                    tableHeader.SpacingBefore = 5f;
                    widthsHeader = new float[] { 1, 2.5f, 1, 1 };
                    tableHeader.SetWidths(widthsHeader);


                    cellHeader = GeneralClass.CellNoBorder("Checkrol Ref No:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_RefNo == null ? " " : GLtoGL.fld_RefNo.ToString(), 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("Sap Document No:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_NoDocSAP == null ? "" : GLtoGL.fld_NoDocSAP.ToString(), 0, 1);
                    tableHeader.AddCell(cellHeader);

                    //----------------------------------------------------

                    cellHeader = GeneralClass.CellNoBorder("Date Generated:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_DocDate.ToString(), 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);

                    //----------------------------------------------------
                    cellHeader = GeneralClass.CellNoBorder("Header Text:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_HeaderText == null ? "" : GLtoGL.fld_HeaderText.ToString(), 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);


                    //----------------------------------------------------
                    cellHeader = GeneralClass.CellNoBorder("Document Type:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_DocType == null ? "" : GLtoGL.fld_DocType.ToString(), 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);

                    //----------------------------------------------------
                    cellHeader = GeneralClass.CellNoBorder("Posting Month:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_Month.ToString(), 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("Posting Year:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_Year.ToString(), 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    tableHeader.AddCell(cellHeader);



                    //----------------------------------------------------

                    cellHeader = GeneralClass.CellNoBorder("Posting Date:", 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder(GLtoGL.fld_PostingDate.ToString(), 0, 1);
                    cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);

                    cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                    tableHeader.AddCell(cellHeader);

                    pdfDoc.Add(tableHeader);
                }

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                float[] widths = new float[] { 0.6f, 1, 1, 1, 2.5f, 1 };
                table.SetWidths(widths);
                PdfPCell cell = new PdfPCell();

                table = new PdfPTable(6);
                table.HeaderRows = 1; //repeat header in each page
                table.WidthPercentage = 100;
                table.SpacingBefore = 5f;
                widths = new float[] { 0.6f, 1, 1, 1, 2.5f, 1 };
                table.SetWidths(widths);

                chunk = new Chunk("Item No", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("GL", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("IO", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)); ;
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("Activity Code", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("Description", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);


                chunk = new Chunk("Amount", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                int bil = 0;
                int i = 1;
                string KdrGaji = "";
                decimal Jumlah = 0;
                GetTriager GetTriager = new GetTriager();
                foreach (var item in postingData.Where(x => x.fld_DocType == "A2").OrderBy(o => o.fld_ItemNo).Distinct())
                {
                    chunk = new Chunk(item.fld_ItemNo.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk((item.fld_GL == null ? "" : item.fld_GL.ToString()), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(item.fld_IO == null ? "" : item.fld_IO.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(item.fld_SAPActivityCode == null ? "" : item.fld_SAPActivityCode.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(item.fld_Desc == null ? "" : item.fld_Desc.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk(GetTriager.GetTotalForMoney(item.fld_Amount).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);


                    i++;
                }

                pdfDoc.Add(table);


            }
            else
            {
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            Response.AppendHeader("Content-Disposition", "inline; filename=" + "GLtoGL" + ".pdf");
            return new FileStreamResult(output, "application/pdf");

        }

        public FileStreamResult exportPDFGLtoVendor(int? MonthList, int? YearList)
        {
            Document pdfDoc = new Document(PageSize.A4, 30, 30, 40, 40);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var message = "";

            var postingData = new List<vw_SAPPostData>();
            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()))
            {
                postingData = dbr.vw_SAPPostData
                    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).ToList();

                var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList).FirstOrDefault();
                ViewBag.ClosingStatus = ClosingTransaction.fld_StsTtpUrsNiaga;

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

            }
            int dataCount = 1;
            if (postingData.Count() > 0)
            {

                if (postingData.Any(x => x.fld_DocType == "KR"))
                    dataCount++;
                {
                    foreach (var GLtoVendor in postingData.DistinctBy(s => s.fld_SAPPostRefID).Where(x => x.fld_DocType == "KR"))

                    {
                        var postRefGuid = postingData.Where(x => x.fld_DocType == "KR").Select(s => s.fld_SAPPostRefID).Distinct().FirstOrDefault();

                        pdfDoc.NewPage();
                        //Header
                        //Modify by Shazana 19/6/2023
                        pdfDoc = GeneralClass.Header(pdfDoc, Syarikat.fld_NamaSyarikat, "(" + GlobalResEstate.lblCompanyNo + " : " + Syarikat.fld_NoSyarikat + ")", "GL to Vendor");

                        PdfPTable tableHeader = new PdfPTable(4);
                        tableHeader.WidthPercentage = 100;
                        tableHeader.SpacingBefore = 10f;
                        float[] widthsHeader = new float[] { 1, 2.5f, 1, 1 };
                        tableHeader.SetWidths(widthsHeader);
                        PdfPCell cellHeader = new PdfPCell();

                        tableHeader = new PdfPTable(4);
                        tableHeader.HeaderRows = 1; //repeat header in each page
                        tableHeader.WidthPercentage = 100;
                        tableHeader.SpacingBefore = 5f;
                        widthsHeader = new float[] { 1, 2.5f, 1, 1 };
                        tableHeader.SetWidths(widthsHeader);

                        cellHeader = GeneralClass.CellNoBorder("Checkroll Group No:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder(GLtoVendor.fld_RefNo == null ? "" : GLtoVendor.fld_RefNo.ToString(), 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("SAP Return Status:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        //----------------------------------------------------

                        cellHeader = GeneralClass.CellNoBorder("Date Generated:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder(GLtoVendor.fld_DocDate.ToString(), 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        //----------------------------------------------------
                        cellHeader = GeneralClass.CellNoBorder("Header Text:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder(GLtoVendor.fld_HeaderText == null ? "" : GLtoVendor.fld_HeaderText.ToString(), 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);


                        //----------------------------------------------------
                        cellHeader = GeneralClass.CellNoBorder("Document Type:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder(GLtoVendor.fld_DocType == null ? "" : GLtoVendor.fld_DocType.ToString(), 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        //----------------------------------------------------
                        cellHeader = GeneralClass.CellNoBorder("Posting Month:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder(GLtoVendor.fld_Month.ToString(), 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("Posting Year:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder(GLtoVendor.fld_Year.ToString(), 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                        tableHeader.AddCell(cellHeader);



                        //----------------------------------------------------

                        cellHeader = GeneralClass.CellNoBorder("Posting Date:", 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_RIGHT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder(GLtoVendor.fld_PostingDate.ToString(), 0, 1);
                        cellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        cellHeader = GeneralClass.CellNoBorder("", 0, 1);
                        tableHeader.AddCell(cellHeader);

                        pdfDoc.Add(tableHeader);
                    }


                }

                int flag = 1;
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                float[] widths = new float[] { 0.8f, 1.2f, 1.2f, 2, 0.7f };
                table.SetWidths(widths);
                PdfPCell cell = new PdfPCell();

                table = new PdfPTable(5);
                table.HeaderRows = 1; //repeat header in each page
                table.WidthPercentage = 100;
                table.SpacingBefore = 5f;
                widths = new float[] { 0.8f, 1.2f, 1.2f, 2, 0.7f };
                table.SetWidths(widths);

                foreach (var krFlag in postingData.DistinctBy(s => s.fld_flag).Where(x => x.fld_DocType == "KR" && x.fld_flag == flag))
                {

                    var DocNo = postingData.DistinctBy(s => s.fld_DocNoSAP).Where(x => x.fld_DocType == "KR" && x.fld_flag == flag).Select(s => s.fld_DocNoSAP).FirstOrDefault();
                    int bill = 0;


                    chunk = new Chunk("Reference No: " + (krFlag.fld_RefNo == null ? "" : krFlag.fld_RefNo.ToString()) + " - " + (flag == null ? "" : flag.ToString()), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Colspan = 2;
                    table.AddCell(cell);

                    chunk = new Chunk("SAP Document No: " + (krFlag.fld_DocNoSAP == null ? "" : krFlag.fld_DocNoSAP.ToString()), FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    cell.Colspan = 4;
                    table.AddCell(cell);

                    chunk = new Chunk("Item No", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("GL", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Vendor No", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK)); ;
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Description", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);

                    chunk = new Chunk("Amount", FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
                    cell = new PdfPCell(new Phrase(chunk));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    cell.BorderColor = BaseColor.BLACK;
                    table.AddCell(cell);





                    int i1 = 1;
                    foreach (var GLtoVendorDetails in postingData.Where(x => x.fld_DocType == "KR" && x.fld_flag == flag).OrderBy(o => o.fld_ItemNo).Distinct())

                    {

                        bill = bill + 1;
                        chunk = new Chunk(bill.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk((GLtoVendorDetails.fld_GL == null ? "" : GLtoVendorDetails.fld_GL.ToString()), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GLtoVendorDetails.fld_VendorCode == null ? "" : GLtoVendorDetails.fld_VendorCode.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GLtoVendorDetails.fld_Desc == null ? "" : GLtoVendorDetails.fld_Desc.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        chunk = new Chunk(GetTriager.GetTotalForMoney(GLtoVendorDetails.fld_Amount) == null ? "" : GetTriager.GetTotalForMoney(GLtoVendorDetails.fld_Amount).ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                    }
                    i1++;
                    flag++;

                }
                pdfDoc.Add(table);


            }
            else
            {
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            //Modify by Shazana 19/6/2023
            Response.AppendHeader("Content-Disposition", "inline; filename=" + "GLtoVendor" + ".pdf");
            return new FileStreamResult(output, "application/pdf");

        }

        //Added by Shazana 19/6/2023
        public void exportExcelGLtoGL(int? MonthList, int? YearList)
        {
            int Userid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            GridView gv = new GridView();
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var message = "";

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var postingData = new List<vw_SAPPostData>();
            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();


            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()))
            {
                postingData = dbr.vw_SAPPostData
                    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).ToList();

                var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList).FirstOrDefault();
                ViewBag.ClosingStatus = ClosingTransaction.fld_StsTtpUrsNiaga;

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

            }

            int j = 1; ;
            gv.AutoGenerateColumns = false;
            gv.Columns.Add(new BoundField { HeaderText = "Item No.", DataField = "j" });
            gv.Columns.Add(new BoundField { HeaderText = "GL", DataField = "fld_GL" });
            gv.Columns.Add(new BoundField { HeaderText = "IO", DataField = "fld_IO" });
            gv.Columns.Add(new BoundField { HeaderText = "Activity Code", DataField = "fld_SAPActivityCode" });
            gv.Columns.Add(new BoundField { HeaderText = "Description", DataField = "fld_Desc" });
            gv.Columns.Add(new BoundField { HeaderText = "Amount", DataField = "fld_Amount" });

            DataTable dt = new DataTable();
            dt.Columns.Add("j");
            dt.Columns.Add("fld_GL");
            dt.Columns.Add("fld_IO");
            dt.Columns.Add("fld_SAPActivityCode");
            dt.Columns.Add("fld_Desc");
            dt.Columns.Add("fld_Amount");

            foreach (var item in postingData.Where(x => x.fld_DocType == "A2").OrderBy(o => o.fld_ItemNo).Distinct())
            {
                dt.Rows.Add(j, item.fld_GL, item.fld_IO, item.fld_SAPActivityCode, item.fld_Desc, item.fld_Amount);
                j++;
            }

            gv.DataSource = dt;

            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=GLtoGL.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            gv.RenderControl(htw);

            var GLtoGL = postingData.DistinctBy(s => s.fld_SAPPostRefID).Where(x => x.fld_DocType == "A2").FirstOrDefault();
            var postRefGuid = postingData.Where(x => x.fld_DocType == "A2").Select(s => s.fld_SAPPostRefID).Distinct().FirstOrDefault();

            if (GLtoGL == null)
            { }
            else
            {
                string headerTable = @"<Table>            
            <tr><td colspan=6 align =center style=font-size:medium>" + "GL To GL" + "</td></tr>" +

                "<tr><td align =right style=font-size:medium >Checkrol Ref No:</td>" + "<td colspan = 2 align = left style=font-size:medium > " + (GLtoGL.fld_RefNo == null ? " " : GLtoGL.fld_RefNo) + "</td>" +
               "<td align =right style=font-size:medium >Sap Document No</td>" + "<td colspan = 2 align = left style=font-size:medium >" + (GLtoGL.fld_NoDocSAP == null ? "" : ":" + GLtoGL.fld_NoDocSAP) + "</td></tr> " +
                 "<tr><td align =right style=font-size:medium >Date Generated:</td>" + "<td colspan=2 align=left style=font-size:medium >" + GLtoGL.fld_DocDate.ToString() + "</td></tr>" +
                 "</ Table>";

                string headerTable2 = @"<Table>            
         <tr><td align=right style=font-size:medium >Header Text:</td>" + "<td colspan=2 align=left style=font-size:medium > " + (GLtoGL.fld_HeaderText == null ? "" : GLtoGL.fld_HeaderText.ToString()) + "</td></tr>" +
             "<tr><td colspan =1 align =right style=font-size:medium >Document Type:</td>" + "<td colspan=2 align=left style= font-size:medium > " + (GLtoGL.fld_DocType == null ? "" : GLtoGL.fld_DocType.ToString()) + "</td></tr>" +
             "</ Table>";

                string headerTable3 = @"<Table>            
       <tr><td colspan =1 align =right style=font-size:medium >Posting Month:</td>" + "<td colspan = 2 align = left style = font-size:medium > " + GLtoGL.fld_Month.ToString() + "</td>" +
           "<td colspan =1 align =right style=font-size:medium >Posting Year:</td>" + "<td colspan = 2 align = left style = font-size:medium > " + GLtoGL.fld_Year.ToString() + "</td>" + "</tr> " +

           "<tr><td colspan =1 align =right style=font-size:medium >Posting Date:</td>" + "<td colspan = 2 align = left style = font-size:medium > " + GLtoGL.fld_PostingDate.ToString() + "</td>" +
           "</tr> " +
           "</ Table>";


                Response.Write(headerTable);
                Response.Write(headerTable2);
                Response.Write(headerTable3);
            }
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

        }

        public void exportExcelGLtoVendor(int? MonthList, int? YearList)
        {
            int Userid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            GridView gv = new GridView();
            GridView gvk = new GridView();
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var message = "";

            var postingData = new List<vw_SAPPostData>();
            var Syarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            ViewBag.NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            ViewBag.NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_NoSyarikat).FirstOrDefault();

            if (!String.IsNullOrEmpty(MonthList.ToString()) && !String.IsNullOrEmpty(YearList.ToString()))
            {
                postingData = dbr.vw_SAPPostData
                    .Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID).ToList();

                var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthList && x.fld_Year == YearList).FirstOrDefault();
                ViewBag.ClosingStatus = ClosingTransaction.fld_StsTtpUrsNiaga;

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

            }
            int dataCount = 1;


            if (postingData.Count() > 0)
            {

                int k = 1; ;
                gv.AutoGenerateColumns = false;
                gv.Columns.Add(new BoundField { HeaderText = "Item No", DataField = "k" });
                gv.Columns.Add(new BoundField { HeaderText = "GL", DataField = "fld_GL" });
                gv.Columns.Add(new BoundField { HeaderText = "Vendor No", DataField = "fld_VendorCode" });
                gv.Columns.Add(new BoundField { HeaderText = "Description", DataField = "fld_Desc" });
                gv.Columns.Add(new BoundField { HeaderText = "Amount", DataField = "fld_Amount" });


                DataTable dt = new DataTable();
                dt.Columns.Add("k");
                dt.Columns.Add("fld_GL");
                dt.Columns.Add("fld_VendorCode");
                dt.Columns.Add("fld_Desc");
                dt.Columns.Add("fld_Amount");




                int flag = 1;
                int flag1 = 0;
                foreach (var krFlag in postingData.DistinctBy(s => s.fld_flag).Where(x => x.fld_DocType == "KR" && x.fld_flag == flag))
                {
                    //dt.Rows.Add("Reference No. " + krFlag.fld_RefNo + "-" + flag);

                    int i1 = 1;
                    foreach (var GLtoVendorDetails in postingData.Where(x => x.fld_DocType == "KR" && x.fld_flag == flag).OrderBy(o => o.fld_ItemNo).Distinct())
                    {

                        if (flag != flag1)
                        {
                            dt.Rows.Add("Reference No. " + krFlag.fld_RefNo + "-" + flag, "SAP Document No " + (krFlag.fld_DocNoSAP == null ? "" : krFlag.fld_DocNoSAP.ToString()));
                            dt.Rows.Add(i1, (GLtoVendorDetails.fld_GL == null ? "" : GLtoVendorDetails.fld_GL.ToString()), GLtoVendorDetails.fld_VendorCode == null ? "" : GLtoVendorDetails.fld_VendorCode.ToString(), GLtoVendorDetails.fld_Desc == null ? "" : GLtoVendorDetails.fld_Desc.ToString(), GetTriager.GetTotalForMoney(GLtoVendorDetails.fld_Amount) == null ? "" : GetTriager.GetTotalForMoney(GLtoVendorDetails.fld_Amount).ToString());
                            flag1 = flag;
                        }
                        else
                        {
                            dt.Rows.Add(i1, (GLtoVendorDetails.fld_GL == null ? "" : GLtoVendorDetails.fld_GL.ToString()), GLtoVendorDetails.fld_VendorCode == null ? "" : GLtoVendorDetails.fld_VendorCode.ToString(), GLtoVendorDetails.fld_Desc == null ? "" : GLtoVendorDetails.fld_Desc.ToString(), GetTriager.GetTotalForMoney(GLtoVendorDetails.fld_Amount) == null ? "" : GetTriager.GetTotalForMoney(GLtoVendorDetails.fld_Amount).ToString());
                            flag1 = flag;
                        }

                        k++;
                        i1++;
                        // flag1 = flag;
                    }

                    // j++;
                    flag++;
                }
                gv.DataSource = dt;
                //gv.DataSource = dt;
                gv.DataBind();
                //gv.DataBind();

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=GLtoVendor.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";

                var GLtoGL = postingData.DistinctBy(s => s.fld_SAPPostRefID).Where(x => x.fld_DocType == "A2").FirstOrDefault();
                var postRefGuid = postingData.Where(x => x.fld_DocType == "A2").Select(s => s.fld_SAPPostRefID).Distinct().FirstOrDefault();
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);

                gv.RenderControl(htw);


                var GLtoVendor = postingData.DistinctBy(s => s.fld_SAPPostRefID).Where(x => x.fld_DocType == "KR").FirstOrDefault();

                if (GLtoVendor == null)
                { }
                else
                {
                    string headerTable = @"<Table>            
            <tr><td colspan=5 align =center style=font-size:medium>" + "GL To Vendor" + " </td></tr>" +
                    "<tr><td align =right style=font-size:medium >Checkroll Group No::</td>" + "<td colspan = 2 align = left style=font-size:medium > " + (GLtoVendor.fld_RefNo == null ? "" : GLtoVendor.fld_RefNo.ToString()) + "</td>" +
                   "<td colspan = 2 align =right style=font-size:medium ></td></tr> " +

                     "<tr><td align =right style=font-size:medium >Date Generated:</td>" + "<td colspan=2 align=left style=font-size:medium > " + GLtoVendor.fld_DocDate.ToString() + "</td>" +
                     "<td colspan = 2 align = center style = font-size:medium > </td></tr>" +
                     "</ Table>";

                    string headerTable2 = @"<Table>            
         <tr><td align=right style=font-size:medium>Header Text:</td>" + "<td colspan=2 align=left style=font-size:medium > " + (GLtoVendor.fld_HeaderText == null ? "" : GLtoVendor.fld_HeaderText.ToString()) + "</td></tr>" +
                 "<tr><td colspan =1 align =right style=font-size:medium >Document Type:</td>" + "<td colspan = 2 align = left style = font-size:medium > " + (GLtoVendor.fld_DocType == null ? "" : GLtoVendor.fld_DocType.ToString()) + "</td></tr>" +
                 "</ Table>";

                    string headerTable3 = @"<Table>            
           <tr><td colspan =1 align =right style=font-size:medium >Posting Month:</td>" + "<td colspan = 2 align = left style = font-size:medium > " + GLtoVendor.fld_Month.ToString() + "</td>" +
               "<td colspan =1 align =right style=font-size:medium >Posting Year:</td>" + "<td colspan = 1 align = left style = font-size:medium > " + GLtoVendor.fld_Year.ToString() + "</td></tr> " +

               "<tr><td colspan =1 align =right style=font-size:medium >Posting Date:</td>" + "<td colspan = 2 align = left style = font-size:medium > " + GLtoVendor.fld_PostingDate.ToString() + "</td>" +
               "</tr> " +
               "</ Table>";

                    Response.Write(headerTable);
                    Response.Write(headerTable2);
                    Response.Write(headerTable3);
                }
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

            }
        }

        //Added by Shazana 23/6/2023
        public ActionResult _EditSAPDataDetail(int? ItemID, Guid SAPPostRefID, string MonthList, string YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models SapModel = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            tbl_SAPPostDataDetails tbl_SAPPostDataDetails = new tbl_SAPPostDataDetails();
            tbl_SAPPostDataDetails = SapModel.tbl_SAPPostDataDetails.Where(x => x.fld_ItemNo == ItemID && x.fld_SAPPostRefID == SAPPostRefID).FirstOrDefault();
            return View(tbl_SAPPostDataDetails);
        }

        public JsonResult _UpdateDataDetail(Guid fld_ID, string fld_GL, string fld_IO)
        {

            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg, statusmsg = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models SapModel = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            tbl_SAPPostDataDetails tbl_SAPPostDataDetails = new tbl_SAPPostDataDetails();
            tbl_SAPPostDataDetails = SapModel.tbl_SAPPostDataDetails.Find(fld_ID);

            if(fld_GL != null && fld_GL =="")
            {
                tbl_SAPPostDataDetails.fld_GL = "";
            }
            else if (fld_GL != null)
            {
                tbl_SAPPostDataDetails.fld_GL = fld_GL.PadLeft(10, '0');
            }
            try
            {
                tbl_SAPPostDataDetails.fld_GL = tbl_SAPPostDataDetails.fld_GL;
                SapModel.Entry(tbl_SAPPostDataDetails).State = EntityState.Modified;
                SapModel.SaveChanges();
                msg = GlobalResEstate.msgUpdate;
                statusmsg = "success";



                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgError;
                statusmsg = "warning";
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
            return Json(new { msg, statusmsg });
            // return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
        }

        //Added by Shazana 1/8/2023
        public JsonResult GetWilayahInfo(string SyarikatID)
        {
            List<SelectListItem> wilayahlist = new List<SelectListItem>();
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0; //Modified by Shazana 1/8/2023
            int? WilayahID = 0; //Modified by Shazana 1/8/2023
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID, out LadangID, getuserid, User.Identity.Name); //Modified by Shazana 1/8/2023
            var syarikatCodeId = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kodSAPSyarikat" && x.fldDeleted == false && x.fldOptConfValue == SyarikatID.ToString() && x.fld_NegaraID == NegaraID).Select(x => x.fld_SyarikatID).FirstOrDefault();
            int SyarikatCode = Convert.ToInt16(syarikatCodeId);

            if (getwilyah.GetAvailableWilayah(SyarikatCode))
            {
                if (WilayahID == 0)
                {
                    //dapatkan ladang filter by costcenter
                    var listladang2 = db.tbl_Ladang.Where(x => x.fld_CostCentre == SyarikatID && x.fld_SyarikatID == SyarikatCode && x.fld_Deleted == false).Select(x => x.fld_WlyhID).ToList();
                    var listwilayah1 = db.tbl_Wilayah.Where(x => x.fld_Deleted == false && listladang2.Contains(x.fld_ID)).ToList();
                    wilayahlist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    wilayahlist.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                    //  ladanglist = new SelectList(listwilayah1.OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));

                }
                else if (WilayahID != 0 && LadangID != 0)
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }

                else
                {
                    wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID2 && x.fld_ID == WilayahID && x.fld_Deleted == false).OrderBy(o => o.fld_WlyhName).Select(s => new SelectListItem { Value = s.fld_ID.ToString(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                    ladanglist.Insert(0, (new SelectListItem { Text = "Semua", Value = "0" }));
                }
            }

            return Json(wilayahlist);
        }

        //Added by Shazana 1/8/2023
        public JsonResult GetLadang(int WilayahID, string SyarikatID, int LadangID)
        {
            List<SelectListItem> ladanglist = new List<SelectListItem>();

            int? NegaraID = 0;
            int? SyarikatID2 = 0;
            int? WilayahID2 = 0;
            int? LadangID2 = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetData(out NegaraID, out SyarikatID2, out WilayahID2, out LadangID2, getuserid, User.Identity.Name);


            if (getwilyah.GetAvailableWilayah(SyarikatID2))
            {
                if (WilayahID == 0)
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_CostCentre == "0" && x.fld_Deleted_L == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
                else
                {
                    ladanglist = new SelectList(db2.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_CostCentre == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted_L == false).OrderBy(o => o.fld_LdgCode).Select(s => new SelectListItem { Value = s.fld_LadangID.ToString(), Text = s.fld_LdgCode + " - " + s.fld_NamaLadang }), "Value", "Text").ToList(); //modified by kamalia 1/2/2022
                }
            }

            return Json(ladanglist);
        }
        //Added by Shazana 1/8/2023

        public JsonResult ReverseSAPDetailsRemove(int Mode)
        //public JsonResult ReverseSAPDetailsRemove(Guid refid, string CekrolRefNo,int NegaraId, int SyarikatId, int WilayahId, int LadangId, int Month, int Year, int Mode )
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            string msg, statusmsg = "";

            try
            {
                //if (Mode == 1 || Mode == 2)
                //{
                //    var returnMsgData = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == refid);
                //    dbr.tbl_SAPPostReturn.RemoveRange(returnMsgData);
                //    dbr.SaveChanges();


                //    var removeDataDetails = dbr.tbl_SAPPostDataDetails.Where(x => x.fld_SAPPostRefID == refid);
                //    dbr.tbl_SAPPostDataDetails.RemoveRange(removeDataDetails);
                //    dbr.SaveChanges();

                //    var removePostRef = dbr.tbl_SAPPostRef.Where(x => x.fld_ID == refid);
                //    dbr.tbl_SAPPostRef.RemoveRange(removePostRef);
                //    dbr.SaveChanges();
                //    msg = "berjaya";
                //}

                msg = "berjaya";


                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }


            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                msg = GlobalResEstate.msgError;
                statusmsg = "warning";
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
            return Json(new {statusmsg });
            // return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
        }

        //Added by Shazana 1/9/2023
        public ActionResult ReverseSAP(string filter)
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

        public ActionResult _ReverseSAP(int? MonthList, int? YearList)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //added by kamalia 24/11/21
            MVC_SYSTEM_MasterModels MasterModel = new MVC_SYSTEM_MasterModels();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Added by Shazana 23/6/2023
            int? roleuser = getidentity.getRoleID(getuserid);
            ViewBag.RoleUser = roleuser;

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
               

                //farahin tambah - 30/12/2021
                var statusProceedA2 = dbr.tbl_SAPPostRef.Where(x => x.fld_Month == MonthList && x.fld_Year == YearList &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_DocType == "A2").FirstOrDefault();


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

            ViewBag.Existing = db.tbl_SokPermhnWang.Where(x => x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_LadangID == LadangID).Any();
            ViewBag.GetSokongWil = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_SokongWilGM_Status == 1).Any();
            ViewBag.GetTerimaHQ = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TerimaHQ_Status == 1).Any();

            ViewBag.GetTolakHQ = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakHQ_Status == 1).Any();
            ViewBag.GetTolakWilGM = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWilGM_Status == 1).Any();
            ViewBag.GetTolakWil = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_TolakWil_Status == 1).Any();
            ViewBag.GetJumPermohonan = MasterModel.tbl_SokPermhnWang.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(s => s.fld_JumlahPermohonan).FirstOrDefault();
            ViewBag.ReferenceNo = dbr.tbl_SAPPostRef.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).Select(s => s.fld_RefNo).FirstOrDefault();

            var verifystatus = dbr.tbl_SAPPostRef.Where(x => x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList && x.fld_DocType == "KR").Select(s => s.fld_RefNo).FirstOrDefault();
            if (verifystatus == null || verifystatus == "")
            { ViewBag.berjayaverify = "0"; }
            else
            { ViewBag.berjayaverify = "1"; }

            var Ladang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID).Select(x => x.fld_LdgCode).FirstOrDefault();
            ViewBag.Ladang = Ladang;

            ViewBag.Year = YearList;
            ViewBag.Month = MonthList;

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

            var statusPermohonan = db.tbl_SokPermhnWang.Where(x => x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == YearList && x.fld_Month == MonthList).FirstOrDefault();
            if (statusPermohonan == null)
            { ViewBag.statusPermohonan = "Permohonan tidak wujud "; }
            else if (statusPermohonan.fld_SemakWil_Status == 1 && statusPermohonan.fld_SokongWilGM_Status == 1 && statusPermohonan.fld_TerimaHQ_Status == 1)
            { ViewBag.statusPermohonan = "Lulus"; }
            else if (statusPermohonan.fld_SemakWil_Status == 0 || statusPermohonan.fld_SokongWilGM_Status == 0 || statusPermohonan.fld_TerimaHQ_Status == 0)
            { ViewBag.statusPermohonan = "Permohonan masih dalam proses "; }
            else
            { ViewBag.statusPermohonan = "Permohonan masih dalam proses "; }


            return View(postingData);


        }
    }
}