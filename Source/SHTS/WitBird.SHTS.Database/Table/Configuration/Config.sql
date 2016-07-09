CREATE TABLE [dbo].[Config]
(
	[Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(100) NULL,
    [Keywords] NVARCHAR(200) NULL,
    [Description] NVARCHAR(300) NULL,
    [Tel] NVARCHAR(50) NULL, 
    [Phone] NVARCHAR(50) NULL, 
    [Domain] NVARCHAR(50) NULL, 
    [Email] NVARCHAR(50) NULL, 
    [QQ1] NVARCHAR(50) NULL, 
    [QQ2] NVARCHAR(50) NULL, 
    [QQ3] NVARCHAR(50) NULL, 
    [Address] NVARCHAR(300) NULL, 
    [Company] NVARCHAR(50) NULL, 
    [Weixin] NVARCHAR(50) NULL, 
    [ICP] NVARCHAR(50) NULL, 
    [StatisticalCode] NCHAR(10) NULL, 
    CONSTRAINT PK_Config PRIMARY KEY([Id] ASC)
)
