CREATE TABLE [dbo].[DemandSubscriptionPushHistory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [WechatUserId] INT NOT NULL, 
    [OpenId] NVARCHAR(100) NOT NULL, 
    [IsMailSubscribed] BIT NOT NULL, 
    [EmailAddress] NVARCHAR(200) NULL, 
    [DemandId] INT NOT NULL, 
    [WechatStatus] NVARCHAR(500) NOT NULL, 
	[WechatExceptionMessage] NVARCHAR(MAX) NULL,
    [EmailStatus] NVARCHAR(500) NULL, 
	[EmailExceptionMessage] NVARCHAR(MAX) NULL,
    [CreatedDateTime] DATETIME NOT NULL
)
