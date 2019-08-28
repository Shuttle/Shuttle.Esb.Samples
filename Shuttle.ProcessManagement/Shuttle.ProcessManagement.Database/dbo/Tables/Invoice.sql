CREATE TABLE [dbo].[Invoice] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [InvoiceNumber]       VARCHAR (20)     NOT NULL,
    [OrderId]             UNIQUEIDENTIFIER NOT NULL,
    [InvoiceDate]         DATETIME         NOT NULL,
    [AccountContactName]  VARCHAR (65)     NOT NULL,
    [AccountContactEMail] VARCHAR (130)    NOT NULL,
    CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED ([Id] ASC)
);

