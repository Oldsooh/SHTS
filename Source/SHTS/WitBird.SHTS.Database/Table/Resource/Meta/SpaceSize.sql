CREATE TABLE [dbo].[SpaceSize]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [MinSize] INT NOT NULL,
    [MaxSize] INT NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [MarkForDelete] BIT NOT NULL,
    CONSTRAINT PK_SpaceSize PRIMARY KEY([Id] ASC)
)
