CREATE TABLE [dbo].[UserVip]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [OrderId] NVARCHAR(50) NULL, 
    [IdentifyImg] NVARCHAR(MAX) NULL, 
    [StartTime] DATETIME NULL, 
    [EndTime] DATETIME NULL, 
    [Duration] INT NULL, 
    [Amount] DECIMAL(18, 2) NULL, 
    [State] INT NULL,
    [CreatedTime] DATETIME NULL, 
    [LastUpdatedTime] DATETIME NULL
)
