﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class KerjaInfoDetails_Result
    {
        public int fld_ID { get; set; }
        public string fld_Nopkj { get; set; }
        public string fld_Nama { get; set; }
        public string fld_Kum { get; set; }
        public Nullable<System.DateTime> fld_Tarikh { get; set; }
        public Nullable<int> fld_Hujan { get; set; }
        public string fld_Kdhdct { get; set; }
        public string fld_JnisAktvt { get; set; }
        public string fld_KodAktvt { get; set; }
        public string fld_Unit { get; set; }
        public Nullable<byte> fld_JnsPkt { get; set; }
        public string fld_KodPkt { get; set; }
        public string fld_KodGL { get; set; }
        public Nullable<decimal> fld_KadarByr { get; set; }
        public Nullable<decimal> fld_JumlahHasil { get; set; }
        public Nullable<decimal> fld_Amount { get; set; }
        public Nullable<decimal> fld_JamOT { get; set; }
        public Nullable<decimal> fld_KadarOT { get; set; }
        public Nullable<decimal> fld_JumlahOT { get; set; }
        public Nullable<byte> fld_Bonus { get; set; }
        public Nullable<decimal> fld_KadarBonus { get; set; }
        public Nullable<decimal> fld_JumlahBonus { get; set; }
        public Nullable<int> fld_CreatedBy { get; set; }
        public Nullable<System.DateTime> fld_CreatedDT { get; set; }
        public Nullable<decimal> fld_HrgaKwsnSkar { get; set; }
        public Nullable<decimal> fld_OverallAmount { get; set; }
        public string fld_SAPIO { get; set; }
        public string fld_SAPGL { get; set; }
    }
}