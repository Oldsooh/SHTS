CREATE TABLE [dbo].[QuotePriceCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Condition] NVARCHAR(500) NOT NULL, 
    [DisplayName] NCHAR(10) NOT NULL, 
    [DisplayOrder] INT NOT NULL, 
    [IsActive] BIT NOT NULL
)
