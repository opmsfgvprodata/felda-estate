namespace MVC_SYSTEM.CustomModels
{
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ViewingModels;


    public class CustMod_KedudukanPekerja
    {

        [StringLength(2)]
        public string SyarikatID { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        public DateTime? fld_Trmlkj { get; set; }

        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }
        public int? fld_NegaraID { get; set; }
        public int? fld_SyarikatID { get; set; }
        public int? fld_WilayahID { get; set; }
        public int? fld_LadangID { get; set; }

        [StringLength(50)]
        public string fldOptConfValue { get; set; }

        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2{ get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(50)]
        public string fld_LdgName { get; set; }

        [StringLength(5)]
        public string fld_LdgCode { get; set; }


    }
}