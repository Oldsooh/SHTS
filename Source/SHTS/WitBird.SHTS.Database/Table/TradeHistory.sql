CREATE TABLE [dbo].[TradeHistory]
(
	[HistoryId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [HistorySubject] NTEXT NOT NULL, 
    [HistoryBody] NTEXT NOT NULL, 
    [TradeId] INT NOT NULL , 
    [UserId] INT NOT NULL, 
	IsAdminUpdate bit NOT NULL, 
	[UserName] nvarchar(100) NOT NULL, 
    [TradeState] INT NOT NULL, 
    [CreatedTime] DATETIME NOT NULL
)
