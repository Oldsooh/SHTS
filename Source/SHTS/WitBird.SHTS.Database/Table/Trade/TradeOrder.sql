CREATE TABLE [dbo].[TradeOrder]
(
	[OrderId] NVARCHAR(50) NOT NULL PRIMARY KEY, 
    [Amount] DECIMAL(18, 2) NOT NULL, 
    [Subject] NVARCHAR(500) NOT NULL, 
    [Body] NTEXT NOT NULL, 
    [UserName] NVARCHAR(100) NOT NULL, 
    [CreatedTime] DATETIME NOT NULL, 
    [LastUpdatedTime] DATETIME NOT NULL,
    [State] INT NOT NULL, 
    [ResourceUrl] NVARCHAR(1000) NULL, 
    [OrderType] INT NULL, 
    [ResourceId] INT NULL 
)
