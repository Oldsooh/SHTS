CREATE PROCEDURE [dbo].[sp_CitySelectProvince]
AS
	SELECT * FROM [City] where EntityType = 1 order by Sort