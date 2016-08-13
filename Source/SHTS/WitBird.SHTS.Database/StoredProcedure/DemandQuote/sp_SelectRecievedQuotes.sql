CREATE PROCEDURE [dbo].[sp_SelectRecievedQuotes]
	@WeChatUserId int,
	@PageSize int,
	@PageIndex int
AS
BEGIN

-- selects all demands which has quote record.
CREATE TABLE #Demands
(
	DemandId int
)
INSERT INTO #Demands
SELECT DISTINCT demand.Id AS DemandId FROM Demand demand 
INNER JOIN WeChatUser wechatUser ON wechatUser.Id = @WeChatUserId AND demand.UserId = wechatUser.UserId
INNER JOIN DemandQuote quote ON quote.DemandId = demand.Id AND quote.IsActive = 1
WHERE demand.IsActive = 1 --AND demand.[Status] = 1

-- total count
SELECT COUNT(1) FROM #Demands

CREATE TABLE #PagingDemands
(
	DemandId int
)
INSERT INTO #PagingDemands
-- paging result
SELECT pageResult.DemandId FROM (
	SELECT temp.DemandId, ROW_NUMBER() OVER(ORDER BY temp.DemandId DESC) AS RowNumber FROM #Demands temp
) AS pageResult 
WHERE pageResult.RowNumber > (@PageIndex-1) * @PageSize and pageResult.RowNumber <= @PageIndex * @PageSize

SELECT demand.* FROM Demand demand INNER JOIN #PagingDemands ON #PagingDemands.DemandId = demand.Id
SELECT quote.* FROM DemandQuote quote INNER JOIN #PagingDemands ON #PagingDemands.DemandId = quote.DemandId

END
GO