CREATE PROCEDURE sp_DemandSelectByUserId
	@UserId			int,
	@PageCount		int,
	@PageIndex		int
AS
BEGIN
	select COUNT(1) FROM [Demand] where UserId = @UserId and IsActive = 1 
	
	select * from 
	(
		select *, ROW_NUMBER() over(order by Id desc) as RowNumber 
		FROM [Demand] where UserId = @UserId and IsActive = 1 
	 ) as temp 
	where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
END