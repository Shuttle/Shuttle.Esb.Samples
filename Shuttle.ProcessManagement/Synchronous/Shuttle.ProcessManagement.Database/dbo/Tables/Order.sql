CREATE TABLE [dbo].[Order] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [OrderNumber]   VARCHAR (20)     NOT NULL,
    [OrderDate]     DATETIME         NOT NULL,
    [CustomerName]  VARCHAR (65)     NOT NULL,
    [CustomerEMail] VARCHAR (130)    NOT NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC)
);

