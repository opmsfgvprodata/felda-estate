using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_RequestDataSLP
    {
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:ddMMyyyy}")]
        public DateTime StartDate { get; set; }
        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:ddMMyyyy}")]
        public DateTime EndDate { get; set; }

        public string startPkt { get; set; }
        public string endPkt { get; set; }
    }
}