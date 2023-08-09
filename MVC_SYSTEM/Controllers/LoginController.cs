using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
//using MVC_SYSTEM.LoginModels;
using MVC_SYSTEM.Security;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using MVC_SYSTEM.App_LocalResources;
//using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;

namespace MVC_SYSTEM.Controllers
{
    public class LoginController : Controller
    {
        //private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        //private MVC_SYSTEM_Login db2 = new MVC_SYSTEM_Login();
        //private MVC_SYSTEM_Auth db3 = new MVC_SYSTEM_Auth();
        private errorlog geterror = new errorlog();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetNSWL GetNSWL = new GetNSWL();
        private Connection Connection = new Connection();
        private GetConfig getConfig = new GetConfig();
        GetIdentity GetIdentity = new GetIdentity();
        private SendEmail SendEmail = new SendEmail();
        private EncryptDecrypt crypto = new EncryptDecrypt();

        // GET: Login
        [Localization("bm")]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Main");
                //return View();
            }
            else
            {
                return View();
            }
        }

        // POST: Login/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AntiForgeryHandleError]
        public ActionResult Index(tblUser Login, string returnUrl)
        {
            string password;
            try
            {
                if (string.IsNullOrEmpty(Login.fldUserName) == false && string.IsNullOrEmpty(Login.fldUserPassword) == false)
                {
                    //getUser user = null;
                    EncryptDecrypt Encrypt = new EncryptDecrypt();
                    password = Encrypt.Encrypt(Login.fldUserPassword);

                    var user = db.tblUsers.Where(u => u.fldUserName == Login.fldUserName.ToUpper() && u.fldUserPassword == password && u.fldDeleted == false).SingleOrDefault();

                    //var estateselection
                    
                    //mas tambah - 05/11/2020
                    // tambah condition user.fldSyarikatID == 1 || user.fldSyarikatID == 2 untuk filter user felda/gmn & ftp
                    if (user != null && user.fldSyarikatID == 1 || user.fldSyarikatID == 2)
                    {
                        if (user.fldNegaraID == 0 || user.fldSyarikatID == 0 || user.fldWilayahID == 0)
                        {
                            var estateselection = db.tbl_EstateSelection.Where(x => x.fld_UserID == user.fldUserID).FirstOrDefault();
                            if (estateselection != null)
                            {
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                string data = js.Serialize(user);
                                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                                string encToken = FormsAuthentication.Encrypt(ticket);
                                HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                                Response.Cookies.Add(authoCookies);

                                getConfig.AddUserAuditTrail(user.fldUserID, "Login to estate");

                                if (!string.IsNullOrEmpty(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }
                                else
                                {
                                    if (user.fldRoleID == 1 || user.fldRoleID == 2)
                                    {
                                        return RedirectToAction("Index", "SuperAdminSelection");
                                    }
                                    else
                                    {
                                        // edited by Zaty
                                        //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                                        //string host, catalog, user2, pass = "";
                                        //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, user.fldUserID, user.fldUserName);
                                        //Connection.GetConnection(out host, out catalog, out user2, out pass, WilayahID, SyarikatID, NegaraID);
                                        //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user2, pass);
                                        var asasldg = db.tbl_Ladang.Where(x => x.fld_WlyhID == estateselection.fld_WilayahID && x.fld_ID == estateselection.fld_LadangID).FirstOrDefault();
                                        if (string.IsNullOrEmpty(asasldg.fld_Pengurus) | string.IsNullOrEmpty(asasldg.fld_Adress) | string.IsNullOrEmpty(asasldg.fld_Tel) | string.IsNullOrEmpty(asasldg.fld_Fax) | string.IsNullOrEmpty(asasldg.fld_LdgEmail))
                                        {
                                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                                            Response.Cache.SetAllowResponseInBrowserHistory(false);
                                            Response.Cache.SetNoStore();
                                            //return RedirectToAction("_PartialPassword", "Main");
                                            return RedirectToAction("EstateReminder", "BasicInfo");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "Main");
                                        }

                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", GlobalResEstate.lblLoginHQMsg);
                            }
                        }
                        else
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();
                            string data = js.Serialize(user);
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.fldUserShortName, timezone.gettimezone(), timezone.gettimezone().Add(FormsAuthentication.Timeout), false, data);
                            string encToken = FormsAuthentication.Encrypt(ticket);
                            HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                            Response.Cookies.Add(authoCookies);

                            getConfig.AddUserAuditTrail(user.fldUserID, "Login to estate");

                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                if (user.fldRoleID == 1 || user.fldRoleID == 2)
                                {
                                    return RedirectToAction("Index", "SuperAdminSelection");
                                }
                                else
                                {
                                    // edited by Zaty
                                    //int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
                                    //string host, catalog, user2, pass = "";
                                    //GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, user.fldUserID, user.fldUserName);
                                    //Connection.GetConnection(out host, out catalog, out user2, out pass, WilayahID, SyarikatID, NegaraID);
                                    //MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user2, pass);
                                    var asasldg = db.tbl_Ladang.Where(x => x.fld_WlyhID == user.fldWilayahID && x.fld_ID == user.fldLadangID).FirstOrDefault();
                                    if (string.IsNullOrEmpty(asasldg.fld_Pengurus) | string.IsNullOrEmpty(asasldg.fld_Adress) | string.IsNullOrEmpty(asasldg.fld_Tel) | string.IsNullOrEmpty(asasldg.fld_Fax) | string.IsNullOrEmpty(asasldg.fld_LdgEmail))
                                    {
                                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                                        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
                                        Response.Cache.SetAllowResponseInBrowserHistory(false);
                                        Response.Cache.SetNoStore();
                                        return RedirectToAction("EstateReminder", "BasicInfo");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Main");
                                    }

                                }
                            }
                        }
                        //if(user.fldWilayahID != 0 && user.fldLadangID !=0)
                        //{
                        //    var routeurl = db3.tbl_Wilayah.Where(x => x.fld_SyarikatID == user.fldSyarikatID && x.fld_ID == user.fldWilayahID).Select(s => s.fld_UrlRoute).FirstOrDefault();
                        //    return Redirect(routeurl + "IntegrationLogin?TokenID=" + user.fld_TokenLadangID);
                        //}
                        //else
                        //{
                        
                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("", GlobalResEstate.lblLoginInvalid);
                    }
                }
                else
                {
                    ModelState.AddModelError("", GlobalResEstate.lblLoginMsg);
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                ModelState.AddModelError("", GlobalResEstate.msgError);
                return View();
            }
            return View(Login);
        }
        public ActionResult Logout()
        {
            //try4:
            Response.Cookies.Clear();
            //try5:
            FormsAuthentication.SetAuthCookie(String.Empty, false);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login", null);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Part 3 - Forgot Password

        public ActionResult ForgotPassword()
        {
            return View();
        }

        //sepul comment 1/4/2021
        //[HttpPost]
        //public ActionResult ForgotPassword(string fldUserEmail, string fldUserPassword)
        //{
        //    //Verify Email ID
        //    //Generate Reset password link 
        //    //Send Email 
        //    string message = "";
        //    bool status = false;

        //    if (ModelState.IsValid)
        //    {
        //        using (MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels())
        //        {
        //            var account = db.tblUsers.Where(a => a.fldUserEmail == fldUserEmail).FirstOrDefault();
        //            if (account != null)
        //            {
        //                //Send email for reset password
        //                string resetCode = Guid.NewGuid().ToString();
        //                SendVerificationLinkEmail(account.fldUserEmail, resetCode, "ResetPassword"); //** tutup
        //                account.fldResetPasswordCode = resetCode;
        //                account.fldUserPassword = fldUserPassword;
        //                account.fld_ModifiedDT = timezone.gettimezone();
        //                db.Configuration.ValidateOnSaveEnabled = false;
        //                db.SaveChanges();
        //                message = GlobalResEstate.msgNewPwd;

        //                //return Json(new { success = true, msg = GlobalResEstate.msgNewPwd, status = "success", checkingdata = "0" });
        //            }
        //            else
        //            {
        //                db.Dispose();

        //                return Json(new
        //                {
        //                    success = false,
        //                    msg = GlobalResEstate.msgErrorData,
        //                    status = "warning",
        //                    checkingdata = "0"
        //                });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        message = GlobalResEstate.msgErrorData;
        //    }
        //    ViewBag.Message = message;
        //    return View();
        //}

        // Sepul Tambah 1/4/2021
        [HttpPost]
        public ActionResult ForgotPassword(string fldUserEmail, string fldUserName)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";

            using (MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels())
            {
                var account = db.tblUsers.Where(a => a.fldUserEmail == fldUserEmail && a.fldUserName == fldUserName).FirstOrDefault();
                //if (account )
                {
                    if (account != null)
                    {
                        //Send email for reset password
                        string katalaluan = "init123";
                        SendVerificationLinkEmail(account.fldUserEmail, katalaluan, "ResetPassword");
                        account.fldUserPassword = crypto.Encrypt(katalaluan);

                        //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                        //in our model class in part 1
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "Sila Semak Emel Untuk Katalaluan Baru.";

                    }
                    else
                    {
                        message = "Sila Pastikan ID Pengguna dan Emel Telah Berdaftar.";
                    }
                }
            }
            ViewBag.Message = message;
            return View("index");
        }
        //sepul tambah 1/4/2021
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "ResetPassword")
        {
            try
            {
                //var verifyUrl = "/User/" + emailFor + "/" + activationCode;
                //var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                string subject = "Tetapan Katalaluan";
                string body = "Assalamualaikum dan Selamat Sejahtera,<br/><br/> Kami menerima permohonan untuk menetapkan semula katalaluan." +
                    "<br/> Katalaluan baru anda adalah <b>init123</b>" +
                    "<br/> <br/> Terima Kasih." +
                    "<br/><br/>___________________________________________________________________________" +
                    "<i><p style='color: gray !important; font-size: 4px !important;'> Emel ini dijana secara automatik.</p></i>";

                SendEmail.SendEmailForgotPassword(subject, body, emailID);
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
        }


        public JsonResult GetUserList(string UserName)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var UserList = db.tblUsers.Where(x => x.fldUserName == UserName && x.fldDeleted == false)
                .Select(s => s.fldUserEmail).FirstOrDefault();

            return Json(UserList);
        }
    }
}




