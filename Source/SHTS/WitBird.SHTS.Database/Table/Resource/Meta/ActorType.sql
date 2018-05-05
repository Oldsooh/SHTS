CREATE TABLE [dbo].[ActorType]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [MarkForDelete] BIT NOT NULL,
    [DisplayOrder] INT NULL DEFAULT 99, 
    CONSTRAINT PK_ActorWork PRIMARY KEY([Id] ASC)
)
