CREATE TABLE [dbo].[AdminRole]
(
	[RoleId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RoleNmae] NVARCHAR(50) NULL, 
    [State] INT NULL, 
    [Permission] NVARCHAR(MAX) NULL, 
    [LastUpdateTime] DATETIME NULL
)
