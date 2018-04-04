CREATE TABLE [dbo].[BudgetFilters]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Condition] NVARCHAR(500) NOT NULL, 
    [DisplayName] NVARCHAR(500) NOT NULL, 
    [DisplayOrder] INT NOT NULL, 
    [IsActive] BIT NOT NULL
)
