namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vw_LbrPrmtPsprtUpdate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long fld_ID { get; set; }

        public Guid? fld_LbrRefID { get; set; }

        [StringLength(15)]
        [Display(Name = "No Passport")]
        public string fld_Nokp { get; set; }

        [StringLength(20)]
        public string fld_Prmtno { get; set; }

        [StringLength(20)]
        [Display(Name = "ID Pekerja")]
        public string fld_Nopkj { get; set; }

        [StringLength(100)]
        [Display(Name = "Nama Pekerja")]
        public string fld_Nama { get; set; }

        [StringLength(100)]
        public string fld_Almt1 { get; set; }

        [StringLength(5)]
        public string fld_Poskod { get; set; }

        [StringLength(5)]
        public string fld_Neg { get; set; }

        [StringLength(5)]
        public string fld_Negara { get; set; }

        [StringLength(15)]
        public string fld_Notel { get; set; }

        [StringLength(1)]
        public string fld_Kdjnt { get; set; }

        [StringLength(2)]
        public string fld_Kdbgsa { get; set; }

        [StringLength(1)]
        public string fld_Kdagma { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trlhr { get; set; }

        [StringLength(2)]
        public string fld_Kdrkyt { get; set; }

        [StringLength(1)]
        public string fld_Kdkwn { get; set; }

        [StringLength(1)]
        public string fld_Kpenrka { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        [StringLength(60)]
        public string fld_Sbtakf { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trtakf { get; set; }

        [StringLength(100)]
        public string fld_Remarks { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trmlkj { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Trshjw { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1pspt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2pspt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T1prmt { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_T2prmt { get; set; }

        [StringLength(2)]
        public string fld_Jenispekerja { get; set; }

        [StringLength(2)]
        public string fld_Ktgpkj { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        [StringLength(3)]
        public string fld_Kodbkl { get; set; }

        [StringLength(20)]
        [Display(Name = "No Passport/Permit Baru")]
        public string fld_NewPrmtPsprtNo { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Tarikh Tamat")]
        public DateTime? fld_NewPrmtPsrtEndDT { get; set; }

        [StringLength(20)]
        [Display(Name = "No Passport/Permit Lama")]
        public string fld_OldPrmtPsprtNo { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Tarikh Tamat")]
        public DateTime? fld_OldPrmtPsrtEndDT { get; set; }

        public short? fld_PurposeIndicator { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
