CREATE TABLE [comms].[HeartbeatConnection]
(
	[HeartbeatConnectionId]		[INT]				NOT NULL	CONSTRAINT [PK_HeartbeatConnection] PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	[ProtocolName]				[NVARCHAR](200)		NOT NULL,
	[InstanceName]				[NVARCHAR](200)		NULL,
	[Status]					[INT]   		    NOT NULL	CONSTRAINT [DF_HeartbeatConnection(Status)] DEFAULT 0,
	[Timestamp]					[DATETIME]			NOT NULL,
	[HeartbeatLatencyInSeconds]	[INT]				NULL,
	[LastChangeReasonId]		[INT]				NULL,
	[RowVersion]				[ROWVERSION]		NOT NULL,
	[GUID]						[UNIQUEIDENTIFIER]	NOT NULL	CONSTRAINT [UK_HeartbeatConnection(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_HeartbeatConnection(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL, 
    
)
