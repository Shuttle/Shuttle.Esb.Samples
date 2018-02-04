SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoice]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Invoice](
	[Id] [uniqueidentifier] NOT NULL,
	[InvoiceNumber] [varchar](20) NOT NULL,
	[OrderId] [uniqueidentifier] NOT NULL,
	[InvoiceDate] [datetime] NOT NULL,
	[AccountContactName] [varchar](65) NOT NULL,
	[AccountContactEMail] [varchar](130) NOT NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InvoiceItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[InvoiceItem](
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](130) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL
) ON [PRIMARY]
END
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Order](
	[Id] [uniqueidentifier] NOT NULL,
	[OrderNumber] [varchar](20) NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[CustomerName] [varchar](65) NOT NULL,
	[CustomerEMail] [varchar](130) NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderItem](
	[OrderId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](130) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL
) ON [PRIMARY]
END
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OrderItem]') AND name = N'IX_OrderItem')
CREATE CLUSTERED INDEX [IX_OrderItem] ON [dbo].[OrderItem]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderProcess]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderProcess](
	[Id] [uniqueidentifier] NOT NULL,
	[OrderId] [uniqueidentifier] NULL,
	[InvoiceId] [uniqueidentifier] NULL,
	[CustomerName] [varchar](65) NOT NULL,
	[CustomerEMail] [varchar](130) NOT NULL,
	[DateRegistered] [datetime] NOT NULL,
	[OrderNumber] [varchar](20) NOT NULL,
	[TargetSystem] [varchar](65) NOT NULL,
	[TargetSystemUri] [varchar](130) NOT NULL,
 CONSTRAINT [PK_OrderProcess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderProcessItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderProcessItem](
	[OrderProcessId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](130) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL
) ON [PRIMARY]
END
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderProcessStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderProcessStatus](
	[OrderProcessId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](35) NOT NULL,
	[StatusDate] [datetime] NOT NULL
) ON [PRIMARY]
END
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderProcessView]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderProcessView](
	[Id] [uniqueidentifier] NOT NULL,
	[CustomerName] [varchar](65) NOT NULL,
	[OrderNumber] [varchar](50) NOT NULL,
	[OrderDate] [datetime] NULL,
	[OrderTotal] [decimal](18, 2) NULL,
	[Status] [varchar](35) NULL,
	[TargetSystem] [varchar](65) NOT NULL,
	[TargetSystemUri] [varchar](130) NOT NULL
) ON [PRIMARY]
END
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InvoiceItem_Invoice]') AND parent_object_id = OBJECT_ID(N'[dbo].[InvoiceItem]'))
ALTER TABLE [dbo].[InvoiceItem]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceItem_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([Id])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_InvoiceItem_Invoice]') AND parent_object_id = OBJECT_ID(N'[dbo].[InvoiceItem]'))
ALTER TABLE [dbo].[InvoiceItem] CHECK CONSTRAINT [FK_InvoiceItem_Invoice]
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderItem_Order]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderItem]'))
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderItem_Order]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderItem]'))
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Order]
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderProcessItem_OrderProcess]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderProcessItem]'))
ALTER TABLE [dbo].[OrderProcessItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderProcessItem_OrderProcess] FOREIGN KEY([OrderProcessId])
REFERENCES [dbo].[OrderProcess] ([Id])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderProcessItem_OrderProcess]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderProcessItem]'))
ALTER TABLE [dbo].[OrderProcessItem] CHECK CONSTRAINT [FK_OrderProcessItem_OrderProcess]
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderProcessStatus_OrderProcess]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderProcessStatus]'))
ALTER TABLE [dbo].[OrderProcessStatus]  WITH CHECK ADD  CONSTRAINT [FK_OrderProcessStatus_OrderProcess] FOREIGN KEY([OrderProcessId])
REFERENCES [dbo].[OrderProcess] ([Id])
ON DELETE CASCADE
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_OrderProcessStatus_OrderProcess]') AND parent_object_id = OBJECT_ID(N'[dbo].[OrderProcessStatus]'))
ALTER TABLE [dbo].[OrderProcessStatus] CHECK CONSTRAINT [FK_OrderProcessStatus_OrderProcess]
