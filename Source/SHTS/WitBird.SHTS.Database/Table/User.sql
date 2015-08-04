CREATE TABLE [dbo].[User]
(
    [UserId] INT NOT NULL IDENTITY(1,1),
    [UserName] NVARCHAR(50) NOT NULL,
    [EncryptedPassword] VARCHAR(MAX) NOT NULL,
    [UserType] INT NULL, 
    [Adress] NVARCHAR(MAX) NULL,
	[LocationId] NVARCHAR(200) NULL, 
    [Cellphone] NVARCHAR(20) NULL, 
	[Email] NVARCHAR(MAX) NULL, 
	[QQ] NVARCHAR(20) NULL,
	[UCard] NVARCHAR(MAX) NULL,
	[SiteUrl] NVARCHAR(max) NULL,
    [LoginIdentiy] NVARCHAR(MAX) NOT NULL, 
	[IdentiyImg] NVARCHAR(MAX) NULL,
	[Photo] NVARCHAR(250) NULL,
    [Vip] INT NULL, 
    [CreateTime] DATETIME NULL, 
    [LastUpdatedTime] DATETIME NULL, 
    [State] INT NULL, 
    CONSTRAINT PK_User PRIMARY KEY([UserId] ASC)
)
