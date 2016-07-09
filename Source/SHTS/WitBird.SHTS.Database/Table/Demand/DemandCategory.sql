CREATE TABLE [dbo].[DemandCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [DisplayOrder] INT NOT NULL, 
    [IsActive] BIT NOT NULL
)
