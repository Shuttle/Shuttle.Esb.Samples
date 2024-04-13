CREATE TABLE [dbo].[EventType] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [TypeName] VARCHAR (160)    NOT NULL,
    CONSTRAINT [PK_EventType] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_EventType]
    ON [dbo].[EventType]([TypeName] ASC);

