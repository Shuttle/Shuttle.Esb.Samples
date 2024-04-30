CREATE TABLE [dbo].[InvoiceItem] (
    [InvoiceId]   UNIQUEIDENTIFIER NOT NULL,
    [Description] VARCHAR (130)    NOT NULL,
    [Price]       DECIMAL (18, 2)  NOT NULL,
    CONSTRAINT [FK_InvoiceItem_Invoice] FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoice] ([Id]) ON DELETE CASCADE
);

