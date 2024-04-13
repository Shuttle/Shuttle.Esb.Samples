CREATE TABLE [dbo].[OrderProcessItem] (
    [OrderProcessId] UNIQUEIDENTIFIER NOT NULL,
    [ProductId]      UNIQUEIDENTIFIER NOT NULL,
    [Description]    VARCHAR (130)    NOT NULL,
    [Price]          DECIMAL (18, 2)  NOT NULL,
    CONSTRAINT [FK_OrderProcessItem_OrderProcess] FOREIGN KEY ([OrderProcessId]) REFERENCES [dbo].[OrderProcess] ([Id]) ON DELETE CASCADE
);

