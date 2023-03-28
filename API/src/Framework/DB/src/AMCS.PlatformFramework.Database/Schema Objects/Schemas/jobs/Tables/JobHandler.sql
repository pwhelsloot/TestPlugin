CREATE TABLE [jobs].[JobHandler]
(
  [JobHandlerId] INT NOT NULL CONSTRAINT [PK_JobHandler] PRIMARY KEY CLUSTERED IDENTITY(1, 1),
  [GUID] UNIQUEIDENTIFIER NOT NULL,
  [Type] NVARCHAR(400) NOT NULL,
  [DisplayName] NVARCHAR(MAX) NULL,
  [AllowScheduling] BIT NOT NULL,
  [DuplicateMode] INT NOT NULL,
  [Running] UNIQUEIDENTIFIER NULL,
  [Queued] NVARCHAR(MAX) NULL,
  [Configuration] NVARCHAR(MAX) NOT NULL
)
GO

CREATE UNIQUE INDEX [IX_JobHandler_GUID] ON [jobs].[JobHandler] ([GUID])
GO

CREATE UNIQUE INDEX [IX_JobHandler_Type] ON [jobs].[JobHandler] ([Type])
GO
