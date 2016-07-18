CREATE PROCEDURE [dbo].[sp_SelectDemandQuotesByWeChatUserId]
	@WeChatUserId int,
	@PageSize int,
	@PageIndex int
AS
BEGIN
-- total count
SELECT COUNT(1) AS TotalCount FROM dbo.DemandQuote WHERE WeChatUserId = @WeChatUserId AND IsActive = 1

-- paging result
SELECT result.*, demand.Title FROM (
	SELECT * FROM (
		SELECT *, ROW_NUMBER() OVER(ORDER BY QuoteId DESC) AS RowNumber 
		FROM dbo.DemandQuote temp 
		WHERE temp.WeChatUserId = @WeChatUserId AND temp.IsActive = 1
	) AS pageResult 
	WHERE pageResult.RowNumber > (@PageIndex-1) * @PageSize and pageResult.RowNumber <= @PageIndex * @PageSize 
) AS result
INNER JOIN dbo.Demand demand ON demand.Id = result.DemandId
ORDER BY result.LastUpdatedTimestamp DESC
END
GO