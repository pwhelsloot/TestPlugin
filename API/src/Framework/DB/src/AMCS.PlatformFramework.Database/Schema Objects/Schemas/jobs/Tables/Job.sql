CREATE TABLE [jobs].[Job]
(
  [JobId] INT NOT NULL CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED IDENTITY(1, 1),
  [GUID] UNIQUEIDENTIFIER NOT NULL,
  [UserId] NVARCHAR(MAX) NOT NULL,
  [FriendlyName] NVARCHAR(MAX) NULL,
  [Handler] NVARCHAR(MAX) NOT NULL,
  [Queue] NVARCHAR(200) NOT NULL,
  [Parameters] NVARCHAR(MAX) NULL,
  [Deadline] DATETIMEOFFSET NULL,
  [DuplicateMode] INT NOT NULL,
  [Created] DATETIMEOFFSET NOT NULL,
  [Updated] DATETIMEOFFSET NOT NULL,
  [Status] INT NOT NULL,
  [Result] NVARCHAR(MAX) NULL,
  [Error] NVARCHAR(MAX) NULL,
  [Log] NVARCHAR(MAX) NULL,
  [Checkpoint] NVARCHAR(MAX) NULL,
  [Runtime] INT NULL,
  [ScheduledJobId] INT NULL,
  CONSTRAINT [FK_Job_ScheduledJob] FOREIGN KEY ([ScheduledJobId]) REFERENCES [jobs].[ScheduledJob]([ScheduledJobId])
)
GO

CREATE UNIQUE INDEX [IX_Job_GUID] ON [jobs].[Job] ([GUID])
GO
