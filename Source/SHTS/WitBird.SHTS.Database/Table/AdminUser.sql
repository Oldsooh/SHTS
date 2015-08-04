CREATE TABLE [dbo].[AdminUser]
(
	[AdminId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserName] NVARCHAR(50) NOT NULL, 
    [EncryptedPassword] NVARCHAR(MAX) NOT NULL,
	[Role] INT NULL DEFAULT 0,
	[CreateTime] DATETIME NULL, 
    [LastUpdatedTime] DATETIME NULL, 
    [State] INT NULL DEFAULT 0
)
