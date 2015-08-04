CREATE TABLE [dbo].[SpaceFeature]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [MarkForDelete] BIT NOT NULL,
    CONSTRAINT PK_SpaceFeature PRIMARY KEY([Id] ASC)
)
