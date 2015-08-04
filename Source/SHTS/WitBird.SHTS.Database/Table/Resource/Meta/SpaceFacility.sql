CREATE TABLE [dbo].[SpaceFacility]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Value] INT NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [MarkForDelete] BIT NOT NULL,
    CONSTRAINT PK_SpaceFacility PRIMARY KEY([Id] ASC)
)
