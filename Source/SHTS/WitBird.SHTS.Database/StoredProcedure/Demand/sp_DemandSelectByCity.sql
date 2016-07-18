CREATE PROCEDURE sp_DemandSelectByCity
	@PageCount		int,
	@PageIndex		int,
	@City			NVARCHAR(50)
AS
BEGIN
	select COUNT(1) FROM [Demand] where IsActive = 1 and ( @City is null or City = @City )
	select * from 
	(
		select [Id],[UserId],[CategoryId],[Title],[Province],[City],[Area],[StartTime],[EndTime],[TimeLength],[PeopleNumber],[Budget],
		[ViewCount],[IsActive],[InsertTime], ROW_NUMBER() over(order by Id desc) as RowNumber 
		FROM [Demand] where IsActive = 1 and ( @City is null or City = @City )
	 ) as temp 
	where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
		
END
GO