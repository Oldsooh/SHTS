CREATE TABLE [dbo].[SinglePage]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[UserId] INT NULL, 
    [ParentId] INT NULL, 
    [EntityType] NVARCHAR(20) NOT NULL, 
    [Category] NVARCHAR(50) NULL, 
    [Title] NVARCHAR(100) NULL, 
    [Keywords] NVARCHAR(200) NULL, 
    [Description] NVARCHAR(300) NULL, 
    [ContentStyle] TEXT NULL, 
    [ContentText] TEXT NULL, 
    [ImageUrl] NVARCHAR(300) NULL, 
    [Link] NVARCHAR(300) NULL, 
    [IsActive] BIT NULL, 
    [ViewCount] INT NULL, 
    [InsertTime] DATETIME NULL
)
