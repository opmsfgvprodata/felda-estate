using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Produktiviti
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ProduktivitifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(10)]
        public string fld_JenisPelan { get; set; }

        public decimal? fld_Targetharian { get; set; }

        [StringLength(10)]
        public string fld_Unit { get; set; }

        public int? fld_HadirKerja { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_ProduktivitiModelViewCreate
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ProduktivitifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_JenisPelan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Targetharian { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        public int? fld_HadirKerja { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_ProduktivitiModelViewEdit
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ProduktivitifID { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_JenisPelan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Targetharian { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        public int? fld_HadirKerja { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_ProduktivitiGroupModelViewCreate
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ProduktivitifID { get; set; }

        public int? fld_KumpulanID { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_JenisPelan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Targetharian { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        public int? fld_HadirKerja { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public string host { get; set; }

        public string catalog { get; set; }

        public string user { get; set; }

        public string pass { get; set; }
    }
}
