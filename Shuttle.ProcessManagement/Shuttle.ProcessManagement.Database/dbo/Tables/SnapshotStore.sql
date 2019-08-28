CREATE TABLE [dbo].[SnapshotStore] (
    [Id]      UNIQUEIDENTIFIER NOT NULL,
    [Version] INT              NOT NULL,
    CONSTRAINT [PK_SnapshotStore] PRIMARY KEY CLUSTERED ([Id] ASC)
);

