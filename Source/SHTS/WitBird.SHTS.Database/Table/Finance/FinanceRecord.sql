CREATE TABLE [dbo].[FinanceRecord]
(
	[Id] INT NOT NULL PRIMARY KEY DEFAULT 1 IDENTITY, 
    [UserId] INT NOT NULL, 
    [FinanceType] NVARCHAR(50) NOT NULL, 
    [Amount] DECIMAL(18, 2) NOT NULL, 
    [Balance] DECIMAL(18, 2) NOT NULL, 
    [Description] NVARCHAR(2000) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NOT NULL
)
