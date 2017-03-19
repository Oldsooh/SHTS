CREATE TABLE [dbo].[FinanceWithdrawRecord]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NULL, 
    [Amount] DECIMAL(18, 2) NULL,
	[WithdrawStatus] NVARCHAR(50) null,
	[BankInfo] NVARCHAR(1000) null, 
    [InsertedTimestamp] NCHAR(10) NULL, 
    [LastUpdatedTimestamp] NCHAR(10) NULL
)
