CREATE TABLE [dbo].[FinanceOrder]
(
	[OrderId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Amount] DECIMAL(18, 2) NOT NULL, 
    [Title] NVARCHAR(200) NOT NULL, 
    [Detail] NVARCHAR(2000) NOT NULL, 
    [UserId] INT NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL, 
    [Status] NVARCHAR(50) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdateTimestamp] DATETIME NOT NULL
)
