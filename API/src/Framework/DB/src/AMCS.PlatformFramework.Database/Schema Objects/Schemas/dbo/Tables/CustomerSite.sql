CREATE TABLE [dbo].[CustomerSite]
(
	[CustomerSiteId]			[INT]				NOT NULL	CONSTRAINT [PK_CustomerSite] PRIMARY KEY NONCLUSTERED  IDENTITY(1, 1),
	[Name]						[NVARCHAR](200)		NOT NULL,
	[LocationId]				[INT]				NOT NULL,
	[LastChangeReasonId]		[INT]				NULL,
	[RowVersion]				[ROWVERSION]		NOT NULL,
	[GUID]						[UNIQUEIDENTIFIER]	NOT NULL	CONSTRAINT [UK_CustomerSite(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_CustomerSite(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
	CONSTRAINT [FK_CustomerSite_Location] FOREIGN KEY ([LocationId]) REFERENCES [Location]([LocationId])
)
GO
