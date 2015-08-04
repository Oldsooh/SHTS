CREATE TABLE [dbo].[AccessAnalytics]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NULL, 
    [AccessUrl] NVARCHAR(MAX) NULL,
	[ReferrerUrl] NVARCHAR(MAX) NULL,
	[Operation] NVARCHAR(50) NULL,   
    [PageTitle] NVARCHAR(50) NULL, 
    [IP] NVARCHAR(50) NULL,
    [CreateTime] DATETIME NULL, 
    [Agent] NVARCHAR(100) NULL, 
    [Device] NVARCHAR(50) NULL
)
