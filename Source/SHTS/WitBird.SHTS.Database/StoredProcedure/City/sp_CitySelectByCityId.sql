CREATE PROCEDURE [dbo].[sp_CitySelectByCityId]
	@CityId		NVARCHAR(50)
AS
BEGIN
	SELECT * FROM [City] 
	where EntityType = 3 and ParentId = @CityId 
	order by Sort
END
GO

