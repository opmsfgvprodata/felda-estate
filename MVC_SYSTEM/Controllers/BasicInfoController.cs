using System;
//using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
//using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ViewingModels;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using MVC_SYSTEM.App_LocalResources;
using System.Web;
using System.IO;
using MVC_SYSTEM.Attributes;
using System.Text.RegularExpressions;
using MVC_SYSTEM.CustomModels;
using static MVC_SYSTEM.Class.GlobalFunction;


namespace MVC_SYSTEM.Controllers
{
    //Check_Balik
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public partial class BasicInfoController : Controller
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
        GetLadang GetLadang = new GetLadang();
        GlobalFunction globalFunction = new GlobalFunction();

        private ChangeTimeZone timezone = new ChangeTimeZone();

        // GET: BasicInfo
        public ActionResult Index()
        {
            //Check_Balik
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getroleid = getidentity.getRoleID(getuserid);
            int?[] reportid = new int?[] { };
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            //string host, catalog, user, pass = "";
            
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.BasicInfo = "class = active";
            ViewBag.BasicInfoList = new SelectList(
                db.tblMenuLists.Where(x => x.fld_Flag == "asasldgList" && x.fldDeleted == false && x.fld_NegaraID == NegaraID &&
                                           x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID),
                "fld_Val", "fld_Desc");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string BasicInfoList, string BasicInfoSubList)
        {
            if (BasicInfoSubList == "0")
            {
                return RedirectToAction(BasicInfoList, "BasicInfo");
            }
            else
            {
                return RedirectToAction(BasicInfoSubList, "BasicInfo");
            }
        }

        //public JsonResult GetSubList(int ReportList)
        //{
        //    //Check_Balik
        //    List<SelectListItem> getsublist = new List<SelectListItem>();

        //    getsublist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "asaspkjsub" && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
        //    //getsublist = new SelectList(db.tblSubReportLists.Where(x => x.fldMainReportID == ReportList && x.fldDeleted == false).OrderBy(o => o.fldSubReportListName).Select(s => new SelectListItem { Value = s.fldSubReportListID.ToString(), Text = s.fldSubReportListName }), "Value", "Text").ToList();

        //    return Json(getsublist);
        //}

        public ActionResult EstateInfo()
        {
            //Check_Balik
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            MasterModels.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
            string kodngri = tbl_Ladang.fld_KodNegeri.ToString();
            string negeri = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fldOptConfValue==kodngri && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false && x.fldOptConfValue == kodngri).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.Negeri = negeri;

