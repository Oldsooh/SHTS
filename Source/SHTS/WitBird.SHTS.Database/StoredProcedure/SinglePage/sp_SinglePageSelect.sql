
CREATE PROCEDURE [dbo].[sp_SinglePageSelect]
	@EntityType		NVARCHAR(50),
	@Category		NVARCHAR(50),
	@IsActive		bit,
	@PageCount		int,
	@PageIndex		int
AS
BEGIN
	select COUNT(1) from SinglePage where IsActive=@IsActive and
	EntityType = @EntityType and
	(@Category is null or Category = @Category) 
	
	select * from 
	(
		select *, ROW_NUMBER() over(order by Id desc) as RowNumber 
		from SinglePage where IsActive=@IsActive and
		EntityType = @EntityType and
		(@Category is null or Category = @Category) 
	 ) as temp 
	where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
END
