CREATE PROCEDURE [dbo].[sp_CitySelectByCityId]
	@CityId		NVARCHAR(50)
AS
	SELECT * FROM [City] 
	where EntityType = 3 and ParentId = @CityId 
	order by Sort

