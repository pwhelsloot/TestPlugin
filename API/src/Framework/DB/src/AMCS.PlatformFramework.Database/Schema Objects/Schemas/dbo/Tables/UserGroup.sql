CREATE TABLE [dbo].[UserGroup] (
    [UserGroupId]                   INT              IDENTITY (1, 1) NOT NULL,
    [Name]                          NVARCHAR(100)    NOT NULL,
    [IsAdministrator]               BIT              NOT NULL,
    [LastChangeReasonId]            INT              NULL,
    [RowVersion]                    ROWVERSION       NOT NULL,
    [GUID]                          UNIQUEIDENTIFIER CONSTRAINT [DF_UserGroup(GUID)] DEFAULT (newsequentialid()) ROWGUIDCOL NOT NULL,
    CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED ([UserGroupId] ASC),
    CONSTRAINT [UK_UserGroup(GUID)] UNIQUE NONCLUSTERED ([GUID] ASC),
    CONSTRAINT [UK_UserGroup(Name)] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO
