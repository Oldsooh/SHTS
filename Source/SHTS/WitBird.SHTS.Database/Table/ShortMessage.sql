CREATE TABLE [dbo].[ShortMessage]
(
	[MessageId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Cellphone] NVARCHAR(50) NOT NULL, 
	[Provider] NVARCHAR(50) NULL, 
    [CreateTime] DATETIME NULL, 
    [State] INT NULL
)
