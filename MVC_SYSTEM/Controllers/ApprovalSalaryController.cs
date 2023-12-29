﻿using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.CorpNewModels;
using MVC_SYSTEM.log;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.ViewingModels; //added by faeza 30.09.2021
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsSAPKOL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class ApprovalSalaryController : Controller
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
        // GET: ApprovalSalary
        public ActionResult RegionReview(int year, int month)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;

            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //   DivisionID = GetNSWL.GetDivisionSelection(getuserid, NegaraID, SyarikatID, WilayahID, LadangID);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models_SAPKOL SapModel = MVC_SYSTEM_Models_SAPKOL.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_CorpNewModels CorpNewModels = new MVC_SYSTEM_CorpNewModels();
            var postingData = SapModel.vw_SAPPostData
                    .Where(x => x.fld_Month == month && x.fld_Year == year &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID && x.fld_Desc.Contains("WORKER NET SALARY")).ToList();
            decimal TotalSalary = 0;
            if (postingData.Count() > 0)
            {
                TotalSalary = postingData.Sum(s => s.fld_Amount.Value * -1);
            }
            tbl_SalaryRequest tbl_SalaryRequest = new tbl_SalaryRequest();
            tbl_SalaryRequest.fld_TotalAmount = TotalSalary;
            tbl_SalaryRequest.fld_Year = year;
            tbl_SalaryRequest.fld_Month = month;
            var gettotalworker = dbr.tbl_GajiBulanan.Where(x => x.fld_Year == year && x.fld_Month == month && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).Distinct().Count();
            tbl_SalaryRequest.fld_TotalWorker = short.Parse(gettotalworker.ToString());
            tbl_SalaryRequest.fld_PostingID = postingData.Select(s => s.fld_SAPPostRefID).Distinct().FirstOrDefault();
            return View(tbl_SalaryRequest);
        }
        [HttpPost]
        public JsonResult RegionReview(tbl_SalaryRequest SalaryRequest)
        {
            SendEmail SendEmailNotification = new SendEmail();
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        
            int getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_CorpNewModels CorpNewModels = new MVC_SYSTEM_CorpNewModels();
            string ApproveBy = GetIdentity.Username(getuserid);
            try
            {
                var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
                //Modify by Shazana 20/4/2023
                string Subject = "Semakan Permohonan Gaji";
                string Message = "";
                string Department = "";
                string[] cc = new string[] { };
                List<string> cclist = new List<string>();
                string[] bcc = new string[] { };
                List<string> bcclist = new List<string>();


                Message = "<html>";
                Message += "<body>";
                Message += "<p>Assalamualaikum,</p>";
                Message += "<p>Mohon pihak HQ menyemak permohonan gaji. Keterangan seperti dibawah:-</p>";
                Message += "<table border=\"1\">";
                Message += "<thead>";
                Message += "<tr>";
                Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th><th>Disahkan Oleh</th>";
                Message += "</tr>";
                Message += "</thead>";
                Message += "<tbody>";
                Message += "<tr>";
                Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + SalaryRequest.fld_Month + "</td><td align=\"center\">" + SalaryRequest.fld_Year + "</td><td align=\"center\">" + SalaryRequest.fld_TotalAmount + "</td><td align=\"center\">" + ApproveBy + "</td>";
                Message += "</tr>";
                Message += "</tbody>";
                Message += "</table>";
                Message += "<p>Terima Kasih.</p>";
                Message += "</body>";
                Message += "</html>";

                cclist.Add(GetEstate.fld_SyarikatEmail);
                cclist.Add(GetEstate.fld_LdgEmail);
                cc = cclist.ToArray();
                if (GetEstate.fld_CostCentre == "1000")
                {
                    Department = "HQ_FINANCE_APPROVAL_FELDA";
                }
                else
                {
                    Department = "HQ_FINANCE_APPROVAL_FPM";
                }
                var ToEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).FirstOrDefault();

                var checkexsting = CorpNewModels.tbl_SalaryRequest.Where(x => x.fld_Year == SalaryRequest.fld_Year && x.fld_Month == SalaryRequest.fld_Month && x.fld_LadangID == LadangID).FirstOrDefault();
                if (checkexsting == null)
                {
                    tbl_SalaryRequest tbl_SalaryRequest = new tbl_SalaryRequest();
                    tbl_SalaryRequest.fld_NegaraID = NegaraID;
                    tbl_SalaryRequest.fld_SyarikatID = SyarikatID;
                    tbl_SalaryRequest.fld_WilayahID = WilayahID;
                    tbl_SalaryRequest.fld_LadangID = LadangID;
                    tbl_SalaryRequest.fld_RequestBy = getuserid;
                    tbl_SalaryRequest.fld_RequestDT = timezone.gettimezone();
                    tbl_SalaryRequest.fld_Purpose = 1;
                    tbl_SalaryRequest.fld_Month = SalaryRequest.fld_Month;
                    tbl_SalaryRequest.fld_Year = SalaryRequest.fld_Year;
                    tbl_SalaryRequest.fld_PostingID = SalaryRequest.fld_PostingID;
                    tbl_SalaryRequest.fld_TotalWorker = SalaryRequest.fld_TotalWorker;
                    tbl_SalaryRequest.fld_TotalAmount = SalaryRequest.fld_TotalAmount;
                    CorpNewModels.tbl_SalaryRequest.Add(tbl_SalaryRequest);
                    CorpNewModels.SaveChanges();

                    //SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
                    msg = "Successfully to requested";
                    statusmsg = "success";
                }
                else
                {
                    if (checkexsting.fld_ApproveStatus == true)
                    {
                        msg = "Already Approved!";
                        statusmsg = "warning";
                    }
                    else
                    {
                        checkexsting.fld_TotalWorker = SalaryRequest.fld_TotalWorker;
                        checkexsting.fld_TotalAmount = SalaryRequest.fld_TotalAmount;
                        checkexsting.fld_PostingID = SalaryRequest.fld_PostingID;
                        checkexsting.fld_RequestBy = getuserid;
                        checkexsting.fld_RequestDT = timezone.gettimezone();
                        CorpNewModels.Entry(checkexsting).State = EntityState.Modified;
                        CorpNewModels.SaveChanges();
                        //SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
                        msg = "Successfully to requested";
                        statusmsg = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "Failure to send an email!";
                statusmsg = "warning";
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }
            return Json(new
            {
                success = false,
                msg = msg,
                status = statusmsg,
                checkingdata = "0"
            });
        }

        public ActionResult RegionVerify(int year, int month)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;

            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models_SAPKOL SapModel = MVC_SYSTEM_Models_SAPKOL.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            MVC_SYSTEM_Viewing vmModel = MVC_SYSTEM_Viewing.ConnectToSqlServer(host, catalog, user, pass); //added by faeza 30.09.2021

            //commented by faeza 03.07.2023
            ////***totalworketnet
            ////modified by kamalia 3/4/2022
            //var postingData = SapModel.vw_SAPPostData
            //        .Where(x => x.fld_Month == month && x.fld_Year == year &&
            //                    x.fld_NegaraID == NegaraID &&
            //                    x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
            //                    x.fld_LadangID == LadangID && x.fld_Desc.Contains("GAJI BERSIH PEKERJA BURUH") && x.fld_DocType=="A2").ToList();

            //decimal TotalWorkerNet = 0;
            //if (postingData.Count() > 0)
            //{
            //    //modified by kamalia 24/2/2022
            //    //TotalWorkerNet = postingData.Select(s => s.fld_Amount.Value * -1).FirstOrDefault();
            //    //modified by faeza 04.06.2023
            //    TotalWorkerNet = postingData.Sum(s => s.fld_Amount.Value * -1);
            //}

            //added by faeza 03.07.2023
            var postingData = SapModel.vw_SAPPostData
                    .Where(x => x.fld_Month == month && x.fld_Year == year &&
                                x.fld_NegaraID == NegaraID &&
                                x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                                x.fld_LadangID == LadangID  && x.fld_DocType == "A2").ToList();


            //***totalworketnet
            var amountWorkerNet = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID).ToList();

            decimal TotalWorkerNet = 0;
            if (amountWorkerNet.Count() > 0)
            {
                TotalWorkerNet = amountWorkerNet.Sum(s => s.fld_GajiBersih.Value);
            }

            //***totalpermohonan (total keseluruhan)
            var amountSctran = dbr.tbl_Sctran
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && x.fld_KdCaj == "C").ToList();

            decimal TotalPermohonan = 0;
            if (amountSctran.Count() > 0)
            {
                //TotalSalary = postingData.Sum(s => s.fld_Amount.Value * -1);
                TotalPermohonan = amountSctran.Sum(s => s.fld_Amt.Value);
            }

            //***totalcash
            //modified by kamalia 28/2/2022
            var amountCash = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && (x.fld_PaymentMode == "1" || x.fld_PaymentMode == null)).ToList();

            decimal TotalCash = 0;
            if (amountCash.Count() > 0)
            {
                TotalCash = amountCash.Sum(s => s.fld_GajiBersih.Value);
            }

            //***totalcheque
            var amountCheque = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && x.fld_PaymentMode == "2").ToList();

            decimal TotalCheque = 0;
            if (amountCheque.Count() > 0)
            {
                TotalCheque = amountCheque.Sum(s => s.fld_GajiBersih.Value);
            }

            //***totalewallet
            var amountEwallet = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && x.fld_PaymentMode == "3").ToList();

            decimal TotalEwallet = 0;
            if (amountEwallet.Count() > 0)
            {
                TotalEwallet = amountEwallet.Sum(s => s.fld_GajiBersih.Value);
            }

            //***totalcdmas
            var amountCdmas = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && x.fld_PaymentMode == "4").ToList();

            decimal TotalCdmas = 0;
            if (amountCdmas.Count() > 0)
            {
                TotalCdmas = amountCdmas.Sum(s => s.fld_GajiBersih.Value);
            }

            //added by faeza 03.07.2023
            //***totalm2u
            var amountM2u = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && x.fld_PaymentMode == "5").ToList();

            decimal TotalM2u = 0;
            if (amountM2u.Count() > 0)
            {
                TotalM2u = amountM2u.Sum(s => s.fld_GajiBersih.Value);
            }

            //***totalm2e
            var amountM2e = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && x.fld_PaymentMode == "6").ToList();

            decimal TotalM2e = 0;
            if (amountM2e.Count() > 0)
            {
                TotalM2e = amountM2e.Sum(s => s.fld_GajiBersih.Value);
            }

            ////***totallain - if paymentmode = null/0
            var amountLain = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID).ToList();        //modified by kamalia 24/11/21

            //Added by Shazana 13/12/2023
            //***totalewallettng
            var amountEwalletTnG = vmModel.vw_PaySheetPekerja
                   .Where(x => x.fld_Month == month && x.fld_Year == year &&
                               x.fld_NegaraID == NegaraID &&
                               x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID &&
                               x.fld_LadangID == LadangID && x.fld_PaymentMode == "7").ToList();

            decimal TotalEwalletTnG = 0;
            if (amountEwalletTnG.Count() > 0)
            {
                TotalEwalletTnG = amountEwalletTnG.Sum(s => s.fld_GajiBersih.Value);
            }


            // add by kamalia 24/11/ 21 
            decimal TotalKwsp = 0;
            if (amountLain.Count() > 0)
            {
                TotalKwsp = amountLain.Sum(s => s.fld_KWSPMjk.Value + s.fld_KWSPPkj.Value);
            }
            decimal TotalSocso = 0;
            if (amountLain.Count() > 0)
            {
                TotalSocso = amountLain.Sum(s => s.fld_SocsoMjk.Value + s.fld_SocsoPkj.Value);
            }
            decimal? TotalSbkp = 0;
            if (amountLain.Count() > 0)
            {
                TotalSbkp = dbr.tbl_ByrCarumanTambahan
                .Where(x => x.fld_NegaraID == NegaraID &&
                             x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year &&
                             x.fld_LadangID == LadangID && x.fld_KodCaruman == "SBKP").Sum(s => s.fld_CarumanMajikan + s.fld_CarumanPekerja);

            }
            decimal? Totalsip = 0;
            if (amountLain.Count() > 0)
            {
                Totalsip = dbr.tbl_ByrCarumanTambahan
                .Where(x => x.fld_NegaraID == NegaraID &&
                             x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year &&
                             x.fld_LadangID == LadangID && x.fld_KodCaruman == "SIP").Sum(s => s.fld_CarumanMajikan + s.fld_CarumanPekerja);

            }
            //
            tbl_SokPermhnWang tbl_SokPermhnWang = new tbl_SokPermhnWang();
            tbl_SokPermhnWang.fld_JumlahWorkerNet = TotalWorkerNet;
            tbl_SokPermhnWang.fld_JumlahPermohonan = TotalPermohonan;
            tbl_SokPermhnWang.fld_JumlahCash = TotalCash;
            tbl_SokPermhnWang.fld_JumlahCheque = TotalCheque;
            tbl_SokPermhnWang.fld_JumlahEwallet = TotalEwallet;
            tbl_SokPermhnWang.fld_JumlahCdmas = TotalCdmas;
            tbl_SokPermhnWang.fld_JumlahM2U = TotalM2u; //added by faeza 03.07.2023
            tbl_SokPermhnWang.fld_JumlahM2E = TotalM2e; //added by faeza 03.07.2023
            tbl_SokPermhnWang.fld_JumlahKwsp = TotalKwsp;        //added by kamalia 24/12/21
            tbl_SokPermhnWang.fld_JumlahSocso = TotalSocso; //added by kamalia 24/12/21
            tbl_SokPermhnWang.fld_JumlahSbkp = TotalSbkp;//added by kamalia 24/12/21
            tbl_SokPermhnWang.fld_JumlahSip = Totalsip;//added by kamalia 24/12/21
            tbl_SokPermhnWang.fld_JumlahEwalletTnG = TotalEwalletTnG; //Added by Shazana 13/12/2023
            // tbl_SokPermhnWang.fld_JumlahLain = TotalLain;
            tbl_SokPermhnWang.fld_Year = year;
            tbl_SokPermhnWang.fld_Month = month;
            var gettotalworker = dbr.tbl_GajiBulanan.Where(x => x.fld_Year == year && x.fld_Month == month && x.fld_LadangID == LadangID).Select(s => s.fld_Nopkj).Distinct().Count();
            //tbl_SalaryRequest.fld_TotalWorker = short.Parse(gettotalworker.ToString());
            tbl_SokPermhnWang.fld_PostingID = postingData.Select(s => s.fld_SAPPostRefID).Distinct().FirstOrDefault();
            return View(tbl_SokPermhnWang);
        }

        [HttpPost]
        public JsonResult RegionVerify(tbl_SokPermhnWang PermohonanWang)
        {
            SendEmail SendEmailNotification = new SendEmail();
            string msg = "";
            string statusmsg = "";
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;

            int getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);
            DateTime getdatetime = timezone.gettimezone();
            //add by kamalia 26/11/21
            string ActionBy = GetIdentity.MyNameFullName(getuserid);
            string ApproveBy = GetIdentity.Username(getuserid);

            //Added by Shazana 3/4/2023
            string CompanyShortName = GetIdentity.getCompanyShortName(SyarikatID);

            try
            {
                var GetEstate = db.vw_NSWL.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_NamaWilayah, s.fld_LdgCode, s.fld_NamaLadang, s.fld_WlyhEmail, s.fld_LdgEmail, s.fld_SyarikatEmail, s.fld_NegaraID, s.fld_SyarikatID, s.fld_WilayahID, s.fld_LadangID, s.fld_CostCentre }).FirstOrDefault();
                //Modified by Shazana 3/4/2023
                /*string Subject = "OPMS FELDA – Kelulusan Pengurus Rancangan bagi  Semakkan Permohonan Gaji";*/ //updated by kamy 26/4/2022
                //Modified by Shazana 20/4/2023
                string Subject = CompanyShortName + " - Semakan Permohonan Gaji ( " + GetEstate.fld_NamaLadang + " )";
                string Message = "";
                string Department = "";
                string[] to = new string[] { };
                List<string> tolist = new List<string>();
                string[] cc = new string[] { };
                List<string> cclist = new List<string>();
                string[] bcc = new string[] { };
                List<string> bcclist = new List<string>();


                Message = "<html>";
                Message += "<body>";
                Message += "<p>Assalamualaikum,</p>";
                Message += "<p>Mohon Pengurus menyemak permohonan gaji. Keterangan seperti dibawah:-</p>";
                Message += "<table border=\"1\">";
                Message += "<thead>";
                Message += "<tr>";
                //Modified by Shazana 20/2/2023
                //Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th><th>Disahkan Oleh</th><th>Pautan</th>";
                Message += "<th>Nama Wilayah</th><th>Kod Ladang</th><th>Nama Ladang</th><th>Bulan</th><th>Tahun</th><th>Jumlah Keseluruhan (RM)</th><th>Disemak Oleh</th><th>Pautan</th>";

                Message += "</tr>";
                Message += "</thead>";
                Message += "<tbody>";
                Message += "<tr>";
                //modified by kamalia 26/11/21
                //Commented by Shazana 2/4/2023
                //Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + PermohonanWang.fld_Month + "</td><td align=\"center\">" + PermohonanWang.fld_Year + "</td><td align=\"center\">" + PermohonanWang.fld_JumlahPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td><a href=\"" + Url.Action("ApplicationSupportRegionHQ", "ApplicationSupport", null, this.Request.Url.Scheme) + "\">Klik untuk semakan</a></td>";

                //Added by Shazana 2/4/2023
                Message += "<td align=\"center\">" + GetEstate.fld_NamaWilayah + "</td><td align=\"center\">" + GetEstate.fld_LdgCode + "</td><td align=\"center\">" + GetEstate.fld_NamaLadang + "</td><td align=\"center\">" + PermohonanWang.fld_Month + "</td><td align=\"center\">" + PermohonanWang.fld_Year + "</td><td align=\"center\">" + PermohonanWang.fld_JumlahPermohonan + "</td><td align=\"center\">" + ActionBy + "</td><td><a href=\"" + Url.Action("ManagerApproval", "BizTransac", null, this.Request.Url.Scheme) + "\">Klik untuk semakan</a></td>";


                Message += "</tr>";
                Message += "</tbody>";
                Message += "</table>";
                Message += "<p>Terima Kasih.</p>";
                Message += "</body>";
                Message += "</html>";


                if (GetEstate.fld_CostCentre == "1000")
                {
                    Department = "MGR_FINANCE_APPROVAL_FELDA";
                }
                else
                {
                    Department = "MGR_FINANCE_APPROVAL_FPM";
                }

                //modified by faeza 24.02.2021
                var ToEmail = db.tblEmailLists
                    .Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldWilayahID == GetEstate.fld_WilayahID &&
                     x.fldLadangID == GetEstate.fld_LadangID && x.fldDepartment == Department && x.fldCategory == "TO" && x.fldDeleted == false)
                        .Select(s => new { s.fldEmail, s.fldName }).ToList();

                if (ToEmail != null)
                {
                    foreach (var toemail in ToEmail)
                    {
                        tolist.Add(toemail.fldEmail);
                    }
                    to = tolist.ToArray();
                }

                //Modify by Shazana 3/4/2023
                var CcEmail = db.tbl_Ladang.Where(x => x.fld_NegaraID == GetEstate.fld_NegaraID && x.fld_SyarikatID == GetEstate.fld_SyarikatID && x.fld_WlyhID == GetEstate.fld_WilayahID && x.fld_ID == GetEstate.fld_LadangID && x.fld_Deleted == false).Select(s => new { s.fld_LdgEmail, s.fld_LdgName }).FirstOrDefault();
                if (CcEmail != null)
                {
                    cclist.Add(CcEmail.fld_LdgEmail);
                    cc = cclist.ToArray();
                }

                var BccEmail = db.tblEmailLists.Where(x => x.fldNegaraID == GetEstate.fld_NegaraID && x.fldSyarikatID == GetEstate.fld_SyarikatID && x.fldDepartment == "Developer" && x.fldCategory == "BCC" && x.fldDeleted == false).Select(s => new { s.fldEmail, s.fldName }).ToList();
                if (BccEmail != null)
                {
                    foreach (var bccemail in BccEmail)
                    {
                        bcclist.Add(bccemail.fldEmail);
                    }
                    bcc = bcclist.ToArray();
                }

                var ClosingTransaction = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == PermohonanWang.fld_Month && x.fld_Year == PermohonanWang.fld_Year).FirstOrDefault();
                var GetLadangDetail = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_WlyhID == WilayahID).FirstOrDefault();
                var checkexsting = db.tbl_SokPermhnWang.Where(x => x.fld_Year == PermohonanWang.fld_Year && x.fld_Month == PermohonanWang.fld_Month && x.fld_LadangID == LadangID).FirstOrDefault();
                if (ClosingTransaction.fld_StsTtpUrsNiaga == true)
                {
                    if (checkexsting == null)
                    {
                        tbl_SokPermhnWang tbl_SokPermhnWang = new tbl_SokPermhnWang();
                        ////tbl_SokPermhnWang.fld_StsTtpUrsNiaga = true;
                        ////tbl_SokPermhnWang.fld_NegaraID = NegaraID;
                        ////tbl_SokPermhnWang.fld_SyarikatID = SyarikatID;
                        ////tbl_SokPermhnWang.fld_WilayahID = WilayahID;
                        ////tbl_SokPermhnWang.fld_LadangID = LadangID;
                        ////tbl_SokPermhnWang.fld_SemakWil_By = getuserid;
                        ////tbl_SokPermhnWang.fld_SemakWil_DT = timezone.gettimezone();
                        ////tbl_SokPermhnWang.fld_SemakWil_Status = 1;
                        ////tbl_SokPermhnWang.fld_Month = PermohonanWang.fld_Month;
                        ////tbl_SokPermhnWang.fld_Year = PermohonanWang.fld_Year;
                        ////tbl_SokPermhnWang.fld_PostingID = PermohonanWang.fld_PostingID;
                        //////tbl_SokPermhnWang.fld_TotalWorker = SalaryRequest.fld_TotalWorker;
                        ////tbl_SokPermhnWang.fld_JumlahPermohonan = PermohonanWang.fld_JumlahPermohonan;

                        tbl_SokPermhnWang.fld_PostingID = PermohonanWang.fld_PostingID;
                        tbl_SokPermhnWang.fld_Verify_By = getuserid; //added by faeza 30.09.2021
                        tbl_SokPermhnWang.fld_Verify_DT = getdatetime; //added by faeza 30.09.2021
                        tbl_SokPermhnWang.fld_SemakWil_Status = 0;
                        tbl_SokPermhnWang.fld_SokongWilGM_Status = 0;
                        tbl_SokPermhnWang.fld_TerimaHQ_Status = 0;
                        tbl_SokPermhnWang.fld_TolakWil_Status = 0;
                        tbl_SokPermhnWang.fld_TolakWilGM_Status = 0;
                        tbl_SokPermhnWang.fld_TolakHQ_Status = 0;
                        tbl_SokPermhnWang.fld_NoCIT = GetLadangDetail.fld_NoCIT;
                        tbl_SokPermhnWang.fld_NoAcc = GetLadangDetail.fld_NoAcc;
                        tbl_SokPermhnWang.fld_NoGL = GetLadangDetail.fld_NoGL;
                        tbl_SokPermhnWang.fld_JumlahPermohonan = PermohonanWang.fld_JumlahPermohonan;
                        tbl_SokPermhnWang.fld_JumlahWorkerNet = PermohonanWang.fld_JumlahWorkerNet;//added by faeza 30.09.2021
                        tbl_SokPermhnWang.fld_JumlahCash = PermohonanWang.fld_JumlahCash;//added by faeza 30.09.2021
                        tbl_SokPermhnWang.fld_JumlahCheque = PermohonanWang.fld_JumlahCheque;//added by faeza 30.09.2021
                        tbl_SokPermhnWang.fld_JumlahEwallet = PermohonanWang.fld_JumlahEwallet;//added by faeza 30.09.2021
                        tbl_SokPermhnWang.fld_JumlahCdmas = PermohonanWang.fld_JumlahCdmas;//added by faeza 30.09.2021
                        tbl_SokPermhnWang.fld_JumlahM2U = PermohonanWang.fld_JumlahM2U;//added by faeza 03.07.2023
                        tbl_SokPermhnWang.fld_JumlahM2E = PermohonanWang.fld_JumlahM2E;//added by faeza 03.07.2023
                        tbl_SokPermhnWang.fld_JumlahKwsp = PermohonanWang.fld_JumlahKwsp;//added by kamalia 24/12/21
                        tbl_SokPermhnWang.fld_JumlahSocso = PermohonanWang.fld_JumlahSocso;//added by kamalia 24/12/21
                        tbl_SokPermhnWang.fld_JumlahSip = PermohonanWang.fld_JumlahSip;//added by kamalia 24/12/21
                        tbl_SokPermhnWang.fld_JumlahSbkp = PermohonanWang.fld_JumlahSbkp;//added by kamalia 24/12/21
                        tbl_SokPermhnWang.fld_JumlahEwalletTnG = PermohonanWang.fld_JumlahEwalletTnG;//Added by Shazana 13/12/2023
                        // tbl_SokPermhnWang.fld_JumlahLain = PermohonanWang.fld_JumlahLain;//comment by kamalia 24/12/21
                        tbl_SokPermhnWang.fld_StsTtpUrsNiaga = true;
                        tbl_SokPermhnWang.fld_NegaraID = NegaraID;
                        tbl_SokPermhnWang.fld_SyarikatID = SyarikatID;
                        tbl_SokPermhnWang.fld_WilayahID = WilayahID;
                        tbl_SokPermhnWang.fld_LadangID = LadangID;
                        tbl_SokPermhnWang.fld_Year = PermohonanWang.fld_Year;
                        tbl_SokPermhnWang.fld_Month = PermohonanWang.fld_Month;
                        db.tbl_SokPermhnWang.Add(tbl_SokPermhnWang);
                        db.SaveChanges();

                        ////SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
                        SendEmailNotification.SendEmailLatest(Subject, Message, to, null, bcc);
                        msg = "Successfully to requested";
                        statusmsg = "success";
                    }
                    else
                    {
                        //if (checkexsting.fld_SemakWil_Status == 1)
                        //{
                        //    msg = "Already Approved!";
                        //    statusmsg = "warning";
                        //}
                        //else
                        //{
                        
                        checkexsting.fld_PostingID = PermohonanWang.fld_PostingID;
                        checkexsting.fld_Verify_By = getuserid; //added by faeza 30.09.2021
                        checkexsting.fld_Verify_DT = getdatetime; //added by faeza 30.09.2021
                        checkexsting.fld_SemakWil_Status = 0;
                        checkexsting.fld_SokongWilGM_Status = 0;
                        checkexsting.fld_TerimaHQ_Status = 0;
                        checkexsting.fld_TolakWil_Status = 0;
                        checkexsting.fld_TolakWilGM_Status = 0;
                        checkexsting.fld_TolakHQ_Status = 0;
                        checkexsting.fld_NoCIT = GetLadangDetail.fld_NoCIT;
                        checkexsting.fld_NoAcc = GetLadangDetail.fld_NoAcc;
                        checkexsting.fld_NoGL = GetLadangDetail.fld_NoGL;
                        checkexsting.fld_JumlahPermohonan = PermohonanWang.fld_JumlahPermohonan;
                        checkexsting.fld_JumlahWorkerNet = PermohonanWang.fld_JumlahWorkerNet;//added by faeza 30.09.2021
                        checkexsting.fld_JumlahCash = PermohonanWang.fld_JumlahCash;//added by faeza 30.09.2021
                        checkexsting.fld_JumlahCheque = PermohonanWang.fld_JumlahCheque;//added by faeza 30.09.2021
                        checkexsting.fld_JumlahEwallet = PermohonanWang.fld_JumlahEwallet;//added by faeza 30.09.2021
                        checkexsting.fld_JumlahEwalletTnG = PermohonanWang.fld_JumlahEwalletTnG;//Added by Shazana 13/12/2023
                        checkexsting.fld_JumlahCdmas = PermohonanWang.fld_JumlahCdmas;//added by faeza 30.09.2021
                        checkexsting.fld_JumlahM2U = PermohonanWang.fld_JumlahM2U;//added by faeza 03.07.2023
                        checkexsting.fld_JumlahM2E = PermohonanWang.fld_JumlahM2E;//added by faeza 03.07.2023
                        checkexsting.fld_JumlahKwsp = PermohonanWang.fld_JumlahKwsp;//added by kamalia 24/12/21
                        checkexsting.fld_JumlahSocso = PermohonanWang.fld_JumlahSocso;//added by kamalia 24/12/21
                        checkexsting.fld_JumlahSip = PermohonanWang.fld_JumlahSip;//added by kamalia 24/12/21
                        checkexsting.fld_JumlahSbkp = PermohonanWang.fld_JumlahSbkp;//added by kamalia 24/12/21
                        db.Entry(checkexsting).State = EntityState.Modified;
                        db.SaveChanges();
                        //SendEmailNotification.SendEmailDetail(Subject, Message, ToEmail.fldEmail, cc, bcc);
                        SendEmailNotification.SendEmailLatest(Subject, Message, to, cc, bcc);
                        msg = "Successfully to requested";
                        statusmsg = "success";
                        //}
                    }
                }
                else
                {
                    //account belum close
                    msg = "Account is not closed";
                    statusmsg = "warning";
                }

            }
            catch (Exception ex)
            {
                msg = "Failure to send an email!";
                statusmsg = "warning";
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            db.Dispose();
            return Json(new
            {
                //success = false,
                success = true,
                msg = "Successfully to verify",
                //status = statusmsg,
                statusmsg = "success",
                checkingdata = "0",
                method = "1",
                div = "closeTransactionDetails",
                action = "_PostingSAP",
                contoller = "ClosingTransaction"

            });
        }
    }
}