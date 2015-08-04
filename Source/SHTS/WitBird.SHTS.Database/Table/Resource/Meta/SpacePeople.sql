CREATE TABLE [dbo].[SpacePeople]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [MinCount] INT NOT NULL,
    [MaxCount] INT NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [MarkForDelete] BIT NOT NULL,
    CONSTRAINT PK_SpacePeople PRIMARY KEY([Id] ASC)
)
