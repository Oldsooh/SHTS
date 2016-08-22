CREATE PROCEDURE sp_DemandSelectByParameters
	@PageCount		int,
	@PageIndex		int,
	@Province		NVARCHAR(50),
	@City			NVARCHAR(50),
	@Area			NVARCHAR(50),
	@ResourceType	NVARCHAR(50),
	@ResourceTypeId	NVARCHAR(50),
	@StartBudget	NVARCHAR(50),
	@EndBudget		NVARCHAR(50),
	@StartTime		NVARCHAR(50),
	@EndTime		NVARCHAR(50)
AS
BEGIN
	select COUNT(1) FROM [Demand] where IsActive = 1
	and (@Province is null or Province = @Province)
	and (@City is null or City = @City) 
	and (@Area is null or Area = @Area) 
	and (@ResourceType is null or ResourceType = @ResourceType)
	and (@ResourceTypeId is null or ResourceTypeId = @ResourceTypeId)
	and (@StartBudget is null or Budget >= @StartBudget)
	and (@EndBudget is null or Budget <= @EndBudget)
	and (@StartTime is null or StartTime >= @StartTime) 
	and (@EndTime is null or EndTime <= @EndTime)   
	
	select tempDemand.*,[User].UserName from
		(select * from 
		(
			select *, ROW_NUMBER() over(order by Id desc) as RowNumber 
			FROM [Demand] where IsActive = 1
			and (@Province is null or Province = @Province)
			and (@City is null or City = @City) 
			and (@Area is null or Area = @Area) 
			and (@ResourceType is null or ResourceType = @ResourceType)
			and (@ResourceTypeId is null or ResourceTypeId = @ResourceTypeId)
			and (@StartBudget is null or Budget >= @StartBudget)
			and (@EndBudget is null or Budget <= @EndBudget)
			and (@StartTime is null or StartTime >= @StartTime) 
			and (@EndTime is null or EndTime <= @EndTime) 
		 ) as temp 
		where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
		) as tempDemand left join [User] on tempDemand.UserId = [User].UserId
		
END
GO