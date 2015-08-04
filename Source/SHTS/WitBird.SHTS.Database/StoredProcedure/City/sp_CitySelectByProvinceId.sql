CREATE PROCEDURE [dbo].[sp_CitySelectByProvinceId]
	@ProvinceId		NVARCHAR(50)
AS
	SELECT * FROM [City] 
	where EntityType = 2 and ParentId = @ProvinceId 
	order by Sort

