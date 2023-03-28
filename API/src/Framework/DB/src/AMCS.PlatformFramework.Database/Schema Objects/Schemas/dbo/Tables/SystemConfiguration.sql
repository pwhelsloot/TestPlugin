CREATE TABLE [dbo].[SystemConfiguration] (
    [SystemConfigurationId]         INT              IDENTITY (1, 1) NOT NULL,
    [Name]                          NVARCHAR(100)    NOT NULL,
    [Value]                         NVARCHAR(MAX)    NULL,
    [LastChangeReasonId]            INT              NULL,
    [RowVersion]                    ROWVERSION       NOT NULL,
    [GUID]                          UNIQUEIDENTIFIER CONSTRAINT [DF_SystemConfiguration(GUID)] DEFAULT (newsequentialid()) ROWGUIDCOL NOT NULL,
    CONSTRAINT [PK_SystemConfiguration] PRIMARY KEY CLUSTERED ([SystemConfigurationId] ASC),
    CONSTRAINT [UK_SystemConfiguration(GUID)] UNIQUE NONCLUSTERED ([GUID] ASC),
    CONSTRAINT [UK_SystemConfiguration(Name)] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO
