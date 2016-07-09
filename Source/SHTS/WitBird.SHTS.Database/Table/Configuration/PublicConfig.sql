CREATE TABLE [dbo].[PublicConfig]
(
	[ConfigId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ConfigName] NVARCHAR(100) NOT NULL, 
    [ConfigValue] NVARCHAR(MAX) NOT NULL, 
    [CreatedTime] DATETIME NOT NULL, 
    [LastUpdatedTime] DATETIME NOT NULL
)
