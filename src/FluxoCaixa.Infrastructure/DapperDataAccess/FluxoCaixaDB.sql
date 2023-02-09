CREATE TABLE [CashFlow](
	[Id] [uniqueidentifier] NOT NULL,
	[Year] [int] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))

GO

CREATE TABLE [Credit](
	[Id] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
	[CashFlowId] [uniqueidentifier] NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))

CREATE TABLE [Debit](
	[Id] [uniqueidentifier] NOT NULL,
	[Amount] [float] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
	[CashFlowId] [uniqueidentifier] NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))

GO
