CREATE TABLE [ext].[UdfMetadata]
(
    [UdfMetadataId]           INT               NOT NULL  CONSTRAINT [PK_UdfMetadata] PRIMARY KEY CLUSTERED IDENTITY(1, 1),
    [BusinessObjectName]      NVARCHAR(MAX)     NOT NULL,
    [Namespace]               NVARCHAR(MAX)     NOT NULL,
    [FieldName]               NVARCHAR(MAX)     NOT NULL,
    [DataType]                INT               NOT NULL,
    [Required]                BIT               NOT NULL  DEFAULT 0,
    [Metadata]                NVARCHAR(MAX)     NULL,
    [LastChangeReasonId]	  INT				  NULL,
    [RowVersion]			  [ROWVERSION]		  NOT NULL,
    [GUID]					  [UNIQUEIDENTIFIER]  NOT NULL	CONSTRAINT [UK_UdfMetadata(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_UdfMetadata(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
)
GO
