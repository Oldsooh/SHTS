﻿CREATE TABLE [dbo].[SpaceType]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [MarkForDelete] BIT NOT NULL,
    [DisplayOrder] INT NULL DEFAULT 1, 
    CONSTRAINT PK_SpaceType PRIMARY KEY([Id] ASC)
)
