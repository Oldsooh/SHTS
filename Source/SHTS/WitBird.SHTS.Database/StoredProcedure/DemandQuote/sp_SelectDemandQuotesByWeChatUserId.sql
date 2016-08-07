CREATE PROCEDURE [dbo].[sp_SelectDemandQuotesByWeChatUserId]
	@WeChatUserId int,
	@PageSize int,
	@PageIndex int
AS
BEGIN
-- total count
SELECT COUNT(1) AS TotalCount FROM dbo.DemandQuote WHERE WeChatUserId = @WeChatUserId AND IsActive = 1

CREATE TABLE #Quotes
(
	QuoteId int,
	DemandId int
)
INSERT INTO #Quotes
-- paging result
SELECT result.QuoteId, result.DemandId FROM (
	SELECT * FROM (
		SELECT *, ROW_NUMBER() OVER(ORDER BY QuoteId DESC) AS RowNumber 
		FROM dbo.DemandQuote temp 
		WHERE temp.WeChatUserId = @WeChatUserId AND temp.IsActive = 1
	) AS pageResult 
	WHERE pageResult.RowNumber > (@PageIndex-1) * @PageSize and pageResult.RowNumber <= @PageIndex * @PageSize 
) AS result
INNER JOIN dbo.Demand demand ON demand.Id = result.DemandId
ORDER BY result.LastUpdatedTimestamp DESC

SELECT * FROM DemandQuote quote INNER JOIN #Quotes ON #Quotes.QuoteId = quote.QuoteId
SELECT * FROM Demand demand INNER JOIN #Quotes ON #Quotes.DemandId = demand.Id

END
GO