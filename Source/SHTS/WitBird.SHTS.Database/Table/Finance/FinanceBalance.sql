CREATE TABLE [dbo].[FinanceBalance]
(
	[Id] INT NOT NULL PRIMARY KEY  IDENTITY, 
    [UserId] INT NOT NULL, 
    [AvailableBalance] DECIMAL(18, 2) NOT NULL, 
    [FrozenBalance] DECIMAL(18, 2) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NOT NULL
)
