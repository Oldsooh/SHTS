CREATE TABLE [dbo].[WeChatUser]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(112816, 1), 
    [UserId] INT NULL, 
    [OpenId] NVARCHAR(50) NULL, 
    [NickName] NVARCHAR(50) NULL, 
    [Sex] INT NULL DEFAULT 0, 
    [Province] NVARCHAR(50) NULL, 
    [City] NVARCHAR(50) NULL, 
    [County] NVARCHAR(50) NULL, 
    [Photo] NVARCHAR(500) NULL, 
    [AccessToken] NVARCHAR(50) NULL, 
    [AccessTokenExpired] BIT NULL DEFAULT 0, 
    [AccessTokenExpireTime] DATETIME NULL, 
    [HasSubscribed] BIT NULL, 
	[HasAuthorized] BIT NULL, 
    [CreatedTime] DATETIME NULL, 
    [LastRequestTimestamp] DATETIME NULL 
)
