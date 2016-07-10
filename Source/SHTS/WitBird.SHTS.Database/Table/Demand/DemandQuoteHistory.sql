CREATE TABLE [dbo].[DemandQuoteHistory]
(
	[HistoryId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [QuoteId] INT NOT NULL, 
    [WeChatUserId] INT NOT NULL, 
    [Comments] NVARCHAR(1000) NOT NULL, 
    [HasRead] BIT NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL--, 
    --CONSTRAINT [FK_DemandQuoteHistory_DemandQuotes] FOREIGN KEY ([QuoteId]) REFERENCES [DemandQuotes]([QuoteId])
)
