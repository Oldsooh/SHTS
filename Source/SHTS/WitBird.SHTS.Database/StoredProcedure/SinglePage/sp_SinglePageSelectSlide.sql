CREATE PROCEDURE [dbo].[sp_SinglePageSelectSlide]
AS
BEGIN
	select top 5 * from [SinglePage]
	where EntityType = 'News' and IsActive = 1
	order by Id desc
END
GO
