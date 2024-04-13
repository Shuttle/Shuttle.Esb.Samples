CREATE TABLE [dbo].[OrderProcessStatus] (
    [OrderProcessId] UNIQUEIDENTIFIER NOT NULL,
    [Status]         VARCHAR (35)     NOT NULL,
    [StatusDate]     DATETIME         NOT NULL,
    CONSTRAINT [FK_OrderProcessStatus_OrderProcess] FOREIGN KEY ([OrderProcessId]) REFERENCES [dbo].[OrderProcess] ([Id]) ON DELETE CASCADE
);

