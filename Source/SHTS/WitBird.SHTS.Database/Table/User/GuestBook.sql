CREATE TABLE [dbo].[GuestBook]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [Content] NVARCHAR(1000) NOT NULL,
    [CreateTime] DATETIME NULL,
    [LastUpdatedTime] DATETIME NULL,
    [State] INT NULL,
    CONSTRAINT PK_GuestBook PRIMARY KEY([Id] ASC)
)
