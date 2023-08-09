namespace MVC_SYSTEM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_DataAuditTrail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(100)]
        public string fld_ActionFor { get; set; }

        public string fld_RawDataBefore { get; set; }

        public string fld_RawDataAfter { get; set; }

        public string fld_DataChange { get; set; }

        [StringLength(1)]
        public string fld_Action { get; set; }

        public DateTime? fld_ActionDT { get; set; }

        public int? fld_ActionBy { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
