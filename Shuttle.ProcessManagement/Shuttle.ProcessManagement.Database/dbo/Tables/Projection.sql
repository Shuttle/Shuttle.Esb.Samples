CREATE TABLE [dbo].[Projection] (
    [Name]           VARCHAR (650) NOT NULL,
    [SequenceNumber] BIGINT        CONSTRAINT [DF_Projection_SequenceNumber] DEFAULT ((0)) NOT NULL,
    [MachineName]    VARCHAR (255) NULL,
    [BaseDirectory]  VARCHAR (260) NULL,
    CONSTRAINT [PK_Projection] PRIMARY KEY NONCLUSTERED ([Name] ASC)
);

