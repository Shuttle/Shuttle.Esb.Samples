CREATE TABLE [dbo].[Projection] (
    [Name]           VARCHAR (650) NOT NULL,
    [SequenceNumber] BIGINT        CONSTRAINT [DF_Projection_SequenceNumber] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Projection] PRIMARY KEY NONCLUSTERED ([Name] ASC)
);

