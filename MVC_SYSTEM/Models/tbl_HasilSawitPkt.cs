using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_HasilSawitPkt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodPeringkat { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_HasilTan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasHektar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bulan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(10)]
        public string fld_YieldType { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_HasilSawitPktModelViewCreate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodPeringkat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        [Column(TypeName = "numeric")]
        public decimal? fld_HasilTan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        //[RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxHectarageModelValidation")]
        //[Remote("IsPktAreaWithinRange", "WorkerInfo", AdditionalFields = "fld_KodPeringkat", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "MsgModelValidationAreaExceedRange")]
        [Column(TypeName = "numeric")]
        public decimal? fld_LuasHektar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bulan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(10)]
        public string fld_YieldType { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }

    public partial class tbl_HasilSawitPktModelViewEdit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodPeringkat { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgNumberModelValidation")]
        [Column(TypeName = "numeric")]
        public decimal? fld_HasilTan { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgModelValidation")]
        //[RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "msgMaxHectarageModelValidation")]
        //[Remote("IsPktAreaWithinRange", "WorkerInfo", AdditionalFields = "fld_KodPeringkat", ErrorMessageResourceType = typeof(GlobalResEstate), ErrorMessageResourceName = "MsgModelValidationAreaExceedRange")]
        [Column(TypeName = "numeric")]
        public decimal? fld_LuasHektar { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Bulan { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(10)]
        public string fld_YieldType { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
