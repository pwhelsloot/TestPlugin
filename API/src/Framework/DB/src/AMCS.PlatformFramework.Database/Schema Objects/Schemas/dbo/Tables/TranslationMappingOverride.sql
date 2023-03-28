CREATE TABLE [dbo].[TranslationMappingOverride] --FOR FRAMEWORK TO WORK
(
	[TranslationMappingOverrideId]		[INT]				NOT NULL	CONSTRAINT [PK_TranslationMappingOverride] PRIMARY KEY CLUSTERED	IDENTITY(1, 1),
	[ProjectIdentifier]					[NVARCHAR](120)		NOT NULL,
	[MappingId]							[INT]				NOT NULL,
	[Mapping]							[NVARCHAR](510)		NOT NULL,
	[LastChangeReasonId]				[INT]				NULL,
	[RowVersion]						[ROWVERSION]		NOT NULL,
	[GUID]								[UNIQUEIDENTIFIER]	NOT NULL	CONSTRAINT [UK_TranslationMappingOverride(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_TranslationMappingOverride(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL
)
