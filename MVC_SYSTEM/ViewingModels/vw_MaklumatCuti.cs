namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_MaklumatCuti
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long fld_CutiDiambilID { get; set; }

        [StringLength(50)]
        public string fld_NoPkj { get; set; }

        [StringLength(50)]
        public string fld_KodCuti { get; set; }

        [StringLength(50)]
        public string fld_KeteranganCuti { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhAmbilCuti { get; set; }

        public int? fld_tempohCuti { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }
    }
}