            return View(tbl_Ladang);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstateInfo(MasterModels.tbl_Ladang tbl_Ladang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            tbl_Ladang.fld_PengurusSblm = "-";
            tbl_Ladang.fld_AdressSblm = "-";
            tbl_Ladang.fld_TelSblm = "-";
            tbl_Ladang.fld_FaxSblm = "-";
            tbl_Ladang.fld_LdgEmailSblm = "-";

            if (ModelState.IsValid)
            {
                try
                {

                    //var getdata = db.tbl_Ladang.Where(w => w.fld_ID == tbl_Ladang.fld_ID && w.fld_WlyhID == tbl_Ladang.fld_WlyhID).FirstOrDefault();
                    //getdata.fld_Pengurus = tbl_Ladang.fld_Pengurus.ToUpper();
                    //getdata.fld_Adress = tbl_Ladang.fld_Adress.ToUpper();
                    //getdata.fld_Tel = tbl_Ladang.fld_Tel;
                    //getdata.fld_Fax = tbl_Ladang.fld_Fax;
                    //db.Entry(getdata).State = EntityState.Modified;
                    //db.SaveChanges();


                    var getdatahq = db.tbl_Ladang.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_ID == tbl_Ladang.fld_ID && w.fld_WlyhID == tbl_Ladang.fld_WlyhID).FirstOrDefault();
                    getdatahq.fld_PengurusSblm = getdatahq.fld_Pengurus;
                    getdatahq.fld_AdressSblm = getdatahq.fld_Adress;
                    getdatahq.fld_TelSblm = getdatahq.fld_Tel;
                    getdatahq.fld_FaxSblm = getdatahq.fld_Fax;
                    getdatahq.fld_LdgEmailSblm = getdatahq.fld_LdgEmail;

                    getdatahq.fld_Pengurus = tbl_Ladang.fld_Pengurus.ToUpper();
                    getdatahq.fld_Adress = tbl_Ladang.fld_Adress.ToUpper();
                    getdatahq.fld_Tel = tbl_Ladang.fld_Tel;
                    getdatahq.fld_Fax = tbl_Ladang.fld_Fax;
                    getdatahq.fld_LdgEmail = tbl_Ladang.fld_LdgEmail;


                    db.Entry(getdatahq).State = EntityState.Modified;
                    db.SaveChanges();

                    var getid = tbl_Ladang.fld_ID;
                    //return RedirectToAction("EstateInfo", "BasicInfo");
                    //string a = "alert('this is alert')";
                    //return JavaScript(a);
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    //return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            return RedirectToAction("EstateInfo", "BasicInfo");
            //else
            //{
            //    return Json(new { success = true, msg = "Please check field you inserted.", status = "warning", checkingdata = "1" });
            //}

            //return View();
        }

        public ActionResult LevelsInfoGMN()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.BasicInfo = "class = active";

            List<SelectListItem> CostCentreSearch = new List<SelectListItem>();

            CostCentreSearch = new SelectList(db.tbl_CostCentre.Where(x=>x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCentre).Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }), "Value", "Text").ToList();
            CostCentreSearch.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.CostCentreSearch = CostCentreSearch;

            return View();
        }

        public PartialViewResult _LevelsInfoGMNSearch(string CostCentreSearch = "", int page = 1, string sort = "fld_Luas", string sortdir = "DESC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value,
                NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<tbl_PktUtamaOthrList>();

            List<tbl_PktUtamaOthrList> PktUtamaOthrList = new List<tbl_PktUtamaOthrList>();
            

            if (CostCentreSearch == "0")
            {
                PktUtamaOthrList = dbview.tbl_PktUtamaOthr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.Content = PktUtamaOthrList;
                records.TotalRecords = PktUtamaOthrList.Count();
            }

            else
            {
                PktUtamaOthrList = dbview.tbl_PktUtamaOthr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CostCentreCode == CostCentreSearch)
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.Content = PktUtamaOthrList;
                records.TotalRecords = PktUtamaOthrList.Count();
            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.pageSize = pageSize;

            return PartialView(records);
        }

        public ActionResult PktOthrCreate()
        {
            string host, catalog, user, pass = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.BasicInfo = "class = active";

            List<SelectListItem> CostCentre = new List<SelectListItem>();

            var GetUsedCC = dbr.tbl_PktUtamaOthr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_CostCentreCode).ToArray();

            CostCentre = new SelectList(db.tbl_CostCentre.Where(x => !GetUsedCC.Contains(x.fld_CostCentre) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCentre).Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }), "Value", "Text").ToList();
            CostCentre.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.CostCentre = CostCentre;

            List<SelectListItem> UnitLuas = new List<SelectListItem>();

            UnitLuas = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "unitLuas" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            UnitLuas.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.UnitLuas = UnitLuas;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PktOthrCreate(tbl_PktUtamaOthrViewModelCreate pktUtamaOthrViewModelCreate, string kodH)
        {
            //Check_Balik
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            tbl_PktUtamaOthr tbl_PktUtamaOthr = new tbl_PktUtamaOthr();

            if (ModelState.IsValid)
            {
                try
                {
                    tbl_PktUtamaOthr.fld_PktCode = pktUtamaOthrViewModelCreate.fld_PktCode.ToUpper();
                    tbl_PktUtamaOthr.fld_CostCentreCode = pktUtamaOthrViewModelCreate.fld_CostCentreCode.ToUpper();
                    tbl_PktUtamaOthr.fld_Luas = pktUtamaOthrViewModelCreate.fld_Luas;
                    tbl_PktUtamaOthr.fld_UnitLuas = GetConfig.UppercaseFirst(pktUtamaOthrViewModelCreate.fld_UnitLuas);
                    tbl_PktUtamaOthr.fld_PktCodeDesc = GetConfig.UppercaseFirst(pktUtamaOthrViewModelCreate.fld_PktCodeDesc);
                    tbl_PktUtamaOthr.fld_NamaPenyelia = GetConfig.UppercaseFirst(pktUtamaOthrViewModelCreate.fld_NamaPenyelia);
                    tbl_PktUtamaOthr.fld_CreatedDT = timezone.gettimezone();
                    tbl_PktUtamaOthr.fld_CreatedBy = getuserid;
                    tbl_PktUtamaOthr.fld_Deleted = false;
                    tbl_PktUtamaOthr.fld_NegaraID = NegaraID;
                    tbl_PktUtamaOthr.fld_SyarikatID = SyarikatID;
                    tbl_PktUtamaOthr.fld_WilayahID = WilayahID;
                    tbl_PktUtamaOthr.fld_LadangID = LadangID;

                    dbr.tbl_PktUtamaOthr.Add(tbl_PktUtamaOthr);
                    dbr.SaveChanges();

                    db.Dispose();
                    dbr.Dispose();

                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgAdd,
                        status = "success",
                        checkingdata = "0",
                        method = "2",
                        div = "searchResult",
                        rootUrl = domain,
                        action = "_LevelsInfoGMNSearch",
                        controller = "BasicInfo",
                        paramName = "CostCentreSearch",
                        paramValue = "0"
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgError,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }
            else
            {
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult PktOthrEdit(long id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.BasicInfo = "class = active";

            List<SelectListItem> CostCentre = new List<SelectListItem>();

            CostCentre = new SelectList(db.tbl_CostCentre.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCentre).Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }), "Value", "Text").ToList();
            CostCentre.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.CostCentre = CostCentre;

            List<SelectListItem> UnitLuas = new List<SelectListItem>();

            UnitLuas = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "unitLuas" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            UnitLuas.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.UnitLuas = UnitLuas;

            var PktOtherDetail = dbr.tbl_PktUtamaOthr.SingleOrDefault(x => x.fld_PktID == id);

            tbl_PktUtamaOthrViewModelEdit pktUtamaOthrViewModelEdit = new tbl_PktUtamaOthrViewModelEdit();

            PropertyCopy.Copy(pktUtamaOthrViewModelEdit, PktOtherDetail);

            string GetKodKategori = db.tbl_CostCentre.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_CostCentre == PktOtherDetail.fld_CostCentreCode).Select(s => s.fld_KodKtgri).FirstOrDefault();
            string ktgriAxtvt = db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodKategori == GetKodKategori).Select(s => s.fld_Kategori).FirstOrDefault();
            ViewBag.id = PktOtherDetail.fld_PktID;
            ViewBag.KategoriAktiviti = ktgriAxtvt;

            return View(pktUtamaOthrViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PktOthrEdit(tbl_PktUtamaOthrViewModelEdit pktUtamaOthrViewModelEdit)
        {
            //Check_Balik
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (ModelState.IsValid)
            {
                try
                {
                    var pktUtamaOthrData = dbr.tbl_PktUtamaOthr.SingleOrDefault(x => x.fld_PktID == pktUtamaOthrViewModelEdit.fld_PktID);

                    pktUtamaOthrData.fld_PktCodeDesc = GetConfig.UppercaseFirst(pktUtamaOthrViewModelEdit.fld_PktCodeDesc);
                    pktUtamaOthrData.fld_Luas = pktUtamaOthrViewModelEdit.fld_Luas;
                    pktUtamaOthrData.fld_UnitLuas = GetConfig.UppercaseFirst(pktUtamaOthrViewModelEdit.fld_UnitLuas);
                    pktUtamaOthrData.fld_NamaPenyelia = GetConfig.UppercaseFirst(pktUtamaOthrViewModelEdit.fld_NamaPenyelia);

                    dbr.Entry(pktUtamaOthrData).State = EntityState.Modified;
                    dbr.SaveChanges();

                    dbr.Dispose();

                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg = GlobalResEstate.msgUpdate,
                        status = "success",
                        checkingdata = "0",
                        method = "2",
                        div = "searchResult",
                        rootUrl = domain,
                        action = "_LevelsInfoGMNSearch",
                        controller = "BasicInfo",
                        paramName = "CostCentreSearch",
                        paramValue = "0"
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgError,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }
            else
            {
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "warning",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult PktOthrDelete(long id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.BasicInfo = "class = active";

            List<SelectListItem> CostCentre = new List<SelectListItem>();

            CostCentre = new SelectList(db.tbl_CostCentre.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_CostCentre).Select(s => new SelectListItem { Value = s.fld_CostCentre, Text = s.fld_CostCentre }), "Value", "Text").ToList();
            CostCentre.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.CostCentre = CostCentre;

            List<SelectListItem> UnitLuas = new List<SelectListItem>();

            UnitLuas = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "unitLuas" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            UnitLuas.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "" }));

            ViewBag.UnitLuas = UnitLuas;

            var PktOtherDetail = dbr.tbl_PktUtamaOthr.SingleOrDefault(x => x.fld_PktID == id);

            string GetKodKategori = db.tbl_CostCentre.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_CostCentre == PktOtherDetail.fld_CostCentreCode).Select(s => s.fld_KodKtgri).FirstOrDefault();
            string ktgriAxtvt = db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodKategori == GetKodKategori).Select(s => s.fld_Kategori).FirstOrDefault();
            ViewBag.id = PktOtherDetail.fld_PktID;
            ViewBag.KategoriAktiviti = ktgriAxtvt;

            return View(PktOtherDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PktOthrDelete(tbl_PktUtamaOthr pktUtamaOthr)
        {
            //Check_Balik
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var pktUtamaOthrData = dbr.tbl_PktUtamaOthr.SingleOrDefault(x => x.fld_PktID == pktUtamaOthr.fld_PktID);

                pktUtamaOthrData.fld_Deleted = true;

                dbr.Entry(pktUtamaOthrData).State = EntityState.Modified;
                dbr.SaveChanges();

                dbr.Dispose();

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "2",
                    div = "searchResult",
                    rootUrl = domain,
                    action = "_LevelsInfoGMNSearch",
                    controller = "BasicInfo",
                    paramName = "CostCentreSearch",
                    paramValue = "0"
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult GetKodPeringkat(string CostCentre)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string GetKodKategori = db.tbl_CostCentre.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_CostCentre == CostCentre).Select(s => s.fld_KodKtgri).FirstOrDefault();
            string GetPrefix = db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodKategori == GetKodKategori).Select(s => s.fld_PrefixPkt).FirstOrDefault();
            
            string newpkt = "";
            string ktgriAxtvt = db.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_KodKategori == GetKodKategori).Select(s => s.fld_Kategori).FirstOrDefault();
            int kodpkt = 0;
            int newkodpkt = 0;

            string tahun = DateTime.Now.ToString("yy");

            var getpkt = dbr.tbl_PktUtamaOthr.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktCode.Contains(GetPrefix)).OrderByDescending(o => o.fld_PktID).Select(s => s.fld_PktCode).Take(1).FirstOrDefault();
            if (getpkt != null)
            {
                kodpkt = Convert.ToInt32(getpkt.Substring(4, 2));
            }
            else
            {
                kodpkt = 0;
            }
            newkodpkt = kodpkt + 1;
            newpkt = GetPrefix + tahun + newkodpkt.ToString("00");
            
            return Json(new { newpkt, ktgriAxtvt });
        }

        public ActionResult LevelsInfo(string JnsPkt="1")
        {
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> JenisPkt = new List<SelectListItem>();
            List<SelectListItem> status = new List<SelectListItem>();


            JenisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            JenisPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            status.Insert(0, (new SelectListItem { Text = "Aktif", Value = "false" }));
            status.Insert(1, (new SelectListItem { Text = "Tidak Aktif", Value = "true" }));

            ViewBag.JnsPkt = JenisPkt;
            ViewBag.Status = status;
            return View();
        }

        //comment by fatin - 10/04/2023
        //public ActionResult LevelsInfoPkt(string JnsPkt = "1", int page = 1, string sort = "fld_PktUtama", string sortdir = "ASC", string status = "false")
        public ActionResult LevelsInfoPkt(string JnsPkt = "1", int page = 1, string sort = "fld_CreateDate", string sortdir = "DESC", string status = "false") //fatin added - 10/04/2023
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            var boolStatus = bool.Parse(status);

            //sarah added 05/01/2023
            List<CustomModels.CustMod_PeringkatUtama> CustMod_PeringkatUtama = new List<CustMod_PeringkatUtama>();

            var pktutama = dbview.tbl_PktUtama.Where(x => (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus).ToList();

            foreach (var tanaman in pktutama)
            {
                //fatin added - 16/05/2023
                dbview.SetCommandTimeout(240);
                //end

                var namatanaman = db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == tanaman.fld_StatusTnmn && x.fldDeleted == false).Select(x => x.fldOptConfDesc).FirstOrDefault();
                CustMod_PeringkatUtama.Add(new CustMod_PeringkatUtama()
                {
                    fld_IOcode = tanaman.fld_IOcode,
                    fld_PktUtama = tanaman.fld_PktUtama,
                    fld_NamaPktUtama = tanaman.fld_NamaPktUtama,
                    fld_LsPktUtama = tanaman.fld_LsPktUtama,
                    fld_StatusTnmn = tanaman.fld_StatusTnmn,
                    namatanaman = namatanaman,
                    fld_ID = tanaman.fld_ID,
                    // fatin added - 10/04/2023
                    fld_CreateDate = tanaman.fld_CreateDate,
                    fld_Deleted = tanaman.fld_Deleted,
                    fld_NegaraID = tanaman.fld_NegaraID,
                    fld_SyarikatID = tanaman.fld_SyarikatID,
                    fld_WilayahID = tanaman.fld_WilayahID,
                    fld_LadangID = tanaman.fld_LadangID
                    //end
                });

            }

            //original code
            //int pageSize = int.Parse(GetConfig.GetData("paging"));
            //var records = new PagedList<ViewingModels.tbl_PktUtama>();
            //records.Content = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
            //       .OrderBy(sort + " " + sortdir)
            //       .Skip((page - 1) * pageSize)
            //       .Take(pageSize)
            //       .ToList();

            //sarah modified 05/01/2023
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<CustomModels.CustMod_PeringkatUtama>();
            records.Content = CustMod_PeringkatUtama
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = CustMod_PeringkatUtama.Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return PartialView(records);

        }
        //sarah end

        public ActionResult LevelsInfoSubPkt(string JnsPkt = "2", int page = 1, string sort = "fld_Pkt", string sortdir = "ASC", string status = "false")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_SubPkt>();
            var boolStatus = bool.Parse(status);

            records.Content = dbview.tbl_SubPkt.Where(x => (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_SubPkt.Where(x => (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        public ActionResult LevelsInfoBlok(string JnsPkt = "3", int page = 1, string sort = "fld_Blok", string sortdir = "ASC", string status = "false")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Blok>();
            var boolStatus = bool.Parse(status);

            records.Content = dbview.tbl_Blok.Where(x => (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_Blok.Where(x => (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        public ActionResult LevelsPktCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            //Added by Shazana 10/7/2023
            int? roleid = getidentity.getRoleID(getuserid);
            string rolename = getidentity.RolesName(roleid);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> JnsTnmn = new List<SelectListItem>();
            List<SelectListItem> StatusTnmn = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMemunggah = new List<SelectListItem>();//added by faeza 18.08.2021
            List<SelectListItem> IOlist = new List<SelectListItem>();
            List<SelectListItem> JnsLotList = new List<SelectListItem>();

            JnsTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "jnsTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            JnsTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            StatusTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            StatusTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //added by faeza 18.08.2021
            TahapKesukaranMemunggah = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            IOlist = new SelectList(db.tbl_IOSAP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_StatusUsed == null), "fld_IOcode", "fld_IOcode").ToList();
            IOlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            JnsLotList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1== "jnsLot" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
            JnsLotList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_JnsTnmn = JnsTnmn;
            ViewBag.fld_StatusTnmn = StatusTnmn;
            ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahPktUtama = TahapKesukaranMemunggah;//added by faeza 18.08.2021
            ViewBag.fld_IOcode = IOlist;
            ViewBag.fld_JnsLot = JnsLotList;
            //Added by Shazana 10/7/2023
            if (rolename == null) { rolename = ""; }
            ViewBag.rolename = rolename;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsPktCreate(Models.tbl_PktUtama tbl_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if ((ModelState.IsValid) && !string.IsNullOrEmpty(tbl_PktUtama.fld_PktUtama) && !string.IsNullOrEmpty(tbl_PktUtama.fld_NamaPktUtama))
            {
                try
                {
                    //fatin added - 12/04/2023
                    if (tbl_PktUtama.fld_IOcode != "0")
                    {
                        var checkdata = dbr.tbl_PktUtama.Where(x => (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_PktUtama == tbl_PktUtama.fld_PktUtama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                        if (checkdata == null)
                        {
                            tbl_PktUtama.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
                            tbl_PktUtama.fld_LadangID = LadangID;
                            tbl_PktUtama.fld_WilayahID = WilayahID;
                            tbl_PktUtama.fld_SyarikatID = SyarikatID;
                            tbl_PktUtama.fld_NegaraID = NegaraID;
                            tbl_PktUtama.fld_CreateDate = DateTime.Now;
                            tbl_PktUtama.fld_Deleted = false;
                            tbl_PktUtama.fld_SAPType = "IO";
                            dbr.tbl_PktUtama.Add(tbl_PktUtama);
                            dbr.SaveChanges();

                            //var checkIo = dbr.tbl_IO.Where(x => x.fld_IOcode == tbl_PktUtama.fld_IOcode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                            //checkIo.fld_Deleted = true;
                            //dbr.Entry(checkIo).State = EntityState.Modified;
                            //dbr.SaveChanges();
                            string RequestForm = Request.Form["listCount"];
                            if (RequestForm != null && RequestForm != "")
                            {
                                int listCount = Convert.ToInt32(Request.Form["listCount"]);
                                if (listCount < 1)
                                {
                                    for (int i = 1; i <= listCount; i++)
                                    {
                                        string idKaw = "ddl" + i;
                                        string idLuas = "textluas" + i;
                                        string JnsKaw = Request.Form[idKaw];
                                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                                        if (checkKwsn == null)
                                        {
                                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                                            KawTidakBerhasil.fld_KodPkt = tbl_PktUtama.fld_PktUtama;
                                            KawTidakBerhasil.fld_LevelPkt = 1;
                                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                                            KawTidakBerhasil.fld_LadangID = LadangID;
                                            KawTidakBerhasil.fld_Deleted = false;
                                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                                            dbr.SaveChanges();
                                        }
                                    }
                                }
                            }

                            //Added by Shazana 13/6/2023

                            string RequestFormKesukaran = Request.Form["listCount_Kesukaran"];
                            if (RequestFormKesukaran != null && RequestFormKesukaran != "")
                            {
                                int listCount_Kesukaran = Convert.ToInt32(Request.Form["listCount_Kesukaran"]);
                                if (listCount_Kesukaran > 0)
                                {
                                    for (int i = 1; i <= listCount_Kesukaran; i++)
                                    {
                                        string idJenisKesukaran = "dd1" + i;
                                        string idTahapKesukaran = "dd2" + i;
                                        string JenisKesukaran = Request.Form[idJenisKesukaran];
                                        string TahapKesukaran = Request.Form[idTahapKesukaran];

                                        if (JenisKesukaran != "0" || TahapKesukaran != "0")
                                        {
                                            var JenisKesukaranDetails = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == JenisKesukaran && x.fldOptConfFlag2.Contains("HargaKesukaran") && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
                                            var TahapKesukaranDetails = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag2 != "HargaKesukaran" && x.fldOptConfFlag1 == JenisKesukaranDetails.fldOptConfFlag1 && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfValue == TahapKesukaran).FirstOrDefault();

                                            //Commented by Shazana 22/7/2023
                                            //var checkKwsn = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_JenisHargaKesukaran == JenisKesukaran && x.fld_PktUtama == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                                            //Added by Shazana 22/7/2023
                                            var checkKwsn = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JenisKesukaranDetails.fldOptConfFlag3 && x.fld_PktUtama == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

                                            if (checkKwsn == null && JenisKesukaranDetails != null && TahapKesukaranDetails != null)
                                            {
                                                Models.tbl_PktHargaKesukaran PktHargaKesukaran = new Models.tbl_PktHargaKesukaran();
                                                PktHargaKesukaran.fld_PktUtama = tbl_PktUtama.fld_PktUtama;
                                                PktHargaKesukaran.fld_KodJenisHargaKesukaran = JenisKesukaranDetails.fldOptConfFlag3;
                                                PktHargaKesukaran.fld_JenisHargaKesukaran = JenisKesukaran;
                                                PktHargaKesukaran.fld_KodHargaKesukaran = TahapKesukaran;
                                                PktHargaKesukaran.fld_KeteranganHargaKesukaran = TahapKesukaranDetails == null ? "" : TahapKesukaranDetails.fldOptConfDesc;
                                                PktHargaKesukaran.fld_HargaKesukaran = TahapKesukaranDetails == null ? 0 : Convert.ToDecimal(TahapKesukaranDetails.fldOptConfFlag2);
                                                PktHargaKesukaran.fld_NegaraID = NegaraID;
                                                PktHargaKesukaran.fld_SyarikatID = SyarikatID;
                                                PktHargaKesukaran.fld_WilayahID = WilayahID;
                                                PktHargaKesukaran.fld_LadangID = LadangID;
                                                PktHargaKesukaran.fld_CreatedBy = getuserid.ToString();
                                                PktHargaKesukaran.fld_CreatedDate = DateTime.Now;

                                                dbr.tbl_PktHargaKesukaran.Add(PktHargaKesukaran);
                                                dbr.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }

                            //Close Added by Shazana 13/6/2023

                            return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                        }
                            //fatin added - 12/04/2023
                    }
                    else
                    {
                        return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                    }
                    //end
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult LevelsPktUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            //Added by Shazana 10/7/2023
            int? roleid = getidentity.getRoleID(getuserid);
            string rolename = getidentity.RolesName(roleid);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID==LadangID && w.fld_Deleted==false).FirstOrDefault();

            List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMemunggah = new List<SelectListItem>();//added by faeza 18.08.2021
            List<SelectListItem> JnsLotList = new List<SelectListItem>();

            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_PktUtama.fld_KesukaranMenuaiPktUtama).ToList();
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_PktUtama.fld_KesukaranMembajaPktUtama).ToList();
            //added by faeza 18.08.2021
            TahapKesukaranMemunggah = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_PktUtama.fld_KesukaranMemunggahPktUtama).ToList();
            JnsLotList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsLot" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc", tbl_PktUtama.fld_JnsLot).ToList();
            JnsLotList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsKawasan" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            Kawasanlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //Added by Shazana 13/6/2023
            List<SelectListItem> JenisKesukaranlist = new List<SelectListItem>();
            JenisKesukaranlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag2 == "HargaKesukaran" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfFlag1, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            JenisKesukaranlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            List<SelectListItem> TahapKesukaranlist = new List<SelectListItem>();
            TahapKesukaranlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_JenisHargaKesukaran = JenisKesukaranlist;
            ViewBag.fld_TahapHargaKesukaran = TahapKesukaranlist;
            //------------------------------------

            ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahPktUtama = TahapKesukaranMemunggah;//added by faeza 18.08.2021
            ViewBag.fld_JnsKaw1 = Kawasanlist;
            ViewBag.fld_JnsLot = JnsLotList;
            //Added by Shazana 10/7/2023
            if (rolename == null) { rolename = ""; }
            ViewBag.rolename = rolename;

            return PartialView(tbl_PktUtama);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsPktUpdate(string id, Models.tbl_PktUtama tbl_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
                getdata.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
                if (GetIdentity.SuperPowerAdmin(User.Identity.Name) || GetIdentity.SuperAdmin(User.Identity.Name) || GetIdentity.Admin1(User.Identity.Name) || GetIdentity.Admin2(User.Identity.Name))
                {
                    getdata.fld_KesukaranMenuaiPktUtama = tbl_PktUtama.fld_KesukaranMenuaiPktUtama;
                    getdata.fld_KesukaranMembajaPktUtama = tbl_PktUtama.fld_KesukaranMembajaPktUtama;
                    getdata.fld_KesukaranMemunggahPktUtama = tbl_PktUtama.fld_KesukaranMemunggahPktUtama;//added by faeza 18.08.2021
                }
                getdata.fld_LsPktUtama = tbl_PktUtama.fld_LsPktUtama;
                getdata.fld_LuasKawTnman = tbl_PktUtama.fld_LuasKawTnman;
                getdata.fld_LuasBerhasil = tbl_PktUtama.fld_LuasBerhasil;
                getdata.fld_LuasBlmBerhasil = tbl_PktUtama.fld_LuasBlmBerhasil;
                getdata.fld_BilPokok = tbl_PktUtama.fld_BilPokok;
                getdata.fld_DirianPokok = tbl_PktUtama.fld_DirianPokok;
                getdata.fld_LuasKawTiadaTanaman = tbl_PktUtama.fld_LuasKawTiadaTanaman;
                getdata.fld_JnsLot = tbl_PktUtama.fld_JnsLot;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                string RequestForm = Request.Form["listCount"];
                if (RequestForm != null && RequestForm != "")
                {
                    var getLuas = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).AsEnumerable();
                    dbr.tbl_KawTidakBerhasil.RemoveRange(getLuas);
                    dbr.SaveChanges();

                    int listCount = Convert.ToInt32(Request.Form["listCount"]);
                    for (int i = 1; i <= listCount; i++)
                    {
                        string idKaw = "ddl" + i;
                        string idLuas = "textluas" + i;
                        string JnsKaw = Request.Form[idKaw];
                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (checkKwsn == null)
                        {
                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                            KawTidakBerhasil.fld_KodPkt = tbl_PktUtama.fld_PktUtama;
                            KawTidakBerhasil.fld_LevelPkt = 1;
                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                            KawTidakBerhasil.fld_LadangID = LadangID;
                            KawTidakBerhasil.fld_Deleted = false;
                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                            dbr.SaveChanges();
                        }
                    }
                }
                return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsPktDelete(string id, string status)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            int DeleteFlag = 1;
            Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID).FirstOrDefault();
            int CountBlok = dbr.tbl_Blok.Where(x => x.fld_KodPktutama == tbl_PktUtama.fld_PktUtama && x.fld_Deleted == false).Select(s => s.fld_Blok).Count();
            int CountSub = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == tbl_PktUtama.fld_PktUtama && x.fld_Deleted == false).Select(s => s.fld_Pkt).Count();
            if (CountBlok > 0)
            {
                DeleteFlag = 3;
            }
            else
            {
                if (CountSub > 0)
                {
                    DeleteFlag = 2;
                }
                else
                {
                    DeleteFlag = 1;
                }
            }
            if (status == "false")
            {
                ViewBag.Title = "Aktifkan";
            }
            else
            {
                ViewBag.Title = "Nyah Aktifkan";
            }
            ViewBag.DeleteFlag = DeleteFlag;
            ViewBag.Status = status;
            return PartialView(tbl_PktUtama);
        }

        [HttpPost, ActionName("LevelsPktDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsPktDeleteConfirmed(string id, string status)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            try
            {
                Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID).FirstOrDefault();
                if (tbl_PktUtama == null)
                {
                    return Json(new { success = true, msg = "Data tidak dijumpai", status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
                else
                {
                    var boolStatus = bool.Parse(status);
                    tbl_PktUtama.fld_Deleted = boolStatus;
                    dbr.Entry(tbl_PktUtama).State = EntityState.Modified;
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = "Data berjaya dikemaskini", status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsSubPktCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMemunggah = new List<SelectListItem>();//added by faeza 18.08.2021

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //added by faeza 18.08.2021
            TahapKesukaranMemunggah = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();

            ViewBag.fld_KodPktUtama = PktUtama;
            ViewBag.fld_KesukaranMenuaiPkt = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPkt = TahapKesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahPkt = TahapKesukaranMemunggah;//added by faeza 18.08.2021
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsSubPktCreate(Models.tbl_SubPkt tbl_SubPkt)
        {
            string kodpkt = tbl_SubPkt.fld_Pkt;
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if ((ModelState.IsValid) && !string.IsNullOrEmpty(tbl_SubPkt.fld_Pkt) && !string.IsNullOrEmpty(tbl_SubPkt.fld_NamaPkt))
            {
                try
                {
                    var checkdata = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == tbl_SubPkt.fld_Pkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                    if (checkdata == null)
                    {
                        tbl_SubPkt.fld_NamaPkt = tbl_SubPkt.fld_NamaPkt.ToUpper();
                        tbl_SubPkt.fld_LadangID = LadangID;
                        tbl_SubPkt.fld_WilayahID = WilayahID;
                        tbl_SubPkt.fld_SyarikatID = SyarikatID;
                        tbl_SubPkt.fld_NegaraID = NegaraID;
                        tbl_SubPkt.fld_CreateDate = DateTime.Now;
                        tbl_SubPkt.fld_Deleted = false;
                        tbl_SubPkt.fld_SAPType = "IO";
                        dbr.tbl_SubPkt.Add(tbl_SubPkt);
                        dbr.SaveChanges();

                        string RequestForm = Request.Form["listCount2"];
                        if (RequestForm != null && RequestForm != "")
                        {
                            int listCount = Convert.ToInt32(Request.Form["listCount2"]);
                            for (int i = 1; i <= listCount; i++)
                            {
                                string idKaw = "ddlPkt" + i;
                                string idLuas = "textluasPkt" + i;
                                string JnsKaw = Request.Form[idKaw];
                                decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                                var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                                if (checkKwsn == null)
                                {
                                    Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                                    KawTidakBerhasil.fld_KodPkt = kodpkt;
                                    KawTidakBerhasil.fld_LevelPkt = 2;
                                    KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                                    KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                                    KawTidakBerhasil.fld_NegaraID = NegaraID;
                                    KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                                    KawTidakBerhasil.fld_WilayahID = WilayahID;
                                    KawTidakBerhasil.fld_LadangID = LadangID;
                                    KawTidakBerhasil.fld_Deleted = false;
                                    dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                                    dbr.SaveChanges();
                                }
                            }
                        }
                        return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                    }
                    else
                    {
                        return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult LevelsSubPktUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_SubPkt tbl_SubPkt = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> TahapKesukaranMemunggah = new List<SelectListItem>();//addedby faeza 18.08.2021

            //modified by faeza 18.08.2021
            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_SubPkt.fld_KesukaranMenuaiPkt).ToList();
            //TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_SubPkt.fld_KesukaranMembajaPkt).ToList();
            //TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            TahapKesukaranMemunggah = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_SubPkt.fld_KesukaranMemunggahPkt).ToList();
            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsKawasan" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            Kawasanlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_KodPktUtama = PktUtama;
            ViewBag.fld_KesukaranMenuaiPkt = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPkt = TahapKesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahPkt = TahapKesukaranMemunggah;//added by faeza 18.08.2021
            ViewBag.fld_JnsKaw = Kawasanlist;
            return PartialView(tbl_SubPkt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsSubPktUpdate(string id, Models.tbl_SubPkt tbl_SubPkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted==false).FirstOrDefault();
                getdata.fld_NamaPkt = tbl_SubPkt.fld_NamaPkt.ToUpper();
                if (GetIdentity.SuperPowerAdmin(User.Identity.Name) || GetIdentity.SuperAdmin(User.Identity.Name) || GetIdentity.Admin1(User.Identity.Name) || GetIdentity.Admin2(User.Identity.Name))
                {
                    getdata.fld_KesukaranMenuaiPkt = tbl_SubPkt.fld_KesukaranMenuaiPkt;
                    getdata.fld_KesukaranMembajaPkt = tbl_SubPkt.fld_KesukaranMembajaPkt;
                    getdata.fld_KesukaranMemunggahPkt = tbl_SubPkt.fld_KesukaranMemunggahPkt;//added by faeza 18.08.2021
                }
                getdata.fld_LsPkt = tbl_SubPkt.fld_LsPkt;
                getdata.fld_LuasKawTnmanPkt = tbl_SubPkt.fld_LuasKawTnmanPkt;
                getdata.fld_LuasBerhasilPkt = tbl_SubPkt.fld_LuasBerhasilPkt;
                getdata.fld_LuasBlmBerhasilPkt = tbl_SubPkt.fld_LuasBlmBerhasilPkt;
                getdata.fld_BilPokokPkt = tbl_SubPkt.fld_BilPokokPkt;
                getdata.fld_DirianPokokPkt = tbl_SubPkt.fld_DirianPokokPkt;
                getdata.fld_LuasKawTiadaTanamanPkt = tbl_SubPkt.fld_LuasKawTiadaTanamanPkt;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                string RequestForm = Request.Form["listCount2"];
                if (RequestForm != null && RequestForm != "")
                {
                    var getLuas = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == tbl_SubPkt.fld_Pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).AsEnumerable();
                    dbr.tbl_KawTidakBerhasil.RemoveRange(getLuas);
                    dbr.SaveChanges();

                    int listCount = Convert.ToInt32(Request.Form["listCount2"]);
                    for (int i = 1; i <= listCount; i++)
                    {
                        string idKaw = "ddlPkt" + i;
                        string idLuas = "textluasPkt" + i;
                        string JnsKaw = Request.Form[idKaw];
                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_SubPkt.fld_Pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (checkKwsn == null)
                        {
                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                            KawTidakBerhasil.fld_KodPkt = tbl_SubPkt.fld_Pkt;
                            KawTidakBerhasil.fld_LevelPkt = 2;
                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                            KawTidakBerhasil.fld_LadangID = LadangID;
                            KawTidakBerhasil.fld_Deleted = false;
                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                            dbr.SaveChanges();
                        }
                    }
                }
                    
                return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                //return RedirectToAction("LevelsInfo", new { JnsPkt = 1 });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsSubPktDelete(string id, string status)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            int DeleteFlag = 1;
            Models.tbl_SubPkt tbl_SubPkt = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID).FirstOrDefault();
            int CountBlok = dbr.tbl_Blok.Where(x => x.fld_KodPkt == tbl_SubPkt.fld_Pkt).Select(s => s.fld_Blok).Count();
            //int CountSub = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == tbl_PktUtama.fld_PktUtama).Select(s => s.fld_Pkt).Count();
            if (CountBlok > 0)
            {
                DeleteFlag = 2;
            }
            else
            {
                DeleteFlag = 1;
            }
            if (status == "false")
            {
                ViewBag.Title = "Aktifkan";
            }
            else
            {
                ViewBag.Title = "Nyah Aktifkan";
            }
            ViewBag.DeleteFlag = DeleteFlag;
            ViewBag.Status = status;
            return PartialView(tbl_SubPkt);
        }

        [HttpPost, ActionName("LevelsSubPktDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsSubPktDeleteConfirmed(string id, string status)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            try
            {
                Models.tbl_SubPkt tbl_SubPkt = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID).FirstOrDefault();
                if (tbl_SubPkt == null)
                {
                    return Json(new { success = true, msg = "Data tidak dijumpai", status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
                else
                {
                    var boolStatus = bool.Parse(status);
                    tbl_SubPkt.fld_Deleted = boolStatus;
                    dbr.Entry(tbl_SubPkt).State = EntityState.Modified;
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = "Data berjaya dikemaskini", status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsSubPktUpdateKwsn(string Kodpkt = "", int page = 1, string sort = "fld_JnsKaw", string sortdir = "ASC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_KawTidakBerhasil>();
            records.Content = dbview.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == Kodpkt && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == Kodpkt && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        public ActionResult LevelsBlokCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> Pkt = new List<SelectListItem>();
            //List<SelectListItem> JnsKesukaran = new List<SelectListItem>();
            List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> KesukaranMemunggah = new List<SelectListItem>();//added by faeza 18.08.2021

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //KesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            //KesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //added by faeza 18.08.2021
            KesukaranMemunggah = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();

            ViewBag.fld_KodPktutama = PktUtama;
            ViewBag.fld_KodPkt = Pkt;
            ViewBag.fld_KesukaranMenuaiBlok = KesukaranMenuai;
            ViewBag.fld_KesukaranMembajaBlok = KesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahBlok = KesukaranMemunggah;//added by faeza 18.08.2021
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsBlokCreate(Models.tbl_Blok tbl_Blok)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
           
            if ((ModelState.IsValid) && !string.IsNullOrEmpty(tbl_Blok.fld_Blok) && !string.IsNullOrEmpty(tbl_Blok.fld_NamaBlok))
            {
                try
                {
                    var checkdata = dbr.tbl_Blok.Where(x => x.fld_Blok == tbl_Blok.fld_Blok && x.fld_KodPkt == tbl_Blok.fld_KodPkt && x.fld_KodPktutama == tbl_Blok.fld_KodPktutama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                    if (checkdata == null && tbl_Blok.fld_Blok!="" && tbl_Blok.fld_NamaBlok!="")
                    {
                        //GetLadang GetLadang = new GetLadang();
                        //string ldgcode = GetLadang.GetCodeLadangFromID2(LadangID.Value);
                        tbl_Blok.fld_NamaBlok = tbl_Blok.fld_NamaBlok.ToUpper();
                        tbl_Blok.fld_LadangID = LadangID;
                        tbl_Blok.fld_WilayahID = WilayahID;
                        tbl_Blok.fld_SyarikatID = SyarikatID;
                        tbl_Blok.fld_NegaraID = NegaraID;
                        tbl_Blok.fld_CreateDate = DateTime.Now;
                        tbl_Blok.fld_SAPType = "IO";
                        tbl_Blok.fld_Deleted = false;
                        dbr.tbl_Blok.Add(tbl_Blok);
                        dbr.SaveChanges();

                        string RequestForm = Request.Form["listCount3"];
                        if (RequestForm != null && RequestForm != "")
                        {
                            int listCount = Convert.ToInt32(Request.Form["listCount3"]);
                            for (int i = 1; i <= listCount; i++)
                            {
                                string idKaw = "ddlBlock" + i;
                                string idLuas = "textluasBlock" + i;
                                string JnsKaw = Request.Form[idKaw];
                                decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                                var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                                if (checkKwsn == null)
                                {
                                    Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                                    KawTidakBerhasil.fld_KodPkt = tbl_Blok.fld_Blok;
                                    KawTidakBerhasil.fld_LevelPkt = 1;
                                    KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                                    KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                                    KawTidakBerhasil.fld_NegaraID = NegaraID;
                                    KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                                    KawTidakBerhasil.fld_WilayahID = WilayahID;
                                    KawTidakBerhasil.fld_LadangID = LadangID;
                                    KawTidakBerhasil.fld_Deleted = false;
                                    dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                                    dbr.SaveChanges();
                                }
                            }
                        }

                        //dbr.SaveChanges();
                        //var getid = "";
                        //return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "", data3 = "" });
                        return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                    }
                    else
                    {
                        //return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
                        return Json(new { success = false, msg = GlobalResEstate.msgDataExist, status = "warning", checkingdata = "1" });
                    }

                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                return Json(new { success = false, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
            }
        }

        public ActionResult LevelsBlokUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_Blok tbl_Blok = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> Pkt = new List<SelectListItem>();
            //List<SelectListItem> JnsKesukaran = new List<SelectListItem>();
            List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> KesukaranMemunggah = new List<SelectListItem>();//added by faeza 18.08.2021

            //modified by faeza 18.08.2021
            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_SAPType == "IO" || x.fld_SAPType == null) && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_Blok.fld_KesukaranMenuaiBlok).ToList();
            //KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_Blok.fld_KesukaranMembajaBlok).ToList();
            //KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            KesukaranMemunggah = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMemunggah" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_Blok.fld_KesukaranMemunggahBlok).ToList();
            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsKawasan" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();
            Kawasanlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //end modified

            ViewBag.fld_KodPktutama = PktUtama;
            ViewBag.fld_KodPkt = Pkt;
            ViewBag.fld_KesukaranMenuaiBlok = KesukaranMenuai;
            ViewBag.fld_KesukaranMembajaBlok = KesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahBlok = KesukaranMemunggah;//added by faeza 18.08.2021
            ViewBag.fld_JnsKaw3 = Kawasanlist;
            return PartialView(tbl_Blok);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsBlokUpdate(string id, Models.tbl_Blok tbl_Blok)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
                getdata.fld_NamaBlok = tbl_Blok.fld_NamaBlok.ToUpper();
                if (GetIdentity.SuperPowerAdmin(User.Identity.Name) || GetIdentity.SuperAdmin(User.Identity.Name) || GetIdentity.Admin1(User.Identity.Name) || GetIdentity.Admin2(User.Identity.Name))
                {
                    getdata.fld_KesukaranMenuaiBlok = tbl_Blok.fld_KesukaranMenuaiBlok;
                    getdata.fld_KesukaranMembajaBlok = tbl_Blok.fld_KesukaranMembajaBlok;
                    getdata.fld_KesukaranMemunggahBlok = tbl_Blok.fld_KesukaranMemunggahBlok;//added by faeza 18.08.2021
                }
                getdata.fld_LsBlok = tbl_Blok.fld_LsBlok;
                getdata.fld_LuasKawTnmanBlok = tbl_Blok.fld_LuasKawTnmanBlok;
                getdata.fld_LuasBerhasilBlok = tbl_Blok.fld_LuasBerhasilBlok;
                getdata.fld_LuasBlmBerhasilBlok = tbl_Blok.fld_LuasBlmBerhasilBlok;
                getdata.fld_BilPokokBlok = tbl_Blok.fld_BilPokokBlok;
                getdata.fld_DirianPokokBlok = tbl_Blok.fld_DirianPokokBlok;
                getdata.fld_LuasKawTiadaTanamanBlok = tbl_Blok.fld_LuasKawTiadaTanamanBlok;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                string RequestForm = Request.Form["listCount3"];
                if (RequestForm != null && RequestForm != "")
                {
                    var getLuas = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).AsEnumerable();
                    dbr.tbl_KawTidakBerhasil.RemoveRange(getLuas);
                    dbr.SaveChanges();

                    int listCount = Convert.ToInt32(Request.Form["listCount3"]);
                    for (int i = 1; i <= listCount; i++)
                    {
                        string idKaw = "ddlBlock" + i;
                        string idLuas = "textluasBlock" + i;
                        string JnsKaw = Request.Form[idKaw];
                        decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
                        var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (checkKwsn == null)
                        {
                            Models.tbl_KawTidakBerhasil KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
                            KawTidakBerhasil.fld_KodPkt = tbl_Blok.fld_Blok;
                            KawTidakBerhasil.fld_LevelPkt = 1;
                            KawTidakBerhasil.fld_JnsKaw = JnsKaw;
                            KawTidakBerhasil.fld_LuasKaw = LuasKaw;
                            KawTidakBerhasil.fld_NegaraID = NegaraID;
                            KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                            KawTidakBerhasil.fld_WilayahID = WilayahID;
                            KawTidakBerhasil.fld_LadangID = LadangID;
                            KawTidakBerhasil.fld_Deleted = false;
                            dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
                            dbr.SaveChanges();
                        }
                    }
                }
                  
                var getid = id;
                return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult LevelsBlokDelete(string id, string status)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_Blok tbl_Blok = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID).FirstOrDefault();
            if (status == "false")
            {
                ViewBag.Title = "Aktifkan";
            }
            else
            {
                ViewBag.Title = "Nyah Aktifkan";
            }
            ViewBag.Status = status;
            return PartialView(tbl_Blok);
        }

        [HttpPost, ActionName("LevelsBlokDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsBlokDeleteConfirmed(string id, string status)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            try
            {
                Models.tbl_Blok tbl_Blok = dbr.tbl_Blok.Where(w => w.fld_Blok == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID).FirstOrDefault();
                if (tbl_Blok == null)
                {
                    return Json(new { success = true, msg = "Data tidak dijumpai", status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
                else
                {
                    var boolStatus = bool.Parse(status);
                    tbl_Blok.fld_Deleted = boolStatus;
                    dbr.Entry(tbl_Blok).State = EntityState.Modified;
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = "Data berjaya dikemaskini", status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        //public ActionResult LevelsMain()
        //{

        //    //string IOcode = Request.Form["IO_code"];
        //    //string IOlevel = Request.Form["RadioGroup"];
        //    //string IOref = Request.Form["IO_reff"];
        //    //string IO_code = IOcode;
        //    //var activeTab = "tab1primary";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<SelectListItem> JnsTnmn = new List<SelectListItem>();
        //    List<SelectListItem> StatusTnmn = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();

        //    JnsTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "jnsTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
        //    JnsTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    StatusTnmn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
        //    StatusTnmn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_JnsTnmn = JnsTnmn;
        //    ViewBag.fld_StatusTnmn = StatusTnmn;
        //    ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
        //    ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
        //    return PartialView();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsMain(Models.tbl_PktUtama tbl_PktUtama)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var checkdata = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_PktUtama.fld_PktUtama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                tbl_PktUtama.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
        //                tbl_PktUtama.fld_LadangID = LadangID;
        //                tbl_PktUtama.fld_WilayahID = WilayahID;
        //                tbl_PktUtama.fld_SyarikatID = SyarikatID;
        //                tbl_PktUtama.fld_NegaraID = NegaraID;
        //                tbl_PktUtama.fld_CreateDate = DateTime.Now;
        //                tbl_PktUtama.fld_Deleted = false;
        //                dbr.tbl_PktUtama.Add(tbl_PktUtama);
        //                //dbr.SaveChanges();

        //                var checkIo = dbr.tbl_IO.Where(x => x.fld_IOcode == tbl_PktUtama.fld_IOcode && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                checkIo.fld_Deleted = true;
        //                dbr.Entry(checkIo).State = EntityState.Modified;
        //                //dbr.SaveChanges();

        //                int listCount = Convert.ToInt32(Request.Form["listCount"]);
        //                for (int i = 1; i <= listCount; i++)
        //                {
        //                    string idKaw = "ddl" + i;
        //                    string idLuas = "textluas" + i;
        //                    string JnsKaw = Request.Form[idKaw];
        //                    decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
        //                    var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_PktUtama.fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                    if (checkKwsn==null)
        //                    {
        //                        Models.tbl_KawTidakBerhasil KawTidakBerhasil = new tbl_KawTidakBerhasil();
        //                        KawTidakBerhasil.fld_KodPkt = tbl_PktUtama.fld_PktUtama;
        //                        KawTidakBerhasil.fld_LevelPkt = 1;
        //                        KawTidakBerhasil.fld_JnsKaw = JnsKaw;
        //                        KawTidakBerhasil.fld_LuasKaw = LuasKaw;
        //                        KawTidakBerhasil.fld_NegaraID = NegaraID;
        //                        KawTidakBerhasil.fld_SyarikatID = SyarikatID;
        //                        KawTidakBerhasil.fld_WilayahID = WilayahID;
        //                        KawTidakBerhasil.fld_LadangID = LadangID;
        //                        KawTidakBerhasil.fld_Deleted = false;
        //                        dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
        //                        dbr.SaveChanges();
        //                    }
        //                }

        //                //var getid = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_PktUtama.fld_PktUtama).FirstOrDefault();
        //                //return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "", data3 = "" });


        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }
        //            else
        //            {
        //                //return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult LevelsMainUpdate()
        //{
        //    return View();
        //}

        //public ActionResult LevelsSub()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> PktUtama = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMenuai = new List<SelectListItem>();
        //    List<SelectListItem> TahapKesukaranMembaja = new List<SelectListItem>();

        //    PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //TahapKesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_KodPktUtama = PktUtama;
        //    ViewBag.fld_KesukaranMenuaiPkt = TahapKesukaranMenuai;
        //    ViewBag.fld_KesukaranMembajaPkt = TahapKesukaranMembaja;
        //    return PartialView();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsSub(Models.tbl_SubPkt tbl_SubPkt)
        //{
        //    string kodpkt = tbl_SubPkt.fld_Pkt;
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    //MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var checkdata = dbr.tbl_SubPkt.Where(x => x.fld_Pkt== tbl_SubPkt.fld_Pkt && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                tbl_SubPkt.fld_NamaPkt = tbl_SubPkt.fld_NamaPkt.ToUpper();
        //                tbl_SubPkt.fld_LadangID = LadangID;
        //                tbl_SubPkt.fld_WilayahID = WilayahID;
        //                tbl_SubPkt.fld_SyarikatID = SyarikatID;
        //                tbl_SubPkt.fld_NegaraID = NegaraID;
        //                tbl_SubPkt.fld_CreateDate = DateTime.Now;
        //                tbl_SubPkt.fld_Deleted = false;
        //                dbr.tbl_SubPkt.Add(tbl_SubPkt);
        //                dbr.SaveChanges();

        //                int listCount = Convert.ToInt32(Request.Form["listCount2"]);
        //                for (int i = 1; i <= listCount; i++)
        //                {
        //                    string idKaw = "ddlPkt" + i;
        //                    string idLuas = "textluasPkt" + i;
        //                    string JnsKaw = Request.Form[idKaw];
        //                    decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
        //                    var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                    if (checkKwsn == null)
        //                    {
        //                        Models.tbl_KawTidakBerhasil KawTidakBerhasil = new tbl_KawTidakBerhasil();
        //                        KawTidakBerhasil.fld_KodPkt = kodpkt;
        //                        KawTidakBerhasil.fld_LevelPkt = 2;
        //                        KawTidakBerhasil.fld_JnsKaw = JnsKaw;
        //                        KawTidakBerhasil.fld_LuasKaw = LuasKaw;
        //                        KawTidakBerhasil.fld_NegaraID = NegaraID;
        //                        KawTidakBerhasil.fld_SyarikatID = SyarikatID;
        //                        KawTidakBerhasil.fld_WilayahID = WilayahID;
        //                        KawTidakBerhasil.fld_LadangID = LadangID;
        //                        KawTidakBerhasil.fld_Deleted = false;
        //                        dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
        //                        dbr.SaveChanges();
        //                    }
        //                }
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }
        //            else
        //            {
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }

        //}

        //public ActionResult LevelsBlock()   
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> PktUtama = new List<SelectListItem>();
        //    List<SelectListItem> Pkt = new List<SelectListItem>();
        //    //List<SelectListItem> JnsKesukaran = new List<SelectListItem>();
        //    List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
        //    List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();

        //    PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
        //    Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //KesukaranMenuai.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
        //    //KesukaranMembaja.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_KodPktutama = PktUtama;
        //    ViewBag.fld_KodPkt = Pkt;
        //    ViewBag.fld_KesukaranMenuaiBlok = KesukaranMenuai;
        //    ViewBag.fld_KesukaranMembajaBlok = KesukaranMembaja;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsBlock(Models.tbl_Blok tbl_Blok)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var checkdata = dbr.tbl_Blok.Where(x =>x.fld_Blok==tbl_Blok.fld_Blok && x.fld_KodPkt==tbl_Blok.fld_KodPkt && x.fld_KodPktutama == tbl_Blok.fld_KodPktutama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
        //            if (checkdata == null)
        //            {
        //                //GetLadang GetLadang = new GetLadang();
        //                //string ldgcode = GetLadang.GetCodeLadangFromID2(LadangID.Value);
        //                tbl_Blok.fld_NamaBlok = tbl_Blok.fld_NamaBlok.ToUpper();
        //                tbl_Blok.fld_LadangID = LadangID;
        //                tbl_Blok.fld_WilayahID = WilayahID;
        //                tbl_Blok.fld_SyarikatID = SyarikatID;
        //                tbl_Blok.fld_NegaraID = NegaraID;
        //                tbl_Blok.fld_CreateDate = DateTime.Now;
        //                tbl_Blok.fld_Deleted = false;
        //                dbr.tbl_Blok.Add(tbl_Blok);
        //                dbr.SaveChanges();

        //                int listCount = Convert.ToInt32(Request.Form["listCount3"]);
        //                for (int i = 1; i <= listCount; i++)
        //                {
        //                    string idKaw = "ddlBlock" + i;
        //                    string idLuas = "textluasBlock" + i;
        //                    string JnsKaw = Request.Form[idKaw];
        //                    decimal LuasKaw = Convert.ToDecimal(Request.Form[idLuas]);
        //                    var checkKwsn = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_JnsKaw == JnsKaw && x.fld_KodPkt == tbl_Blok.fld_Blok && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
        //                    if (checkKwsn == null)
        //                    {
        //                        Models.tbl_KawTidakBerhasil KawTidakBerhasil = new tbl_KawTidakBerhasil();
        //                        KawTidakBerhasil.fld_KodPkt = tbl_Blok.fld_Blok;
        //                        KawTidakBerhasil.fld_LevelPkt = 1;
        //                        KawTidakBerhasil.fld_JnsKaw = JnsKaw;
        //                        KawTidakBerhasil.fld_LuasKaw = LuasKaw;
        //                        KawTidakBerhasil.fld_NegaraID = NegaraID;
        //                        KawTidakBerhasil.fld_SyarikatID = SyarikatID;
        //                        KawTidakBerhasil.fld_WilayahID = WilayahID;
        //                        KawTidakBerhasil.fld_LadangID = LadangID;
        //                        KawTidakBerhasil.fld_Deleted = false;
        //                        dbr.tbl_KawTidakBerhasil.Add(KawTidakBerhasil);
        //                        dbr.SaveChanges();
        //                    }
        //                }
        //                //dbr.SaveChanges();
        //                //var getid = "";
        //                //return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = getid, data1 = "", data2 = "", data3 = "" });
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }
        //            else
        //            {
        //                //return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //                return RedirectToAction("LevelsInfo", "BasicInfo");
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
        //    }
        //}

        //public ActionResult LevelsChange()
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    List<SelectListItem> PktUtama1 = new List<SelectListItem>();
        //    List<SelectListItem> PktUtama2 = new List<SelectListItem>();

        //    PktUtama1 = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama1.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    PktUtama2 = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
        //    PktUtama2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

        //    ViewBag.fld_PktUtama1 = PktUtama1;
        //    ViewBag.fld_PktUtama2 = PktUtama2;
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LevelsChange(string fld_PktUtama1, string fld_PktUtama2)
        //{
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

        //    if (fld_PktUtama1 != "0" && fld_PktUtama2 != "0")
        //    {
        //        //x aktif
        //        try
        //        {
        //            var getdata = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == fld_PktUtama1 && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
        //            getdata.fld_Deleted = true;
        //            dbr.Entry(getdata).State = EntityState.Modified;
        //            dbr.SaveChanges();

        //            var getdatacount = dbr.tbl_PktUtama.Where(w => w.fld_RefKey == fld_PktUtama1 && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).Count();
        //            var getdata2 = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == fld_PktUtama2 && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
        //            getdata2.fld_RefKey = fld_PktUtama1;
        //            getdata2.fld_Level = getdatacount + 1;
        //            dbr.Entry(getdata2).State = EntityState.Modified;
        //            dbr.SaveChanges();
        //            //return Json(new { success = true, msg = "Data successfully edited.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
        //            return RedirectToAction("LevelsInfo", "BasicInfo");
        //        }
        //        catch (Exception ex)
        //        {
        //            geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //            return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = true, msg = GlobalResEstate.msgErrorSubmit, status = "danger", checkingdata = "1" });
        //    }

        //}

        public ActionResult WorkerInfo(string statusApprove="0", string Active = "1", int page = 1, string sort = "fld_Nama", string sortdir = "ASC", string filter = "")
        {
            GetIdentity GetIdentity = new GetIdentity();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> statusApprove2 = new List<SelectListItem>();

            statusApprove2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "apprvlPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            statusApprove2.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            List<SelectListItem> Active2 = new List<SelectListItem>();
            Active2.Insert(0, (new SelectListItem { Text = "Tidak Aktif", Value = "2" }));
            Active2.Insert(0, (new SelectListItem { Text = "Aktif", Value = "1" }));

            ViewBag.BasicInfo = "class = active";
            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pkjmast>();
            
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            int role = GetIdentity.RoleID(getuserid).Value;

            if (statusApprove == "0")
            {
                if (filter != "")
                {
                    records.Content = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && (x.fld_Nama.Contains(filter) || x.fld_Nopkj.Contains(filter)))
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                    records.TotalRecords = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }
                else
                {
                    records.Content = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Kdaktf == Active)
                      .OrderBy(sort + " " + sortdir)
                      .Skip((page - 1) * pageSize)
                      .Take(pageSize)
                      .ToList();

                    records.TotalRecords = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Kdaktf == Active).Count();
                    records.CurrentPage = page;
                    records.PageSize = pageSize;
                }

            }
            else
            {
                int status = Int32.Parse(statusApprove);
                records.Content = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_StatusApproved == status)
                       .OrderBy(sort + " " + sortdir)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .ToList();

                records.TotalRecords = dbview.tbl_Pkjmast.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_StatusApproved == status).Count();
                records.CurrentPage = page;
                records.PageSize = pageSize;
            }

            ViewBag.statusApprove = statusApprove2;
            ViewBag.Active = Active2;
            ViewBag.RoleID = role;
            return View(records);
        }

        public ActionResult WorkerRequest()
        {
            //Check_Balik
            //string batch = Request.Form["CreateBatch"].ToString();
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> Gender = new List<SelectListItem>();
            List<SelectListItem> statusKahwin = new List<SelectListItem>();
            List<SelectListItem> peneroka = new List<SelectListItem>();
            List<SelectListItem> bangsa = new List<SelectListItem>();
            List<SelectListItem> agama = new List<SelectListItem>();
            List<SelectListItem> krytn = new List<SelectListItem>();
            List<SelectListItem> negeri = new List<SelectListItem>();
            List<SelectListItem> jnsPkj = new List<SelectListItem>();
            List<SelectListItem> ktgrPkj = new List<SelectListItem>();
            List<SelectListItem> pembekal = new List<SelectListItem>();
            List<SelectListItem> paymentMode = new List<SelectListItem>();//added by faeza 08.11.2021

            //List<SelectListItem> statusAktif = new List<SelectListItem>();
            //List<SelectListItem> banklist = new List<SelectListItem>();
            //List<SelectListItem> jnsKwsp = new List<SelectListItem>();

            Gender = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            Gender.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusKahwin = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "tarafKahwin" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            statusKahwin.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            peneroka = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusPeneroka" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            peneroka.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            bangsa = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "bangsa" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            bangsa.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            agama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "agama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            agama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            krytn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            krytn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negeri = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            negeri.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            jnsPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc.ToUpper() }).Distinct(), "Value", "Text").ToList();
            jnsPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ktgrPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            ktgrPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //sepul tukar order 30/09/2021 list dari idpembekal kpd nama pembekal 
            pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").OrderBy(o => o.Text).ToList();
            //pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NamaPbkl).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").ToList();
            pembekal.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //added by faeza 08.11.2021
            paymentMode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            paymentMode.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif2").OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            //statusAktif.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //banklist = new SelectList(db.tbl_Bank.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text").ToList();
            //banklist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //jnsKwsp = new SelectList(db.tbl_JenisCaruman.Where(x =>x.fld_JenisCaruman== "KWSP" && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodCaruman).Select(s => new SelectListItem { Value = s.fld_KodCaruman, Text = s.fld_Keterangan}).Distinct(), "Value", "Text").ToList();
            //jnsKwsp.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            string requestCode = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_RequestCode).FirstOrDefault();
            //ViewBag.fld_Batch = fld_Batch;
            ViewBag.fld_Kdjnt = Gender;
            ViewBag.fld_Kdkwn = statusKahwin;
            ViewBag.fld_Kpenrka = peneroka;
            ViewBag.fld_Kdbgsa = bangsa;
            ViewBag.fld_Kdagma = agama;
            ViewBag.fld_Kdrkyt = krytn;
            ViewBag.fld_Neg = negeri;
            ViewBag.fld_Negara = krytn;
            ViewBag.fld_Jenispekerja = jnsPkj;
            ViewBag.fld_Ktgpkj = ktgrPkj;
            ViewBag.fld_Kodbkl = pembekal;
            ViewBag.LdgID = LadangID;
            ViewBag.requestCode = requestCode;
            ViewBag.fld_PaymentMode = paymentMode;//added by faeza 08.11.2021

            //ViewBag.fld_Kdaktf = statusAktif;
            //ViewBag.fld_Kdbank = banklist;
            //ViewBag.fld_KodKWSP = jnsKwsp;
            ViewBag.Flag = 1;
            return View();
        }


        [HttpPost]
        public ActionResult WorkerRequest(Models.tbl_Pkjmast Pkjmast, string jnsPermohonan, int wlyhAsal, string pkjAsal)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            string msg = "";
            string statusmsg = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DatabaseAction DatabaseAction = new DatabaseAction();
            try
            {
                var checkdata = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && (x.fld_Nopkj == Pkjmast.fld_Nopkj || x.fld_Nokp == Pkjmast.fld_Nokp) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                int FileID = DatabaseAction.insertTotblASCApprovalFileDetail(Pkjmast.fld_Batch, Pkjmast.fld_Kdldg, NegaraID, SyarikatID, WilayahID, LadangID, 2);
                if (checkdata == null && Pkjmast.fld_Nopkj != null)
                {
                    Pkjmast.fld_Nama = Pkjmast.fld_Nama.ToUpper();
                    Pkjmast.fld_Daerah = Pkjmast.fld_Daerah.Trim();
                    //  Pkjmast.fld_IDpkj = Pkjmast.fld_IDpkj; // commented by kamalia 2/11/2021
                    Pkjmast.fld_NoPkjLama = Pkjmast.fld_NoPkjLama;  //added by kamalia 2/11/2021
                    Pkjmast.fld_IDpkj = Pkjmast.fld_Nopkj;
                    Pkjmast.fld_Ktgpkj = Pkjmast.fld_Ktgpkj.ToString().Trim();
                    Pkjmast.fld_DateApply = DateTime.Now;
                    Pkjmast.fld_StatusApproved = 2;
                    Pkjmast.fld_AppliedBy = User.Identity.Name;
                    Pkjmast.fld_NegaraID = NegaraID;
                    Pkjmast.fld_SyarikatID = SyarikatID;
                    Pkjmast.fld_WilayahID = WilayahID;
                    Pkjmast.fld_LadangID = LadangID;
                    Pkjmast.fld_Kdldg = GetLadang.GetLadangCode(LadangID.Value);
                    Pkjmast.fld_PurposeRequest = jnsPermohonan;
                    dbr.tbl_Pkjmast.Add(Pkjmast);
                    dbr.SaveChanges();

                    DateTime t = DateTime.Now;
                    DateTime tf = DateTime.Now.AddSeconds(3);

                    while (t < tf)
                    {
                        t = DateTime.Now;
                    }

                    MasterModels.tblPkjmastApp tblPkjmastApp = new MasterModels.tblPkjmastApp();
                    tblPkjmastApp.fldSbbMsk = jnsPermohonan;
                    //  tblPkjmastApp.fldNoPkjAsal = Pkjmast.fld_IDpkj;  //commented by kamalia 2/11/2021
                    tblPkjmastApp.fldNoPkjLama = Pkjmast.fld_NoPkjLama;   //added by kamalia 2/11/2021
                    tblPkjmastApp.fldNoPkj = Pkjmast.fld_Nopkj;
                    tblPkjmastApp.fldNama1 = Pkjmast.fld_Nama.ToUpper();
                    tblPkjmastApp.fldNoKP = Pkjmast.fld_Nokp;
                    tblPkjmastApp.fldKdJnsPkj = Pkjmast.fld_Ktgpkj.ToString().Trim();
                    tblPkjmastApp.fldKdRkyt = Pkjmast.fld_Kdrkyt;
                    tblPkjmastApp.fldTrshjw = Pkjmast.fld_Trshjw;
                    tblPkjmastApp.fldFileID = FileID;
                    tblPkjmastApp.fldKdLdg = Pkjmast.fld_Kdldg;
                    tblPkjmastApp.fldStatus = Pkjmast.fld_StatusApproved;
                    tblPkjmastApp.fldNegaraID = NegaraID;
                    tblPkjmastApp.fldSyarikatID = SyarikatID;
                    tblPkjmastApp.fldWilayahID = WilayahID;
                    tblPkjmastApp.fldLadangID = LadangID;

                    if (jnsPermohonan == "PL")
                    {
                        string host2, catalog2, user2, pass2 = "";
                        var GetWilayahData = db.tbl_Wilayah.Find(wlyhAsal);
                        Connection.GetConnection(out host2, out catalog2, out user2, out pass2, GetWilayahData.fld_ID, GetWilayahData.fld_SyarikatID, GetWilayahData.fld_NegaraID);
                        MVC_SYSTEM_Models dbr2 = MVC_SYSTEM_Models.ConnectToSqlServer(host2, catalog2, user2, pass2);

                        var GetDataPkjAsal = dbr2.tbl_Pkjmast.Where(x => x.fld_Nopkj == pkjAsal).FirstOrDefault();

                        tblPkjmastApp.fldLadangAsal = GetDataPkjAsal.fld_LadangID;
                        tblPkjmastApp.fldWilayahAsal = GetDataPkjAsal.fld_WilayahID;
                        tblPkjmastApp.fldSyarikatAsal = GetDataPkjAsal.fld_SyarikatID;
                        tblPkjmastApp.fldNegaraAsal = GetDataPkjAsal.fld_NegaraID;
                        tblPkjmastApp.fldNoPkjAsal = pkjAsal.ToUpper();
                    }
                    db.tblPkjmastApps.Add(tblPkjmastApp);
                    db.SaveChanges();
                    msg = GlobalResEstate.msgAdd;
                    statusmsg = "success";
                }
                else
                {
                    msg = GlobalResEstate.msgIcPassExits;
                    statusmsg = "warning";
                }
            }
            catch(Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                geterror.catcherro(jnsPermohonan, Pkjmast.fld_Nopkj, NegaraID.ToString(), WilayahID.ToString());
                return Json(new { success = true, msg = "Error occur please contact IT.", statusmsg = "danger" });
            }

            var result = dbr.tbl_Pkjmast.Where(x => x.fld_Batch == Pkjmast.fld_Batch && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            string bodyview = RenderRazorViewToString("WorkerRequestList", result);
            return Json(new { bodyview , msg, statusmsg });
            //return RedirectToAction("WorkerRequestWaris", "BasicInfo",new { nopkj=Pkjmast.fld_Nopkj });
        }

        public string RenderRazorViewToString(string viewname, object dataview)
        {
            ViewData.Model = dataview;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewname);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult WorkerTransfer()
        {
            //Check_Balik
            //string batch = Request.Form["CreateBatch"].ToString();
            int[] wlyhid = new int[] { };
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> SyarikatIDList = new List<SelectListItem>();
            List<SelectListItem> WilayahIDList = new List<SelectListItem>();
            List<SelectListItem> Gender = new List<SelectListItem>();
            List<SelectListItem> statusKahwin = new List<SelectListItem>();
            List<SelectListItem> peneroka = new List<SelectListItem>();
            List<SelectListItem> bangsa = new List<SelectListItem>();
            List<SelectListItem> agama = new List<SelectListItem>();
            List<SelectListItem> krytn = new List<SelectListItem>();
            List<SelectListItem> negeri = new List<SelectListItem>();
            List<SelectListItem> jnsPkj = new List<SelectListItem>();
            List<SelectListItem> ktgrPkj = new List<SelectListItem>();
            List<SelectListItem> pembekal = new List<SelectListItem>();
            List<SelectListItem> statusAktif = new List<SelectListItem>();
            List<SelectListItem> paymentMode = new List<SelectListItem>();//added by faeza 08.11.2021
            //List<SelectListItem> banklist = new List<SelectListItem>();

            //sepul komen 22/09/2021
            //SyarikatIDList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NamaSyarikat), "fld_SyarikatID", "fld_NamaSyarikat").ToList();
            //sepul tambah filter felda je  22/09/2021
            SyarikatIDList = new SelectList(db.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_Deleted == false && x.fld_SyarikatID == 1).OrderBy(o => o.fld_NamaSyarikat), "fld_SyarikatID", "fld_NamaSyarikat").ToList();
            //SyarikatIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            //sepul 8/10/2021
            wlyhid = GetWilayah.GetWilayahID(SyarikatID);
            WilayahIDList = new SelectList(db.tbl_Wilayah.Where(x => wlyhid.Contains(x.fld_ID) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).OrderBy(o => o.fld_ID), "fld_ID", "fld_WlyhName").ToList();
            WilayahIDList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            Gender = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            Gender.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusKahwin = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "tarafKahwin" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            statusKahwin.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            peneroka = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusPeneroka" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            peneroka.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            bangsa = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "bangsa" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            bangsa.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            agama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "agama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            agama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            krytn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            krytn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negeri = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            negeri.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            jnsPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc.ToUpper() }).Distinct(), "Value", "Text").ToList();
            jnsPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ktgrPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.Trim(), Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            ktgrPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //sepul tukar order kodpembekal kpd nama pembekal 30/09/2021
            pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").OrderBy(o => o.Text).ToList();
            //pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NamaPbkl).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text").ToList();
            pembekal.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif2" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            //statusAktif.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //added by faeza 08.11.2021
            paymentMode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            paymentMode.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //banklist = new SelectList(db.tbl_Bank.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text").ToList();
            //banklist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));

            //ViewBag.Batch = batch;
            ViewBag.fld_SyarikatID = SyarikatIDList;
            ViewBag.fld_WilayahID = WilayahIDList;
            ViewBag.fld_Kdjnt = Gender;
            ViewBag.fld_Kdkwn = statusKahwin;
            ViewBag.fld_Kpenrka = peneroka;
            ViewBag.fld_Kdbgsa = bangsa;
            ViewBag.fld_Kdagma = agama;
            ViewBag.fld_Kdrkyt = krytn;
            ViewBag.fld_Neg = negeri;
            ViewBag.fld_Negara = krytn;
            ViewBag.fld_Jenispekerja = jnsPkj;
            ViewBag.fld_Ktgpkj = ktgrPkj;
            ViewBag.fld_Kodbkl = pembekal;
            ViewBag.fld_Kdaktf = statusAktif;
            ViewBag.fld_PaymentMode = paymentMode;//added by faeza 08.11.2021
            //ViewBag.fld_Kdbank = banklist;

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult WorkerTransfer(Models.tbl_Pkjmast Pkjmast)
        //{
        //    GetLadang GetLadang = new GetLadang();
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
        //    //if (ModelState.IsValid)
        //    //{

        //    try
        //    {
        //        var checkdata = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == Pkjmast.fld_Nopkj).FirstOrDefault();
        //        if (checkdata == null)
        //        {

        //            //tbl_Ladang.fld_Deleted = false;
        //            Pkjmast.fld_Almt1 = Pkjmast.fld_Almt1.Trim();
        //            Pkjmast.fld_IDpkj = Pkjmast.fld_IDpkj;
        //            Pkjmast.fld_DateApply = DateTime.Now;
        //            Pkjmast.fld_StatusApproved =2;
        //            Pkjmast.fld_AppliedBy = Convert.ToString(getuserid);
        //            Pkjmast.fld_NegaraID = NegaraID;
        //            Pkjmast.fld_SyarikatID = SyarikatID;
        //            Pkjmast.fld_WilayahID = WilayahID;
        //            Pkjmast.fld_LadangID = LadangID;
        //            Pkjmast.fld_Kdldg = GetLadang.GetCodeLadangFromID2(LadangID.Value);
        //            dbr.tbl_Pkjmast.Add(Pkjmast);
        //            dbr.SaveChanges();

        //            var notes = db.tblPkjmastApps.Where(x => x.fldNoPkj == Pkjmast.fld_Nopkj && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID).FirstOrDefault();
        //            notes.fldSbbMsk = "PL";
        //            db.SaveChanges();
        //            //var getid = db.tbl_Ladang.Where(w => w.fld_ID == tbl_Ladang.fld_ID).FirstOrDefault();
        //            return Json(new { success = true, msg = "Data successfully added.", status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });
        //        }
        //        else
        //        {
        //            return Json(new { success = true, msg = "Data already exist.", status = "warning", checkingdata = "1" });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
        //        return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
        //    }
        //    //}
        //    //else
        //    //{
        //    //    return Json(new { success = true, msg = "Please check fill you inserted.", status = "warning", checkingdata = "1" });
        //    //    //return RedirectToAction("Thankyou");
        //    //}
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GetWorkerDetail(string PkjTransfer, int? fld_WilayahID)
        //{
        //    Models.tbl_Pkjmast tbl_Pkjmast = db.tbl_Pkjmast.Where(w => w.fld_Nopkj == PkjTransfer && w.fld_WilayahID == fld_WilayahID).FirstOrDefault();

        //    return PartialView("WorkerTransfer", tbl_Pkjmast);
        //}

        public ActionResult WorkerUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (id == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            string statuspkj = GetStatus.GetWorkerStatus(tbl_Pkjmast.fld_Kdaktf);
            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerInfo");
            }

            List<SelectListItem> Gender = new List<SelectListItem>();
            List<SelectListItem> statusKahwin = new List<SelectListItem>();
            List<SelectListItem> peneroka = new List<SelectListItem>();
            List<SelectListItem> bangsa = new List<SelectListItem>();
            List<SelectListItem> agama = new List<SelectListItem>();
            List<SelectListItem> krytn = new List<SelectListItem>();
            List<SelectListItem> negeri = new List<SelectListItem>();
            List<SelectListItem> negara = new List<SelectListItem>();
            List<SelectListItem> jnsPkj = new List<SelectListItem>();
            List<SelectListItem> ktgrPkj = new List<SelectListItem>();
            List<SelectListItem> pembekal = new List<SelectListItem>();
            List<SelectListItem> statusAktif = new List<SelectListItem>();
            List<SelectListItem> banklist = new List<SelectListItem>();
            List<SelectListItem> paymentMode = new List<SelectListItem>();//added by faeza 08.11.2021


            Gender = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jantina" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdjnt).ToList();
            Gender.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusKahwin = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "tarafKahwin" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdkwn).ToList();
            statusKahwin.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            peneroka = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusPeneroka" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfID).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kpenrka).ToList();
            peneroka.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            bangsa = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "bangsa" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdbgsa).ToList();
            bangsa.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            agama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "agama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdagma).ToList();
            agama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            krytn = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdrkyt).ToList();
            krytn.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negeri = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Neg.Trim()).ToList();
            negeri.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            negara = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Negara.Trim()).ToList();
            negara.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            jnsPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc.ToUpper() }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Jenispekerja).ToList();
            jnsPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ktgrPkj = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "designation" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue.Trim(), Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Ktgpkj.Trim()).ToList();
            ktgrPkj.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //tukar list order kod kpd nama  30/09/2021
            //modified by faeza 08.11.2021
            pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kodbkl).OrderBy(o => o.Text).ToList();
            //pembekal = new SelectList(db.tbl_Pembekal.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_NamaPbkl).Select(s => new SelectListItem { Value = s.fld_KodPbkl, Text = s.fld_NamaPbkl }).Distinct(), "Value", "Text",tbl_Pkjmast.fld_Kodbkl).ToList();
            pembekal.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }).Distinct(), "Value", "Text", statuspkj).ToList();
            statusAktif.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //added by faeza 08.11.2021
            paymentMode = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "paymentmode" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_PaymentMode).ToList();
            paymentMode.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            //banklist = new SelectList(db.tbl_Bank.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_KodBank).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdbank).ToList();
            //banklist.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            var findImage = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == id && x.fld_Flag == "picPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_Url).FirstOrDefault();
            if (findImage == null)
            {
                findImage = "/Asset/Images/default-user.png";
            }
            ViewBag.fld_Kdjnt = Gender;
            ViewBag.fld_Kdkwn = statusKahwin;
            ViewBag.fld_Kpenrka = peneroka;
            ViewBag.fld_Kdbgsa = bangsa;
            ViewBag.fld_Kdagma = agama;
            ViewBag.fld_Kdrkyt = krytn;
            ViewBag.fld_Neg = negeri;
            ViewBag.fld_Negara = negara;
            ViewBag.fld_Jenispekerja = jnsPkj;
            ViewBag.fld_Ktgpkj = ktgrPkj;
            ViewBag.fld_Kodbkl = pembekal;
            ViewBag.fld_Kdaktf = statusAktif;
            ViewBag.ImageSource = findImage;
            ViewBag.fld_PaymentMode = paymentMode;//added by faeza 08.11.2021

            return PartialView("WorkerUpdate", tbl_Pkjmast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkerUpdate(string id, Models.tbl_Pkjmast tbl_Pkjmast)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                //added by faeza on 08.11.2021
                if (tbl_Pkjmast.fld_Nopkj == "" || tbl_Pkjmast.fld_Nama == "" || tbl_Pkjmast.fld_Nokp == null || tbl_Pkjmast.fld_Nokp == "" || tbl_Pkjmast.fld_Trlhr == null || tbl_Pkjmast.fld_Kdjnt == "0" || tbl_Pkjmast.fld_Kdkwn == "0" || tbl_Pkjmast.fld_Kpenrka == "0" || tbl_Pkjmast.fld_Kdbgsa == "0" || tbl_Pkjmast.fld_Kdagma == "0" ||
                    tbl_Pkjmast.fld_Kdrkyt == "0" || tbl_Pkjmast.fld_Almt1 == "" || tbl_Pkjmast.fld_Poskod == "" || tbl_Pkjmast.fld_Daerah == "" || tbl_Pkjmast.fld_Neg == "0" || tbl_Pkjmast.fld_Negara == "0" || tbl_Pkjmast.fld_Jenispekerja == "0" || tbl_Pkjmast.fld_Ktgpkj == "0" ||
                    tbl_Pkjmast.fld_Trmlkj == null || tbl_Pkjmast.fld_Trshjw == null || (tbl_Pkjmast.fld_Kdrkyt != "MA" && tbl_Pkjmast.fld_T2pspt == null) || (tbl_Pkjmast.fld_Kdrkyt != "MA" && (tbl_Pkjmast.fld_Kodbkl == "0" || tbl_Pkjmast.fld_Kodbkl == "")) || tbl_Pkjmast.fld_Notel == "" || tbl_Pkjmast.fld_Notel == null ||
                    tbl_Pkjmast.fld_PaymentMode == "0" || (tbl_Pkjmast.fld_PaymentMode == "3" && tbl_Pkjmast.fld_Last4Pan == "") || (tbl_Pkjmast.fld_PaymentMode == "3" && tbl_Pkjmast.fld_Last4Pan == null) || (tbl_Pkjmast.fld_PaymentMode == "3" && tbl_Pkjmast.fld_Last4Pan.Length < 4))
                {
                    return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
                else
                {
                    var getdata = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                    getdata.fld_Nama = tbl_Pkjmast.fld_Nama;
                    //   getdata.fld_IDpkj = tbl_Pkjmast.fld_IDpkj;   //commented by kamalia 2/11/2021
                    getdata.fld_NoPkjLama = tbl_Pkjmast.fld_NoPkjLama;    //added by kamalia 2/11/2021
                    getdata.fld_Trlhr = tbl_Pkjmast.fld_Trlhr;
                    getdata.fld_Kdjnt = tbl_Pkjmast.fld_Kdjnt;
                    getdata.fld_Kdkwn = tbl_Pkjmast.fld_Kdkwn;
                    getdata.fld_Kpenrka = tbl_Pkjmast.fld_Kpenrka;
                    getdata.fld_Kdbgsa = tbl_Pkjmast.fld_Kdbgsa;
                    getdata.fld_Kdagma = tbl_Pkjmast.fld_Kdagma;
                    //getdata.fld_Kdrkyt = tbl_Pkjmast.fld_Kdrkyt; //tidak boleh edit kerakyatan
                    getdata.fld_Almt1 = tbl_Pkjmast.fld_Almt1;
                    getdata.fld_Poskod = tbl_Pkjmast.fld_Poskod;
                    getdata.fld_Almt1 = tbl_Pkjmast.fld_Almt1;
                    getdata.fld_Neg = tbl_Pkjmast.fld_Neg;
                    getdata.fld_Negara = tbl_Pkjmast.fld_Negara;
                    getdata.fld_Jenispekerja = tbl_Pkjmast.fld_Jenispekerja;
                    getdata.fld_Ktgpkj = tbl_Pkjmast.fld_Ktgpkj;
                    getdata.fld_Prmtno = tbl_Pkjmast.fld_Prmtno;
                    //getdata.fld_T1prmt = tbl_Pkjmast.fld_T1prmt;
                    getdata.fld_T2prmt = tbl_Pkjmast.fld_T2prmt;
                    //getdata.fld_T1pspt = tbl_Pkjmast.fld_T1pspt;
                    getdata.fld_T2pspt = tbl_Pkjmast.fld_T2pspt;
                    getdata.fld_Trmlkj = tbl_Pkjmast.fld_Trmlkj;
                    getdata.fld_Kodbkl = tbl_Pkjmast.fld_Kodbkl;
                    getdata.fld_Notel = tbl_Pkjmast.fld_Notel;//added by faeza 08.11.2021
                    getdata.fld_PaymentMode = tbl_Pkjmast.fld_PaymentMode;//added by faeza 08.11.2021
                    getdata.fld_Last4Pan = tbl_Pkjmast.fld_Last4Pan;//added by faeza 08.11.2021
                    getdata.fld_Nokp = tbl_Pkjmast.fld_Nokp; //added by faeza 29.11.2021

                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();
                    var getid = id;
                    //return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "1" });
                    return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "Lihat" });
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult WorkerStatus(string id)
        {
            //Check_Balik
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (id == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            //string statuspkj = GetStatus.GetWorkerStatus(tbl_Pkjmast.fld_Kdaktf);
            //string statuspkj = GetConfig.GetData2(tbl_Pkjmast.fld_Kdaktf, "statusaktif");
            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerInfo");
            }

            List<SelectListItem> statusAktif = new List<SelectListItem>();
            List<SelectListItem> sbbTakAktif = new List<SelectListItem>();
            List<SelectListItem> pkjbarulama = new List<SelectListItem>();

            statusAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusaktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text", tbl_Pkjmast.fld_Kdaktf).ToList();
            //statusAktif.Insert(0, (new SelectListItem { Text = "Sila Pilih", Value = "0" }));
            sbbTakAktif = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "sbbTakAktif" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            sbbTakAktif.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            pkjbarulama = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "pkjbarulama" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
            pkjbarulama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_Kdaktf = statusAktif;
            ViewBag.fld_Sbtakf = sbbTakAktif;
            ViewBag.pkjLamaBaru = pkjbarulama;

            return PartialView("WorkerStatus", tbl_Pkjmast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WorkerStatus(string pkjLamaBaru, string nopkjnew, Models.tbl_Pkjmast tbl_Pkjmast, tblStatusPkj tblStatusPkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (tbl_Pkjmast.fld_Kdaktf=="2")
            {
                //x aktif
                try
                {
                    var getdata = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();

                    getdata.fld_Kdaktf = tbl_Pkjmast.fld_Kdaktf;
                    getdata.fld_Sbtakf = tbl_Pkjmast.fld_Sbtakf;
                    getdata.fld_Trtakf = tbl_Pkjmast.fld_Trtakf;
                    getdata.fld_Remarks = tbl_Pkjmast.fld_Remarks;
                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();
                    //var getid = id;
                    return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
            else
            {
                //aktif
                try
                {
                    if (pkjLamaBaru == "1")
                    {
                        //nopkj baru
                        //insert statuspkj
                        //tblStatusPkj.fldNoPkjLama = tbl_Pkjmast.fld_Nopkj;
                        //tblStatusPkj.fldNama = tbl_Pkjmast.fld_Nama;
                        //tblStatusPkj.fldNoKP = tbl_Pkjmast.fld_Nokp;
                        //tblStatusPkj.fldStatusAktif = 0;
                        //tblStatusPkj.fldAppliedBy = Convert.ToString(getuserid);
                        //tblStatusPkj.fldAppliedDate = DateTime.Now;
                        //tblStatusPkj.fldNegaraID = NegaraID;
                        //tblStatusPkj.fldSyarikatID = SyarikatID;
                        //tblStatusPkj.fldWilayahID = WilayahID;
                        //tblStatusPkj.fldLadangID = LadangID;
                        //tblStatusPkj.fldNoPkjBaru = nopkjnew;
                        //db.tblStatusPkjs.Add(tblStatusPkj);
                        //db.SaveChanges();

                        //insert new nopkj in pkjmast
                        var pkjmastAsal = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                        pkjmastAsal.fld_Nopkj = nopkjnew;
                        pkjmastAsal.fld_Kdaktf = tbl_Pkjmast.fld_Kdaktf;
                        dbr.tbl_Pkjmast.Add(pkjmastAsal);
                        dbr.SaveChanges();

                        return Json(new { success = true, msg =GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });

                    }
                    else if (pkjLamaBaru == "2")
                    {
                        //nopkj lama
                        //tblStatusPkj.fldNoPkjLama = tbl_Pkjmast.fld_Nopkj;
                        //tblStatusPkj.fldNama = tbl_Pkjmast.fld_Nama;
                        //tblStatusPkj.fldNoKP = tbl_Pkjmast.fld_Nokp;
                        //tblStatusPkj.fldStatusAktif = 0;
                        //tblStatusPkj.fldAppliedBy = Convert.ToString(getuserid);
                        //tblStatusPkj.fldAppliedDate = DateTime.Now;
                        //tblStatusPkj.fldNegaraID = NegaraID;
                        //tblStatusPkj.fldSyarikatID = SyarikatID;
                        //tblStatusPkj.fldWilayahID = WilayahID;
                        //tblStatusPkj.fldLadangID = LadangID;
                        //tblStatusPkj.fldNoPkjBaru = tbl_Pkjmast.fld_Nopkj;

                        //db.tblStatusPkjs.Add(tblStatusPkj);
                        //db.SaveChanges();

                        var pkjmastAsal = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == tbl_Pkjmast.fld_Nopkj && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                        pkjmastAsal.fld_Kdaktf = tbl_Pkjmast.fld_Kdaktf;
                        dbr.Entry(pkjmastAsal).State = EntityState.Modified;
                        dbr.SaveChanges();

                        return Json(new { success = true, msg = GlobalResEstate.msgAdd, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "", data3 = "" });
                    }
                    else
                    {
                        return Json(new { success = true, msg =GlobalResEstate.msgErrorData, status = "warning", checkingdata = "1" });
                    }
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
            }
        }

        public ActionResult WorkerDelete(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (id == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerInfo");
            }
            return PartialView("WorkerDelete", tbl_Pkjmast);
        }

        [HttpPost, ActionName("WorkerDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult WorkerDeleteConfirmed(string id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
                if (tbl_Pkjmast == null)
                {
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }
                else
                {
                    dbr.tbl_Pkjmast.Remove(tbl_Pkjmast);
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "1", getid = "", data1 = "", data2 = "" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public JsonResult GetStatusTnmn(string JnsTnmn)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Check_Balik

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            List<SelectListItem> statuslist = new List<SelectListItem>();
            statuslist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "statusTanaman" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            return Json(statuslist);
        }

        public JsonResult GetSubPkt(string Pktutama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            List<SelectListItem> SubPkt = new List<SelectListItem>();
            SubPkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            return Json(SubPkt);
        }

        public JsonResult GetPktUtama(string JnsTnmn, string StatusTnmn, string LotPeneroka)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string codetnmn;
            string newpkt = "";
            int kodpkt = 0;
            int newkodpkt = 0;
            string tahun = DateTime.Now.ToString("yy");

            if (JnsTnmn != "0" && StatusTnmn != "0" && LotPeneroka != "0")
            {
                codetnmn = JnsTnmn + StatusTnmn +LotPeneroka;
                var getpkt = dbr.tbl_PktUtama.Where(x=>x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama.Contains(codetnmn)).Select(s => s.fld_PktUtama).Distinct().OrderByDescending(s=>s).FirstOrDefault();
                
                if (getpkt != null)
                {
                    kodpkt = Convert.ToInt32(getpkt.Substring(getpkt.Length - 2));
                }
                else
                {
                    kodpkt = 0;
                }
                newkodpkt = kodpkt + 1;
                newpkt = codetnmn + tahun + newkodpkt.ToString("00");
            }
            else
            {
                newpkt = "";
            }
            return Json(newpkt);
        }

        public JsonResult GetPkt(string Pktutama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string newpkt = "";
            string kodpkt = "";
            char alphabet = 'A';
            decimal luas = 0;

            if (Pktutama != "0" && Pktutama!="")
            {
                var getluasutama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPktUtama).FirstOrDefault();
                var getluas = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).Sum();
                var getpkt = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama== Pktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Pkt).Distinct().OrderByDescending(s => s).FirstOrDefault();
                luas = getluasutama.Value - getluas.GetValueOrDefault();

                if (getpkt != null)
                {
                    kodpkt = getpkt.Substring(getpkt.Length - 1);
                    alphabet = Convert.ToChar(kodpkt);
                    alphabet = (char)(((int)alphabet) + 1);
                }
                else
                {
                    alphabet = 'A';
                }
                
                newpkt = Pktutama + alphabet;
            }
            else
            {
                newpkt = "";
            }
            return Json(new { newpkt = newpkt, luas = luas.ToString("0.000") });
        }

        public JsonResult GetBlock (string pktutama, string pkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int kodblok = 0;
            int newkodblok = 0;
            string newblok = "";
            decimal luas = 0;

            if (pktutama != "0" && pkt != "0")
            {
                var getluaspkt = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == pktutama && x.fld_Pkt==pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).FirstOrDefault();
                var getluas = dbr.tbl_Blok.Where(x => x.fld_KodPktutama == pktutama && x.fld_KodPkt==pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsBlok).Sum();
                luas = getluaspkt.Value - getluas.GetValueOrDefault();
                var getblok = dbr.tbl_Blok.Where(x => x.fld_KodPktutama == pktutama && x.fld_KodPkt==pkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Blok).Distinct().OrderByDescending(s => s).FirstOrDefault();

                if (getblok != null)
                {
                    kodblok = Convert.ToInt32(getblok.Substring(getblok.Length - 2));
                }
                else
                {
                    kodblok = 0;
                }
                newkodblok = kodblok + 1;
                newblok = pkt + newkodblok.ToString("00");
            }
            else
            {
                newblok = "";
            }
            return Json(new { newblok = newblok, luas = luas.ToString("0.000") });
        }

        public JsonResult checkluas(string kodpktutama, decimal luas)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            decimal? bakiluas = 0;
            int result = 0;
            var koutaluas = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == kodpktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPktUtama).FirstOrDefault();
            var luaspkt = dbr.tbl_SubPkt.Where(x => x.fld_KodPktUtama == kodpktutama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).Sum();
            bakiluas = koutaluas - luaspkt.GetValueOrDefault();

            if (luas > bakiluas)
            {
                result = 0;
            }
            else
            {
                result = 1;
            }
            return Json(result);
        }

        public JsonResult checkluas2(string kodpkt, decimal luas)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int result = 0;
            decimal? bakiluas = 0;

            var koutaluas = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsPkt).FirstOrDefault();
            var luasblok = dbr.tbl_Blok.Where(x => x.fld_KodPkt == kodpkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_LsBlok).Sum();
            bakiluas = koutaluas - luasblok.GetValueOrDefault();

            if (luas > bakiluas)
            {
                result = 0;
            }
            else
            {
                result = 1;
            }
            return Json(result);
        }

        public JsonResult GetNopkj(string krkytn)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetLadang GetLadang = new GetLadang();
            string kuota = "F";
            string kuotafpm = "T"; //added by faeza 09.07.2023
            string warganegara = krkytn;
            string kodldg = GetLadang.GetLadangCode(LadangID.Value);
            string costcenter = GetLadang.GetLadangCostCenter(LadangID.Value); //added by faeza 09.07.2023
            string thnKemasukan = DateTime.Now.ToString("yy");
            string nopkjnew = "";
            int kod = 0;

            //added by faeza 09.07.2023
            if (costcenter == "1000")
            {
                if (krkytn != "0")
                {
                    if (krkytn == "ID")
                    {
                        warganegara = krkytn.Substring(1, 1);
                    }
                    else
                    {
                        warganegara = krkytn.Substring(0, 1);
                    }
                    nopkjnew = kuota + warganegara + kodldg + thnKemasukan;
                    var getpkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj.Trim()).Distinct().OrderByDescending(s => s).FirstOrDefault();

                    if (getpkj != null)
                    {
                        kod = Convert.ToInt32(getpkj.Substring(getpkj.Length - 3));
                    }
                    else
                    {
                        kod = 0;
                    }
                    kod = kod + 1;
                    nopkjnew = nopkjnew + kod.ToString("000");
                }
                else
                {
                    nopkjnew = "";
                }
            }
            else if (costcenter == "8800")
            {
                if (krkytn != "0")
                {
                    if (krkytn == "ID")
                    {
                        warganegara = krkytn.Substring(1, 1);
                    }
                    else
                    {
                        warganegara = krkytn.Substring(0, 1);
                    }
                    nopkjnew = kuotafpm + warganegara + kodldg + thnKemasukan;
                    var getpkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj.Trim()).Distinct().OrderByDescending(s => s).FirstOrDefault();

                    if (getpkj != null)
                    {
                        kod = Convert.ToInt32(getpkj.Substring(getpkj.Length - 3));
                    }
                    else
                    {
                        kod = 0;
                    }
                    kod = kod + 1;
                    nopkjnew = nopkjnew + kod.ToString("000");
                }
                else
                {
                    nopkjnew = "";
                }
            }
            return Json(nopkjnew);

            //commented by faeza 09.07.2023
            //if (krkytn != "0")
            //{
            //    if (krkytn == "ID")
            //    {
            //        warganegara = krkytn.Substring(1, 1);
            //    }
            //    else
            //    {
            //        warganegara = krkytn.Substring(0, 1);
            //    }
            //    nopkjnew = kuota + warganegara + kodldg + thnKemasukan;
            //    var getpkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID==WilayahID && x.fld_LadangID==LadangID).Select(s => s.fld_Nopkj.Trim()).Distinct().OrderByDescending(s => s).FirstOrDefault();

            //    if (getpkj != null)
            //    {
            //        kod = Convert.ToInt32(getpkj.Substring(getpkj.Length - 3));
            //    }
            //    else
            //    {
            //        kod = 0;
            //    }
            //    kod = kod + 1;
            //    nopkjnew = nopkjnew + kod.ToString("000");
            //}
            //else
            //{
            //    nopkjnew = "";
            //}
            //return Json(nopkjnew);
        }

        public JsonResult GetDaerah(string poskod, string negeri, string negara)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //Check_Balik
            string daerah = "";
            string ngri = "";
            string ngra = "";

            if (poskod != "0" && negeri != "0" && negara != "0")
            {
                ngri = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fldOptConfValue == negeri && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();
                ngra = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "krytnlist" && x.fldOptConfValue == negara && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                daerah = db.tbl_Poskod.Where(x => x.fld_Postcode == poskod && x.fld_State.Contains(ngri) && x.fld_Region.Contains(ngra) && x.fld_deleted==false).Select(s=>s.fld_DistrictArea).FirstOrDefault();
                if (daerah == null)
                {
                    daerah = "";
                }
            }
            else
            {
                daerah = "";
            }
            return Json(daerah);
        }

        public JsonResult GetBatchNo(string pkjmstbatchno)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var NSWL = GetNSWL.GetLadangDetail(LadangID.Value);

            int? convertint = 0;
            string genbatchno = "";
            
            var getbatchno = db.tbl_BatchRunNo.Where(x => x.fld_BatchFlag == pkjmstbatchno && x.fld_NegaraID == NSWL.fld_NegaraID && x.fld_SyarikatID == NSWL.fld_SyarikatID && x.fld_WilayahID == NSWL.fld_WilayahID && x.fld_LadangID == NSWL.fld_LadangID).FirstOrDefault();

            if (getbatchno == null)
            {
                tbl_BatchRunNo tbl_BatchRunNo = new tbl_BatchRunNo();
                tbl_BatchRunNo.fld_BatchRunNo = 2;
                tbl_BatchRunNo.fld_BatchFlag = pkjmstbatchno;
                tbl_BatchRunNo.fld_NegaraID = NegaraID;
                tbl_BatchRunNo.fld_SyarikatID = SyarikatID;
                tbl_BatchRunNo.fld_WilayahID = WilayahID;
                tbl_BatchRunNo.fld_LadangID = LadangID;
                db.tbl_BatchRunNo.Add(tbl_BatchRunNo);
                db.SaveChanges();
                convertint = 1;
                //genbatchno = NSWL.fld_LdgCode.ToUpper() + "_WORKER_" + NSWL.fld_LdgCode.ToUpper() + "_" + convertint;
                genbatchno = NSWL.fld_LdgCode.ToUpper() + "_WORKER_" + NSWL.fld_LadangID + "_" + convertint; //modified by faeza 08.07.2023

            }
            else
            {
                convertint = getbatchno.fld_BatchRunNo;
                //genbatchno = NSWL.fld_LdgCode.ToUpper() + "_WORKER_" + NSWL.fld_LdgCode.ToUpper() + "_" + convertint;
                genbatchno = NSWL.fld_LdgCode.ToUpper() + "_WORKER_" + NSWL.fld_LadangID + "_" + convertint; //modified by faeza 08.07.2023
                convertint = convertint + 1;
                getbatchno.fld_BatchRunNo = convertint;
                db.Entry(getbatchno).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(genbatchno);
        }

        public JsonResult GetBatch()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetLadang GetLadang = new GetLadang();
            string kodldg = GetLadang.GetLadangCode(LadangID.Value);
            int batchValue = 0;
            string newbatch = "";

            //var batch = db.tblEmailNotiStatus.Where(x =>x.fldEmailNotiFlag.Contains("pkjmast") && x.fldWilayahID == WilayahID && x.fldLadangID == LadangID && x.fldSyarikatID == SyarikatID && x.fldNegaraID == NegaraID).Select(s => s.fldEmailNotiFlag).Distinct().OrderByDescending(s => s).FirstOrDefault();
            //var getpkj = db.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();
            var batch = dbr.tbl_Pkjmast.Where(x => x.fld_Batch.Contains("pkjmast") &&  x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).Select(s => s.fld_Batch).Distinct().OrderByDescending(s =>s).FirstOrDefault();
            if (batch != null)
            {
                //batchValue = Convert.ToInt32(batch.Substring(13, 3));
                batchValue = Convert.ToInt32(batch.Substring(batch.Length - 3));
               // batchValue = Convert.ToInt32(batch.Substring(10,3));
                batchValue = Convert.ToInt32(batch.Substring(batch.Length - 3));
            }
            else
            {
                batchValue = 0;
            }
            batchValue = batchValue + 1;
            newbatch = kodldg + "pkjmast" + batchValue.ToString("000");

            return Json(newbatch);
        }

        public JsonResult GetNopkjReplace(string nopkjlama, string pilihanNopkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            GetLadang GetLadang = new GetLadang();
            //string kuota = "F";
            string kuotawarganegara = nopkjlama.Substring(0,2);
            string kodldg = GetLadang.GetLadangCode(LadangID.Value);
            string thnKemasukan = DateTime.Now.ToString("yy");
            string nopkjnew = "";
            int kod = 0;

            if (pilihanNopkj=="1")
            {
                //pkjbaru
                nopkjnew = kuotawarganegara + kodldg + thnKemasukan;
                var getpkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();

                if (getpkj != null)
                {
                    kod = Convert.ToInt32(getpkj.Substring(7, 3));
                }
                else
                {
                    kod = 0;
                }
                kod = kod + 1;
                nopkjnew = nopkjnew + kod.ToString("000");
            }
            else if(pilihanNopkj=="2")
            {
                //pkjlama
                nopkjnew = nopkjlama;
            }
            else
            {
                nopkjnew = "";
            }
            return Json(nopkjnew);
        }

        public JsonResult GetPkjPindah(int wlyh, int syrkt, string pkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, wlyh, syrkt, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //add by faeza 03.11.2020
            //get connection estate yg nak pindah (transfer to)
            string hostN, catalogN, userN, passN = "";
            Connection.GetConnection(out hostN, out catalogN, out userN, out passN, WilayahID, SyarikatID, NegaraID.Value);
            MVC_SYSTEM_Models dbN = MVC_SYSTEM_Models.ConnectToSqlServer(hostN, catalogN, userN, passN);

            GetLadang GetLadang = new GetLadang();
            string kuota = "F";
            string kuotafpm = "T"; //added by faeza 09.07.2023
            string warganegara = "";
            string kodldg = "";
            string thnKemasukan = DateTime.Now.ToString("yy");
            string nopkjnew = "";
            int kod = 0;
            Models.tbl_Pkjmast Pkjmast = new Models.tbl_Pkjmast();
            string costcenter = GetLadang.GetLadangCostCenter(LadangID.Value); //added by faeza 09.07.2023

            if (costcenter == "1000")
            {
                if (wlyh != 0 && pkj != "")
                {
                    Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == pkj && w.fld_Kdaktf == "2" && w.fld_WilayahID == wlyh && w.fld_NegaraID == NegaraID && w.fld_SyarikatID == syrkt).FirstOrDefault();
                    //Pkjmast.fld_Trlhr = Pkjmast.fld_Trlhr.ToString();
                    //warganegara = Pkjmast.fld_Kdrkyt.Substring(0,1);
                    kodldg = GetLadang.GetLadangCode(LadangID.Value);

                    if (Pkjmast != null && Pkjmast.fld_Kdrkyt == "ID")
                    {
                        warganegara = Pkjmast.fld_Kdrkyt.Substring(1, 1);
                    }
                    else if (Pkjmast != null && Pkjmast.fld_Kdrkyt != "ID")
                    {
                        warganegara = Pkjmast.fld_Kdrkyt.Substring(0, 1);
                    }

                    nopkjnew = kuota + warganegara + kodldg + thnKemasukan;
                    var getpkj = dbN.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();
                    int getlength = 0;
                    if (getpkj != null)
                    {
                        //original code
                        //kod = Convert.ToInt32(getpkj.Substring(7, 3));

                        //added by faeza 0n 03.11.2020
                        //add checking length condition - ex:for worker id sahabat lot 01 (FI360F20006)
                        getlength = getpkj.Length;
                        if (getlength == 10)
                        {
                            kod = Convert.ToInt32(getpkj.Substring(7, 3));
                        }

                        if (getlength == 11)
                        {
                            kod = Convert.ToInt32(getpkj.Substring(8, 3));
                        }
                    }
                    else
                    {
                        kod = 0;
                    }
                    kod = kod + 1;
                    nopkjnew = nopkjnew + kod.ToString("000");
                }
            }
            else if (costcenter == "8800")
            {
                if (wlyh != 0 && pkj != "")
                {
                    Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == pkj && w.fld_Kdaktf == "2" && w.fld_WilayahID == wlyh && w.fld_NegaraID == NegaraID && w.fld_SyarikatID == syrkt).FirstOrDefault();
                    //Pkjmast.fld_Trlhr = Pkjmast.fld_Trlhr.ToString();
                    //warganegara = Pkjmast.fld_Kdrkyt.Substring(0,1);
                    kodldg = GetLadang.GetLadangCode(LadangID.Value);

                    if (Pkjmast != null && Pkjmast.fld_Kdrkyt == "ID")
                    {
                        warganegara = Pkjmast.fld_Kdrkyt.Substring(1, 1);
                    }
                    else if (Pkjmast != null && Pkjmast.fld_Kdrkyt != "ID")
                    {
                        warganegara = Pkjmast.fld_Kdrkyt.Substring(0, 1);
                    }

                    nopkjnew = kuotafpm + warganegara + kodldg + thnKemasukan;
                    var getpkj = dbN.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();
                    int getlength = 0;
                    if (getpkj != null)
                    {
                        //original code
                        //kod = Convert.ToInt32(getpkj.Substring(7, 3));

                        //added by faeza 0n 03.11.2020
                        //add checking length condition - ex:for worker id sahabat lot 01 (FI360F20006)
                        getlength = getpkj.Length;
                        if (getlength == 10)
                        {
                            kod = Convert.ToInt32(getpkj.Substring(7, 3));
                        }

                        if (getlength == 11)
                        {
                            kod = Convert.ToInt32(getpkj.Substring(8, 3));
                        }
                    }
                    else
                    {
                        kod = 0;
                    }
                    kod = kod + 1;
                    nopkjnew = nopkjnew + kod.ToString("000");
                }
            }
            return Json(new { Pkjmast = Pkjmast, nopkjnew = nopkjnew });

            //if (wlyh != 0 && pkj != "")
            //{
            //    Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_Nopkj == pkj && w.fld_Kdaktf=="2" && w.fld_WilayahID == wlyh && w.fld_NegaraID == NegaraID && w.fld_SyarikatID == syrkt).FirstOrDefault();
            //    //Pkjmast.fld_Trlhr = Pkjmast.fld_Trlhr.ToString();
            //    //warganegara = Pkjmast.fld_Kdrkyt.Substring(0,1);
            //    kodldg = GetLadang.GetLadangCode(LadangID.Value);

            //    if (Pkjmast!=null && Pkjmast.fld_Kdrkyt == "ID")
            //    {
            //        warganegara = Pkjmast.fld_Kdrkyt.Substring(1, 1);
            //    }
            //    else if (Pkjmast != null && Pkjmast.fld_Kdrkyt != "ID")
            //    {
            //        warganegara = Pkjmast.fld_Kdrkyt.Substring(0, 1);
            //    }

            //    nopkjnew = kuota + warganegara + kodldg + thnKemasukan;
            //    var getpkj = dbN.tbl_Pkjmast.Where(x => x.fld_Nopkj.Contains(nopkjnew) && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Nopkj).Distinct().OrderByDescending(s => s).FirstOrDefault();
            //    int getlength = 0;
            //    if (getpkj != null)
            //    {
            //        //original code
            //        //kod = Convert.ToInt32(getpkj.Substring(7, 3));

            //        //added by faeza 0n 03.11.2020
            //        //add checking length condition - ex:for worker id sahabat lot 01 (FI360F20006)
            //        getlength = getpkj.Length;
            //        if (getlength == 10)
            //        {
            //            kod = Convert.ToInt32(getpkj.Substring(7, 3));
            //        }

            //        if (getlength == 11)
            //        {
            //            kod = Convert.ToInt32(getpkj.Substring(8, 3));
            //        }
            //    }
            //    else
            //    {
            //        kod = 0;
            //    }
            //    kod = kod + 1;
            //    nopkjnew = nopkjnew + kod.ToString("000");
            //}
            //return Json(new { Pkjmast = Pkjmast, nopkjnew = nopkjnew });
        }

        public JsonResult GetPermitExpired(DateTime permitDate)
        {
            DateTime exprdDate = permitDate.AddYears(1);
            //exprdDate = exprdDate.ToString("dd/mm/yy");
            string dt = exprdDate.ToString("dd/mm/yyyy");
            return Json(exprdDate);
        }

        //groupinfo action starts here
        public ActionResult GroupInfo(string workerid, string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.filter = filter;
            ViewBag.BasicInfo = "class = active";

            return View();
        }

        public PartialViewResult searchGroup(string workerid, string filter = "", int page = 1, string sort = "fld_KodKumpulan",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.vw_KumpulanKerja>();
            int role = GetIdentity.RoleID(getuserid).Value;
            //ViewBag.filter = filter;

            var GroupList = dbview.vw_KumpulanKerja
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false);

            if (!String.IsNullOrEmpty(filter))
            {
                records.Content = GroupList
                    .Where(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()))
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count(x => x.fld_KodKumpulan.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_KodKerja.ToUpper().Contains(filter.ToUpper()) ||
                                x.fld_Keterangan.ToUpper().Contains(filter.ToUpper()));
            }

            else
            {
                records.Content = GroupList
                    .OrderBy(sort + " " + sortdir)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                records.TotalRecords = GroupList
                    .Count();

            }

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;

            return PartialView(records);
        }

        public ActionResult GroupCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> kodKerja = new List<SelectListItem>();

            kodKerja = new SelectList(
                db.tblOptionConfigsWebs
                    .Where(x => x.fldOptConfFlag1 == "kmplnKategoriList" &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                    .OrderBy(o => o.fldOptConfValue)
                    .Select(s => new SelectListItem
                    {
                        Value = s.fldOptConfValue,
                        Text = s.fldOptConfValue + " - " + s.fldOptConfDesc
                    }), "Value", "Text").ToList();

            ViewBag.kodKerjaList = kodKerja;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupCreate(Models.tbl_KumpulanKerjaViewModelCreate kumpulanKerjaViewModelCreate)
        {
            //Check_Balik
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            if (ModelState.IsValid)
            {
                try
                {
                    String kodKerja = kumpulanKerjaViewModelCreate.fld_KodKerja;

                    Models.tbl_KumpulanKerja kumpulanKerja = new tbl_KumpulanKerja();

                    PropertyCopy.Copy(kumpulanKerja, kumpulanKerjaViewModelCreate);

                    var getCurrentGrpNo = dbr.tbl_KumpulanKerja
                        .Where(x => x.fld_KodKumpulan.Contains(kodKerja) && x.fld_NegaraID == NegaraID &&
                                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                    x.fld_LadangID == LadangID)
                        .Select(s => s.fld_KodKumpulan)
                        .OrderByDescending(s => s)
                        .FirstOrDefault();

                    var getGrpActivity = db.tblOptionConfigsWebs
                        .Where(x => x.fldOptConfValue == kumpulanKerja.fld_KodKerja && x.fldOptConfFlag1 == "kmplnKategoriList" &&
                                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                        .Select(s => s.fldOptConfDesc)
                        .Distinct()
                        .FirstOrDefault();

                    kumpulanKerja.fld_deleted = false;
                    kumpulanKerja.fld_NegaraID = NegaraID;
                    kumpulanKerja.fld_SyarikatID = SyarikatID;
                    kumpulanKerja.fld_WilayahID = WilayahID;
                    kumpulanKerja.fld_LadangID = LadangID;
                    kumpulanKerja.fld_KodKerja = getGrpActivity;
                    kumpulanKerja.fld_Keterangan = kumpulanKerja.fld_Keterangan.ToUpper();

                    if (getCurrentGrpNo == null)
                    {
                        kumpulanKerja.fld_KodKumpulan = kumpulanKerjaViewModelCreate.fld_KodKerja + "001";
                        dbr.tbl_KumpulanKerja.Add(kumpulanKerja);
                        dbr.SaveChanges();

                        db.Dispose();
                        dbr.Dispose();

                        string appname = Request.ApplicationPath;
                        string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                        var lang = Request.RequestContext.RouteData.Values["lang"];

                        if (appname != "/")
                        {
                            domain = domain + appname;
                        }

                        return Json(new
                        {
                            success = true,
                            msg = GlobalResEstate.msgAdd,
                            status = "success",
                            checkingdata = "0",
                            method = "1",
                            div = "searchResult",
                            rootUrl = domain,
                            action = "searchGroup",
                            controller = "BasicInfo"
                        });
                    }

                    else
                    {
                        int generateNewGrpNo = Convert.ToInt32(getCurrentGrpNo.Substring(1)) + 1;

                        kumpulanKerja.fld_KodKumpulan = kumpulanKerjaViewModelCreate.fld_KodKerja + generateNewGrpNo.ToString("000");
                        dbr.tbl_KumpulanKerja.Add(kumpulanKerja);
                        dbr.SaveChanges();

                        db.Dispose();
                        dbr.Dispose();

                        string appname = Request.ApplicationPath;
                        string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                        var lang = Request.RequestContext.RouteData.Values["lang"];

                        if (appname != "/")
                        {
                            domain = domain + appname;
                        }

                        return Json(new
                        {
                            success = true,
                            msg = GlobalResEstate.msgAdd,
                            status = "success",
                            checkingdata = "0",
                            method = "1",
                            div = "searchResult",
                            rootUrl = domain,
                            action = "searchGroup",
                            controller = "BasicInfo"
                        });
                    }
                }

                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgError,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }

            else
            {
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult GroupEdit(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_KumpulanKerja tbl_KumpulanKerja = dbr.tbl_KumpulanKerja
                .Single(u => u.fld_KumpulanID == id && u.fld_WilayahID == WilayahID && u.fld_SyarikatID == SyarikatID &&
                             u.fld_NegaraID == NegaraID && u.fld_deleted == false);

            Models.tbl_KumpulanKerjaViewModelEdit kumpulanKerjaViewModelEdit = new tbl_KumpulanKerjaViewModelEdit();

            PropertyCopy.Copy(kumpulanKerjaViewModelEdit, tbl_KumpulanKerja);

            ViewBag.fld_LadangName = getidentity.estatename(Convert.ToInt32(getuserid));
            ViewBag.fld_WilayahName = getidentity.getWilayahName(Convert.ToInt32(getuserid));

            return PartialView("GroupEdit", kumpulanKerjaViewModelEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupEdit(int id, Models.tbl_KumpulanKerjaViewModelEdit tbl_KumpulanKerja)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            if (ModelState.IsValid)
            {
                try
                {
                    var getdata = dbr.tbl_KumpulanKerja
                        .Single(x => x.fld_KumpulanID == id && x.fld_LadangID == LadangID &&
                                     x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID && x.fld_deleted == false);

                    getdata.fld_Keterangan = tbl_KumpulanKerja.fld_Keterangan.ToUpper();

                    dbr.Entry(getdata).State = EntityState.Modified;
                    dbr.SaveChanges();

                    db.Dispose();
                    dbr.Dispose();

                    string appname = Request.ApplicationPath;
                    string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                    var lang = Request.RequestContext.RouteData.Values["lang"];

                    if (appname != "/")
                    {
                        domain = domain + appname;
                    }

                    return Json(new
                    {
                        success = true,
                        msg =GlobalResEstate.msgUpdate,
                        status = "success",
                        checkingdata = "0",
                        method = "1",
                        div = "searchResult",
                        rootUrl = domain,
                        action = "searchGroup",
                        controller = "BasicInfo"
                    });
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                    db.Dispose();
                    dbr.Dispose();

                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgError,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
            }
            else
            {
                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgErrorData,
                    status = "danger",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult GroupMemberInfo(int id, int page = 1, string sort = "fld_Nopkj",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_Pkjmast>();

            var grpMemberList = dbview.tbl_Pkjmast
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_KumpulanID == id && x.fld_Kdaktf == "1");

            records.Content = grpMemberList
                .OrderBy(sort + " " + sortdir)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            records.TotalRecords = grpMemberList
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.pageSize = pageSize;

            return PartialView(records);
        }

        public ActionResult GroupDelete(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.vw_KumpulanKerja vw_KumpulanKerja = dbr.vw_KumpulanKerja
                .Single(x => x.fld_KumpulanID == id && x.fld_LadangID == LadangID &&
                             x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID && x.fld_deleted == false);

            var getGroupCode = dbr.tbl_KumpulanKerja
                .Where(x => x.fld_KumpulanID == id)
                .Select(s => s.fld_KodKumpulan)
                .OrderByDescending(s => s)
                .FirstOrDefault();

            var getGroupActivity = dbr.tbl_KumpulanKerja
                .Where(x => x.fld_KumpulanID == id)
                .Select(s => s.fld_Keterangan)
                .OrderByDescending(s => s)
                .FirstOrDefault();

            ViewBag.KodKumpulan = getGroupCode;
            ViewBag.Keterangan = getGroupActivity;

            return PartialView("GroupDelete", vw_KumpulanKerja);
        }

        [HttpPost, ActionName("GroupDelete")]
        public ActionResult GroupDeleteConfirm(int id)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_KumpulanKerja
                    .Single(x => x.fld_KumpulanID == id && x.fld_LadangID == LadangID &&
                                 x.fld_WilayahID == WilayahID && x.fld_NegaraID == NegaraID && x.fld_deleted == false);
                getdata.fld_deleted = true;
                dbr.Entry(getdata).State = EntityState.Modified;
                dbr.SaveChanges();

                db.Dispose();
                dbr.Dispose();

                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];

                if (appname != "/")
                {
                    domain = domain + appname;
                }

                return Json(new
                {
                    success = true,
                    msg = GlobalResEstate.msgDelete2,
                    status = "success",
                    checkingdata = "0",
                    method = "1",
                    div = "searchResult",
                    rootUrl = domain,
                    action = "searchGroup",
                    controller = "BasicInfo"
                });
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                db.Dispose();
                dbr.Dispose();

                return Json(new
                {
                    success = false,
                    msg = GlobalResEstate.msgError,
                    status = "danger",
                    checkingdata = "0"
                });
            }
        }

        public ActionResult BankAccInfo()
        {
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var tbl_Ladang = db.tbl_Ladang.Find(LadangID); //added by kamalia 26/4/22 utk pnggil data bank id ladang


            List<SelectListItem> Bank = new List<SelectListItem>();
            Bank = new SelectList(db.tbl_Bank.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_KodBank, Text = s.fld_KodBank + " - " + s.fld_NamaBank }).Distinct(), "Value", "Text", tbl_Ladang.fld_BankCode).ToList();  //added by kamalia 26/4/22 utk pnggil data bank id ladang
            Bank.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            var result = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).FirstOrDefault();
            ViewBag.fld_BankCode = Bank;

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankAccInfo(MasterModels.tbl_Ladang tbl_Ladang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            //string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdatahq = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WlyhID == WilayahID && x.fld_ID == LadangID).FirstOrDefault();
                getdatahq.fld_BankCode = tbl_Ladang.fld_BankCode;
                getdatahq.fld_NoAcc = tbl_Ladang.fld_NoAcc;
                getdatahq.fld_OriginatorID = tbl_Ladang.fld_OriginatorID;
                getdatahq.fld_OriginatorName = tbl_Ladang.fld_OriginatorName.ToUpper();
                getdatahq.fld_BranchCode = tbl_Ladang.fld_BranchCode;
                getdatahq.fld_BranchName = tbl_Ladang.fld_BranchName.ToUpper();
                getdatahq.fld_BankCreatedBy = User.Identity.Name;
                getdatahq.fld_BankCreatedDate = DateTime.Now;
                db.Entry(getdatahq).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { redirectTo = Url.Action("BankAccInfo", "BasicInfo") }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                
                    return Json(new { success = true, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
                }
        }

        public ActionResult LevelsIO()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> IOcode = new List<SelectListItem>();
            IOcode = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1), "fld_IOcode", "fld_IOcode").ToList();
            IOcode.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            List<SelectListItem> IOreff = new List<SelectListItem>();
            IOreff = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 2), "fld_IOcode", "fld_IOcode").ToList();
            IOreff.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.IO_code = IOcode;
            ViewBag.IO_reff = IOreff;
            return PartialView();
        }
         //ActionName("LevelsIO")
        [HttpPost, ActionName("LevelsIO")]
        [ValidateAntiForgeryToken]
        public ActionResult LevelsIOsubmit(string IO_code, string IO_reff)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> IOcode = new List<SelectListItem>();
            IOcode = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1), "fld_IOcode", "fld_IOcode").ToList();
            IOcode.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            List<SelectListItem> IOreff = new List<SelectListItem>();
            IOreff = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 2), "fld_IOcode", "fld_IOcode").ToList();
            IOreff.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            string IO_type = Request.Form["IOType"];

            var checkIo = dbr.tbl_IO.Where(x => x.fld_IOcode == IO_code && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1).FirstOrDefault();
            if (checkIo != null)
            {
                //checkIo.fld_IOtype = IO_type;
                //checkIo.fld_IOref = IO_reff;
                checkIo.fld_Status = 2;
                dbr.Entry(checkIo).State = EntityState.Modified;
                dbr.SaveChanges();
            }
           
            ViewBag.IO_code = IOcode;
            ViewBag.IO_reff = IOreff;
            return PartialView();
        }

        //public ActionResult LevelsInfoTable()
        //{
        //    ViewBag.BasicInfo = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> JenisPkt = new List<SelectListItem>();

        //    JenisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
        //    JenisPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    ViewBag.JnsPkt = JenisPkt;
        //    return View();
        //}

        //public ActionResult LevelInfo()
        //{
        //    ViewBag.BasicInfo = "class = active";
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    int? getuserid = getidentity.ID(User.Identity.Name);
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    List<SelectListItem> JenisPkt = new List<SelectListItem>();

        //    JenisPkt = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnspkt" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }).Distinct(), "Value", "Text").ToList();
        //    JenisPkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
        //    ViewBag.JnsPkt = JenisPkt;
        //    return View();
        //}

        //public ActionResult LevelInfoPkt(string JnsPkt = "1", int page = 1, string sort = "fld_PktUtama", string sortdir = "ASC")
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    var records = new PagedList<ViewingModels.tbl_PktUtama>();
        //    records.Content = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
        //           .OrderBy(sort + " " + sortdir)
        //           .Skip((page - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //    records.TotalRecords = dbview.tbl_PktUtama.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
        //    records.CurrentPage = page;
        //    records.PageSize = pageSize;
        //    return View(records);
        //}
        //public ActionResult LevelInfoSubPkt(string JnsPkt = "2", int page = 1, string sort = "fld_Pkt", string sortdir = "ASC")
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    var records = new PagedList<ViewingModels.tbl_SubPkt>();
        //    records.Content = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
        //           .OrderBy(sort + " " + sortdir)
        //           .Skip((page - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //    records.TotalRecords = dbview.tbl_SubPkt.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
        //    records.CurrentPage = page;
        //    records.PageSize = pageSize;
        //    return View(records);
        //}

        //public ActionResult LevelInfoBlok(string JnsPkt = "3", int page = 1, string sort = "fld_Blok", string sortdir = "ASC")
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    string host, catalog, user, pass = "";
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
        //    Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
        //    MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

        //    int pageSize = int.Parse(GetConfig.GetData("paging"));
        //    var records = new PagedList<ViewingModels.tbl_Blok>();
        //    records.Content = dbview.tbl_Blok.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false)
        //           .OrderBy(sort + " " + sortdir)
        //           .Skip((page - 1) * pageSize)
        //           .Take(pageSize)
        //           .ToList();

        //    records.TotalRecords = dbview.tbl_Blok.Where(x => x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == false).Count();
        //    records.CurrentPage = page;
        //    records.PageSize = pageSize;
        //    return View(records);
        //}

        public ActionResult EstateReminder()
        {
            ViewBag.BasicInfo = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            MasterModels.tbl_Ladang tbl_Ladang = db.tbl_Ladang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
            string kodngri = tbl_Ladang.fld_KodNegeri.ToString();
            string negeri = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fldOptConfValue == kodngri && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false && x.fldOptConfValue == kodngri).Select(s => s.fldOptConfDesc).FirstOrDefault();

            ViewBag.Negeri = negeri;
            return View(tbl_Ladang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstateReminder(MasterModels.tbl_Ladang tbl_Ladang)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);


            tbl_Ladang.fld_PengurusSblm = "-";
            tbl_Ladang.fld_AdressSblm = "-";
            tbl_Ladang.fld_TelSblm = "-";
            tbl_Ladang.fld_FaxSblm = "-";
            tbl_Ladang.fld_LdgEmailSblm = "-";

            if (ModelState.IsValid)
            {
                try
                {
                    var getdatahq = db.tbl_Ladang.Where(w => w.fld_NegaraID == NegaraID && w.fld_SyarikatID == SyarikatID && w.fld_ID == tbl_Ladang.fld_ID && w.fld_WlyhID == tbl_Ladang.fld_WlyhID && w.fld_Deleted==false).FirstOrDefault();
                    getdatahq.fld_PengurusSblm = getdatahq.fld_Pengurus;
                    getdatahq.fld_AdressSblm = getdatahq.fld_Adress;
                    getdatahq.fld_TelSblm = getdatahq.fld_Tel;
                    getdatahq.fld_FaxSblm = getdatahq.fld_Fax;
                    getdatahq.fld_LdgEmailSblm = getdatahq.fld_LdgEmail;

                    getdatahq.fld_Pengurus = tbl_Ladang.fld_Pengurus.ToUpper();
                    getdatahq.fld_Adress = tbl_Ladang.fld_Adress.ToUpper();
                    getdatahq.fld_Tel = tbl_Ladang.fld_Tel;
                    getdatahq.fld_Fax = tbl_Ladang.fld_Fax;
                    getdatahq.fld_LdgEmail = tbl_Ladang.fld_LdgEmail;

                    db.Entry(getdatahq).State = EntityState.Modified;
                    db.SaveChanges();

                    var getid = tbl_Ladang.fld_ID;
                }
                catch (Exception ex)
                {
                    geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                    //return Json(new { success = true, msg = "Error occur please contact IT.", status = "danger", checkingdata = "1" });
                }
            }
            return RedirectToAction("Index", "Login");
        }

        public JsonResult GetPktChange(string pktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_PktUtama tbl_PktUtama = new Models.tbl_PktUtama();

            string jnsTnmn = "";
            string statusTnmn = "";
            decimal luas = 0;

            if (pktUtama != "0")
            {
                tbl_PktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == pktUtama && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).FirstOrDefault();
                jnsTnmn = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsTanaman" && x.fldOptConfValue == tbl_PktUtama.fld_JnsTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                statusTnmn = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == tbl_PktUtama.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                luas = tbl_PktUtama.fld_LsPktUtama.GetValueOrDefault();
            }
            return Json(new { jnsTnmn = jnsTnmn, statusTnmn = statusTnmn, luas = luas });
        }

        public JsonResult GetKawList()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> Kawasanlist = new List<SelectListItem>();
            Kawasanlist = new SelectList(db.tblOptionConfigsWebs.Where(x =>x.fldOptConfFlag1== "jnsKawasan" && x.fld_NegaraID==NegaraID && x.fld_SyarikatID==SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc}), "Value", "Text").ToList();

            return Json(Kawasanlist);
        }

        public JsonResult GetDirianPokok()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            string dirianPokok = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "dirianPokok" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfValue).FirstOrDefault();

            //List<SelectListItem> Dirianlist = new List<SelectListItem>();
            //Dirianlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "dirianPokok" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc }), "Value", "Text").ToList();

            return Json(dirianPokok);
        }

        public JsonResult GetKesukaran(string jnsKesukaran)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> TahapKesukaranlist = new List<SelectListItem>();
            TahapKesukaranlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "tahapKesukaran" && x.fldOptConfValue.StartsWith(jnsKesukaran) && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();
            return Json(TahapKesukaranlist);
        }

        public JsonResult GetLuasFromIO(string IO)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            //List<SelectListItem> IOcode = new List<SelectListItem>();
            //IOcode = new SelectList(dbr.tbl_IO.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_Status == 1), "fld_IOcode", "fld_IOcode").ToList();
            decimal luas = 0;
            decimal luasTnmn = 0;
            decimal luasBerhasil = 0;
            string fld_NamaPktUtama = ""; //Added by Shazana 13/6/2023
            var checkLuas = db.tbl_IOSAP.Where(x => x.fld_IOcode == IO && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
            luas = checkLuas.fld_LuasPkt.GetValueOrDefault();
             luasTnmn = checkLuas.fld_LuasKawTnmn.GetValueOrDefault();
             luasBerhasil = checkLuas.fld_LuasKawBerhasil.GetValueOrDefault();
            fld_NamaPktUtama = checkLuas.fld_PktCode;//Added by Shazana 13/6/2023
            // return Json(new { luas = luas, luasTnmn = luasTnmn, luasBerhasil = luasBerhasil }); //Commented by Shazana 13/6/2023
            return Json(new { luas = luas, luasTnmn = luasTnmn, luasBerhasil = luasBerhasil, fld_NamaPktUtama = fld_NamaPktUtama }); //Added by Shazana 13/6/2023
        }

        public JsonResult EmailValidation(string emel)
        {
            Regex Regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            var result = Regex.IsMatch(emel);
            return Json(result);
        }

        public JsonResult SaveKaw(string pkt, int level, string jnskaw, decimal luas)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_KawTidakBerhasil tbl_KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
            int flag = 0;
            string checkExist = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == pkt && x.fld_LevelPkt == level && x.fld_JnsKaw == jnskaw && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_JnsKaw).FirstOrDefault();
            if (checkExist == null)
            {
                //data not exist
                tbl_KawTidakBerhasil.fld_KodPkt = pkt;
                tbl_KawTidakBerhasil.fld_LevelPkt = level;
                tbl_KawTidakBerhasil.fld_JnsKaw = jnskaw;
                tbl_KawTidakBerhasil.fld_LuasKaw = luas;
                tbl_KawTidakBerhasil.fld_NegaraID = NegaraID;
                tbl_KawTidakBerhasil.fld_SyarikatID = SyarikatID;
                tbl_KawTidakBerhasil.fld_WilayahID = WilayahID;
                tbl_KawTidakBerhasil.fld_LadangID = LadangID;
                tbl_KawTidakBerhasil.fld_Deleted = false;
                dbr.tbl_KawTidakBerhasil.Add(tbl_KawTidakBerhasil);
                dbr.SaveChanges();
                flag = 1;
            }
            else
            {
                //data already exist
                flag = 2;
            }
            return Json(flag);
        }

        public JsonResult RemoveKaw(string kodpkt, string jnskaw)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int flag = 0;
            var getkaw = dbr.tbl_KawTidakBerhasil.Where(x => x.fld_KodPkt == kodpkt && x.fld_JnsKaw == jnskaw && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            if (getkaw == null)
            {
                flag = 1;
            }
            else
            {
                getkaw.fld_Deleted = true;
                dbr.Entry(getkaw).State = EntityState.Modified;
                dbr.SaveChanges();
                flag = 2;
            }
            return Json(flag);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_SupportedDoc SupportedDoc = new Models.tbl_SupportedDoc();

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname = file.FileName;
                        string nopkj = fname.Split('.').FirstOrDefault().Trim();
                        var folder = Server.MapPath("~/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID);
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        string savePath = Path.Combine(folder, fname);
                        file.SaveAs(savePath);

                        var findRoute = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag=="picPkj" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (findRoute == null)
                        {
                            //add
                            SupportedDoc.fld_Nopkj = nopkj;
                            SupportedDoc.fld_NamaFile = fname;
                            SupportedDoc.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            SupportedDoc.fld_Flag = "picPkj";
                            SupportedDoc.fld_NegaraID = NegaraID;
                            SupportedDoc.fld_SyarikatID = SyarikatID;
                            SupportedDoc.fld_WilayahID = WilayahID;
                            SupportedDoc.fld_LadangID = LadangID;
                            SupportedDoc.fld_Deleted = false;
                            dbr.tbl_SupportedDoc.Add(SupportedDoc);
                            dbr.SaveChanges();
                        }
                        else
                        {
                            //update
                            findRoute.fld_NamaFile = fname;
                            findRoute.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            findRoute.fld_Deleted = false;
                            dbr.Entry(findRoute).State = EntityState.Modified;
                            dbr.SaveChanges();
                        }


                    }
                    return Json(GlobalResEstate.msgFileUpload);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json(GlobalResEstate.msgNofile);
            }
        }

        [HttpPost]
        public ActionResult UploadSupportedFiles()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_SupportedDoc SupportedDoc = new Models.tbl_SupportedDoc();

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname = file.FileName;
                        string nopkj = fname.Split('_').FirstOrDefault().Trim();
                        var folder = Server.MapPath("~/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID);
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        string savePath = Path.Combine(folder, fname);
                        file.SaveAs(savePath);

                        var findRoute = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_NamaFile==fname && x.fld_Flag == "supportedDoc" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        if (findRoute == null)
                        {
                            //add
                            SupportedDoc.fld_Nopkj = nopkj;
                            SupportedDoc.fld_NamaFile = fname;
                            SupportedDoc.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            SupportedDoc.fld_Flag = "supportedDoc";
                            SupportedDoc.fld_NegaraID = NegaraID;
                            SupportedDoc.fld_SyarikatID = SyarikatID;
                            SupportedDoc.fld_WilayahID = WilayahID;
                            SupportedDoc.fld_LadangID = LadangID;
                            SupportedDoc.fld_Deleted = false;
                            dbr.tbl_SupportedDoc.Add(SupportedDoc);
                            dbr.SaveChanges();
                        }
                        else
                        {
                            //update
                            findRoute.fld_NamaFile = fname;
                            findRoute.fld_Url = "/Asset/Upload/" + NegaraID + SyarikatID + WilayahID + "/" + LadangID + "/" + fname;
                            findRoute.fld_Deleted = false;
                            dbr.Entry(findRoute).State = EntityState.Modified;
                            dbr.SaveChanges();
                        }


                    }
                    //return Json("File Uploaded Successfully!");
                    return Json(GlobalResEstate.msgFileUpload);
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json(GlobalResEstate.msgNofile);
            }
        }

        public JsonResult GetUmurPkj(string sdate)
        {
            int age = 0;
            if (sdate != "")
            {
                DateTime dateBirth = DateTime.Parse(sdate);
                DateTime dateToday = DateTime.Today;
                age = dateToday.Year - dateBirth.Year;
                if (dateToday < dateBirth.AddYears(age))
                {
                    age--;
                }
               
            }
            return Json(age);
        }

        [HttpPost]
        public ActionResult WorkerDoc(string nopkj)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            var result = dbr.tbl_SupportedDoc.Where(x => x.fld_Nopkj == nopkj && x.fld_Flag == "supportedDoc" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);
            return PartialView(result);
        }

        public JsonResult RemoveDoc(int docID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            string msg = "";
            Boolean statusTrans = false;

            var findDoc = dbr.tbl_SupportedDoc.Where(x => x.fld_ID == docID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
            if (findDoc != null)
            {
                findDoc.fld_Deleted = true;
                dbr.Entry(findDoc).State = EntityState.Modified;
                dbr.SaveChanges();
                statusTrans = true;
                msg = GlobalResEstate.msgAdd;
            }
            else
            {
                statusTrans = false;
                msg = GlobalResEstate.msgDataExist;
            }
            return Json(new { msg = msg, statusTrans = statusTrans });
        }

        public JsonResult GetWilayahList(int syrktID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> wilayahlist = new List<SelectListItem>();

            if (syrktID == 0)
            {
                wilayahlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            }
            else
            {
                wilayahlist = new SelectList(db.tbl_Wilayah.Where(x => x.fld_SyarikatID == syrktID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).OrderBy(o => o.fld_ID).Select(s => new SelectListItem { Value = s.fld_ID.ToString().Trim(), Text = s.fld_WlyhName }), "Value", "Text").ToList();
                wilayahlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            }

            return Json(wilayahlist);
        }


        public ActionResult EstatePublicHolidayMaintenance(int page = 1, string sort = "fld_TarikhCuti",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            ViewBag.Maintenance = "class = active";

            int drpyear = 0;
            int drprangeyear = 0;
            int month = timezone.gettimezone().Month;

            drpyear = timezone.gettimezone().Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            drprangeyear = timezone.gettimezone().Year;

            var nextYear = timezone.gettimezone().Year + 1;

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

            yearlist.Add(new SelectListItem { Text = nextYear.ToString(), Value = nextYear.ToString() });

            ViewBag.YearList = yearlist;

            return View();
        }

        public ActionResult _EstatePublicHolidayMaintenance(int? YearList, int page = 1, string sort = "fld_TarikhCuti",
            string sortdir = "ASC")
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var message = "";
            if (String.IsNullOrEmpty(YearList.ToString()))
            {
                message = GlobalResEstate.msgChoosePublicHolidayAllocation;
                ViewBag.Message = message;
            }

            else
            {
                message = GlobalResEstate.msgErrorSearch;
                ViewBag.Message = message;
            }

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<MasterModels.vw_CutiUmumKelayakanDetails>();
            int role = GetIdentity.RoleID(getuserid).Value;

            var publicHolidayData = db.vw_CutiUmumKelayakanDetails
                .Where(x => x.fld_Tahun == YearList && x.fld_NegaraID == NegaraID &&
                            x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false)
                .OrderBy(o => o.fld_TarikhCuti);

            records.Content = publicHolidayData.ToList();

            records.TotalRecords = publicHolidayData
                .Count();

            records.CurrentPage = page;
            records.PageSize = pageSize;
            ViewBag.RoleID = role;
            ViewBag.pageSize = pageSize;
            ViewBag.TotalRecord = publicHolidayData
                .Count();

            var cutiPilihanHistoryData = db.tbl_CutiPilihanHistory.SingleOrDefault(x => x.fld_Flag == "EST" &&
                x.fld_Tahun == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

            var selectionStatus = false;

            if (cutiPilihanHistoryData != null)
            {
                selectionStatus = true;
            }

            ViewBag.SelectionStatus = selectionStatus;

            return View(records);
        }

        public ActionResult _EstatePublicHolidayMaintenanceEdit(List<int?> leaveID, int year)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                List<tbl_CutiUmumLdg> cutiUmumLdgList = new List<tbl_CutiUmumLdg>();

                if (leaveID != null)
                {
                    //remove checkall value from leaveID list
                    leaveID.Remove(0);
                }

                var leaveSelectionData = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                    x.fldOptConfFlag1 == "cutiPilihanLdg" && x.fld_NegaraID == NegaraID &&
                    x.fld_SyarikatID == SyarikatID && x.fldDeleted == false);

                var maxLeaveSelection = Convert.ToInt32(leaveSelectionData.fldOptConfValue);

                if (leaveID != null)
                {
                    if (leaveID.Count == maxLeaveSelection)
                    {
                        var publicHolidayDataAll = db.tbl_CutiUmumLdg.Where(x =>
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID &&
                            x.fld_Deleted == false);

                        foreach (var publicHoliday in publicHolidayDataAll)
                        {
                            if (publicHoliday.fld_CreatedInd != "HQ")
                            {
                                publicHoliday.fld_Deleted = true;
                                publicHoliday.fld_ModifiedBy = getuserid;
                                publicHoliday.fld_ModifiedDT = timezone.gettimezone();
                            }
                        }

                        db.SaveChanges();

                        foreach (var leave in leaveID)
                        {
                            var publicHolidayData = db.tbl_CutiUmumLdg.SingleOrDefault(x =>
                                x.fld_CutiMasterID == leave && x.fld_CreatedInd != "HQ" &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                                x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID);

                            if (publicHolidayData != null)
                            {
                                publicHolidayData.fld_Deleted = false;
                                publicHolidayData.fld_ModifiedBy = getuserid;
                                publicHolidayData.fld_ModifiedDT = timezone.gettimezone();

                                db.SaveChanges();
                            }

                            else
                            {
                                cutiUmumLdgList.Add(new MasterModels.tbl_CutiUmumLdg
                                {
                                    fld_CutiMasterID = leave,
                                    fld_Year = year,
                                    fld_NegaraID = NegaraID,
                                    fld_SyarikatID = SyarikatID,
                                    fld_WilayahID = WilayahID,
                                    fld_LadangID = LadangID,
                                    fld_Deleted = false,
                                    fld_CreatedBy = getuserid,
                                    fld_CreatedDT = timezone.gettimezone(),
                                    fld_CreatedInd = "EST"
                                });
                            }
                        }

                        if (cutiUmumLdgList.Count != 0)
                        {
                            db.tbl_CutiUmumLdg.AddRange(cutiUmumLdgList);
                            db.SaveChanges();
                        }

                        var publicHolidayCode = db.tblOptionConfigsWebs.SingleOrDefault(x =>
                                x.fldOptConfFlag1 == "cutiUmumFlag" &&
                                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false)
                            .fldOptConfValue;

                        var selectedPublicHolidayCount = db.tbl_CutiUmumLdg.Count(x =>
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                            x.fld_LadangID == LadangID && x.fld_Deleted == false);

                        var workerPublicHolidayData = dbr.tbl_CutiPeruntukan.Where(x =>
                            x.fld_KodCuti == publicHolidayCode && x.fld_Tahun == year &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID &&
                            x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false);

                        foreach (var workerPublicHoliday in workerPublicHolidayData)
                        {
                            workerPublicHoliday.fld_JumlahCuti = selectedPublicHolidayCount;
                        }

                        dbr.SaveChanges();

                        tbl_CutiPilihanHistory cutiPilihanHistory = new tbl_CutiPilihanHistory();

                        cutiPilihanHistory.fld_BilCutiDipilih = leaveID.Count;
                        cutiPilihanHistory.fld_Flag = "EST";
                        cutiPilihanHistory.fld_Tahun = year;
                        cutiPilihanHistory.fld_NegaraID = NegaraID;
                        cutiPilihanHistory.fld_SyarikatID = SyarikatID;
                        cutiPilihanHistory.fld_WilayahID = WilayahID;
                        cutiPilihanHistory.fld_LadangID = LadangID;
                        cutiPilihanHistory.fld_Deleted = false;

                        db.tbl_CutiPilihanHistory.Add(cutiPilihanHistory);
                        db.SaveChanges();

                        string appname = Request.ApplicationPath;
                        string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                        var lang = Request.RequestContext.RouteData.Values["lang"];

                        if (appname != "/")
                        {
                            domain = domain + appname;
                        }

                        return Json(new
                        {
                            success = true,
                            msg = GlobalResEstate.msgAdd,
                            status = "success",
                            year = year,
                            checkingdata = "1"
                        });
                    }

                    else
                    {
                        return Json(new
                        {
                            success = false,
                            msg = GlobalResEstate.msgLeaveMaintenance1 + " " + maxLeaveSelection + " " + GlobalResEstate.msgLeaveMaintenance2,
                            status = "danger",
                            checkingdata = "0"
                        });
                    }
                }

                else
                {
                    return Json(new
                    {
                        success = false,
                        msg = GlobalResEstate.msgLeaveMaintenance1 + " " + maxLeaveSelection + " " + GlobalResEstate.msgLeaveMaintenance2,
                        status = "danger",
                        checkingdata = "0"
                    });
                }
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

        //kamy tambah 30/09/2021
        public JsonResult GetBatchNoCancel(string pkjmstbatchno)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);

            var NSWL = GetNSWL.GetLadangDetail(LadangID.Value);

            int? convertint = 0;
            string genbatchno = "";

            var getbatchno = db.tbl_BatchRunNo.Where(x => x.fld_BatchFlag == pkjmstbatchno && x.fld_NegaraID == NSWL.fld_NegaraID && x.fld_SyarikatID == NSWL.fld_SyarikatID && x.fld_WilayahID == NSWL.fld_WilayahID && x.fld_LadangID == NSWL.fld_LadangID).FirstOrDefault();

            if (getbatchno == null)
            {
                tbl_BatchRunNo tbl_BatchRunNo = new tbl_BatchRunNo();
                tbl_BatchRunNo.fld_BatchRunNo = 2;
                tbl_BatchRunNo.fld_BatchFlag = pkjmstbatchno;
                tbl_BatchRunNo.fld_NegaraID = NegaraID;
                tbl_BatchRunNo.fld_SyarikatID = SyarikatID;
                tbl_BatchRunNo.fld_WilayahID = WilayahID;
                tbl_BatchRunNo.fld_LadangID = LadangID;
                db.tbl_BatchRunNo.Add(tbl_BatchRunNo);
                db.SaveChanges();
                convertint = 1;
                genbatchno = NSWL.fld_LdgCode.ToUpper() + "WORKER" + NSWL.fld_LdgCode.ToUpper() + "_" + convertint;
            }
            else
            {
                convertint = getbatchno.fld_BatchRunNo;
                genbatchno = NSWL.fld_LdgCode.ToUpper() + "WORKER" + NSWL.fld_LdgCode.ToUpper() + "_" + convertint;
                convertint = convertint - 1;
                getbatchno.fld_BatchRunNo = convertint;
                db.Entry(getbatchno).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(genbatchno);
        }

        //farahin - popup senarai io
        public ActionResult DataSLP()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetLadangDetails = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();

            string ladangcode = "";
            ladangcode = GetLadangDetails.fld_LdgCode;

            var result = db.vw_SAPIODetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderByDescending(o => o.fld_DTModified);
            return View(result);
        }
        //by farahin
        //integration Maklumat Asas Rancangan
        public ActionResult _SLPDataPeringkat(CustomModels.CustMod_RequestDataSLP requestDataSLP, MasterModels.tbl_SAPLog tbl_SAPLog, MasterModels.tbl_IOSAP tbl_IOSAP)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            //string startDate = Convert.ToString(requestDataSLP.StartDate);
            //string endDate = Convert.ToString(requestDataSLP.EndDate);
            string compCode;
            //if (SyarikatID == 1)
            //{
            //    compCode = "1000";
            //}
            string ladangcode = "";
            string today = DateTime.Now.ToString("yyyyMMdd");
            string type = "", id = "", number = "", logno = "", logmsgno = "", message = "";
            string message1 = "", message2 = "", message3 = "", message4 = "", parameter = "", row = "", field = "", system = "";
            string kodComp = "", IndRanc = "", kodRanc = "", kodPkt = "", kodSubPkt = "", thnPembangunan = "", thnPembangunantanamsemula = "", busArea = "", IO1 = "", IO2 = "", IO3 = "", IO4 = "", IO5 = "", IO6 = "", tkhTanamMulaBhsl = "", PktPembgnn = "", tkhTahapPmbgnn = "", tkhMulaTanam = "", jnsTanaman = "", kodBlok = "", indJnsKiraan = "", jnsBlok = "", jnsKawasan = "", bilPenerokadlmBlok = "";
            decimal bilPeneroka = 0M, bilPenerokaPkt = 0M, jumLuasKeseluruhan = 0M, luasKwsnTanaman = 0M, luasKwsnBhasil = 0M, luasKwsnBhasilFelda = 0M, LuasKwsnBhasilPeneroka = 0M, jumLuasLotLdgFelda = 0M, jumLuasLotLdgPeneroka = 0M, bilKwsnUtama = 0M, bilKwsnRezab = 0M;

            var GetLadangDetails = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();

            ladangcode = GetLadangDetails.fld_LdgCode;
            compCode = GetLadangDetails.fld_CostCentre;

            var oClient = new SAPMD_FLQ.ZWS_OPMS_MASTERClient();
            var request = new SAPMD_FLQ.ZfmOpmsMaster();
            SAPMD_FLQ.ZfmOpmsMasterResponse iresponse = new SAPMD_FLQ.ZfmOpmsMasterResponse();

            SAPMD_FLQ.Zopmsslp[] zopmsslp = new SAPMD_FLQ.Zopmsslp[1];
            SAPMD_FLQ.Zopmsslp zopmsslps = new SAPMD_FLQ.Zopmsslp();

            SAPMD_FLQ.Bapiret2[] bapirtn = new SAPMD_FLQ.Bapiret2[1];
            SAPMD_FLQ.Bapiret2 bapiret2_return = new SAPMD_FLQ.Bapiret2();

            oClient.ClientCredentials.UserName.UserName = "FELDAOPMSRFC";
            oClient.ClientCredentials.UserName.Password = "@12345bnm";

            oClient.Open();

            try
            {
                request = new SAPMD_FLQ.ZfmOpmsMaster();

                request.DateBegin = "";
                request.DateEnd = "";
                request.SlpComp = compCode;
                request.SlpIndBegin = "3";
                request.SlpIndEnd = "9";
                request.SlpPktBegin = "001";
                request.SlpPktEnd = "999";
                request.SlpRanBegin = ladangcode;
                request.SlpRanEnd = ladangcode;
                request.ItSlp = zopmsslp;

                iresponse = oClient.ZfmOpmsMaster(request);

                bapirtn = iresponse.Return;
                zopmsslp = iresponse.ItSlp;

                if (zopmsslp.Count() - 1 >= 0)
                {
                    foreach (SAPMD_FLQ.Zopmsslp a in zopmsslp)
                    {

                        kodComp = a.Zbukrs;
                        IndRanc = a.Zkdrgi;
                        kodRanc = a.Zkdrgn;
                        kodPkt = a.Zkdpkt;
                        kodSubPkt = a.Zkdpk2;
                        thnPembangunan = a.Zthnpb;
                        thnPembangunantanamsemula = a.Zthpts;
                        busArea = a.Zgpcos;
                        IO1 = a.Zpsnd1;
                        IO2 = a.Zpsnd2;
                        IO3 = a.Zpsnd3;
                        IO4 = a.Zpsnd4;
                        IO5 = a.Zpsnd5;
                        IO6 = a.Zpsnd6;
                        tkhTanamMulaBhsl = a.Zthtmb;
                        PktPembgnn = a.Zpkpbg;
                        tkhTahapPmbgnn = a.Zththp;
                        tkhMulaTanam = a.Zthmtm;
                        jnsTanaman = a.Zjstnm;
                        kodBlok = a.Zkdblk;
                        indJnsKiraan = a.Zjenki;
                        jnsBlok = a.Zjsblk;
                        jnsKawasan = a.Zjskws;

                        bilPenerokadlmBlok = a.Zblpr3;
                        bilPeneroka = a.Zblpn2;
                        bilPenerokaPkt = a.Zblot2;
                        jumLuasKeseluruhan = a.Zjmltf;
                        luasKwsnTanaman = a.Zlkwtn;
                        luasKwsnBhasil = a.Zlskbh;
                        luasKwsnBhasilFelda = a.Zlskbf;
                        LuasKwsnBhasilPeneroka = a.Zlskbp;
                        jumLuasLotLdgFelda = a.Zldltf;
                        jumLuasLotLdgPeneroka = a.Zldltp;
                        bilKwsnUtama = a.Zblkwu;
                        bilKwsnRezab = a.Zblkwr;

                        //save db
                        var getIODetails = db.tbl_IOSAP.Where(x => x.fld_IOcode == IO1).FirstOrDefault();

                        if (getIODetails == null)
                        {
                            tbl_IOSAP = new tbl_IOSAP();

                            tbl_IOSAP.fld_IOcode = IO1;
                            tbl_IOSAP.fld_PktCode = kodPkt;
                            tbl_IOSAP.fld_SubPktCode = kodSubPkt;
                            tbl_IOSAP.fld_LuasPkt = jumLuasKeseluruhan;
                            tbl_IOSAP.fld_LuasKawTnmn = luasKwsnTanaman;
                            tbl_IOSAP.fld_LuasKawBerhasil = luasKwsnBhasil;
                            tbl_IOSAP.fld_LdgIndicator = IndRanc;
                            tbl_IOSAP.fld_LdgKod = kodRanc;
                            //tbl_IOSAP.fld_StatusUsed = "NULL";
                            tbl_IOSAP.fld_JnsLot = "";
                            tbl_IOSAP.fld_NegaraID = Convert.ToInt32(NegaraID);
                            tbl_IOSAP.fld_SyarikatID = Convert.ToInt32(SyarikatID);
                            tbl_IOSAP.fld_WilayahID = Convert.ToInt32(WilayahID);
                            tbl_IOSAP.fld_LadangID = Convert.ToInt32(LadangID);
                            tbl_IOSAP.fld_Deleted = false;
                            tbl_IOSAP.fld_DTCreated = DateTime.Now;
                            tbl_IOSAP.fld_DTModified = DateTime.Now;
                            tbl_IOSAP.fld_thnPembangunan = thnPembangunan;
                            tbl_IOSAP.fld_thnPembangunantanamsemula = thnPembangunantanamsemula;
                            tbl_IOSAP.fld_busArea = busArea;
                            tbl_IOSAP.fld_IO2 = IO2;
                            tbl_IOSAP.fld_IO3 = IO3;
                            tbl_IOSAP.fld_IO4 = IO4;
                            tbl_IOSAP.fld_IO5 = IO5;
                            tbl_IOSAP.fld_IO6 = IO6;
                            if (tkhTanamMulaBhsl != "0000-00-00")
                            {
                                if (tkhTanamMulaBhsl != "0 - -  ")
                                {

                                }
                                else
                                {
                                    tbl_IOSAP.fld_tkhTanamMulaBhsl = Convert.ToDateTime(tkhTanamMulaBhsl);
                                }
                            }
                            else
                            {

                            }
                            if (tkhTahapPmbgnn != "0000-00-00")
                            {
                                if (tkhTanamMulaBhsl != "0 - -  ")
                                {

                                }
                                else
                                {
                                    tbl_IOSAP.fld_tkhTahapPmbgnn = Convert.ToDateTime(tkhTahapPmbgnn);
                                }
                            }
                            else
                            {

                            }

                            if (tkhMulaTanam != "0000-00-00")
                            {
                                if (tkhTanamMulaBhsl != "0 - -  ")
                                {

                                }
                                else
                                {
                                    tbl_IOSAP.fld_tkhMulaTanam = Convert.ToDateTime(tkhMulaTanam);
                                }
                            }
                            else
                            {

                            }



                            tbl_IOSAP.fld_jnsTanaman = jnsTanaman;
                            tbl_IOSAP.fld_kodBlok = kodBlok;
                            tbl_IOSAP.fld_indJnsKiraan = indJnsKiraan;
                            tbl_IOSAP.fld_jnsBlok = jnsBlok;
                            tbl_IOSAP.fld_jnsKawasan = jnsKawasan;
                            tbl_IOSAP.fld_bilPenerokadlmBlok = Convert.ToInt32(bilPenerokadlmBlok);
                            tbl_IOSAP.fld_bilPeneroka = Convert.ToInt32(bilPeneroka);
                            tbl_IOSAP.fld_bilPenerokaPkt = Convert.ToInt32(bilPenerokaPkt);
                            tbl_IOSAP.fld_luasKwsnBhasilFelda = Convert.ToDecimal(luasKwsnBhasilFelda);
                            tbl_IOSAP.fld_LuasKwsnBhasilPeneroka = Convert.ToDecimal(LuasKwsnBhasilPeneroka);
                            tbl_IOSAP.fld_jumLuasLotLdgFelda = Convert.ToDecimal(jumLuasLotLdgFelda);
                            tbl_IOSAP.fld_jumLuasLotLdgPeneroka = Convert.ToDecimal(jumLuasLotLdgPeneroka);
                            tbl_IOSAP.fld_bilKwsnUtama = Convert.ToInt32(bilKwsnUtama);
                            tbl_IOSAP.fld_bilKwsnRezab = Convert.ToInt32(bilKwsnRezab);

                            db.tbl_IOSAP.Add(tbl_IOSAP);
                            db.SaveChanges();
                            db.Entry(tbl_IOSAP).State = EntityState.Detached;


                            tbl_SAPLog.fld_type = "S";
                            tbl_SAPLog.fld_message = "IO inbound success";
                            tbl_SAPLog.fld_msg1 = IO1;
                            tbl_SAPLog.fld_system = "SLP IO";
                            tbl_SAPLog.fld_logDate = DateTime.Now;
                            tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                            tbl_SAPLog.fld_negaraID = "1";
                            tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                            db.tbl_sapLog.Add(tbl_SAPLog);
                            db.SaveChanges();
                        }


                    }

                    tbl_SAPLog.fld_type = "S";
                    tbl_SAPLog.fld_message = "IO inbound success";
                    tbl_SAPLog.fld_row = Convert.ToString(iresponse.ItSlp.Count());
                    tbl_SAPLog.fld_system = "SLP IO";
                    tbl_SAPLog.fld_logDate = DateTime.Now;
                    tbl_SAPLog.fld_msg4 = getuserid + "-" + User.Identity.Name;
                    tbl_SAPLog.fld_negaraID = "1";
                    tbl_SAPLog.fld_syarikatID = Convert.ToString(SyarikatID);

                    db.tbl_sapLog.Add(tbl_SAPLog);
                    db.SaveChanges();
                }

                if (iresponse.Return.Count() - 1 >= 1)
                {
                    foreach (SAPMD_FLQ.Bapiret2 a in bapirtn)
                    {
                        type = a.Type;
                        id = a.Id;
                        number = a.Number;
                        logno = a.LogNo;
                        logmsgno = a.LogMsgNo;
                        message = a.Message;
                        message1 = a.MessageV1;
                        message2 = a.MessageV2;
                        message3 = a.MessageV3;
                        message4 = a.MessageV4;
                        parameter = a.Parameter;
                        row = a.Row.ToString();
                        field = a.Field;
                        system = a.System;

                        //save dlm db

                        tbl_SAPLog.fld_type = type;
                        tbl_SAPLog.fld_number = number;
                        tbl_SAPLog.fld_id = id;
                        tbl_SAPLog.fld_logno = logno;
                        tbl_SAPLog.fld_logmsgno = logmsgno;
                        tbl_SAPLog.fld_message = message;
                        tbl_SAPLog.fld_msg1 = message1;
                        tbl_SAPLog.fld_msg2 = message2;
                        tbl_SAPLog.fld_msg3 = message3;
                        tbl_SAPLog.fld_msg4 = message4;
                        tbl_SAPLog.fld_parameter = parameter;
                        tbl_SAPLog.fld_row = row;
                        tbl_SAPLog.fld_field = field;
                        tbl_SAPLog.fld_system = system;

                        tbl_SAPLog.fld_negaraID = NegaraID.ToString();
                        tbl_SAPLog.fld_syarikatID = SyarikatID.ToString();
                        tbl_SAPLog.fld_logDate = DateTime.Now;

                        db.tbl_sapLog.Add(tbl_SAPLog);
                        db.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                oClient.Close();
            }



            var result = db.vw_SAPIODetails.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).OrderByDescending(o => o.fld_DTModified);
            return View(result);
        }

        //Modified by Shazana 22/7/2023
        public ActionResult LevelsSubPktUpdateKesukaran(string Kodpkt = "", String rolename = "", int page = 1, string sort = "fld_KodHargaKesukaran", string sortdir = "ASC")
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<Models.tbl_PktHargaKesukaran>();
            records.Content = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_PktUtama == Kodpkt && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_PktUtama == Kodpkt && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            //Added by Shazana 22/7/2023
            ViewBag.rolename = rolename;
            return View(records);
        }

        public ActionResult RemoveKesukaran(string fld_KodPkt, string KodHargaKesukaran)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            int flag = 0;

            tbl_PktHargaKesukaran PktHargaKesukaran = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_PktUtama == fld_KodPkt && x.fld_KodHargaKesukaran == KodHargaKesukaran && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            dbr.tbl_PktHargaKesukaran.Remove(PktHargaKesukaran);
            dbr.SaveChanges();
            flag = 1;
            //return Json(flag);
            return RedirectToAction("LevelsInfo");
        }

        public ActionResult SaveKesukaran(string fld_JenisHargaKesukaran, string fld_TahapHargaKesukaran, string fld_PktUtama)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            Models.tbl_KawTidakBerhasil tbl_KawTidakBerhasil = new Models.tbl_KawTidakBerhasil();
            Models.tbl_PktHargaKesukaran tbl_PktHargaKesukaran = new Models.tbl_PktHargaKesukaran();

            int flag = 0;

            var JenisKesukaranDetails = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == fld_JenisHargaKesukaran && x.fldOptConfFlag2.Contains("HargaKesukaran") && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            var TahapKesukaranDetails = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag2 != "HargaKesukaran" && x.fldOptConfFlag1 == JenisKesukaranDetails.fldOptConfFlag1 && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfValue == fld_TahapHargaKesukaran).FirstOrDefault();

            //Commented by Shazana 22/7/2023
            //var checkKwsn = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_JenisHargaKesukaran == fld_JenisHargaKesukaran && x.fld_PktUtama == fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

            //Added by Shazana 22/7/2023
            var checkKwsn = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_KodJenisHargaKesukaran == JenisKesukaranDetails.fldOptConfFlag3 && x.fld_PktUtama == fld_PktUtama && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();

            if (checkKwsn == null && JenisKesukaranDetails != null && TahapKesukaranDetails != null)
            {
                tbl_PktHargaKesukaran.fld_PktUtama = fld_PktUtama;
                tbl_PktHargaKesukaran.fld_KodJenisHargaKesukaran = JenisKesukaranDetails.fldOptConfFlag3; //Modify by Shazana 13/6/2023
                tbl_PktHargaKesukaran.fld_JenisHargaKesukaran = fld_JenisHargaKesukaran;
                tbl_PktHargaKesukaran.fld_KodHargaKesukaran = TahapKesukaranDetails.fldOptConfValue;
                tbl_PktHargaKesukaran.fld_KeteranganHargaKesukaran = TahapKesukaranDetails == null ? "" : TahapKesukaranDetails.fldOptConfDesc;
                tbl_PktHargaKesukaran.fld_HargaKesukaran = TahapKesukaranDetails == null ? 0 : Convert.ToDecimal(TahapKesukaranDetails.fldOptConfFlag2);
                tbl_PktHargaKesukaran.fld_NegaraID = NegaraID;
                tbl_PktHargaKesukaran.fld_SyarikatID = SyarikatID;
                tbl_PktHargaKesukaran.fld_WilayahID = WilayahID;
                tbl_PktHargaKesukaran.fld_LadangID = LadangID;
                tbl_PktHargaKesukaran.fld_CreatedBy = getuserid.ToString();
                tbl_PktHargaKesukaran.fld_CreatedDate = DateTime.Now;

                dbr.tbl_PktHargaKesukaran.Add(tbl_PktHargaKesukaran);
                dbr.SaveChanges();
                flag = 1;
            }
            else
            {
                //data already exist
                flag = 2;
            }

            // return RedirectToAction("LevelsInfos",new { JnsPkt = "1" });
            //return Json(new { success = true, msg = GlobalResEstate.msgUpdate, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
            return Json(flag);
        }


        //Added by Shazana 13/6/2023
        public JsonResult GetHargaKesukaranlist()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> HargaKesukaranlist = new List<SelectListItem>();


            HargaKesukaranlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag2 == "HargaKesukaran" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => new SelectListItem { Value = s.fldOptConfFlag1, Text = s.fldOptConfValue + " - " + s.fldOptConfDesc }), "Value", "Text").ToList();

            return Json(HargaKesukaranlist);
        }
        public JsonResult GetTahapKesukaranlist(string JenisKesukaran)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<SelectListItem> TahapKesukaranlist = new List<SelectListItem>();


            TahapKesukaranlist = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag2 != "HargaKesukaran" && x.fldOptConfFlag1 == JenisKesukaran && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).OrderBy(x => x.fldOptConfDesc).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " - " + s.fldOptConfFlag2 }), "Value", "Text").ToList();

            return Json(TahapKesukaranlist);
        }

        //fatin added - 16/06/2023
        public ActionResult WorkerPrmtPsprt(string FreeText, string StartDate, string EndDate, string sortOrder)
        {

            ViewBag.WorkerPrmtPsprt = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);


            DateTime DT = new DateTime();
            DT = timezone.gettimezone();

            int Month = DT.AddMonths(-1).Month;
            int Year = DT.Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            int RangeYear = DT.Year + 1;

            var YearList1 = new List<SelectListItem>();
            for (var i = Year; i <= RangeYear; i++)
            {
                if (i == DT.Year)
                {
                    YearList1.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    YearList1.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            var MonthList1 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");
            var MonthList_2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");

            ViewBag.YearList = YearList1;
            ViewBag.MonthList = MonthList1;
            ViewBag.MonthList2 = MonthList_2;

            DateTime? StartDate1 = Convert.ToDateTime(StartDate);
            DateTime? EndDate1 = Convert.ToDateTime(EndDate);

            //if (StartDate == null || FreeText == null)
            //{
            //    ViewBag.Message = "Sila pilih tarikh";

            //}

            if (string.IsNullOrEmpty(FreeText))
            {

                var WkrDataInfo = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && (x.fld_T2prmt >= StartDate1 && x.fld_T2prmt <= EndDate1) && (x.fld_T2pspt >= StartDate1 && x.fld_T2pspt <= EndDate1) && x.fld_Kdrkyt != "MA").OrderBy(x => x.fld_Nopkj).ToList();
                return View(WkrDataInfo);
            }
            else
            {
                var WkrDataInfo = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Kdrkyt != "MA" && (x.fld_T2prmt >= StartDate1 && x.fld_T2prmt <= EndDate1) && (x.fld_T2pspt >= StartDate1 && x.fld_T2pspt <= EndDate1) && (x.fld_Nopkj.Contains(FreeText) || x.fld_Nama.Contains(FreeText) || x.fld_Nokp.Contains(FreeText))).OrderBy(x => x.fld_Nopkj).ToList();
                return View(WkrDataInfo);
            }


            ViewBag.fld_Nopkj = string.IsNullOrEmpty(sortOrder) ? "fld_Nopkj" : "";
            ViewBag.fld_Nokp = sortOrder == "fld_Nokp" ? "fld_Nokp desc" : "fld_Nokp";
            ViewBag.fld_Nama = sortOrder == "fld_Nama" ? "fld_Nama desc" : "fld_Nama";

        }

        public ActionResult _WorkerPrmtPsprt(string FreeText, string StartDate, string EndDate, string sortOrder, string print)
        {
            ViewBag.WorkerPrmtPsprt = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime DT = new DateTime();
            DT = timezone.gettimezone();

            int Month = DT.AddMonths(-1).Month;
            int Year = DT.Year - int.Parse(GetConfig.GetData("yeardisplay")) + 1;
            int RangeYear = DT.Year + 1;

            var YearList1 = new List<SelectListItem>();
            for (var i = Year; i <= RangeYear; i++)
            {
                if (i == DT.Year)
                {
                    YearList1.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    YearList1.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            var MonthList1 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");
            var MonthList_2 = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "monthlist" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID), "fldOptConfValue", "fldOptConfDesc");

            ViewBag.YearList = YearList1;
            ViewBag.MonthList = MonthList1;
            ViewBag.MonthList2 = MonthList_2;

            //if (WilayahIDList == null && LadangIDList == null)
            //{
            //    ViewBag.Message = GlobalResEstate.msgChooseRegionEstateMonthYear;

            //}
            if (StartDate == null && EndDate == null && FreeText == null)
            {
                ViewBag.Message = "Sila pilih tarikh atau passport/id/nama pekerja";

            }

            DT = timezone.gettimezone();
            DateTime StartDate1 = DT.Date;
            string Start = StartDate;
            if (Start != null)
            {
                StartDate1 = DateTime.ParseExact(Start, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }

            DateTime EndDate1 = DT.Date;
            string End = EndDate;
            if (End != null)
            {
                EndDate1 = DateTime.ParseExact(End, @"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }


            if (string.IsNullOrEmpty(FreeText))
            {

                var WkrDataInfo = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && (x.fld_T2prmt >= StartDate1 && x.fld_T2prmt <= EndDate1) && (x.fld_T2pspt >= StartDate1 && x.fld_T2pspt <= EndDate1) && x.fld_Kdrkyt != "MA").OrderBy(x => x.fld_Nopkj).ToList();
                return View(WkrDataInfo);


            }
            //else if (!string.IsNullOrEmpty(FreeText))
            //{

            //    var WkrDataInfo = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && (x.fld_Nopkj.Contains(FreeText) || x.fld_Nama.Contains(FreeText) || x.fld_Nokp.Contains(FreeText)) && x.fld_Kdrkyt != "MA").OrderBy(x => x.fld_Nopkj).ToList();
            //    return View(WkrDataInfo);


            //}
            else
            {
                var WkrDataInfo = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Kdrkyt != "MA" && (x.fld_Nopkj.Contains(FreeText) || x.fld_Nama.Contains(FreeText) || x.fld_Nokp.Contains(FreeText))).OrderBy(x => x.fld_Nopkj).ToList();
                return View(WkrDataInfo);
            }

            ViewBag.fld_Nopkj = string.IsNullOrEmpty(sortOrder) ? "fld_Nopkj" : "";
            ViewBag.fld_Nokp = sortOrder == "fld_Nokp" ? "fld_Nokp desc" : "fld_Nokp";
            ViewBag.fld_Nama = sortOrder == "fld_Nama" ? "fld_Nama desc" : "fld_Nama";

        }

        public ActionResult PermitUpdate(Guid? id)
        {
            ViewBag.WorkerPrmtPsprt = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime DT = new DateTime();
            DT = timezone.gettimezone();

            if (id == null)
            {
                return RedirectToAction("WorkerPrmtPsprt");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_UniqueID == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();

            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerPrmtPsprt");
            }

            return PartialView(tbl_Pkjmast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PermitUpdate(Models.tbl_LbrPrmtPsprtUpdate tbl_LbrPrmtPsprtUpdate, Models.tbl_Pkjmast tbl_Pkjmast)
        {
            //fld_PurposeIndicator = 1 is for permit
            ViewBag.WorkerPrmtPsprt = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime DT = new DateTime();
            DT = timezone.gettimezone();

            try
            {
                var GetExistingPermit = dbr.tbl_LbrPrmtPsprtUpdate.Where(x => x.fld_LbrRefID == tbl_LbrPrmtPsprtUpdate.fld_LbrRefID && (x.fld_NewPrmtPsprtNo == tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo || x.fld_OldPrmtPsprtNo == tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo) && x.fld_PurposeIndicator == 1 && x.fld_Deleted == false).Count();
                var GetWorkerDetails = dbr.tbl_Pkjmast.Find(tbl_LbrPrmtPsprtUpdate.fld_LbrRefID);
                if (GetExistingPermit == 0)
                {
                    tbl_LbrPrmtPsprtUpdate LbrPrmtPsprtUpdate = new tbl_LbrPrmtPsprtUpdate();
                    LbrPrmtPsprtUpdate.fld_LbrRefID = tbl_LbrPrmtPsprtUpdate.fld_LbrRefID;
                    LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo;
                    LbrPrmtPsprtUpdate.fld_NewPrmtPsrtEndDT = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsrtEndDT;
                    LbrPrmtPsprtUpdate.fld_OldPrmtPsprtNo = tbl_LbrPrmtPsprtUpdate.fld_OldPrmtPsprtNo;
                    LbrPrmtPsprtUpdate.fld_OldPrmtPsrtEndDT = GetWorkerDetails.fld_T2prmt;
                    LbrPrmtPsprtUpdate.fld_CreatedBy = getuserid;
                    LbrPrmtPsprtUpdate.fld_CreatedDT = DT;
                    LbrPrmtPsprtUpdate.fld_PurposeIndicator = 1;
                    LbrPrmtPsprtUpdate.fld_LadangID = GetWorkerDetails.fld_LadangID;
                    LbrPrmtPsprtUpdate.fld_NegaraID = GetWorkerDetails.fld_NegaraID;
                    LbrPrmtPsprtUpdate.fld_SyarikatID = GetWorkerDetails.fld_SyarikatID;
                    LbrPrmtPsprtUpdate.fld_WilayahID = GetWorkerDetails.fld_WilayahID;
                    LbrPrmtPsprtUpdate.fld_Deleted = false;
                    dbr.tbl_LbrPrmtPsprtUpdate.Add(LbrPrmtPsprtUpdate);
                    dbr.SaveChanges();


                    GetWorkerDetails.fld_Prmtno = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo;
                    GetWorkerDetails.fld_T2prmt = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsrtEndDT;
                    //GetWorkerDetails.fld_ActionBy = GetUserID;
                    //GetWorkerDetails.fld_ActionDate = DT;
                    dbr.Entry(GetWorkerDetails).State = EntityState.Modified;
                    dbr.SaveChanges();


                    ModelState.AddModelError("", "Maklumat berjaya dikemaskini");
                    ViewBag.MsgColor = "color: green";
                }
                else
                {
                    ModelState.AddModelError("", "Permit telah diperbaharui");
                    ViewBag.MsgColor = "color: orange";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Sila isi ruang bertanda *");
                ViewBag.MsgColor = "color: red";
            }

            var WkrDataInfo = dbr.tbl_Pkjmast.Where(w => w.fld_UniqueID == tbl_LbrPrmtPsprtUpdate.fld_LbrRefID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            return View(WkrDataInfo);
        }

        public ActionResult _WorkerPermitDetail(Guid WorkerID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            return View(dbr.vw_LbrPrmtPsprtUpdate.Where(x => x.fld_LbrRefID == WorkerID && x.fld_PurposeIndicator == 1).ToList());
        }

        public ActionResult PassportUpdate(Guid? id)
        {
            ViewBag.WorkerPrmtPsprt = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime DT = new DateTime();
            DT = timezone.gettimezone();

            if (id == null)
            {
                return RedirectToAction("WorkerPrmtPsprt");
            }
            Models.tbl_Pkjmast tbl_Pkjmast = dbr.tbl_Pkjmast.Where(w => w.fld_UniqueID == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();

            if (tbl_Pkjmast == null)
            {
                return RedirectToAction("WorkerPrmtPsprt");
            }

            return PartialView("PassportUpdate", tbl_Pkjmast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PassportUpdate(tbl_LbrPrmtPsprtUpdate tbl_LbrPrmtPsprtUpdate)
        {
            //fld_PurposeIndicator = 1 is for permit
            ViewBag.WorkerPrmtPsprt = "class = active";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            DateTime DT = new DateTime();
            DT = timezone.gettimezone();

            try
            {
                var GetExistingPassport = dbr.tbl_LbrPrmtPsprtUpdate.Where(x => x.fld_LbrRefID == tbl_LbrPrmtPsprtUpdate.fld_LbrRefID && (x.fld_NewPrmtPsprtNo == tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo || x.fld_OldPrmtPsprtNo == tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo) && x.fld_PurposeIndicator == 2 && x.fld_Deleted == false).Count();
                var GetWorkerDetails = dbr.tbl_Pkjmast.Find(tbl_LbrPrmtPsprtUpdate.fld_LbrRefID);
                if (GetExistingPassport == 0)
                {
                    tbl_LbrPrmtPsprtUpdate LbrPrmtPsprtUpdate = new tbl_LbrPrmtPsprtUpdate();
                    LbrPrmtPsprtUpdate.fld_LbrRefID = tbl_LbrPrmtPsprtUpdate.fld_LbrRefID;
                    LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo;
                    LbrPrmtPsprtUpdate.fld_NewPrmtPsrtEndDT = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsrtEndDT;
                    LbrPrmtPsprtUpdate.fld_OldPrmtPsprtNo = tbl_LbrPrmtPsprtUpdate.fld_OldPrmtPsprtNo;
                    LbrPrmtPsprtUpdate.fld_OldPrmtPsrtEndDT = GetWorkerDetails.fld_T2pspt;
                    LbrPrmtPsprtUpdate.fld_CreatedBy = getuserid;
                    LbrPrmtPsprtUpdate.fld_CreatedDT = DT;
                    LbrPrmtPsprtUpdate.fld_PurposeIndicator = 2;
                    LbrPrmtPsprtUpdate.fld_LadangID = GetWorkerDetails.fld_LadangID;
                    LbrPrmtPsprtUpdate.fld_NegaraID = GetWorkerDetails.fld_NegaraID;
                    LbrPrmtPsprtUpdate.fld_SyarikatID = GetWorkerDetails.fld_SyarikatID;
                    LbrPrmtPsprtUpdate.fld_WilayahID = GetWorkerDetails.fld_WilayahID;
                    LbrPrmtPsprtUpdate.fld_Deleted = false;
                    dbr.tbl_LbrPrmtPsprtUpdate.Add(LbrPrmtPsprtUpdate);
                    dbr.SaveChanges();


                    GetWorkerDetails.fld_Nokp = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsprtNo;
                    GetWorkerDetails.fld_T2pspt = tbl_LbrPrmtPsprtUpdate.fld_NewPrmtPsrtEndDT;
                    //GetWorkerDetails.fld_ActionBy = GetUserID;
                    //GetWorkerDetails.fld_ActionDate = DT;
                    dbr.Entry(GetWorkerDetails).State = EntityState.Modified;
                    dbr.SaveChanges();

                    ModelState.AddModelError("", "Maklumat berjaya dikemaskini");
                    ViewBag.MsgColor = "color: green";
                }
                else
                {
                    ModelState.AddModelError("", "Passport telah diperbaharui");
                    ViewBag.MsgColor = "color: orange";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Sila isi ruang bertanda *");
                ViewBag.MsgColor = "color: red";
            }

           
            var WkrDataInfo = dbr.tbl_Pkjmast.Where(w => w.fld_UniqueID == tbl_LbrPrmtPsprtUpdate.fld_LbrRefID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID).FirstOrDefault();
            return View(WkrDataInfo);
        }

        public ActionResult _WorkerPassportDetail(Guid WorkerID)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            return View(dbr.vw_LbrPrmtPsprtUpdate.Where(x => x.fld_LbrRefID == WorkerID && x.fld_PurposeIndicator == 2).ToList());
        }

        //end

    }
}