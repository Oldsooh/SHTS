﻿CREATE TABLE [dbo].[AccessRecord]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserIP] NVARCHAR(50) NOT NULL, 
    [AccessUrl] NVARCHAR(1000) NOT NULL, 
    [InsertedTimestamp] DATETIME NOT NULL
)
