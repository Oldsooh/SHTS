CREATE TABLE [dbo].[DemandSubscriptionPushHistory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WechatUserId] INT NOT NULL, 
    [OpenId] NVARCHAR(50) NOT NULL, 
    [IsMailSubscribed] BIT NOT NULL, 
    [EmailAddress] NVARCHAR(100) NULL, 
    [DemandId] INT NOT NULL, 
    [WechatStatus] NVARCHAR(50) NOT NULL, 
	[WechatExceptionMessage] NVARCHAR(500) NULL,
    [EmailStatus] NVARCHAR(50) NULL, 
	[EmailExceptionMessage] NVARCHAR(500) NULL,
    [CreatedDateTime] DATETIME NOT NULL
)
