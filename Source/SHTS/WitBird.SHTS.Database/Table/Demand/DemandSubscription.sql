﻿CREATE TABLE [dbo].[DemandSubscription]
(
    [SubscriptionId] INT NOT NULL IDENTITY, 
    [WeChatUserId] INT NOT NULL, 
    [IsSubscribed] BIT NOT NULL, 
    [LastRequestTimestamp] DATETIME NULL, 
    [LastPushTimestamp] DATETIME NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NOT NULL, 
    CONSTRAINT [PK_DemandSubscription] PRIMARY KEY ([SubscriptionId]), 
    CONSTRAINT [FK_DemandSubscription_WeChatUser] FOREIGN KEY ([WeChatUserId]) REFERENCES [WeChatUser]([Id]) 
)
