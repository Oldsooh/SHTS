CREATE TABLE [dbo].[DemandSubscription]
(
    [SubscriptionId] INT NOT NULL IDENTITY, 
    [WeChatUserId] INT NOT NULL, 
    [IsSubscribed] BIT NOT NULL, 
    [LastPushTimestamp] DATETIME NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NOT NULL,
    CONSTRAINT [PK_DemandSubscription] PRIMARY KEY ([SubscriptionId])
)
