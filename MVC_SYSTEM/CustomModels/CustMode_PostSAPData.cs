using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMode_PostSAPData
    {
        public tbl_SAPPostRef GetSAPPostRef { get; set; }

       

        public List<tbl_SAPPostDataDetails> postDataDetails { get; set; }
    }
}