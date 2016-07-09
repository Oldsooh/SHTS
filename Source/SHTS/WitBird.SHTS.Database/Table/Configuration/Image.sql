CREATE TABLE [dbo].[Image]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [ResourceId] INT NOT NULL,
    [Url] VARCHAR(100) NOT NULL,
    [CreateTime] DATETIME NULL,
    [LastUpdatedTime] DATETIME NULL,
    [State] INT NULL,
    CONSTRAINT PK_Image PRIMARY KEY([Id] ASC)
)
