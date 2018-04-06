CREATE PROCEDURE sp_DemandSelectByParameters
	@PageCount		int,
	@PageIndex		int,
	@Province		NVARCHAR(50),
	@City			NVARCHAR(50),
	@Area			NVARCHAR(50),
	@ResourceType	NVARCHAR(50),
	@ResourceTypeId	NVARCHAR(50),
	-- @StartBudget	NVARCHAR(50),
	-- @EndBudget		NVARCHAR(50),
	@StartTime		NVARCHAR(50),
	@EndTime		NVARCHAR(50),
	@budgetCondition NVARCHAR(200)
AS
BEGIN

	DECLARE @sqlText NVARCHAR(max)
	SET @sqlText = N'
	select COUNT(1) FROM [Demand] where IsActive = 1
	and (@Province is null or Province = @Province)
	and (@City is null or City = @City) 
	and (@Area is null or Area = @Area) 
	and (@ResourceType is null or ResourceType = @ResourceType)
	and (@ResourceTypeId is null or ResourceTypeId = @ResourceTypeId)
	-- and (@StartBudget is null or Budget >= @StartBudget)
	-- and (@EndBudget is null or Budget <= @EndBudget)
	and (@StartTime is null or StartTime >= @StartTime) 
	and (@EndTime is null or EndTime <= @EndTime)   '
	
	IF (@budgetCondition <> '')
	BEGIN
		SET @sqlText += N'
		AND ' + @budgetCondition
	END

	SET @sqlText += N'
	select tempDemand.*,[User].UserName, tOrder.Amount as DemandBonus from
		(select * from 
		(
			select *, ROW_NUMBER() over(order by Id desc) as RowNumber 
			FROM [Demand] where IsActive = 1
			and (@Province is null or Province = @Province)
			and (@City is null or City = @City) 
			and (@Area is null or Area = @Area) 
			and (@ResourceType is null or ResourceType = @ResourceType)
			and (@ResourceTypeId is null or ResourceTypeId = @ResourceTypeId)
			-- and (@StartBudget is null or Budget >= @StartBudget)
			-- and (@EndBudget is null or Budget <= @EndBudget)
			and (@StartTime is null or StartTime >= @StartTime) 
			and (@EndTime is null or EndTime <= @EndTime) '
	IF (@budgetCondition <> '')
	BEGIN
		SET @sqlText += N'
		AND ' + @budgetCondition
	END

	SET @sqlText += N'
		 ) as temp 
		where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
		) as tempDemand 
		left join [User] on tempDemand.UserId = [User].UserId
		left join TradeOrder tOrder on tOrder.ResourceId = tempDemand.Id and tOrder.OrderType = 3 -- 需求鼓励金信息'
	
	EXEC sp_executesql @sqlText,
	N'@PageCount		int,
	@PageIndex		int,
	@Province		NVARCHAR(50),
	@City			NVARCHAR(50),
	@Area			NVARCHAR(50),
	@ResourceType	NVARCHAR(50),
	@ResourceTypeId	NVARCHAR(50),
	@StartTime		NVARCHAR(50),
	@EndTime		NVARCHAR(50),
	@budgetCondition NVARCHAR(200)',
	@PageCount		,
	@PageIndex		,
	@Province		,
	@City			,
	@Area			,
	@ResourceType	,
	@ResourceTypeId	,
	@StartTime		,
	@EndTime		,
	@budgetCondition 

END
GO