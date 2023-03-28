CREATE TABLE [jobs].[JobLog]
(
  [JobLogId] INT NOT NULL CONSTRAINT [PK_JobLog] PRIMARY KEY CLUSTERED IDENTITY(1, 1),
  [JobId] INT NOT NULL,
  [Created] DATETIMEOFFSET NOT NULL,
  [Status] INT NOT NULL,
  CONSTRAINT [FK_JobLog_Job] FOREIGN KEY ([JobId]) REFERENCES [jobs].[Job]([JobId])
)
GO

CREATE INDEX [IX_JobLog_JobId_Created] ON [jobs].[JobLog] ([JobId], [Created])
GO
