CREATE TABLE [dbo].[DBTimeZoneConfiguration]
(
	[DBTimeZoneConfigurationId]	[INT]				NOT NULL	CONSTRAINT [PK_DBTimeZoneConfiguration] PRIMARY KEY NONCLUSTERED  IDENTITY(1, 1),
	[NeutralTimeZoneId]			[NVARCHAR](200)		NOT NULL	CONSTRAINT [UK_DBTimeZoneConfiguration(DBTimeZoneConfigurationName)] UNIQUE NONCLUSTERED,
	[LastChangeReasonId]		[INT]				NULL,
	[RowVersion]				[ROWVERSION]		NOT NULL,
	[GUID]						[UNIQUEIDENTIFIER]	NOT NULL	CONSTRAINT [UK_DBTimeZoneConfiguration(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_DBTimeZoneConfiguration(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
)
GO
