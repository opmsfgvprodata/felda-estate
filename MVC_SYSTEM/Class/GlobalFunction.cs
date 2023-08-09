using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KellermanSoftware.CompareNetObjects;
using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using Newtonsoft.Json;

namespace MVC_SYSTEM.Class
{
    public class GlobalFunction
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();

        public static class PropertyCopy
        {
            public static void Copy<TDest, TSource>(TDest destination, TSource source)
                where TSource : class
                where TDest : class
            {
                var destProperties = destination.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var sourceProperties = source.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var copyProperties = sourceProperties.Join(destProperties, x => x.Name, y => y.Name, (x, y) => x);
                foreach (var sourceProperty in copyProperties)
                {
                    var prop = destProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);
                    prop.SetValue(destination, sourceProperty.GetValue(source));
                }
            }
        }

        public void CreateAuditTrail(MVC_SYSTEM_Models dbr, string Action, string Purpose, int UserID, Object OldObject, Object NewObject, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            ChangeTimeZone timezone = new ChangeTimeZone();
            // get the differance
            CompareLogic compObjects = new CompareLogic();
            compObjects.Config.MaxDifferences = 99;
            ComparisonResult compResult = compObjects.Compare(OldObject, NewObject);
            List<CustMod_AuditDelta> DeltaList = new List<CustMod_AuditDelta>();
            foreach (var change in compResult.Differences)
            {
                CustMod_AuditDelta delta = new CustMod_AuditDelta();
                if (change.PropertyName != "")
                {
                    if (change.PropertyName.Substring(0, 1) == ".")
                        delta.FieldName = change.PropertyName.Substring(1, change.PropertyName.Length - 1);
                    delta.ValueBefore = change.Object1Value;
                    delta.ValueAfter = change.Object2Value;
                    DeltaList.Add(delta);
                }   
            }

            tbl_DataAuditTrail audit = new tbl_DataAuditTrail();
            audit.fld_Action = Action;
            audit.fld_ActionFor = Purpose;
            audit.fld_ActionDT = timezone.gettimezone();
            audit.fld_ActionBy = UserID;
            audit.fld_RawDataBefore = JsonConvert.SerializeObject(OldObject); // if use xml instead of json, can use xml annotation to describe field names etc better
            if (DeltaList.Count > 0)
            {
                audit.fld_RawDataAfter = JsonConvert.SerializeObject(NewObject);
            }
            else
            {
                audit.fld_RawDataAfter = "[]";
            }
            audit.fld_DataChange = JsonConvert.SerializeObject(DeltaList);
            audit.fld_NegaraID = NegaraID;
            audit.fld_SyarikatID = SyarikatID;
            audit.fld_WilayahID = WilayahID;
            audit.fld_LadangID = LadangID;
            dbr.tbl_DataAuditTrail.Add(audit);
            dbr.SaveChanges();

        }

        public string BatchNoSAPPostFunc(int? LadangID, string BatchWord, string BatchFlag, string BatchFlag2, int Month, int Year)
        {
            GetNSWL GetNSWL = new GetNSWL();
            var GetNSWLDetail = GetNSWL.GetLadangDetail(LadangID.Value);
            var getbatchno = db.tbl_BatchRunRefNo.Where(x => x.fld_BatchFlag == BatchFlag && x.fld_NegaraID == GetNSWLDetail.fld_NegaraID && x.fld_SyarikatID == GetNSWLDetail.fld_SyarikatID && x.fld_Month == Month && x.fld_Year == Year && x.fld_BatchFlag2 == BatchFlag2).FirstOrDefault();
            int? convertint = 0;
            string genbatchno = "";

            if (getbatchno == null)
            {
                tbl_BatchRunRefNo tbl_BatchRunRefNo = new tbl_BatchRunRefNo();
                tbl_BatchRunRefNo.fld_BatchRunNo = 1;
                tbl_BatchRunRefNo.fld_BatchFlag = BatchFlag;
                tbl_BatchRunRefNo.fld_BatchFlag2 = BatchFlag2;
                tbl_BatchRunRefNo.fld_NegaraID = GetNSWLDetail.fld_NegaraID;
                tbl_BatchRunRefNo.fld_SyarikatID = GetNSWLDetail.fld_SyarikatID;
                tbl_BatchRunRefNo.fld_WilayahID = 0;
                tbl_BatchRunRefNo.fld_LadangID = 0;
                tbl_BatchRunRefNo.fld_Month = Month;
                tbl_BatchRunRefNo.fld_Year = Year;
                db.tbl_BatchRunRefNo.Add(tbl_BatchRunRefNo);
                db.SaveChanges();
                convertint = 1;
                genbatchno = BatchWord + convertint.Value.ToString("000");
            }
            else
            {
                convertint = getbatchno.fld_BatchRunNo;
                convertint = convertint + 1;
                getbatchno.fld_BatchRunNo = convertint;
                db.Entry(getbatchno).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                genbatchno = BatchWord + convertint.Value.ToString("000");
            }

            return genbatchno;
        }
    }
}