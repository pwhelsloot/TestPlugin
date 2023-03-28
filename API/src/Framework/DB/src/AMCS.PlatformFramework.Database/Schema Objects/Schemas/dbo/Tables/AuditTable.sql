CREATE TABLE [dbo].[AuditTable]
(
	[AuditTableId]				[int]				NOT NULL	CONSTRAINT [PK_AuditTable] PRIMARY KEY CLUSTERED	IDENTITY(1, 1),
	[TableName]					[sysname]			NOT NULL	CONSTRAINT [UK_AuditTable(TableName)] UNIQUE NONCLUSTERED,
	[AuditOn]					[bit]				NOT NULL	CONSTRAINT [DF_AuditTable(AuditOn)] DEFAULT 0,
	[ChangeReasonRequirementId] [int]				NULL,
	--the category for when creating a new entity
	[NewReasonCategoryId]		[int]				NULL,
	--the category for when editing an existing entity, or if no value for NewReasonCategory this will be displayed when creating 
	--a new entity - this is why the field isn't called EditReasonCategoryId.
	[ReasonCategoryId]			[int]				NULL,
	[LastChangeReasonId]		[int]				NULL,
	[RowVersion]				[rowversion]		NOT NULL,
	[GUID]						[uniqueidentifier]	NOT NULL	CONSTRAINT [UK_AuditTable(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_AuditTable(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AuditTable(TableName)] ON
	[dbo].[AuditTable]([TableName])
INCLUDE
(
	[AuditOn]
)
GO