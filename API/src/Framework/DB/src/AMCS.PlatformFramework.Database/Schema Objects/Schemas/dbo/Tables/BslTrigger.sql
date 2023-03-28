CREATE TABLE [dbo].[BslTrigger]
(
	[BslTriggerId]				[INT]								NOT NULL	CONSTRAINT [PK_BslTriggerId] PRIMARY KEY CLUSTERED IDENTITY(1,1),
	[Description]				[NVARCHAR](60)						NULL,
	[TriggerEntity]				[NVARCHAR](300)						NOT NULL,
	[TriggerOnCreate]			[BIT]								NOT NULL,
	[TriggerOnUpdate]			[BIT]								NOT NULL,
	[TriggerOnDelete]			[BIT]								NOT NULL,
	[Action]					[NVARCHAR](MAX)						NOT NULL,
	[ActionConfiguration]		[NVARCHAR](MAX)						NULL,
	[ActionConfigurationHash]	[NVARCHAR](40)						NULL,
	[ActionGuid]				[UNIQUEIDENTIFIER]					NOT NULL,
	[UseJobSystem]				[BIT]								NOT NULL,
	[SystemCategory]			[NVARCHAR](50)						NULL,
	[LastChangeReasonId]		[INT]								NULL,
	[RowVersion]				[ROWVERSION]						NOT NULL,
	[GUID]						[UNIQUEIDENTIFIER]					NOT NULL	CONSTRAINT [UK_BslTrigger(GUID)] UNIQUE NONCLUSTERED CONSTRAINT [DF_BslTrigger(GUID)] DEFAULT NEWSEQUENTIALID() ROWGUIDCOL
)
GO
CREATE UNIQUE NONCLUSTERED INDEX  [UK_BslTrigger([TriggerEntity,ActionConfigurationHash,ActionGuid,SystemCategory)] ON [dbo].[BslTrigger] ([TriggerEntity],[ActionConfigurationHash],[ActionGuid],[SystemCategory])
GO