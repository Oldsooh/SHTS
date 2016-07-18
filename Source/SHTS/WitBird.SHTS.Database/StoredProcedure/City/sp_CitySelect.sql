CREATE PROCEDURE [dbo].[sp_CitySelect]
AS
BEGIN
SELECT * FROM [City] order by EntityType,Sort
END
GO
