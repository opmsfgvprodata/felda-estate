namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_TamatPermitPassport
    {
        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2prmt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2pspt { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(20)]
        public string fld_Psptno { get; set; }

        [StringLength(20)]
        public string fld_Prmtno { get; set; }

        [StringLength(20)]
        public string fld_KategoriSebab { get; set; }

        [StringLength(200)]
        public string fld_SebabDesc { get; set; }

        public bool? fld_Deleted { get; set; }

        [Key]
        public Guid fld_UniqueID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        public Guid? fld_ReasonID { get; set; }
    }
}
