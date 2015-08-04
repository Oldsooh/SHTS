CREATE PROCEDURE [dbo].[sp_CitySelect]
AS

SELECT * FROM [City] order by EntityType,Sort
