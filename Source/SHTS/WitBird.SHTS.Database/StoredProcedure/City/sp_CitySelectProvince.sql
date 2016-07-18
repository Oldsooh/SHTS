CREATE PROCEDURE [dbo].[sp_CitySelectProvince]
AS
BEGIN
	SELECT * FROM [City] where EntityType = 1 order by Sort
END
GO