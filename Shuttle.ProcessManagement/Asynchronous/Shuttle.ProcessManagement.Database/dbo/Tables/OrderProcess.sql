CREATE TABLE [dbo].[OrderProcess] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [OrderId]         UNIQUEIDENTIFIER NULL,
    [InvoiceId]       UNIQUEIDENTIFIER NULL,
    [CustomerName]    VARCHAR (65)     NOT NULL,
    [CustomerEMail]   VARCHAR (130)    NOT NULL,
    [DateRegistered]  DATETIME         NOT NULL,
    [OrderNumber]     VARCHAR (20)     NOT NULL,
    [TargetSystem]    VARCHAR (65)     NOT NULL,
    [TargetSystemUri] VARCHAR (130)    NOT NULL,
    CONSTRAINT [PK_OrderProcess] PRIMARY KEY CLUSTERED ([Id] ASC)
);

