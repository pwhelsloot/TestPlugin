CREATE TABLE [dbo].[DBVersion] (
    [DBVersionId]        INT              IDENTITY (1, 1) NOT NULL,
    [Version]            NVARCHAR (50)    NOT NULL,
    [TimeStamp]          SMALLDATETIME    CONSTRAINT [DF_DBVersion(TimeStamp)] DEFAULT (GETDATE()) NOT NULL,
    [SystemUser]         NVARCHAR (50)    CONSTRAINT [DF_DBVersion(SystemUser)] DEFAULT (suser_sname()) NULL,
    [LastChangeReasonId] INT              NULL,
    [RowVersion]         ROWVERSION       NOT NULL,
    [GUID]               UNIQUEIDENTIFIER CONSTRAINT [DF_DBVersion(GUID)] DEFAULT (newsequentialid()) ROWGUIDCOL NOT NULL,
    CONSTRAINT [PK_DBVersion] PRIMARY KEY CLUSTERED ([DBVersionId] ASC),
    CONSTRAINT [UK_DBVersion(GUID)] UNIQUE NONCLUSTERED ([GUID] ASC),
    CONSTRAINT [UK_DBVersion(Version)] UNIQUE NONCLUSTERED ([Version] ASC)
);
GO
