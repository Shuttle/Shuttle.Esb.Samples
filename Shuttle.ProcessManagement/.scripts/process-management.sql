/****** Object:  Table [dbo].[Invoice]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Invoice](
	[Id] [uniqueidentifier] NOT NULL,
	[InvoiceNumber] [varchar](20) NOT NULL,
	[OrderId] [uniqueidentifier] NOT NULL,
	[TotalAmountDue] [decimal](18, 2) NOT NULL,
	[InvoiceDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InvoiceItem]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InvoiceItem](
	[InvoiceId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](130) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Order]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
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

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderItem]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderItem](
	[OrderId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](130) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderProcess]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderProcess](
	[Id] [uniqueidentifier] NOT NULL,
	[OrderId] [uniqueidentifier] NULL,
	[InvoiceId] [uniqueidentifier] NULL,
	[CustomerName] [varchar](65) NOT NULL,
	[CustomerEMail] [varchar](130) NOT NULL,
 CONSTRAINT [PK_OrderProcess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderProcessItem]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderProcessItem](
	[OrderProcessId] [uniqueidentifier] NOT NULL,
	[ProductId] [uniqueidentifier] NOT NULL,
	[Description] [varchar](130) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderProcessStatus]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderProcessStatus](
	[OrderProcessId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](35) NOT NULL,
	[StatusChangeDate] [datetime] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderProcessView]    Script Date: 10/29/2015 4:33:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrderProcessView](
	[Id] [uniqueidentifier] NOT NULL,
	[CustomerName] [varchar](65) NOT NULL,
	[OrderNumber] [varbinary](20) NULL,
	[OrderDate] [datetime] NULL,
	[OrderTotal] [decimal](18, 2) NULL,
	[Status] [varchar](35) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IX_OrderItem]    Script Date: 10/29/2015 4:33:52 PM ******/
CREATE CLUSTERED INDEX [IX_OrderItem] ON [dbo].[OrderItem]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InvoiceItem]  WITH CHECK ADD  CONSTRAINT [FK_InvoiceItem_Invoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoice] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InvoiceItem] CHECK CONSTRAINT [FK_InvoiceItem_Invoice]
GO
ALTER TABLE [dbo].[OrderItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO
ALTER TABLE [dbo].[OrderItem] CHECK CONSTRAINT [FK_OrderItem_Order]
GO
ALTER TABLE [dbo].[OrderProcessItem]  WITH CHECK ADD  CONSTRAINT [FK_OrderProcessItem_OrderProcess] FOREIGN KEY([OrderProcessId])
REFERENCES [dbo].[OrderProcess] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderProcessItem] CHECK CONSTRAINT [FK_OrderProcessItem_OrderProcess]
GO
ALTER TABLE [dbo].[OrderProcessStatus]  WITH CHECK ADD  CONSTRAINT [FK_OrderProcessStatus_OrderProcess] FOREIGN KEY([OrderProcessId])
REFERENCES [dbo].[OrderProcess] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderProcessStatus] CHECK CONSTRAINT [FK_OrderProcessStatus_OrderProcess]
GO
