CREATE PROCEDURE sp_DemandSelectByUserId
	@UserId			int,
	@PageCount		int,
	@PageIndex		int
AS
BEGIN
	
	CREATE TABLE #DemandIds (DemandId int)
	INSERt INTO #DemandIds
	SELECT Id FROM 
	(
		SELECT Id, ROW_NUMBER() OVER(ORDER BY Id DESC) AS RowNumber 
		FROM [Demand] where UserId = @UserId and IsActive = 1 
	 ) AS temp 
	WHERE temp.RowNumber > (@PageIndex-1) * @PageCount AND temp.RowNumber <= @PageIndex * @PageCount

	-- Select total count
	SELECT COUNT(1) FROM [Demand] WHERE UserId = @UserId AND IsActive = 1 
	-- Select demand list
	SELECT * FROM dbo.Demand demand INNER JOIN #DemandIds ids ON ids.DemandId = demand.Id
	-- Select demand quotes without histories
	SELECT * FROM dbo.DemandQuote quote INNER JOIN #DemandIds ids ON quote.DemandId = ids.DemandId AND IsActive = 1

END