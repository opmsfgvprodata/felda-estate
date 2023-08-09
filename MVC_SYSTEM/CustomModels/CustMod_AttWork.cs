﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.CustomModels
{
    public partial class CustMod_AttWork
    {
        [Key]
        public int ID { get; set; }

        public string Nopkj { get; set; }

        public string Namapkj { get; set; }

        public string Keteranganhdr { get; set; }

        public string statushdr { get; set; }

        public decimal? Kadar { get; set; }

        public short? KadarByrn { get; set; }

        public string KdhByr { get; set; }

        public short? disabletextbox { get; set; }

        public string Unit { get; set; }

        public decimal? MaximumHsl { get; set; }

        public string GLCode { get; set; }

        public string EstateCostCenter { get; set; }

        public string PaysheetID { get; set; }
    }
}