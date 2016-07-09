CREATE TABLE [dbo].[UserBankInfo]
(
	[BankId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL,
    [BankName] NVARCHAR(200) NOT NULL, 
    [BankAccount] NVARCHAR(200) NOT NULL, 
    [BankUserName] NVARCHAR(50) NOT NULL, 
    [BankAddress] NVARCHAR(MAX) NOT NULL, 
    [IsDefault] BIT NOT NULL, 
    [CreatedTime] DATETIME NOT NULL, 
    [LastUpdatedTime] DATETIME NOT NULL
)
