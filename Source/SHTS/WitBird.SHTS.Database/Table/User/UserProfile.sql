CREATE TABLE [dbo].[UserProfile]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] NCHAR(10) NULL, 
    [ProfileType] NVARCHAR(50) NULL, 
    [Value] NVARCHAR(MAX) NULL, 
    [CreatedTime] DATETIME NULL, 
    [LastUpdatedTime] DATETIME NULL, 
    [State] INT NULL
)
