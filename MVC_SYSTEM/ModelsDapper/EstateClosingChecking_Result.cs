using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class EstateClosingChecking_Result
    {
        public Guid fld_id { get; set; }
        public short? fld_purpose { get; set; }
        public decimal? jumgl2gl { get; set; }
        public decimal? jumgl2vdDt { get; set; }
        public decimal? jumgl2vdCt { get; set; }
        public decimal? TL_Credit { get; set; }
        public decimal? TL_Debit { get; set; }
    }
}