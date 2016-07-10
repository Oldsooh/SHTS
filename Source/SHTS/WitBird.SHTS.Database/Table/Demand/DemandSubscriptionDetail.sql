CREATE TABLE [dbo].[DemandSubscriptionDetail]
(
	[SubscriptionDetailId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SubscriptionId] INT NOT NULL, 
    [SubscriptionType] NVARCHAR(50) NOT NULL, 
    [SubscriptionValue] NVARCHAR(500) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL
)
