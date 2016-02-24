CREATE TABLE [dbo].[ShortMessageCounter]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [IPAddress] NVARCHAR(20) NOT NULL, 
    [IPCounter] INT NOT NULL, 
    [CellPhone] NVARCHAR(20) NOT NULL, 
    [CellPhoneCounter] INT NOT NULL, 
    [LastSendTime] DATETIME NOT NULL, 
    [IPMaxCounter] INT NOT NULL, 
    [CellPhoneMaxCounter] INT NOT NULL
)
