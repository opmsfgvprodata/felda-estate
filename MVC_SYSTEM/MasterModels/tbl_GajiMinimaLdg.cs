//kamalia - 18.02.2021
namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    public partial class tbl_GajiMinimaLdg
    {
        [Key]
        public int fld_ID { get; set; }
        public decimal? fld_NilaiGajiMinima { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }
        public int? fld_OptConfigID { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}