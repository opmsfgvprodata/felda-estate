namespace MVC_SYSTEM.MasterModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_InsentifKGK
    {
        [Key]
        public int fld_ID { get; set; }

        public decimal? fld_KGKLower { get; set; }

        public decimal? fld_KGKUpper { get; set; }

        public decimal? fld_KadarUpah { get; set; }

        public int? fld_TahunToreh { get; set; }

        public bool? fld_Deleted { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }
        public int? fld_Nombor { get; set; }
    }
}
