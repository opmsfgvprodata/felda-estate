﻿using MVC_SYSTEM.Models;
using MVC_SYSTEM.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetIDDataFromDatabase
    {
        private MVC_SYSTEM_MasterModels dbCorp = new MVC_SYSTEM_MasterModels();
        //Modified by Shazana 1/8/2023
        public List<long> tbl_SokPermhnWangID(int? NegaraID, int? SyarikatID, int WilayahID, int LadangID, string flag, int month, int year)
        {
            var ID = new List<long>();

            switch (flag)
            {
                case "ApplicationSupportAllLadang":
                    //Modified by Shazana 1/8/2023
                    ID = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_Month == month && x.fld_Year == year).OrderBy(o => o.fld_LadangID).Select(s => s.fld_ID).ToList();
                    break;

                case "ApplicationSupportSelectedLadang":
                    //Modified by Shazana 1/8/2023
                    ID = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == month && x.fld_Year == year).OrderBy(o => o.fld_LadangID).Select(s => s.fld_ID).ToList();
                    break;
            }
            return ID;
        }

        //added by kamalia 24/11/21
        public List<long> tbl_SokPermhnWangID2(int? NegaraID, int? SyarikatID, int LadangID, string flag, int month, int year)
        {
            var ID = new List<long>();

            switch (flag)
            {
                case "ApplicationSupportAllLadang":
                    ID = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Month == month && x.fld_Year == year).OrderBy(o => o.fld_LadangID).Select(s => s.fld_ID).ToList();
                    break;

                case "ApplicationSupportSelectedLadang":
                    ID = dbCorp.vw_PermohonanKewangan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_LadangID == LadangID && x.fld_Month == month && x.fld_Year == year).OrderBy(o => o.fld_LadangID).Select(s => s.fld_ID).ToList();
                    break;
            }
            return ID;
        }
        //end

    }
}