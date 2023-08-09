using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MVC_SYSTEM.SAPPostIntegration;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class UnitTestFunction
    {

        private Connection Connection = new Connection();
        public void SapPostData(string userName, string password, Guid? postGLToGL, Guid? postGLToVendor, Guid? postGLToCustomer)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = GetIdentity.ID("FELHQADMIN");
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, "FELHQADMIN");
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

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




            Guid sapPostRefID = new Guid();

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


                                if (GLtoGLItem.fld_SAPType == "CC")
                                {
                                    bapiacgl09_details.Costcenter = GLtoGLItem.fld_IO;
                                }
                                else
                                {
                                    if (GLtoGLItem.fld_IO != null)
                                        bapiacgl09_details.Orderid = GLtoGLItem.fld_IO;
                                    else
                                        bapiacgl09_details.Orderid = null;
                                }

                                bapiacgl09[i] = bapiacgl09_details;

                                //farahin tambah - new IO/CC assigned utk cater wilayah sahabat - 19/4/2023
                                //if (GLtoGLItem.fld_WilayahID == 11)
                                //{
                                //    if (GLtoGLItem.fld_SAPType == null || GLtoGLItem.fld_SAPType == "IO")
                                //    {
                                //        bapiacgl09_details.Orderid = GLtoGLItem.fld_IO;
                                //    }
                                //    else if (GLtoGLItem.fld_SAPType == "CC")
                                //    {
                                //        bapiacgl09_details.Costcenter = GLtoGLItem.fld_IO;
                                //    }
                                //}
                                //else if (GLtoGLItem.fld_WilayahID == 12)
                                //{
                                //    bapiacgl09_details.Costcenter = GLtoGLItem.fld_IO;
                                //}
                                //else
                                //{
                                //    if (GLtoGLItem.fld_IO != null)
                                //    {
                                //        bapiacgl09_details.Orderid = GLtoGLItem.fld_IO;
                                //    }
                                //    else
                                //    {
                                //        bapiacgl09_details.Orderid = null;
                                //    }
                                //}

                                //Currency Amount
                                bapiaccr09_details.ItemnoAcc = GLtoGLItem.fld_ItemNo.ToString().PadLeft(10, '0');
                                bapiaccr09_details.Currency = GLtoGLItem.fld_Currency;
                                //modified by kamalia 24/11/21
                                bapiaccr09_details.AmtDoccur = (decimal)GLtoGLItem.fld_Amount;


                                bapiaccr09[i] = bapiaccr09_details;
                            }

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

                    }
                }
            }

            /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

            //Gl to Vendor Process

            //farahin ubah whole function - 10/2/2022

        }

    }
}
