namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CutiUmumMaster
    {
        [Key]
        public int fld_CutiUmumMasterID { get; set; }

        [StringLength(100)]
        public string fld_KeteranganCuti { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhCuti { get; set; }

        public short? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
