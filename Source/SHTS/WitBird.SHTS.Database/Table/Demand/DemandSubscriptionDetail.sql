CREATE TABLE [dbo].[DemandSubscriptionDetail]
(
	[SubscriptionDetailId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SubscriptionId] INT NOT NULL, 
    [SubscriptionType] NVARCHAR(50) NOT NULL, 
    [SubscriptionValue] NVARCHAR(500) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    CONSTRAINT [FK_DemandSubscriptionDetail_DemandSubscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [DemandSubscription]([SubscriptionId])
)
