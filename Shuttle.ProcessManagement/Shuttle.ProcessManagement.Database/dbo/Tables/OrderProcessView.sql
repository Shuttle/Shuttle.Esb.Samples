CREATE TABLE [dbo].[OrderProcessView] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [CustomerName]    VARCHAR (65)     NOT NULL,
    [OrderNumber]     VARCHAR (50)     NOT NULL,
    [OrderDate]       DATETIME         NULL,
    [OrderTotal]      DECIMAL (18, 2)  NULL,
    [Status]          VARCHAR (35)     NULL,
    [TargetSystem]    VARCHAR (65)     NOT NULL,
    [TargetSystemUri] VARCHAR (130)    NOT NULL
);

