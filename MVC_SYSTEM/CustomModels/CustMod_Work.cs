using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_Work
    {
        [Key]
        public int ID { get; set; }

        public string nopkj { get; set; }

        public decimal? kadar { get; set; }

        public byte? gandaankadar { get; set; }

        public decimal? hasil { get; set; }

        public decimal? jumlah { get; set; }

        public decimal? jumlahOA { get; set; }

        public short? kualiti { get; set; }

        public byte? bonus { get; set; }

        public decimal? ot { get; set; }

        public string kdhmnuai { get; set; }

        public short? checkpurpose { get; set; }

        public string GLCode { get; set; }

        public string PaysheetID { get; set; }
    }

    public class Kesukaran
    {
        public string fld_KodHargaKesukaran { get; set;}
        public decimal fld_HargaKesukaran { get; set; }
        public string fldOptConfFlag2 { get;set; }
    }
}