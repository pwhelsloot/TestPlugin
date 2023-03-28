CREATE TABLE [audit].[RecordException]
(
	[RecordExceptionId]	[int]				NOT NULL	CONSTRAINT [PK_RecordException] PRIMARY KEY CLUSTERED	IDENTITY(1, 1),
	[PostSQL]			[nvarchar](max)		NULL,
	[TableName]			[sysname]			NULL,
	[KeyName]			[sysname]			NULL,
	[ErrorMessage]		[nvarchar](max)		NULL,
	[RowVersion]		[rowversion]		NOT NULL,
	[GUID]				[uniqueidentifier]	NOT NULL	CONSTRAINT [UK_RecordException(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_RecordException(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL
)
