/****** Object:  StoredProcedure [dbo].[sp_Payslip]    Script Date: 27/8/2023 7:15:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		< Zaty >
-- Create date: < 17/04/2018 >
-- Description:	< Report Payslip>
-- =============================================
ALTER PROCEDURE [dbo].[sp_Payslip]
	@NegaraID INT,
	@SyarikatID INT,
	@WilayahID INT,
	@LadangID INT,
	@Month INT,
	@Year INT,
	@Nopkj varchar(20)
AS
	DECLARE @flag int
	DECLARE @desc varchar(10)
	DECLARE @dbhq nvarchar(50)
	DECLARE @dbname nvarchar(50)
	
	DECLARE @tbl_payslip TABLE(
		fldID INT IDENTITY(1,1),
		fldNopkj varchar(20),
		fldKodPkt varchar(10),
		fldKod varchar(10),
		fldKeterangan varchar(200),
		fldKuantiti numeric(18,2),
		fldUnit varchar(10),
		fldKadar numeric(18,2),
		fldGandaan int,
		fldJumlah numeric(18,2),
		fldBulan int,
		fldTahun int,
		fldNegaraID int,
		fldSyarikatID int,
		fldWilayahID int,
		fldLadangID int,
		fldFlag int,
		primary key (fldID)
	)
BEGIN

--------------------------------------------------CARUMAN---------------------------------------------------
set @flag=1	

	--KWSP/SOCSO (M)--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		fld_Nopkj,
		Caruman,
		case Caruman
			when 'KWSP_M' then 'KWSP (M)'
			when 'SOCSO_M' then 'SOCSO (M)'
			else Caruman end as 'Caruman',
		Kadar,
		@flag,
		fld_Month,
		fld_Year,
		fld_NegaraID,
		fld_SyarikatID,
		fld_WilayahID,
		fld_LadangID
		from(
			select fld_Nopkj,
			fld_KWSPMjk as KWSP_M, fld_SocsoMjk as SOCSO_M,
			fld_Month,fld_Year,fld_NegaraID,fld_SyarikatID,fld_WilayahID,fld_LadangID
			from [tbl_GajiBulanan]
			where fld_Nopkj=@Nopkj and fld_Month=@Month and fld_Year=@Year and 
			fld_NegaraID=@NegaraID and fld_SyarikatID=@SyarikatID and  fld_WilayahID=@WilayahID and fld_LadangID=@LadangID)p
		unpivot
			(Kadar for Caruman in (KWSP_M,Socso_M) )
		as unpvt
	)
	
	--SIP/caruman tambahan (M)--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_KodCaruman,
		b.fld_KodCaruman,
		b.fld_CarumanMajikan,
		@flag,
		b.fld_Month,
		b.fld_Year,
		b.fld_NegaraID,
        b.fld_SyarikatID,
        b.fld_WilayahID,
        b.fld_LadangID
		from [tbl_GajiBulanan] a,[tbl_ByrCarumanTambahan] b
		where a.fld_Nopkj=@Nopkj and b.fld_Month=@Month and b.fld_Year=@Year and a.fld_ID=b.fld_GajiID
		and b.fld_NegaraID=@NegaraID and b.fld_SyarikatID=@SyarikatID and b.fld_WilayahID=@WilayahID and b.fld_LadangID=@LadangID
	)
--------------------------------------------------PENDAPATAN---------------------------------------------------	
set @flag=2	
	
	--Kerja--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKodPkt,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldGandaan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodPkt,
		a.fld_KodAktvt,
		b.fld_Desc + ' - ' + a.fld_KodPkt,
		SUM(a.fld_JumlahHasil)as JumlahHasil, 
		a.fld_Unit,
		a.fld_KadarByr, 
		d.fldOptConfFlag3,
		SUM(a.fld_OverallAmount)as Amount,
		@flag,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		FROM [tbl_Kerja] a,[OPMSCORP].[dbo].[tbl_UpahAktiviti] b,[tbl_Kerjahdr] c,[OPMSCORP].[dbo].[tblOptionConfigsWeb] d
		where a.fld_Nopkj=@Nopkj and MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_KodAktvt=b.fld_KodAktvt and b.fld_NegaraID=a.fld_NegaraID and b.fld_SyarikatID=a.fld_SyarikatID
		and c.fld_Nopkj=a.fld_Nopkj and c.fld_Tarikh=a.fld_Tarikh and c.fld_Kum=a.fld_Kum and c.fld_NegaraID=a.fld_NegaraID and c.fld_SyarikatID=a.fld_SyarikatID and c.fld_WilayahID=a.fld_WilayahID and c.fld_LadangID=a.fld_LadangID
		and d.fldOptConfFlag1='cuti' and d.fldOptConfValue=c.fld_Kdhdct and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID
		and c.fld_Hujan = 0
		group by a.fld_KodAktvt,d.fldOptConfFlag3,a.fld_Nopkj,a.fld_Unit,a.fld_KadarByr,b.fld_Desc,a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh),a.fld_KodPkt
	)

	--Kerja terabai--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKodPkt,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldGandaan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodPkt,
		a.fld_KodAktvt,
		b.fld_Desc + ' - ' + a.fld_KodPkt + ' (Hari Terabai)',
		SUM(a.fld_JumlahHasil)as JumlahHasil, 
		a.fld_Unit,
		a.fld_KadarByr, 
		d.fldOptConfFlag3,
		SUM(a.fld_OverallAmount)as Amount,
		@flag,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		FROM [tbl_Kerja] a,[OPMSCORP].[dbo].[tbl_UpahAktiviti] b,[tbl_Kerjahdr] c,[OPMSCORP].[dbo].[tblOptionConfigsWeb] d
		where a.fld_Nopkj=@Nopkj and MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_KodAktvt=b.fld_KodAktvt and b.fld_NegaraID=a.fld_NegaraID and b.fld_SyarikatID=a.fld_SyarikatID
		and c.fld_Nopkj=a.fld_Nopkj and c.fld_Tarikh=a.fld_Tarikh and c.fld_Kum=a.fld_Kum and c.fld_NegaraID=a.fld_NegaraID and c.fld_SyarikatID=a.fld_SyarikatID and c.fld_WilayahID=a.fld_WilayahID and c.fld_LadangID=a.fld_LadangID
		and d.fldOptConfFlag1='cuti' and d.fldOptConfValue=c.fld_Kdhdct and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID
		and c.fld_Hujan = 1
		group by a.fld_KodAktvt,d.fldOptConfFlag3,a.fld_Nopkj,a.fld_Unit,a.fld_KadarByr,b.fld_Desc,a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh),a.fld_KodPkt
	)
	
	--OT--
	set @desc='OT'
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		@desc,
		'Lebih Masa '+d.fldOptConfFlag3,
		SUM(a.fld_JamOT),
		b.fldOptConfDesc,
		a.fld_Kadar,
		SUM(a.fld_Jumlah),
		@flag, 
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjaOT] a, [OPMSCORP].[dbo].[tblOptionConfigsWeb] b, [tbl_Kerjahdr] c, [OPMSCORP].[dbo].[tblOptionConfigsWeb] d
		where a.fld_Nopkj=@Nopkj and MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and b.fldOptConfFlag1='unit' and b.fldOptConfValue='jam' and b.fldDeleted=0 and b.fld_NegaraID=a.fld_NegaraID and b.fld_SyarikatID=a.fld_SyarikatID
		and c.fld_Nopkj=a.fld_Nopkj and c.fld_Tarikh=a.fld_Tarikh 
		and c.fld_NegaraID=a.fld_NegaraID and c.fld_SyarikatID=a.fld_SyarikatID and c.fld_WilayahID=a.fld_WilayahID and c.fld_LadangID=a.fld_LadangID
		and d.fldOptConfFlag1='kiraot' and d.fldOptConfFlag2=c.fld_Kdhdct and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID and d.fldDeleted=0
		group by d.fldOptConfFlag3,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh),a.fld_Nopkj,a.fld_Kadar,b.fldOptConfDesc,
		a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)
	
	--bonus harian hari biasa--
	set @desc='Bonus'
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKodPkt,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		NULL as fld_KodPkt,
		b.fld_KodAktvt,
		d.fld_Desc + ' - ' + b.fld_KodPkt + ' - Insentif Harian Hadir Hari Biasa (' + cast(a.fld_Bonus as nvarchar) + '%)',
		COUNT(a.fld_Tarikh),
		c.fldOptConfDesc,
		(cast(a.fld_Bonus as decimal) / 100) * a.fld_Kadar as fld_Kadar,
		SUM(a.fld_Jumlah),
		@flag,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjaBonus] a,[tbl_Kerja] b,[OPMSCORP].[dbo].[tblOptionConfigsWeb] c,[OPMSCORP].[dbo].[tbl_UpahAktiviti] d, [tbl_Kerjahdr] e
		where a.fld_Nopkj=@Nopkj and MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID and a.fld_WilayahID=b.fld_WilayahID and a.fld_LadangID=b.fld_LadangID
		and a.fld_KerjaID=b.fld_ID and c.fldOptConfFlag1='unit' and c.fldOptConfValue='HARI' and d.fld_KodAktvt=b.fld_KodAktvt and c.fld_NegaraID = a.fld_NegaraID and c.fld_SyarikatID = a.fld_SyarikatID
		and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID
		and e.fld_Tarikh = a.fld_Tarikh and e.fld_Nopkj = a.fld_Nopkj and e.fld_LadangID = a.fld_LadangID and e.fld_Kdhdct = 'H01'
		group by  b.fld_KodPkt,b.fld_KodAktvt,a.fld_Nopkj,a.fld_Kadar,c.fldOptConfDesc,d.fld_Desc, a.fld_Bonus,
		YEAR(a.fld_Tarikh),MONTH(a.fld_Tarikh),a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)

	--bonus harian bukan hari biasa--
	set @desc='Bonus'
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKodPkt,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		NULL as fld_KodPkt,
		b.fld_KodAktvt,
		d.fld_Desc + ' - ' + b.fld_KodPkt + ' - Insentif Harian Hadir Cuti Rehat/Umum (' + cast(a.fld_Bonus as nvarchar) + '%)',
		COUNT(a.fld_Tarikh),
		c.fldOptConfDesc,
		(cast(a.fld_Bonus as decimal) / 100) * a.fld_Kadar as fld_Kadar,
		SUM(a.fld_Jumlah),
		@flag,
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjaBonus] a,[tbl_Kerja] b,[OPMSCORP].[dbo].[tblOptionConfigsWeb] c,[OPMSCORP].[dbo].[tbl_UpahAktiviti] d, [tbl_Kerjahdr] e
		where a.fld_Nopkj=@Nopkj and MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID and a.fld_WilayahID=b.fld_WilayahID and a.fld_LadangID=b.fld_LadangID
		and a.fld_KerjaID=b.fld_ID and c.fldOptConfFlag1='unit' and c.fldOptConfValue='HARI' and d.fld_KodAktvt=b.fld_KodAktvt and c.fld_NegaraID = a.fld_NegaraID and c.fld_SyarikatID = a.fld_SyarikatID
		and d.fld_NegaraID=a.fld_NegaraID and d.fld_SyarikatID=a.fld_SyarikatID
		and e.fld_Tarikh = a.fld_Tarikh and e.fld_Nopkj = a.fld_Nopkj and e.fld_LadangID = a.fld_LadangID and e.fld_Kdhdct <> 'H01'
		group by  b.fld_KodPkt,b.fld_KodAktvt,a.fld_Nopkj,a.fld_Kadar,c.fldOptConfDesc,d.fld_Desc, a.fld_Bonus,
		YEAR(a.fld_Tarikh),MONTH(a.fld_Tarikh),a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)

	--byr cuti--
	set @desc='HARI'
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_Kdhdct,
		c.fldOptConfDesc,
		COUNT(b.fld_Kdhdct),
		@desc,
		a.fld_Jumlah,
		SUM(a.fld_Jumlah),
		@flag, 
		MONTH(a.fld_Tarikh),
		YEAR(a.fld_Tarikh),
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_KerjahdrCuti] a,[tbl_Kerjahdr] b, [OPMSCORP].[dbo].[tblOptionConfigsWeb] c
		where a.fld_Nopkj=@Nopkj and MONTH(a.fld_Tarikh)=@Month and YEAR(a.fld_Tarikh)=@Year 
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID 
		and a.[fld_KerjahdrID]=b.[fld_UniqueID] and c.fldOptConfFlag1='cuti' 
		--and b.[fld_Kdhdct] NOT IN ('C02') --comment sbb nak bayar cuti tahunan pd current month 
		and c.fldOptConfValue=b.fld_Kdhdct and c.fld_NegaraID=b.fld_NegaraID and c.fld_SyarikatID=b.fld_SyarikatID
		GROUP BY a.fld_Nopkj,b.fld_Kdhdct,a.fld_Jumlah,c.fldOptConfDesc,MONTH(a.fld_Tarikh),YEAR(a.fld_Tarikh),
		a.fld_NegaraID,a.fld_SyarikatID,a.fld_WilayahID,a.fld_LadangID
	)
	
	--byr cuti tahunan--
	set @desc='HARI'
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldKuantiti,
		fldUnit,
		fldKadar,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodCuti,
		b.fldOptConfDesc,
		a.fld_JumlahCuti,
		@desc,
		a.fld_Kadar,
		a.fld_JumlahAmt,
		@flag,
		a.fld_Month,
		a.fld_Year,
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		FROM [tbl_KerjahdrCutiTahunan] a,[OPMSCORP].[dbo].[tblOptionConfigsWeb]b 
		where a.fld_Nopkj=@Nopkj and a.fld_Month=@Month and a.fld_Year=@Year and 
		a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID and 
		a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID and b.fldOptConfFlag1='cuti' and b.fldOptConfValue=a.fld_KodCuti
	)	

	
	--insentif--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodInsentif,
		b.fld_Keterangan,
		a.fld_NilaiInsentif,
		@flag,
		a.fld_Month,
		a.fld_Year,
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_Insentif] a,[OPMSCORP].[dbo].[tbl_JenisInsentif] b
		where a.fld_Nopkj=@Nopkj and a.fld_KodInsentif like 'P%' and a.fld_KodInsentif=b.fld_KodInsentif and a.fld_Deleted=0
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID
		and a.fld_Month=@Month and a.fld_Year=@Year
	)

	--AIPS--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		fld_Nopkj,
		'AIPS',
		'AIPS',
		fld_AIPS,
		@flag,
		fld_Month,
		fld_Year,
		fld_NegaraID,
        fld_SyarikatID,
        fld_WilayahID,
        fld_LadangID
		from [tbl_GajiBulanan]
		where fld_Nopkj=@Nopkj and fld_Month=@Month and fld_Year=@Year
		and fld_NegaraID=@NegaraID and fld_SyarikatID=@SyarikatID and fld_WilayahID=@WilayahID and fld_LadangID=@LadangID
	)
	
	
	
--------------------------------------------------POTONGAN---------------------------------------------------
set @flag=3

	--KWSP/SOCSO (P)--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		fld_Nopkj,
		Caruman,
		case Caruman
			when 'KWSP_P' then 'KWSP (P)'
			when 'SOCSO_P' then 'SOCSO (P)'
			else Caruman end as 'Caruman',
		Kadar,
		@flag,
		fld_Month,
		fld_Year,
		fld_NegaraID,
		fld_SyarikatID,
		fld_WilayahID,
		fld_LadangID
		from(
			select fld_Nopkj,fld_KWSPPkj as KWSP_P,fld_SocsoPkj as SOCSO_P,
			fld_Month,fld_Year,fld_NegaraID,fld_SyarikatID,fld_WilayahID,fld_LadangID
			from [tbl_GajiBulanan]
			where fld_Nopkj=@Nopkj and fld_Month=@Month and fld_Year=@Year and 
			fld_NegaraID=@NegaraID and fld_SyarikatID=@SyarikatID and  fld_WilayahID=@WilayahID and fld_LadangID=@LadangID)p
		unpivot
			(Kadar for Caruman in (KWSP_P,Socso_P) )
		as unpvt
	)
	
	--SIP/caruman tambahan (P)--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		b.fld_KodCaruman,
		b.fld_KodCaruman,
		b.fld_CarumanPekerja,
		@flag,
		b.fld_Month,
		b.fld_Year,
		b.fld_NegaraID,
        b.fld_SyarikatID,
        b.fld_WilayahID,
        b.fld_LadangID
		from [tbl_GajiBulanan] a,[tbl_ByrCarumanTambahan] b
		where fld_Nopkj=@Nopkj and b.fld_Month=@Month and b.fld_Year=@Year and a.fld_ID=b.fld_GajiID
		and b.fld_NegaraID=@NegaraID and b.fld_SyarikatID=@SyarikatID and b.fld_WilayahID=@WilayahID and b.fld_LadangID=@LadangID
	)
	
	--potongan--
	insert into @tbl_payslip
	(
		fldNopkj,
		fldKod,
		fldKeterangan,
		fldJumlah,
		fldFlag,
		fldBulan,
		fldTahun,
		fldNegaraID,
		fldSyarikatID,
		fldWilayahID,
		fldLadangID
	)
	(
	select 
		a.fld_Nopkj,
		a.fld_KodInsentif,
		b.fld_Keterangan,
		a.fld_NilaiInsentif,
		@flag,
		a.fld_Month,
		a.fld_Year,
		a.fld_NegaraID,
		a.fld_SyarikatID,
		a.fld_WilayahID,
		a.fld_LadangID
		from [tbl_Insentif] a,[OPMSCORP].[dbo].[tbl_JenisInsentif] b
		where a.fld_Nopkj=@Nopkj and a.fld_KodInsentif like 'T%' and a.fld_KodInsentif=b.fld_KodInsentif and a.fld_Deleted=0
		and a.fld_NegaraID=@NegaraID and a.fld_SyarikatID=@SyarikatID and a.fld_WilayahID=@WilayahID and a.fld_LadangID=@LadangID
		and a.fld_NegaraID=b.fld_NegaraID and a.fld_SyarikatID=b.fld_SyarikatID
		and a.fld_Month=@Month and a.fld_Year=@Year
	)
	
	
END
SELECT * FROM @tbl_payslip
RETURN 0
