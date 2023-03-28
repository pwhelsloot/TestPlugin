CREATE TABLE [dbo].[GlossaryInternalCache]
(
	[GlossaryInternalCacheId]	[INT]				NOT NULL	CONSTRAINT [PK_GlossaryInternalCache] PRIMARY KEY NONCLUSTERED IDENTITY(1, 1),
	[Original]					[NVARCHAR](200)		NOT NULL,
	[Translated]				[NVARCHAR](200)		NOT NULL,
	[LanguageCode]				[NVARCHAR](30)		NOT NULL,
	[LastChangeReasonId]		[INT]				NULL,
	[RowVersion]				[ROWVERSION]		NOT NULL,
	[GUID]						[UNIQUEIDENTIFIER]	NOT NULL	CONSTRAINT [UK_GlossaryInternalCache(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_GlossaryInternalCache(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
)
GO
CREATE NONCLUSTERED INDEX IX_GlossaryInternalCache_LanguageCode_Original_Translated
ON [GlossaryInternalCache] ([LanguageCode], [Original])
INCLUDE ([Translated]);
