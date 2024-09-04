using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ViewingModels;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_KekuatanPekerja
    {
        public string fld_NamaWilayah { get; set; }
        public string fld_KodLadang { get; set; }
        public string fld_NamaLadang { get; set; }
        public int? fld_KeperluanSebenar { get; set; }
        public decimal? fld_LuasKeseluruhan { get; set; }
        //public int? fld_PekerjaTempatan { get; set; }
        //public int? fld_PekerjaAsing { get; set; }

        public string fld_Kdrkyt { get; set; }
        public int? fld_PekerjaKontraktor { get; set; }
        public int? NegaraID { get; set; }
        public int? SyarikatID { get; set; }
        public int? WilayahID { get; set; }
        public int? LadangID { get; set; }
        public string fld_CompCode { get; set; }
    }
}