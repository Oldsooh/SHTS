CREATE TABLE [dbo].[DemandSubscription]
(
    [SubscriptionId] INT NOT NULL IDENTITY, 
    [WeChatUserId] INT NOT NULL, 
    [IsSubscribed] BIT NOT NULL, 
    [LastPushTimestamp] DATETIME NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NOT NULL,
    [IsEnableEmailSubscription] BIT NULL, 
    [EmailAddress] NVARCHAR(100) NULL, 
    CONSTRAINT [PK_DemandSubscription] PRIMARY KEY ([SubscriptionId])
)
