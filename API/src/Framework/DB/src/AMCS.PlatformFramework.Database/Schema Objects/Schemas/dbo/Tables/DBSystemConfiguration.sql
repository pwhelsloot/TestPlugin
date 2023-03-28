CREATE TABLE [dbo].[DBSystemConfiguration]
(
    [DbSystemConfigurationId]       [INT]                NOT NULL    CONSTRAINT [PK_DbSystemConfiguration] PRIMARY KEY NONCLUSTERED  IDENTITY(1, 1),
    [Key]                           [NVARCHAR](200)      NOT NULL    CONSTRAINT [UK_DBSystemConfiguration(Name)] UNIQUE CLUSTERED,
    [Value]                         [NVARCHAR](MAX)      NULL,
    [LastChangeReasonId]            [INT]                NULL,
    [RowVersion]                    [ROWVERSION]         NOT NULL,
    [GUID]                          [UNIQUEIDENTIFIER]   NOT NULL    CONSTRAINT [UK_DbSystemConfiguration(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_DbSystemConfiguration(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
)
GO