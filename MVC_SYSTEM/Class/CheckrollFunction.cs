using MVC_SYSTEM.CustomModels;
using MVC_SYSTEM.MasterModels;
using MVC_SYSTEM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class CheckrollFunction
    {
        private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetConfig GetConfig = new GetConfig();
        private Connection Connection = new Connection();

        public bool LeaveCalBal(MVC_SYSTEM_Models dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            bool result = false;

            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti && x.fld_Tahun == year).Select(s => s.fld_JumlahCuti - s.fld_JumlahCutiDiambil).FirstOrDefault();

            result = getdata > 0 ? true : false;

            return result;
        }
        //Kamalia 16/02/2020
        public decimal GetSalaryTheDay(MVC_SYSTEM_Models dbr, DateTime? DT, string NoPkj, string kodkumplan, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string AttCode)
        {
            decimal? firstNo = 0;
            decimal? secondNo = 0;
            decimal? thirdNo = 0;
            decimal OTRate = 0;
            decimal OTPay = 0;
            decimal? AfterRounded = 0;
            decimal Salary = 0m;
            decimal? BonusRate = 0;
            decimal? BonusPay = 0;
            DateTime CurrentMonth = new DateTime(DT.Value.Year, DT.Value.Month, 15);
            int LastMonth = CurrentMonth.AddMonths(-1).Month;
            int LastYear = CurrentMonth.AddMonths(-1).Year;
            var Working = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == NoPkj && x.fld_Kum == kodkumplan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == DT).OrderByDescending(o => o.fld_JamOT).FirstOrDefault();
            List<tbl_JenisAktiviti> JenisAktiviti = new List<tbl_JenisAktiviti>();
            JenisAktiviti = db.tbl_JenisAktiviti.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_DisabledFlag != 3 && x.fld_Deleted == false).ToList();
            var JenisAktvkod = JenisAktiviti.Select(s => s.fld_KodJnsAktvt).ToList();
            var Working2 = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == NoPkj && x.fld_Kum == kodkumplan && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Tarikh == DT && JenisAktvkod.Contains(x.fld_JnisAktvt)).OrderByDescending(o => o.fld_Bonus).FirstOrDefault();
            if (Working != null)
            {
                decimal WorkingPay = Working.fld_OverallAmount == null ? 0 : Working.fld_OverallAmount.Value;
                if (Working.fld_JamOT > 0)
                {
                    var OTCulData = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kadarot" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();
                    var OTKadar = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "kiraot" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).ToList();

                    firstNo = decimal.Parse(OTCulData.Where(x => x.fldOptConfFlag2 == "1").Select(s => s.fldOptConfValue).FirstOrDefault());
                    secondNo = decimal.Parse(OTCulData.Where(x => x.fldOptConfFlag2 == "2").Select(s => s.fldOptConfValue).FirstOrDefault());
                    thirdNo = decimal.Parse(OTKadar.Where(x => x.fldOptConfFlag2 == AttCode).Select(s => s.fldOptConfValue).FirstOrDefault());
                    var SalaryIncrement = dbr.tbl_PkjIncrmntSalary.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_AppStatus == true).Select(s => s.fld_IncrmntSalary).FirstOrDefault();
                    if (SalaryIncrement != null)
                    {
                        firstNo = firstNo + SalaryIncrement;
                    }
                    AfterRounded = firstNo / secondNo;
                    OTRate = Math.Round(decimal.Parse(AfterRounded.ToString()), 2) * thirdNo.Value;
                    OTPay = Working.fld_JamOT.Value * OTRate;
                }
                else
                {
                    OTPay = 0;
                }
                BonusRate = db.tbl_HargaSawitSemasa.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Bulan == LastMonth && x.fld_Tahun == LastYear).Select(s => s.fld_Insentif).FirstOrDefault();
                if (BonusRate != null)
                {
                    if (Working2.fld_Bonus > 0)
                    {
                        BonusPay = Working2.fld_Bonus == 100 ? BonusRate : BonusRate / 2;
                    }
                    else
                    {
                        BonusPay = 0;
                    }
                }
                else
                {
                    BonusPay = 0;
                }
                Salary = WorkingPay + OTPay + BonusPay.Value;
            }
            var PaidLeaveCode = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodCuti).ToList();
            if (PaidLeaveCode.Contains(AttCode))
            {
                var AverageSalary = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Nopkj == NoPkj && x.fld_Year == LastYear && x.fld_Month == LastMonth).Select(s => s.fld_PurataGaji).FirstOrDefault();
                if (AverageSalary == null)
                {
                    var getgajiminima = db.tbl_GajiMinimaLdg.Where(x => x.fld_LadangID == LadangID).FirstOrDefault();
                    AverageSalary = getgajiminima != null ? getgajiminima.fld_NilaiGajiMinima : 42.310m;
                }
                Salary = AverageSalary.Value;
            }
            return Math.Round(Salary, 2);
        }
        //Kamalia 16/02/2020

        public void GetPaidLeaveLabour(MVC_SYSTEM_Models dbr, List<string> noPkjs, int year, int month, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            var paidLeaveCode = db.tbl_CutiKategori.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).Select(s => s.fld_KodCuti).ToList();
            var attandances = dbr.tbl_Kerjahdr.Where(x => noPkjs.Contains(x.fld_Nopkj) && x.fld_LadangID == LadangID && x.fld_Tarikh.Value.Year == year && x.fld_Tarikh.Value.Month == month).ToList();
            var getgajiminima = db.tbl_GajiMinimaLdg.Where(x => x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
            DateTime SelectDateOri = new DateTime(year, month, 15);
            DateTime LastSelectMonthDate = SelectDateOri.AddMonths(-1);
            var lastMonthWorksSalary = dbr.tbl_GajiBulanan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && noPkjs.Contains(x.fld_Nopkj) && x.fld_Year == LastSelectMonthDate.Year && x.fld_Month == LastSelectMonthDate.Month).ToList();
            foreach (var noPkj in noPkjs)
            {
                foreach (var attandance in attandances.Where(x => x.fld_Nopkj == noPkj).OrderBy(o => o.fld_Tarikh).ToList())
                {
                    var leavePayment = 0m;
                    if (attandance.fld_Kdhdct == "C01")
                    {
                        DateTime? TwoDayBefore = attandance.fld_Tarikh.Value.AddDays(-1);
                        DateTime? TwoDayAfter = attandance.fld_Tarikh.Value.AddDays(1);

                        var GetTwoAftBefDayHdrCts = attandances.Where(x => x.fld_Nopkj == noPkj && x.fld_Tarikh >= TwoDayBefore && x.fld_Tarikh <= TwoDayAfter).Select(s => new { s.fld_Tarikh, s.fld_Kdhdct }).OrderBy(o => o.fld_Tarikh).ToList();
                        if (GetTwoAftBefDayHdrCts.Select(s => s.fld_Kdhdct).Contains("P01") == true)
                        {
                            leavePayment = 0m;
                        }
                        else
                        {
                            var lastMonthAverageSalary = lastMonthWorksSalary.Where(x => x.fld_Nopkj == noPkj).Select(s => s.fld_PurataGaji).FirstOrDefault();
                            leavePayment = Math.Round(decimal.Parse(lastMonthAverageSalary.ToString()), 2); ;
                        }
                    }
                }
            }
        }

        public bool CheckLeaveType(string KodKatCuti, int? NegaraID, int? SyarikatID)
        {
            bool result = false;

            var getdata = db.tbl_CutiKategori.Where(x => x.fld_KodCuti == KodKatCuti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Count();

            result = getdata > 0 ? true : false;

            return result;
        }

        public void LeaveDeduct(MVC_SYSTEM_Models dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti && x.fld_Tahun == year).FirstOrDefault();
            getdata.fld_JumlahCutiDiambil = getdata.fld_JumlahCutiDiambil + 1;
            dbr.SaveChanges();
        }

        public void LeaveAdd(MVC_SYSTEM_Models dbr, int year, string NoPkj, string KodKatCuti, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            var getdata = dbr.tbl_CutiPeruntukan.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_NoPkj == NoPkj && x.fld_KodCuti == KodKatCuti && x.fld_Tahun == year).FirstOrDefault();
            getdata.fld_JumlahCutiDiambil = getdata.fld_JumlahCutiDiambil - 1;
            dbr.SaveChanges();
        }

        public bool GroupCheckLeaveTake(List<tbl_Kerjahdr> tbl_Kerjahdrs, int? NegaraID, int? SyarikatID, out List<tbl_Kerjahdr> returntbl_Kerjahdr)
        {
            bool result = false;
            returntbl_Kerjahdr = new List<tbl_Kerjahdr>();

            var getkodct = tbl_Kerjahdrs.Select(s => s.fld_Kdhdct).ToArray();
            var getdata = db.tbl_CutiKategori.Where(x => getkodct.Contains(x.fld_KodCuti) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_KodCuti).ToArray();

            result = getdata.Count() > 0 ? true : false;

            if (result)
            {
                returntbl_Kerjahdr = tbl_Kerjahdrs.Where(x => getdata.Contains(x.fld_Kdhdct)).ToList();
            }

            return result;
        }

        public bool IndividuCheckLeaveTake(string kodcuti, int? NegaraID, int? SyarikatID)
        {
            bool result = false;

            var getdata = db.tbl_CutiKategori.Where(x => x.fld_KodCuti == kodcuti && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_KodCuti).Count();

            result = getdata > 0 ? true : false;

            return result;
        }

        public string PkjName(MVC_SYSTEM_Models dbr, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string Nopkj)
        {
            string namepkj = "";

            namepkj = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == Nopkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_Nama).FirstOrDefault();

            return namepkj;
        }

        public List<CustMod_WorkerWork> RecordWorkingList(MVC_SYSTEM_Models dbr, int SelectionCategory, string SelectionData, DateTime SelectDate, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            List<tbl_Kerja> tbl_KerjaList = new List<tbl_Kerja>();
            List<CustMod_WorkerWork> CustMod_WorkerWorks = new List<CustMod_WorkerWork>();
            string namepkj = "";
            if (SelectionCategory == 1)
            {
                int KumpulanID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan.Trim() == SelectionData.Trim() && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KumpulanID).FirstOrDefault();
                var datainpkjmast = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == KumpulanID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1 && x.fld_Kdaktf == "1").Select(s => s.fld_Nopkj.Trim()).ToList();
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Kum == SelectionData && datainpkjmast.Contains(x.fld_Nopkj) && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }
            else
            {
                tbl_KerjaList = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == SelectionData && x.fld_Tarikh == SelectDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Distinct().ToList();
            }

            foreach (var tbl_KerjaData in tbl_KerjaList)
            {
                namepkj = PkjName(dbr, NegaraID, SyarikatID, WilayahID, LadangID, tbl_KerjaData.fld_Nopkj);
                CustMod_WorkerWorks.Add(new CustMod_WorkerWork() { fld_ID = tbl_KerjaData.fld_ID, fld_Nopkj = tbl_KerjaData.fld_Nopkj, fld_NamaPkj = namepkj, fld_Amount = tbl_KerjaData.fld_Amount, fld_JumlahHasil = tbl_KerjaData.fld_JumlahHasil, fld_KodAktvt = tbl_KerjaData.fld_KodAktvt, fld_KodGL = tbl_KerjaData.fld_KodGL, fld_KodPkt = tbl_KerjaData.fld_KodPkt, fld_Kum = tbl_KerjaData.fld_Kum, fld_Tarikh = tbl_KerjaData.fld_Tarikh, fld_Unit = tbl_KerjaData.fld_Unit, fld_JamOT = tbl_KerjaData.fld_JamOT, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_AmountOA = tbl_KerjaData.fld_OverallAmount });
            }

            return CustMod_WorkerWorks;
        }

        public string GetHariTerabaiJnsCharge(MVC_SYSTEM_Models dbr, string NoPkj, DateTime? SelectDate, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            string Result = "";
            Result = dbr.tbl_KerjaHariTerabai.Where(x => x.fld_Tarikh == SelectDate && x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_JenisCharge).FirstOrDefault();
            return Result;
        }

        public bool CheckSAPGLMap(MVC_SYSTEM_Models dbr, byte? JenisPkt, string GetPkt, string AktvtCd, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, bool HariTerabai, string JenisKiraanHariTerabai, out string GLCode, int transferPktID)
        {
            bool Result = false;
            GLCode = "";
            var tbl_PktUtama = new List<tbl_PktUtama>();
            string GetPaySheetID = "";
            var GetLadang = new GetLadang();
            var estateCostCenter = GetLadang.GetLadangCostCenter(LadangID);
            if (!HariTerabai)
            {
                if (transferPktID == 0)
                {
                    switch (JenisPkt)
                    {
                        case 1:
                            //Take GetPkt Direct
                            break;
                        case 2:
                            GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                            break;
                        case 3:
                            GetPkt = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktutama).FirstOrDefault();
                            break;
                    }
                    tbl_PktUtama = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).ToList();

                }
                else
                {
                    switch (JenisPkt)
                    {
                        case 1:
                            GetPkt = dbr.tbl_PktUtama.Where(x => x.fld_ID == transferPktID).Select(s => s.fld_PktUtama).FirstOrDefault();
                            break;
                        case 2:
                            GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_ID == transferPktID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                            break;
                        case 3:
                            GetPkt = dbr.tbl_Blok.Where(x => x.fld_ID == transferPktID).Select(s => s.fld_KodPktutama).FirstOrDefault();
                            break;
                    }
                    tbl_PktUtama = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).ToList();
                }
                //Check untuk lot felda and peneroka
                var PktData = tbl_PktUtama.Select(s => new { s.fld_StatusTnmn, s.fld_IOcode, s.fld_JnsLot }).FirstOrDefault();
                GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();

                //get GL Code
                var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetPaySheetID && x.fld_JnsLot == PktData.fld_JnsLot && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                Result = GLMap != null ? true : false;
                GLCode = GLMap != null ? GLMap.fld_KodGL : "";
            }
            else
            {
                if (estateCostCenter == "1000")
                {
                    if (JenisKiraanHariTerabai == "kong")
                    {
                        switch (JenisPkt)
                        {
                            case 1:
                                //Take GetPkt Direct
                                break;
                            case 2:
                                GetPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktUtama).FirstOrDefault();
                                break;
                            case 3:
                                GetPkt = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_KodPktutama).FirstOrDefault();
                                break;
                        }

                        var PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).Select(s => new { s.fld_StatusTnmn, s.fld_IOcode }).FirstOrDefault();
                        GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();

                        //get GL Code
                        var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetPaySheetID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                        Result = GLMap != null ? true : false;
                        GLCode = GLMap != null ? GLMap.fld_KodGL : "";
                    }
                    else
                    {
                        Result = true;
                        GLCode = "";
                    }
                }
                else
                {
                    var mapGLs = db.tbl_MapGL.Where(x => x.fld_SyarikatID == 1 && (x.fld_Paysheet == "PT" || x.fld_Paysheet == "PA") && x.fld_KodAktvt == AktvtCd && x.fld_Deleted == false).ToList();
                    var GL8800 = new List<GL8800>();
                    foreach (var mapGL in mapGLs)
                    {
                        GL8800.Add(new GL8800 { GL = mapGL.fld_KodGL, Paysheet = mapGL.fld_Paysheet });
                    }
                    if (GL8800.Count() > 1)
                    {
                        GLCode = JsonConvert.SerializeObject(GL8800);
                        Result = true;
                    }
                }
            }
            return Result;
        }

        public bool CheckSAPGLMap2(MVC_SYSTEM_Models dbr, string NoPkj, string AktvtCd, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, bool HariTerabai, string JenisKiraanHariTerabai, out string GLCode, out string PaySheetID)
        {
            bool Result = false;
            GLCode = "";
            PaySheetID = "";
            var GetJenisPkjGL = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "jnsGL" && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => new { s.fldOptConfValue, s.fldOptConfFlag3 }).ToList();
            var GetPkerja = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == NoPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
            var GetGLKodPkjRefer = GetPkerja.Join(GetJenisPkjGL, j => new { krkytan = j.fld_Kdrkyt }, k => new { krkytan = k.fldOptConfFlag3 }, (j, k) => new { k.fldOptConfValue, j.fld_Nopkj, j.fld_NegaraID, j.fld_SyarikatID, j.fld_WilayahID, j.fld_LadangID }).Select(s => s.fldOptConfValue).FirstOrDefault();
            var GetJnsPkj = GetGLKodPkjRefer == null ? GetJenisPkjGL.Where(x => x.fldOptConfFlag3 == "OTH").Select(s => s.fldOptConfValue).FirstOrDefault() : GetGLKodPkjRefer;
            if (!HariTerabai)
            {
                //get GL Code
                var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetJnsPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                Result = GLMap != null ? true : false;
                GLCode = GLMap != null ? GLMap.fld_KodGL : "";
                PaySheetID = GetJnsPkj;
            }
            else
            {
                if (JenisKiraanHariTerabai == "kong")
                {
                    //get GL Code
                    var GLMap = db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetJnsPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false).FirstOrDefault();

                    Result = GLMap != null ? true : false;
                    GLCode = GLMap != null ? GLMap.fld_KodGL : "";
                }
                else
                {
                    Result = true;
                    GLCode = "";
                }
            }
            return Result;
        }

        public void SaveDataKerjaSAP(MVC_SYSTEM_Models dbr, List<tbl_Kerja> DataKerja, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string GLCode, bool TransferPkt, string transferPktCode)
        {
            //get PaysheetID
            var sapType = "";
            var GetPkt = "";
            string GetPaySheetID = "";
            var PktData = new tbl_PktUtama();
            if (!TransferPkt)
            {
                int? JenisPkt = DataKerja.Select(s => s.fld_JnsPkt).Take(1).FirstOrDefault();
                GetPkt = DataKerja.Select(s => s.fld_KodPkt).Take(1).FirstOrDefault();
                switch (JenisPkt)
                {
                    case 1:
                        //Take GetPkt Direct
                        var tbl_PktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        sapType = string.IsNullOrEmpty(tbl_PktUtama.fld_SAPType) ? "IO" : tbl_PktUtama.fld_SAPType;
                        break;
                    case 2:
                        var tbl_SubPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        sapType = string.IsNullOrEmpty(tbl_SubPkt.fld_SAPType) ? "IO" : tbl_SubPkt.fld_SAPType;
                        GetPkt = tbl_SubPkt.fld_KodPktUtama;
                        break;
                    case 3:
                        var tbl_Blok = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).FirstOrDefault();
                        sapType = string.IsNullOrEmpty(tbl_Blok.fld_SAPType) ? "IO" : tbl_Blok.fld_SAPType;
                        GetPkt = tbl_Blok.fld_KodPktutama;
                        break;
                }
                PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt && x.fld_Deleted == false).FirstOrDefault();
                GetPaySheetID = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "statusTanaman" && x.fldOptConfValue == PktData.fld_StatusTnmn && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfFlag2).FirstOrDefault();
            }
            else
            {
                var transferPktDetail = dbr.tbl_PktPinjam.Where(x => x.fld_KodPkt == transferPktCode && x.fld_LadangID == LadangID).FirstOrDefault();
                PktData = dbr.tbl_PktUtama.Where(x => x.fld_ID == transferPktDetail.fld_OriginPktID && x.fld_LadangID == transferPktDetail.fld_LadangIDAsal && x.fld_Deleted == false).FirstOrDefault();
                sapType = string.IsNullOrEmpty(PktData.fld_SAPType) ? "IO" : PktData.fld_SAPType;
            }

            //get GL Code
            //string AktvtCd = DataKerja.Select(s => s.fld_KodAktvt).Take(1).FirstOrDefault();
            string GLCd = GLCode;//db.tbl_MapGL.Where(x => x.fld_KodAktvt == AktvtCd && x.fld_Paysheet == GetPaySheetID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => s.fld_KodGL).FirstOrDefault();

            List<tbl_KerjaSAPData> KerjaSAPDatas = new List<tbl_KerjaSAPData>();

            foreach (var EachDataKerja in DataKerja)
            {
                KerjaSAPDatas.Add(new tbl_KerjaSAPData { fld_GLKod = GLCd, fld_IOKod = PktData.fld_IOcode, fld_KerjaID = EachDataKerja.fld_ID, fld_PaySheetID = GetPaySheetID, fld_NegaraID = EachDataKerja.fld_NegaraID, fld_SyarikatID = EachDataKerja.fld_SyarikatID, fld_WilayahID = EachDataKerja.fld_WilayahID, fld_LadangID = EachDataKerja.fld_LadangID, fld_SAPType = sapType });
            }

            dbr.tbl_KerjaSAPData.AddRange(KerjaSAPDatas);
            dbr.SaveChanges();

        }

        public void SaveDataKerjaSAPFPM(MVC_SYSTEM_Models dbr, List<tbl_Kerja> DataKerja, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, List<CustMod_Work> HadirData, bool TransferPkt, string transferPktCode)
        {
            //get PaysheetID
            var sapType = "";
            var GetPkt = "";
            var PktData = new tbl_PktUtama();
            if (!TransferPkt)
            {
                int? JenisPkt = DataKerja.Select(s => s.fld_JnsPkt).Take(1).FirstOrDefault();
                GetPkt = DataKerja.Select(s => s.fld_KodPkt).Take(1).FirstOrDefault();
                switch (JenisPkt)
                {
                    case 1:
                        //Take GetPkt Direct
                        var tbl_PktUtama = dbr.tbl_PktUtama.Where(x => x.fld_PktUtama == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        sapType = string.IsNullOrEmpty(tbl_PktUtama.fld_SAPType) ? "IO" : tbl_PktUtama.fld_SAPType;
                        break;
                    case 2:
                        var tbl_SubPkt = dbr.tbl_SubPkt.Where(x => x.fld_Pkt == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        sapType = string.IsNullOrEmpty(tbl_SubPkt.fld_SAPType) ? "IO" : tbl_SubPkt.fld_SAPType;
                        GetPkt = tbl_SubPkt.fld_KodPktUtama;
                        break;
                    case 3:
                        var tbl_Blok = dbr.tbl_Blok.Where(x => x.fld_Blok == GetPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                        sapType = string.IsNullOrEmpty(tbl_Blok.fld_SAPType) ? "IO" : tbl_Blok.fld_SAPType;
                        GetPkt = tbl_Blok.fld_KodPktutama;
                        break;
                }
                PktData = dbr.tbl_PktUtama.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_PktUtama == GetPkt).FirstOrDefault();
            }
            else
            {
                var transferPktDetail = dbr.tbl_PktPinjam.Where(x => x.fld_KodPkt == transferPktCode && x.fld_LadangID == LadangID).FirstOrDefault();
                PktData = dbr.tbl_PktUtama.Where(x => x.fld_ID == transferPktDetail.fld_OriginPktID && x.fld_LadangID == transferPktDetail.fld_LadangIDAsal).FirstOrDefault();
                sapType = string.IsNullOrEmpty(PktData.fld_SAPType) ? "IO" : PktData.fld_SAPType;
            }


            List<tbl_KerjaSAPData> KerjaSAPDatas = new List<tbl_KerjaSAPData>();

            foreach (var EachDataKerja in DataKerja)
            {
                string GLCd = HadirData.Where(x => x.nopkj == EachDataKerja.fld_Nopkj).Select(s => s.GLCode).FirstOrDefault();
                string GetPaySheetID = HadirData.Where(x => x.nopkj == EachDataKerja.fld_Nopkj).Select(s => s.PaysheetID).FirstOrDefault();
                KerjaSAPDatas.Add(new tbl_KerjaSAPData { fld_GLKod = GLCd, fld_IOKod = PktData.fld_IOcode, fld_KerjaID = EachDataKerja.fld_ID, fld_PaySheetID = GetPaySheetID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID, fld_SAPType = sapType });
            }

            dbr.tbl_KerjaSAPData.AddRange(KerjaSAPDatas);
            dbr.SaveChanges();

        }

        public void SaveDataKerjaKesukaran(MVC_SYSTEM_Models dbr, List<tbl_Kerja> DataKerja, List<Kesukaran> kesukaran, int? NegaraID, int? SyarikatID)
        {
            List<tbl_KerjaKesukaran> KerjaKesukaran = new List<tbl_KerjaKesukaran>();
            var keteranganhdr = "";
            var statushdr = "";
            short Gandaan = 0;
            short gandaanSave = 0;

            if (kesukaran != null)
            {
                var oneDataKerja = DataKerja.FirstOrDefault();

                var kodKehadiran = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == oneDataKerja.fld_Nopkj && x.fld_LadangID == oneDataKerja.fld_LadangID && x.fld_Tarikh == oneDataKerja.fld_Tarikh).Select(s => s.fld_Kdhdct).FirstOrDefault();

                GetConfig.GetCutiDesc(kodKehadiran, "cuti", out keteranganhdr, out statushdr, out Gandaan, NegaraID, SyarikatID);

                foreach (var EachDataKerja in DataKerja)
                {
                    decimal? jumlahKeseluruhan = 0m;
                    foreach (var item in kesukaran)
                    {
                        gandaanSave = item.fldOptConfFlag2 == "HargaTambahan" ? Gandaan : (short)1;
                        var jumlah = EachDataKerja.fld_JumlahHasil * item.fld_HargaKesukaran * gandaanSave;
                        jumlah = Math.Round(jumlah.Value, 2);
                        jumlahKeseluruhan += jumlah;
                        KerjaKesukaran.Add(new tbl_KerjaKesukaran { fld_KerjaID = EachDataKerja.fld_ID, fld_Kadar = item.fld_HargaKesukaran, fld_KodKesukaran = item.fld_KodHargaKesukaran, fld_Gandaan = gandaanSave, fld_Nopkj = EachDataKerja.fld_Nopkj, fld_Kuantiti = EachDataKerja.fld_JumlahHasil, fld_Kum = EachDataKerja.fld_Kum, fld_Tarikh = EachDataKerja.fld_Tarikh, fld_Jumlah = jumlah, fld_NegaraID = EachDataKerja.fld_NegaraID, fld_SyarikatID = EachDataKerja.fld_SyarikatID, fld_WilayahID = EachDataKerja.fld_WilayahID, fld_LadangID = EachDataKerja.fld_LadangID });
                    }
                    EachDataKerja.fld_HrgaKwsnSkar = jumlahKeseluruhan;
                    EachDataKerja.fld_KodKwsnSkar = "**";
                }

                dbr.tbl_KerjaKesukaran.AddRange(KerjaKesukaran);
                dbr.SaveChanges();
            }
        }

        public void SaveDataKerjaSAP2(MVC_SYSTEM_Models dbr, List<tbl_Kerja> DataKerja, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            //get PaysheetID
            string GetPkt = DataKerja.Select(s => s.fld_KodPkt).Take(1).FirstOrDefault();

            string GetPaySheetID = "";

            //get GL Code
            string GLCd = "";

            List<tbl_KerjaSAPData> KerjaSAPDatas = new List<tbl_KerjaSAPData>();

            foreach (var EachDataKerja in DataKerja)
            {
                CheckSAPGLMap2(dbr, EachDataKerja.fld_Nopkj, EachDataKerja.fld_KodAktvt, NegaraID, SyarikatID, WilayahID, LadangID, false, "-", out GLCd, out GetPaySheetID);
                KerjaSAPDatas.Add(new tbl_KerjaSAPData { fld_GLKod = GLCd, fld_IOKod = "-", fld_KerjaID = EachDataKerja.fld_ID, fld_PaySheetID = GetPaySheetID, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_WilayahID = WilayahID, fld_LadangID = LadangID });
            }

            dbr.tbl_KerjaSAPData.AddRange(KerjaSAPDatas);
            dbr.SaveChanges();

        }

        public bool GetStatusCutProcess(MVC_SYSTEM_Models dbr, DateTime? DateSelected, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            bool result = false;
            DateTime NowDate = timezone.gettimezone();
            DateTime FirstDayNowDate = new DateTime(NowDate.Year, NowDate.Month, 1);
            DateTime FirstDaySelectedDate = new DateTime(DateSelected.Value.Year, DateSelected.Value.Month, 1);
            int GetCutOfDateDay = int.Parse(GetConfig.GetWebConfigValue("haritrakhir", NegaraID, SyarikatID));
            DateTime CutOfDate = FirstDayNowDate.AddDays(GetCutOfDateDay);

            var getTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == DateSelected.Value.Year && x.fld_Month == DateSelected.Value.Month).FirstOrDefault();

            if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga != null)
            {
                result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
            }
            else if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga == null)
            {
                result = false;
            }
            else if (FirstDayNowDate > FirstDaySelectedDate && (getTtpUrsNiaga != null || getTtpUrsNiaga == null))
            {
                if (NowDate >= CutOfDate && (getTtpUrsNiaga == null || getTtpUrsNiaga != null))
                {
                    result = true;
                }
                else if (NowDate < CutOfDate && getTtpUrsNiaga != null)
                {
                    result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
                }
                else if (NowDate < CutOfDate && getTtpUrsNiaga == null)
                {
                    result = false;
                }
            }

            return result;
        }

        public bool GetStatusCutGenProcess(MVC_SYSTEM_Models dbr, DateTime? DateSelected, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out bool LastCloseBizStatus)
        {
            bool result = false;
            LastCloseBizStatus = false;
            DateTime NowDate = timezone.gettimezone();
            DateTime FirstDayNowDate = new DateTime(NowDate.Year, NowDate.Month, 1);
            DateTime FirstDaySelectedDate = new DateTime(DateSelected.Value.Year, DateSelected.Value.Month, 1);
            int GetCutOfDateDay = int.Parse(GetConfig.GetWebConfigValue("haritrakhir", NegaraID, SyarikatID));
            DateTime CutOfDate = FirstDayNowDate.AddDays(GetCutOfDateDay);
            DateTime LastMonthCheck = DateSelected.Value.AddMonths(-1);

            var getTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == DateSelected.Value.Year && x.fld_Month == DateSelected.Value.Month).FirstOrDefault();
            var LastMonthStatus = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Year == LastMonthCheck.Year && x.fld_Month == LastMonthCheck.Month && x.fld_StsTtpUrsNiaga == true).FirstOrDefault();

            if (LastMonthStatus != null)
            {
                LastCloseBizStatus = true;
                if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga != null)
                {
                    result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
                }
                else if (FirstDayNowDate == FirstDaySelectedDate && getTtpUrsNiaga == null)
                {
                    result = false;
                }
                else if (FirstDayNowDate > FirstDaySelectedDate && (getTtpUrsNiaga != null || getTtpUrsNiaga == null))
                {
                    if (NowDate >= CutOfDate && (getTtpUrsNiaga == null || getTtpUrsNiaga != null))
                    {
                        result = true;
                    }
                    else if (NowDate < CutOfDate && getTtpUrsNiaga != null)
                    {
                        result = getTtpUrsNiaga.fld_StsTtpUrsNiaga == true ? true : false;
                    }
                    else if (NowDate < CutOfDate && getTtpUrsNiaga == null)
                    {
                        result = false;
                    }
                }
            }
            else
            {
                var NullDataTtpUrsNiaga = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                if (NullDataTtpUrsNiaga > 0)
                {
                    result = true;
                    LastCloseBizStatus = false;
                }
                else
                {
                    result = false;
                    LastCloseBizStatus = true;
                }
            }
            return result;
        }

        public decimal? YieldBracket(DateTime SelectDate, int JnisPkt, string PilihanPkt, string kdhmnuai, MVC_SYSTEM_Models dbr, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, out bool YieldBracketFullMonth)
        {
            decimal? kadar = 0;
            decimal? hasilsawit = 0;
            decimal? hasilsawit2 = 0;
            DateTime Today = new DateTime(SelectDate.Year, SelectDate.Month, 1);
            DateTime Last12Month = Today.AddMonths(-12);
            DateTime LatestMonth = Today;
            YieldBracketFullMonth = false;
            SelectDate = Today;

            int[] blockYield = new int[] { 2, 4, 6, 8, 10, 12 };
            var isBlockYield = blockYield.Contains(Today.Month);

            if (isBlockYield)
            {
                SelectDate = SelectDate.AddMonths(-1);
                Today = new DateTime(SelectDate.Year, SelectDate.Month, 1);
                Last12Month = Today.AddMonths(-12);
                LatestMonth = Today;
                SelectDate = Today;
            }

            switch (JnisPkt)
            {
                case 1:
                    for (var fordate = Last12Month; SelectDate > fordate; fordate = fordate.AddMonths(1))
                    {

                        var hasilluas = dbr.tbl_HasilSawitPkt.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodPeringkat == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => new { s.fld_HasilTan, s.fld_LuasHektar }).FirstOrDefault();
                        if (hasilluas != null)
                        {
                            if (hasilluas.fld_HasilTan == null)
                            {
                                hasilsawit2 = 0;
                                YieldBracketFullMonth = false;
                                break;
                            }
                            else
                            {
                                hasilsawit = hasilluas.fld_HasilTan;
                                hasilsawit = hasilsawit / hasilluas.fld_LuasHektar;
                                YieldBracketFullMonth = true;
                                hasilsawit2 += Math.Round(hasilsawit.Value, 2);
                            }
                        }
                        else
                        {
                            hasilsawit2 = 0;
                            YieldBracketFullMonth = false;
                            break;
                        }
                    }
                    break;
                case 2:
                    for (var fordate = Last12Month; SelectDate > fordate; fordate = fordate.AddMonths(1))
                    {
                        var hasilluas = dbr.tbl_HasilSawitSubPkt.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodSubPeringkat == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => new { s.fld_HasilTan, s.fld_LuasHektar }).FirstOrDefault();
                        if (hasilluas != null)
                        {
                            if (hasilluas.fld_HasilTan == null)
                            {
                                hasilsawit2 = 0;
                                YieldBracketFullMonth = false;
                                break;
                            }
                            else
                            {
                                hasilsawit = hasilluas.fld_HasilTan;
                                hasilsawit = hasilsawit / hasilluas.fld_LuasHektar;
                                YieldBracketFullMonth = true;
                                hasilsawit2 += Math.Round(hasilsawit.Value, 2);
                            }
                        }
                        else
                        {
                            hasilsawit2 = 0;
                            YieldBracketFullMonth = false;
                            break;
                        }
                    }
                    break;
                case 3:
                    for (var fordate = Last12Month; SelectDate > fordate; fordate = fordate.AddMonths(1))
                    {
                        var hasilluas = dbr.tbl_HasilSawitBlok.Where(x => x.fld_Bulan == fordate.Month && x.fld_Tahun == fordate.Year && x.fld_KodBlok == PilihanPkt && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false).Select(s => new { s.fld_HasilTan, s.fld_LuasHektar }).FirstOrDefault();
                        if (hasilluas != null)
                        {
                            if (hasilluas.fld_HasilTan == null)
                            {
                                hasilsawit2 = 0;
                                YieldBracketFullMonth = false;
                                break;
                            }
                            else
                            {
                                hasilsawit = hasilluas.fld_HasilTan;
                                hasilsawit = hasilsawit / hasilluas.fld_LuasHektar;
                                YieldBracketFullMonth = true;
                                hasilsawit2 += Math.Round(hasilsawit.Value, 2);
                            }
                        }
                        else
                        {
                            hasilsawit2 = 0;
                            YieldBracketFullMonth = false;
                            break;
                        }

                    }
                    break;
            }
            hasilsawit2 = Math.Round(hasilsawit2.Value, 2);
            kadar = hasilsawit2 == 0 ? 0 : db.tbl_UpahMenuai.Where(x => hasilsawit2 >= x.fld_HasilLower && hasilsawit2 <= x.fld_HasilUpper && x.fld_Jadual == kdhmnuai && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fld_Kadar).FirstOrDefault();
            return kadar;
        }

        public bool CheckHariTerabai(string nopkj, DateTime? Tarikh, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            bool Result = false;
            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var GetHariTerabai = dbr.tbl_KerjaHariTerabai.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh == Tarikh && x.fld_JenisCharge == "kong" && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();

            Result = GetHariTerabai > 0 ? true : false;

            return Result;
        }

        public string GetAttType(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, DateTime SelectedDate)
        {
            string Result = "";
            string DefaultCode = "H01";
            //Cuti Hari Minggu
            int getday = (int)SelectedDate.DayOfWeek;//SelectedDate.DayOfWeek.ToString();
            var CheckData = db.tbl_JenisMingguLadang.Where(x => x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            Result = CheckData != null ? "C07" : DefaultCode;
            //Cuti Am
            var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
            Result = CheckHoliday != null ? "C01" : Result;

            return Result;
        }

        public string GetEasyAttType(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, DateTime SelectedDate)
        {
            string Result = "";
            string DefaultCode = "H01";
            //Cuti Hari Minggu
            int getday = (int)SelectedDate.DayOfWeek;//SelectedDate.DayOfWeek.ToString();
            var CheckData = db.tbl_JenisMingguLadang.Where(x => x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
            Result = CheckData != null ? "H02" : DefaultCode;
            //Cuti Am
            var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
            Result = CheckHoliday != null ? "H03" : Result;

            return Result;
        }

        public string GetHadirStatus(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            string ReturnData = "";
            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfFlag2)
                .FirstOrDefault();

            if (getvalue == "hadirkerja")
            {
                ReturnData = "Hadir";
            }
            else
            {
                ReturnData = "Tidak Hadir";
            }

            return ReturnData;
        }

        public string GetHadirCutiDesc(string data, string flag1, int? NegaraID, int? SyarikatID)
        {
            string ReturnData = "";
            ReturnData = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfValue == data && x.fldOptConfFlag1 == flag1 && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfDesc)
                .FirstOrDefault();

            return ReturnData;
        }

        public bool GetCutiAmMgguMatchDate(int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, DateTime SelectedDate, string CutiCode, out string Msg)
        {
            bool Result = true;
            Msg = "";
            if (CutiCode == "C01" || CutiCode == "H03")
            {
                var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
                Result = CheckHoliday != null ? true : false;
                Msg = "Tarikh pilihan bukan cuti am";
            }
            //else if (CutiCode == "C07" || CutiCode == "H02")
            //{
            //    int getday = (int)SelectedDate.DayOfWeek;
            //    var CheckData = db.tbl_MingguNegeri.Where(x => x.fld_NegeriID == CodeNegeri && x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).FirstOrDefault();
            //    Result = CheckData != null ? true : false;
            //    Msg = "Tarikh pilihan bukan hari minggu";
            //}
            else
            {
                var CheckHoliday = db.vw_CutiUmumLdgDetails.Where(x => x.fld_TarikhCuti == SelectedDate && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Deleted == false && x.fld_CutiUmumDeleted == false).FirstOrDefault();
                int getday = (int)SelectedDate.DayOfWeek;
                var CheckData = db.tbl_JenisMingguLadang.Where(x => x.fld_JenisMinggu == getday && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).FirstOrDefault();
                if (CheckHoliday == null && CheckData == null)
                {
                    Result = true;
                }
                else
                {
                    if (CheckHoliday != null)
                    {
                        Msg = "Tarikh pilihan adalah cuti am";
                        Result = false;
                    }
                    //else if (CheckData != null)
                    //{
                    //    Msg = "Tarikh pilihan adalah cuti mingguan";
                    //    Result = false;
                    //}
                    else
                    {
                        Result = true;
                    }


                }
            }
            return Result;
        }

        public short GetDataKerjaStatus(string nopkj, DateTime? Tarikh, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID)
        {
            short Return = 0;

            string host, catalog, user, pass = "";
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID, SyarikatID, NegaraID);
            MVC_SYSTEM_Models dbr = MVC_SYSTEM_Models.ConnectToSqlServer(host, catalog, user, pass);

            var getvalue = db.tblOptionConfigsWebs
                .Where(x => x.fldOptConfFlag1 == "cuti" && x.fldOptConfFlag2 == "xhadirkerja" && x.fldDeleted == false &&
                            x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID)
                .Select(s => s.fldOptConfValue).ToArray();

            var GetKhdiran = dbr.tbl_Kerjahdr.Where(x => x.fld_Tarikh == Tarikh && x.fld_Nopkj == nopkj && getvalue.Contains(x.fld_Kdhdct) && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
            if (GetKhdiran > 0)
            {
                Return = 1;
            }
            else
            {
                var GetDataKerja = dbr.tbl_Kerja.Where(x => x.fld_Nopkj == nopkj && x.fld_Tarikh == Tarikh && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Count();
                if (GetDataKerja > 0)
                {
                    Return = 2;
                }
                else
                {
                    Return = 3;
                }

            }

            return Return;
        }

        public void DeleteReturnSAPPost(Guid PostingID, MVC_SYSTEM_Models dbr)
        {
            var SAPPostReturn = dbr.tbl_SAPPostReturn.Where(x => x.fld_SAPPostRefID == PostingID).ToList();

            if (SAPPostReturn.Count > 0)
            {
                dbr.tbl_SAPPostReturn.RemoveRange(SAPPostReturn);
                dbr.SaveChanges();
            }
        }

        public void AddReturnSAPPost(MVC_SYSTEM_Models dbr, List<tbl_SAPPostReturn> SAPReturnList)
        {
            dbr.tbl_SAPPostReturn.AddRange(SAPReturnList);
            dbr.SaveChanges();
        }

        public bool GetCloseTransaction(MVC_SYSTEM_Models dbr, int NegaraID, int SyarikatID, int WilayahID, int LadangID, int ValidDate, DateTime NowDate)
        {
            bool ReturnData = false;
            DateTime LastMonth = NowDate.AddMonths(-1);
            int MonthLastMonth = LastMonth.Month;
            int YearLastMonth = LastMonth.Year;
            var GetClosingBiz = dbr.tbl_TutupUrusNiaga.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_Month == MonthLastMonth && x.fld_Year == YearLastMonth && x.fld_StsTtpUrsNiaga == true).Count();

            if (GetClosingBiz > 0)
            {
                ReturnData = true;
            }
            else
            {
                if (ValidDate >= NowDate.Day)
                {
                    ReturnData = true;
                }
                else
                {
                    ReturnData = false;
                }
            }

            return ReturnData;
        }
    }
}