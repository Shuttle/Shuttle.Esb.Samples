CREATE TABLE [dbo].[SubscriberMessageType] (
    [MessageType]       VARCHAR (250) NOT NULL,
    [InboxWorkQueueUri] VARCHAR (130) NOT NULL,
    CONSTRAINT [PK_SubscriberMessageType] PRIMARY KEY CLUSTERED ([MessageType] ASC, [InboxWorkQueueUri] ASC)
);

