CREATE TABLE [CashFlow](
	[Id] [uniqueidentifier] NOT NULL,	
	[Version] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))

CREATE TABLE [Entry](
	[Id] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
	[CashFlowId] [uniqueidentifier] NULL,
	[EntryType] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))

GO
