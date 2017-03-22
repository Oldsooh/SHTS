CREATE TABLE [dbo].[FinanceWithdrawRecord]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [Amount] DECIMAL(18, 2) NOT NULL,
	[WithdrawStatus] NVARCHAR(50) NOT null,
	[BankInfo] NVARCHAR(1000) NOT null, 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NOT NULL
)
