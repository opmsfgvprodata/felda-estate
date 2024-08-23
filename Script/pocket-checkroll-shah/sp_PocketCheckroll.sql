-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE sp_PocketCheckroll 
	-- Add the parameters for the stored procedure here
	@NegaraID INT,
	@SyarikatID INT,
	@WilayahID INT,
	@LadangID INT,
	@Month INT,
	@Year INT,
	@Workers [dbo].[Workers] readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM tbl_Pkjmast WHERE fld_LadangID = @LadangID AND fld_Nopkj IN (SELECT fld_Nopkj FROM @Workers) ORDER BY fld_Nopkj ASC

	SELECT * FROM tbl_Kerjahdr WITH (NOLOCK) WHERE fld_LadangID = @LadangID AND YEAR(fld_Tarikh) = @Year AND MONTH(fld_Tarikh) = @Month AND fld_Nopkj IN (SELECT fld_Nopkj FROM @Workers) ORDER BY fld_Nopkj ASC, fld_Tarikh ASC

	SELECT * FROM tbl_Kerja WITH (NOLOCK) WHERE fld_LadangID = @LadangID AND YEAR(fld_Tarikh) = @Year AND MONTH(fld_Tarikh) = @Month AND fld_Nopkj IN (SELECT fld_Nopkj FROM @Workers) ORDER BY fld_Nopkj ASC, fld_Tarikh ASC

	SELECT * FROM tbl_KerjaKesukaran WITH (NOLOCK) WHERE fld_LadangID = @LadangID AND YEAR(fld_Tarikh) = @Year AND MONTH(fld_Tarikh) = @Month AND fld_Nopkj IN (SELECT fld_Nopkj FROM @Workers) ORDER BY fld_Nopkj ASC, fld_Tarikh ASC

	SELECT * FROM tbl_KerjahdrCuti WITH (NOLOCK) WHERE fld_LadangID = @LadangID AND YEAR(fld_Tarikh) = @Year AND MONTH(fld_Tarikh) = @Month AND fld_Nopkj IN (SELECT fld_Nopkj FROM @Workers) ORDER BY fld_Nopkj ASC, fld_Tarikh ASC

	SELECT * FROM tbl_KerjaOT WITH (NOLOCK) WHERE fld_LadangID = @LadangID AND YEAR(fld_Tarikh) = @Year AND MONTH(fld_Tarikh) = @Month AND fld_Nopkj IN (SELECT fld_Nopkj FROM @Workers) ORDER BY fld_Nopkj ASC, fld_Tarikh ASC

	SELECT * FROM tbl_Insentif WITH (NOLOCK) WHERE fld_LadangID = @LadangID AND fld_Year = @Year AND fld_Month = @Month AND fld_Nopkj IN (SELECT fld_Nopkj FROM @Workers) ORDER BY fld_Nopkj ASC
END
GO
