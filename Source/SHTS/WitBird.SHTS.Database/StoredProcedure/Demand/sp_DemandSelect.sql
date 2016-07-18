CREATE PROCEDURE sp_DemandSelect
	@PageCount		int,
	@PageIndex		int
AS
BEGIN
	select COUNT(1) FROM [Demand] where IsActive = 1 
	
	select tempDemand.*,[User].UserName from
		(select * from 
		(
			select *, ROW_NUMBER() over(order by Id desc) as RowNumber 
			FROM [Demand] where IsActive = 1 
		 ) as temp 
		where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
		) as tempDemand left join [User] on tempDemand.UserId = [User].UserId
		
END
GO