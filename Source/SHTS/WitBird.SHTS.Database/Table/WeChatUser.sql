CREATE TABLE [dbo].[WeChatUser]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 112816), 
    [UserId] INT NULL, 
    [WeChatId] NVARCHAR(50) NULL, 
    [OpenId] NVARCHAR(50) NULL, 
    [NickName] NVARCHAR(50) NULL, 
    [Sex] INT NULL DEFAULT 0, 
    [Province] NVARCHAR(50) NULL, 
    [City] NVARCHAR(50) NULL, 
    [County] NVARCHAR(50) NULL, 
    [Photo] NVARCHAR(50) NULL, 
    [AccessToken] NVARCHAR(50) NULL, 
    [AccessTokenExpired] BIT NULL DEFAULT 0, 
    [AccessTokenExpireTime] DATETIME NULL, 
    [State] INT NULL, 
    [CreatedTime] DATETIME NULL 
)
