CREATE PROCEDURE [dbo].[sp_SinglePageSelectById]
	@Id		int
AS
BEGIN
	select * from SinglePage
	where Id=@Id and IsActive = 1 
END