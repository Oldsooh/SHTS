CREATE TABLE [dbo].[City]
(
    [Id] NVARCHAR(50) NOT NULL,
	[EntityType] INT NOT NULL , 
    [Name] NVARCHAR(50) NOT NULL, 
    [ParentId] NVARCHAR(50) NOT NULL , 
    [Sort] INT NOT NULL DEFAULT 0, 
    [IsActive] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_City] PRIMARY KEY ([Id])
)
