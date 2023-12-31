﻿//using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetWilayah
    {
        //private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private MVC_SYSTEM_MasterModels dbCorp = new MVC_SYSTEM_MasterModels();  //Shazana 17/11/2022
        public int[] GetWilayahID(int ? SyarikatID)
        {
            IEnumerable<int> enumerable = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).ToArray();
            int[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        public int[] GetWilayahID2(int? SyarikatID, int? WilayahID)
        {
            IEnumerable<int> enumerable = db.tbl_Wilayah.Where(x => x.fld_ID == WilayahID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_ID).ToArray();
            int[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        public int?[] GetWilayahIDForApplicationSupport(int? NegaraID, int? SyarikatID, int? WilayahID, int? month, int? year)
        {
            IEnumerable<int?> enumerable = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        //Shazana 17/11/2022
        public int?[] GetWilayahIDForApplicationSupport2(int? NegaraID, int? SyarikatID, int? WilayahID, int? month, int? year)
        {

            IEnumerable<int?> enumerable = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }
        //Close Shazana 17/11/2022

        public int?[] GetWilayahIDForApplicationSupport2(int? NegaraID, int? SyarikatID, int? month, int? year)
        {
            IEnumerable<int?> enumerable = db.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

        public string GetWilayahName(int wlyhid)
        {
            string name;

            name = db.tbl_Wilayah.Where(x => x.fld_ID == wlyhid).Select(s => s.fld_WlyhName).FirstOrDefault();

            return name;
        }

        public string GetWilayahName2(int wlyhid)
        {
            string name = "";

            if (wlyhid != 0)
            {
                name = db.tbl_Wilayah.Where(x => x.fld_ID == wlyhid).Select(s => s.fld_WlyhName).FirstOrDefault();
            }
            else
            {
                name = "HQ";
            }

            return name;
        }

        public bool GetAvailableWilayah(int ? SyarikatID)
        {
            bool result = false;

            var getwilayah = db.tbl_Wilayah.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Count();

            if( getwilayah > 0)
            {
                result = true;
            }

            return result;
        }

        //Added by Shazana 1/8/2023
        public int?[] GetWilayahIDForApplicationSupport3(int? NegaraID, int? SyarikatID, int? WilayahID, int? month, int? year, string SyarikatIDList)
        {
            IEnumerable<int?> enumerable = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }
        //Added by Shazana 1/8/2023
        public int?[] GetWilayahIDForApplicationSupport3(int? NegaraID, int? SyarikatID, int? month, int? year, string SyarikatIDList)
        {
            //IEnumerable<int?> enumerable = db2.tbl_SokPermhnWang.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Month == month && x.fld_Year == year).Select(s => s.fld_WilayahID).Distinct().ToArray();
            IEnumerable<int?> enumerable = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Month == month && x.fld_Year == year && x.fld_CostCentre == SyarikatIDList).Select(s => s.fld_WilayahID).Distinct().ToArray();
            int?[] wlyhid = enumerable.ToArray();

            return wlyhid;
        }

    }
}