using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_AttWorkAuditTrail
    {
        public string ActionFor { get; set; }
        public string ActionBy { get; set; }
        public DateTime ActionDT { get; set; }
        public List<tbl_Kerja> ListKerja { get; set; }
    }
}