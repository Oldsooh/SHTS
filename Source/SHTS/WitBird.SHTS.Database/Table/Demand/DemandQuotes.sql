CREATE TABLE [dbo].[DemandQuotes]
(
	[QuoteId] INT NOT NULL  IDENTITY, 
    [WeChatUserId] INT NOT NULL, 
    [DemandId] INT NOT NULL, 
    [ContactName] NVARCHAR(50) NULL, 
    [ContactPhoneNumber] NVARCHAR(50) NULL, 
    [QuotePrice] DECIMAL(18, 2) NOT NULL, 
    [AcceptStatus] NVARCHAR(50) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NULL, 
    CONSTRAINT [FK_DemandQuotes_WeChatUser] FOREIGN KEY ([WeChatUserId]) REFERENCES [WeChatUser]([Id]), 
    PRIMARY KEY ([QuoteId]), 
    CONSTRAINT [FK_DemandQuotes_Demand] FOREIGN KEY ([DemandId]) REFERENCES [Demand]([Id])
)
