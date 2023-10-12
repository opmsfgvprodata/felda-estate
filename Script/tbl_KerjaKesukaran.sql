/****** Object:  Table [dbo].[tbl_KerjaKesukaran]    Script Date: 5/10/2023 9:35:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_KerjaKesukaran](
	[fld_ID] [uniqueidentifier] NOT NULL,
	[fld_Nopkj] [nvarchar](20) NULL,
	[fld_Kum] [nvarchar](50) NULL,
	[fld_Tarikh] [date] NULL,
	[fld_KodKesukaran] [nvarchar](20) NULL,
	[fld_Kuantiti] [numeric](18, 2) NULL,
	[fld_Kadar] [numeric](18, 2) NULL,
	[fld_Gandaan] [smallint] NULL,
	[fld_Jumlah] [numeric](18, 2) NULL,
	[fld_KerjaID] [uniqueidentifier] NULL,
	[fld_NegaraID] [int] NULL,
	[fld_SyarikatID] [int] NULL,
	[fld_WilayahID] [int] NULL,
	[fld_LadangID] [int] NULL,
	[fld_CreatedBy] [int] NULL,
	[fld_CreatedDT] [datetime] NULL,
 CONSTRAINT [PK_fld_KerjaKesukaran] PRIMARY KEY CLUSTERED 
(
	[fld_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbl_KerjaKesukaran] ADD  CONSTRAINT [DF_fld_KerjaKesukaran_fld_ID]  DEFAULT (newid()) FOR [fld_ID]
GO




/****** Object:  Trigger [dbo].[AfterDeleteTrigger]    Script Date: 5/10/2023 9:34:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER TRIGGER [dbo].[AfterDeleteTrigger] 
	ON [dbo].[tbl_Kerja]
    FOR DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
    DELETE FROM dbo.tbl_KerjaSAPData
    WHERE fld_KerjaID IN(SELECT deleted.fld_ID FROM deleted)
    AND fld_NegaraID IN (SELECT deleted.fld_NegaraID FROM deleted)
    AND fld_SyarikatID IN (SELECT deleted.fld_SyarikatID FROM deleted)
    AND fld_WilayahID IN (SELECT deleted.fld_WilayahID FROM deleted)
    AND fld_LadangID IN (SELECT deleted.fld_LadangID FROM deleted)
    
    
    DELETE FROM dbo.tbl_KerjaOT
    WHERE fld_KerjaID IN(SELECT deleted.fld_ID FROM deleted)
    AND fld_NegaraID IN (SELECT deleted.fld_NegaraID FROM deleted)
    AND fld_SyarikatID IN (SELECT deleted.fld_SyarikatID FROM deleted)
    AND fld_WilayahID IN (SELECT deleted.fld_WilayahID FROM deleted)
    AND fld_LadangID IN (SELECT deleted.fld_LadangID FROM deleted)
    
    DELETE FROM dbo.tbl_KerjaBonus
    WHERE fld_KerjaID IN(SELECT deleted.fld_ID FROM deleted)
    AND fld_NegaraID IN (SELECT deleted.fld_NegaraID FROM deleted)
    AND fld_SyarikatID IN (SELECT deleted.fld_SyarikatID FROM deleted)
    AND fld_WilayahID IN (SELECT deleted.fld_WilayahID FROM deleted)
    AND fld_LadangID IN (SELECT deleted.fld_LadangID FROM deleted)

	 DELETE FROM dbo.tbl_KerjaKesukaran
    WHERE fld_KerjaID IN(SELECT deleted.fld_ID FROM deleted)
    AND fld_NegaraID IN (SELECT deleted.fld_NegaraID FROM deleted)
    AND fld_SyarikatID IN (SELECT deleted.fld_SyarikatID FROM deleted)
    AND fld_WilayahID IN (SELECT deleted.fld_WilayahID FROM deleted)
    AND fld_LadangID IN (SELECT deleted.fld_LadangID FROM deleted)

END
GO

