using MVC_SYSTEM.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public class CustMod_OptionConfig
    {
        [Key]
        public int fldOptConfID { get; set; }

        [StringLength(50)]
        public string fldOptConfValue { get; set; }

        [StringLength(50)]
        public string fldOptConfDesc { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag1 { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag2 { get; set; }

        public bool? fldDeleted { get; set; }

        [StringLength(50)]
        public string fldOptConfFlag3 { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
    }
}