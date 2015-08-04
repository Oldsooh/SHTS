CREATE TABLE [dbo].[Video]
(
    [Id] INT NOT NULL,
    [ResourceId] INT NOT NULL,
    [Href] NVARCHAR(100) NOT NULL,
    [CreateTime] DATETIME NULL,
    [LastUpdatedTime] DATETIME NULL,
    [State] INT NULL,
    CONSTRAINT PK_Video PRIMARY KEY ([Id] ASC)
)
