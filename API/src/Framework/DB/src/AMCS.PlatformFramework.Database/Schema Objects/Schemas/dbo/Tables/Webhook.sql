CREATE TABLE [dbo].[WebHook]
(
	[WebHookId]				[INT]							NOT NULL	CONSTRAINT [PK_WebHook] PRIMARY KEY NONCLUSTERED IDENTITY(1, 1),
	[SystemCategory]		[NVARCHAR](200)					NULL,
	[Name]					[NVARCHAR](200)					NOT NULL,
	[TenantId]             	[UNIQUEIDENTIFIER]				NULL,
	[Trigger]				[NVARCHAR](MAX)					NOT NULL,
	[Format]				[INT]							NOT NULL,
	[HttpMethod]			[NVARCHAR](20)					NOT NULL,
	[BasicCredentials]		[NVARCHAR](MAX)					NULL,
	[Headers]	  			[NVARCHAR](MAX)					NULL,
	[Url]					[NVARCHAR](MAX)					NOT NULL,
	[Filter]				[NVARCHAR](MAX)					NULL,
	[Environment]			[NVARCHAR](MAX)					NOT NULL,
	[InstalledViaRest]		[BIT]							NOT NULL 	CONSTRAINT [DF_WebHook(InstalledViaRest)] DEFAULT 0,
	[LastChangeReasonId] 	[INT]							NULL,
	[RowVersion]	      	[ROWVERSION]					NOT NULL,
	[GUID]	              	[UNIQUEIDENTIFIER]				NOT NULL	CONSTRAINT [UK_WebHook(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_WebHook(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL,
)