CREATE TABLE [dbo].[ActivityType]
(
	[ActivityTypeId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ActivityTypeName] NVARCHAR(50) NULL, 
    [Description] NCHAR(10) NULL,
	[ImageUrl] NVARCHAR(300) NULL, 
    [ViewCount] INT NULL, 
	[State] int NULL, 
    [CreatedTime] DATETIME NULL,
	[LastUpdatedTime] DATETIME NULL
)
