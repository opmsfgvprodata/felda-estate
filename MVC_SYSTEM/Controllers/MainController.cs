using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using MVC_SYSTEM.App_LocalResources;
using MVC_SYSTEM.Models;



namespace MVC_SYSTEM.Controllers
{

    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class MainController : Controller
    {
        // GET: Main
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        GetIdentity getidentity = new GetIdentity();
        EncryptDecrypt crypto = new EncryptDecrypt();
        GetNSWL GetNSWL = new GetNSWL();
        GetIdentity GetIdentity = new GetIdentity();
        private Connection Connection = new Connection();


        public ActionResult Index()
        {
            //original code
            //ViewBag.Main = "class = active";
            //ViewBag.Dropdown = "dropdown";
            ////Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //return View();

            //aini modified 28042023
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_SP_Models dbsp = MVC_SYSTEM_SP_Models.ConnectToSqlServer(host, catalog, user, pass);

            ViewBag.Main = "class = active";
            ViewBag.Dropdown = "dropdown";

            int currentMonth = DateTime.Now.Month - 1;
            int currentYear = DateTime.Now.Year;

            ViewBag.month = currentMonth;
            ViewBag.year = currentYear;

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);
            List<sp_DashAllKerakyatan_Result> kerakyatanresult = new List<sp_DashAllKerakyatan_Result>();

            //aini add checking jumlah != 0 15062023
            kerakyatanresult = dbsp.sp_DashAllKerakyatan(SyarikatID, LadangID).Where(x => x.fld_Jumlah != 0).ToList();

            var position = getidentity.Penjawatan(User.Identity.Name); //aini update 18072023
            var getdata = db.tblOptionConfigsWebs.Where(x => x.fldOptConfValue == position && x.fldOptConfFlag1 == "position" && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            if (getdata == null)
            {
                ViewBag.Position = "";
            }
            else
            {
                ViewBag.Position = getdata.fldOptConfDesc;
            }

            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            return View(kerakyatanresult);
        }

        public JsonResult ChangePassword(string oldpswd, string newpswd, string confirmpswd)
        {
            if (!string.IsNullOrEmpty(oldpswd))
            {
                int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                int? getuserid = GetIdentity.ID(User.Identity.Name);
                GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

                // mas delete x.fldWilayahID==WilayahID && x.fldLadangID==LadangID pd 15/9/2020
                var getdata = db.tblUsers.Where(x => x.fldUserID == getuserid && x.fldNegaraID == NegaraID && x.fldSyarikatID == SyarikatID).FirstOrDefault();
                string userpswd = crypto.Encrypt(oldpswd);
                if (getdata != null && getdata.fldUserPassword == userpswd)
                {
                    if (!string.IsNullOrEmpty(newpswd) && confirmpswd == newpswd && newpswd != oldpswd)
                    {
                        //var pswdpattern = "((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{6,20})";
                        var pswdpattern = new Regex(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20})");

                        // mas tambah crypto.Encrypt pd 15/9/20
                        if (pswdpattern.IsMatch(crypto.Encrypt((newpswd))))
                        {
                            getdata.fldUserPassword = crypto.Encrypt(newpswd);
                            db.Entry(getdata).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { success = true, msg = GlobalResEstate.msgPwdSucc, status = "success" });
                        }
                        else
                        {
                            return Json(new { success = false, msg = GlobalResEstate.msgPwdxSah, status = "warning" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, msg = "Error.", status = "warning" });
                    }

                }
                else
                {
                    return Json(new { success = false, msg = GlobalResEstate.msgErrorIT, status = "warning" });
                }
            }
            else
            {
                return Json(new { success = false, msg = GlobalResEstate.msgInstPwd, status = "warning" });
            }

        }

        public ActionResult pwd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult pwdchnge(string pass, int processType)
        {
            string code = "";
            if (!string.IsNullOrEmpty(pass))
            {
                if (processType==1)
                {
                    code = crypto.Encrypt(pass);
                }
                else
                {
                    code = crypto.Decrypt(pass);
                }
            }
            return Json(code);
        }

        //aini add calendar 28042023
        public ActionResult Calendar()
        {
            int? SyarikatID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);

            GetNSWL.GetSyarikat(out SyarikatID, getuserid, User.Identity.Name);

            var cuti2 = db.tbl_CutiUmumMaster.Where(x => x.fld_SyarikatID == 1).ToArray();
            return Json(cuti2);
        }

    }
}