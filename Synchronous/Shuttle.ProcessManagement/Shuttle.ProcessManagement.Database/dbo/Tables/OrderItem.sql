CREATE TABLE [dbo].[OrderItem] (
    [OrderId]     UNIQUEIDENTIFIER NOT NULL,
    [Description] VARCHAR (130)    NOT NULL,
    [Price]       DECIMAL (18, 2)  NOT NULL,
    CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id]) ON DELETE CASCADE
);


GO
CREATE CLUSTERED INDEX [IX_OrderItem]
    ON [dbo].[OrderItem]([OrderId] ASC);

