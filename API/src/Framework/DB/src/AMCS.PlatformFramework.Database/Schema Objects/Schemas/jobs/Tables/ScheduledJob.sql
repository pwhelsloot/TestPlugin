CREATE TABLE [jobs].[ScheduledJob]
(
  [ScheduledJobId] INT NOT NULL CONSTRAINT [PK_ScheduledJob] PRIMARY KEY CLUSTERED IDENTITY(1, 1),
  [GUID] UNIQUEIDENTIFIER NOT NULL,
  [Trigger] NVARCHAR(MAX) NOT NULL,
  [NextFireTime] DATETIMEOFFSET NULL,
  [FriendlyName] NVARCHAR(MAX) NULL,
  [Handler] NVARCHAR(MAX) NOT NULL,
  [Queue] NVARCHAR(200) NULL,
  [Parameters] NVARCHAR(MAX) NULL,
  [Deadline] INT NULL,
  [PersistenceMode] INT NULL,
  [DuplicateMode] INT NOT NULL,
  [Created] DATETIMEOFFSET NOT NULL,
  [Updated] DATETIMEOFFSET NOT NULL,
)
GO

CREATE UNIQUE INDEX [IX_ScheduledJob_GUID] ON [jobs].[ScheduledJob] ([GUID])
GO
