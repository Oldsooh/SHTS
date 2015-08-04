CREATE TABLE [dbo].[Comment]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [ResourceId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [MarkForDelete] BIT NOT NULL,
    [CreateTime] DATETIME NOT NULL,
    CONSTRAINT PK_Comment PRIMARY KEY([Id] ASC)
)
