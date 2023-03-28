CREATE TABLE [dbo].[Job]
(
	[JobId]						[INT]				NOT NULL	CONSTRAINT [PK_Job] PRIMARY KEY NONCLUSTERED  IDENTITY(1, 1),
	[Date]						[DATETIME]			NOT NULL,
	[Created]					[DATETIME]			NOT NULL,
	[Modified]					[DATETIME]			NOT NULL,
	[Executed]					[DATETIME]			NULL,
	[DateRequired]				[DATETIME]			NULL,
	[StartTimeRequired]			[DATETIME]			NULL,
	[EndTimeRequired]			[DATETIME]			NULL,
	[CustomerSiteId]			[INT]				NOT NULL,
	[LastChangeReasonId]		[INT]				NULL,
	[RowVersion]				[ROWVERSION]		NOT NULL,
	[GUID]						[UNIQUEIDENTIFIER]	NOT NULL	CONSTRAINT [UK_Job(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_Job(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
	CONSTRAINT [FK_Job_CustomerSite] FOREIGN KEY ([CustomerSiteId]) REFERENCES [CustomerSite]([CustomerSiteId])
)
GO
