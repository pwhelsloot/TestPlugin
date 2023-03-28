CREATE TABLE [dbo].[DBTimeZoneDatabase]
(
	[DBTimeZoneDatabaseId]	[INT]				NOT NULL	CONSTRAINT [DF_DBTimeZoneDatabase_DBTimeZoneDatabaseId] DEFAULT 1,
	[Name]					[NVARCHAR](200)		NOT NULL,
	[Data]					[VARBINARY](MAX)	NOT NULL,
	[Online]				[BIT]				NOT NULL,
	[LastChangeReasonId]	[INT]				NULL,
	[RowVersion]			[ROWVERSION]		NOT NULL,
	[GUID]					[UNIQUEIDENTIFIER]	NOT NULL	CONSTRAINT [UK_DBTimeZoneDatabase(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_DBTimeZoneDatabase(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
	CONSTRAINT [PK_DBTimeZoneDatabase]			PRIMARY KEY NONCLUSTERED (DBTimeZoneDatabaseId),
    CONSTRAINT [CK_DBTimeZoneDatabase_DBTimeZoneDatabaseId]		CHECK (DBTimeZoneDatabaseId=1)
)
GO
