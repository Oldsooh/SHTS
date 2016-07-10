CREATE TABLE [dbo].[DemandQuotes]
(
	[QuoteId] INT NOT NULL  IDENTITY, 
    [WeChatUserId] INT NOT NULL, 
    [DemandId] INT NOT NULL, 
    [ContactName] NVARCHAR(50) NULL, 
    [ContactPhoneNumber] NVARCHAR(50) NULL, 
    [QuotePrice] DECIMAL(18, 2) NOT NULL, 
	[HandleStatus] BIT NOT NULL,
    [AcceptStatus] NVARCHAR(50) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NULL,
    CONSTRAINT [PK_DemandQuotes] PRIMARY KEY ([QuoteId]))
