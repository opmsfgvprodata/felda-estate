using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVC_SYSTEM.Class;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public partial class BasicInfoController : Controller
    {
        public ActionResult CCLevelsInfo(string JnsPkt = "1")
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

        //comment by fatin - 11/04/2023
        //public ActionResult CCLevelsInfoPkt(string JnsPkt = "1", int page = 1, string sort = "fld_PktUtama", string sortdir = "ASC", string status = "false")
        public ActionResult CCLevelsInfoPkt(string JnsPkt = "1", int page = 1, string sort = "fld_CreateDate", string sortdir = "DESC", string status = "false") //fatin added - 11/04/2023
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Viewing dbview = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass);

            int pageSize = int.Parse(GetConfig.GetData("paging"));
            var records = new PagedList<ViewingModels.tbl_PktUtama>();
            var boolStatus = bool.Parse(status);
            records.Content = dbview.tbl_PktUtama.Where(x => x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_PktUtama.Where(x => x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return PartialView(records);
        }

        public ActionResult CCLevelsInfoSubPkt(string JnsPkt = "2", int page = 1, string sort = "fld_Pkt", string sortdir = "ASC", string status = "false")
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
            records.Content = dbview.tbl_SubPkt.Where(x => x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_SubPkt.Where(x => x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        public ActionResult CCLevelsInfoBlok(string JnsPkt = "3", int page = 1, string sort = "fld_Blok", string sortdir = "ASC", string status = "false")
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
            records.Content = dbview.tbl_Blok.Where(x => x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus)
                   .OrderBy(sort + " " + sortdir)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();

            records.TotalRecords = dbview.tbl_Blok.Where(x => x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Deleted == boolStatus).Count();
            records.CurrentPage = page;
            records.PageSize = pageSize;
            return View(records);
        }

        public ActionResult CCLevelsPktCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
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
            IOlist = new SelectList(db.tbl_CCSAP.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false), "fld_CstCnter", "fld_CstCnter").ToList();
            IOlist.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            JnsLotList = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsLot" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false), "fldOptConfValue", "fldOptConfDesc").ToList();
            JnsLotList.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));

            ViewBag.fld_JnsTnmn = JnsTnmn;
            ViewBag.fld_StatusTnmn = StatusTnmn;
            ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahPktUtama = TahapKesukaranMemunggah;//added by faeza 18.08.2021
            ViewBag.fld_IOcode = IOlist;
            ViewBag.fld_JnsLot = JnsLotList;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CCLevelsPktCreate(Models.tbl_PktUtama tbl_PktUtama)
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

                        var checkdata = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == tbl_PktUtama.fld_PktUtama && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                        if (checkdata == null)
                        {
                            tbl_PktUtama.fld_NamaPktUtama = tbl_PktUtama.fld_NamaPktUtama.ToUpper();
                            tbl_PktUtama.fld_LadangID = LadangID;
                            tbl_PktUtama.fld_WilayahID = WilayahID;
                            tbl_PktUtama.fld_SyarikatID = SyarikatID;
                            tbl_PktUtama.fld_NegaraID = NegaraID;
                            tbl_PktUtama.fld_CreateDate = DateTime.Now;
                            tbl_PktUtama.fld_Deleted = false;
                            tbl_PktUtama.fld_SAPType = "CC";
                            dbr.tbl_PktUtama.Add(tbl_PktUtama);
                            dbr.SaveChanges();

                            string RequestForm = Request.Form["listCount"];
                            if (RequestForm != null && RequestForm != "")
                            {
                                int listCount = Convert.ToInt32(Request.Form["listCount"]);
                                if (listCount >= 1)
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

        public ActionResult CCLevelsPktUpdate(string id)
        {
            GetStatus GetStatus = new GetStatus();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            Models.tbl_PktUtama tbl_PktUtama = dbr.tbl_PktUtama.Where(w => w.fld_PktUtama == id && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_LadangID == LadangID && w.fld_Deleted == false).FirstOrDefault();

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

            ViewBag.fld_KesukaranMenuaiPktUtama = TahapKesukaranMenuai;
            ViewBag.fld_KesukaranMembajaPktUtama = TahapKesukaranMembaja;
            ViewBag.fld_KesukaranMemunggahPktUtama = TahapKesukaranMemunggah;//added by faeza 18.08.2021
            ViewBag.fld_JnsKaw1 = Kawasanlist;
            ViewBag.fld_JnsLot = JnsLotList;
            return PartialView(tbl_PktUtama);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CCLevelsPktUpdate(string id, Models.tbl_PktUtama tbl_PktUtama)
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

        public ActionResult CCLevelsPktDelete(string id, string status)
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

        [HttpPost, ActionName("CCLevelsPktDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CCLevelsPktDeleteConfirmed(string id, string status)
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

        public ActionResult CCLevelsSubPktCreate()
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

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
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
        public ActionResult CCLevelsSubPktCreate(Models.tbl_SubPkt tbl_SubPkt)
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
                        tbl_SubPkt.fld_SAPType = "CC";
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

        public ActionResult CCLevelsSubPktUpdate(string id)
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
            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            TahapKesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_SubPkt.fld_KesukaranMenuaiPkt).ToList();
            TahapKesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_SubPkt.fld_KesukaranMembajaPkt).ToList();
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
        public ActionResult CCLevelsSubPktUpdate(string id, Models.tbl_SubPkt tbl_SubPkt)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            try
            {
                var getdata = dbr.tbl_SubPkt.Where(w => w.fld_Pkt == id && w.fld_LadangID == LadangID && w.fld_WilayahID == WilayahID && w.fld_SyarikatID == SyarikatID && w.fld_NegaraID == NegaraID && w.fld_Deleted == false).FirstOrDefault();
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
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }
        }

        public ActionResult CCLevelsSubPktDelete(string id, string status)
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

        [HttpPost, ActionName("CCLevelsSubPktDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CCLevelsSubPktDeleteConfirmed(string id, string status)
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
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
                else
                {
                    var boolStatus = bool.Parse(status);
                    tbl_SubPkt.fld_Deleted = boolStatus;
                    dbr.Entry(tbl_SubPkt).State = EntityState.Modified;
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = "Data tidak dijumpai", status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = "Data berjaya dikemaskini", status = "danger", checkingdata = "1" });
            }

        }

        public ActionResult CCLevelsSubPktUpdateKwsn(string Kodpkt = "", int page = 1, string sort = "fld_JnsKaw", string sortdir = "ASC")
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

        public ActionResult CCLevelsBlokCreate()
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            List<SelectListItem> PktUtama = new List<SelectListItem>();
            List<SelectListItem> Pkt = new List<SelectListItem>();
            List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> KesukaranMemunggah = new List<SelectListItem>();//added by faeza 18.08.2021

            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
            KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text").ToList();
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
        public ActionResult CCLevelsBlokCreate(Models.tbl_Blok tbl_Blok)
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
                    var checkdata = dbr.tbl_Blok.Where(x => x.fld_Blok == tbl_Blok.fld_Blok && x.fld_KodPkt == tbl_Blok.fld_KodPkt && x.fld_KodPktutama == tbl_Blok.fld_KodPktutama && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_WilayahID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID).FirstOrDefault();
                    if (checkdata == null && tbl_Blok.fld_Blok != "" && tbl_Blok.fld_NamaBlok != "")
                    {
                        tbl_Blok.fld_NamaBlok = tbl_Blok.fld_NamaBlok.ToUpper();
                        tbl_Blok.fld_LadangID = LadangID;
                        tbl_Blok.fld_WilayahID = WilayahID;
                        tbl_Blok.fld_SyarikatID = SyarikatID;
                        tbl_Blok.fld_NegaraID = NegaraID;
                        tbl_Blok.fld_CreateDate = DateTime.Now;
                        tbl_Blok.fld_SAPType = "CC";
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

        public ActionResult CCLevelsBlokUpdate(string id)
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
            List<SelectListItem> KesukaranMenuai = new List<SelectListItem>();
            List<SelectListItem> KesukaranMembaja = new List<SelectListItem>();
            List<SelectListItem> KesukaranMemunggah = new List<SelectListItem>();//added by faeza 18.08.2021

            //modified by faeza 18.08.2021
            PktUtama = new SelectList(dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_PktUtama).Select(s => new SelectListItem { Value = s.fld_PktUtama, Text = s.fld_PktUtama + " - " + s.fld_NamaPktUtama }).Distinct(), "Value", "Text").ToList();
            PktUtama.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            Pkt = new SelectList(dbr.tbl_SubPkt.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_Deleted == false).OrderBy(o => o.fld_Pkt).Select(s => new SelectListItem { Value = s.fld_Pkt, Text = s.fld_Pkt + " - " + s.fld_NamaPkt }).Distinct(), "Value", "Text").ToList();
            Pkt.Insert(0, (new SelectListItem { Text = GlobalResEstate.lblChoose, Value = "0" }));
            KesukaranMenuai = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMenuai" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_Blok.fld_KesukaranMenuaiBlok).ToList();
            KesukaranMembaja = new SelectList(db.tblOptionConfigsWebs.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldOptConfFlag1 == "KesukaranMembaja" && x.fldDeleted == false).OrderBy(o => o.fldOptConfValue).Select(s => new SelectListItem { Value = s.fldOptConfValue, Text = s.fldOptConfDesc + " (RM" + s.fldOptConfFlag2 + ")" }), "Value", "Text", tbl_Blok.fld_KesukaranMembajaBlok).ToList();
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
        public ActionResult CCLevelsBlokUpdate(string id, Models.tbl_Blok tbl_Blok)
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

        public ActionResult CCLevelsBlokDelete(string id, string status)
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

        [HttpPost, ActionName("CCLevelsBlokDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult CCLevelsBlokDeleteConfirmed(string id, string status)
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
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }
                else
                {
                    var boolStatus = bool.Parse(status);
                    tbl_Blok.fld_Deleted = boolStatus;
                    dbr.Entry(tbl_Blok).State = EntityState.Modified;
                    dbr.SaveChanges();
                    return Json(new { success = true, msg = GlobalResEstate.msgDelete2, status = "success", checkingdata = "0", method = "2", btn = "btnSrch" });
                }

            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                return Json(new { success = false, msg = GlobalResEstate.msgError, status = "danger", checkingdata = "1" });
            }

        }

        public JsonResult GetCCPktUtama(string JnsTnmn, string StatusTnmn, string LotPeneroka)
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
                codetnmn = JnsTnmn + StatusTnmn + LotPeneroka;
                var getpkt = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_SAPType == "CC" && x.fld_LadangID == LadangID && x.fld_PktUtama.Contains(codetnmn)).Select(s => s.fld_PktUtama).Distinct().OrderByDescending(s => s).FirstOrDefault();

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
        [HttpGet]
        public JsonResult GetSubList(string menuVal)
        {
            List<SelectListItem> subList = new List<SelectListItem>();

            var fetchSubMenu = db.tblMenuLists.Where(x => x.fld_Val == menuVal).Select(s => s.fld_Sub).FirstOrDefault();

            if (fetchSubMenu != null)
            {
                subList = new SelectList(db.tblSubMenuLists.Where(x => x.fld_Flag == fetchSubMenu && x.fldDeleted == false).OrderBy(o => o.fld_Desc).Select(s => new SelectListItem { Value = s.fld_Val, Text = s.fld_Desc }), "Value", "Text").ToList();
            }

            return Json(subList, JsonRequestBehavior.AllowGet);
        }

    }
}