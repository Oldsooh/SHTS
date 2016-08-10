CREATE PROCEDURE [dbo].[sp_SelectDemandQuotesByDemandId]
	@DemandId int--,
	--@PageSize int,
	--@PageIndex int
AS
BEGIN
-- total count
SELECT COUNT(1) AS TotalCount FROM dbo.DemandQuote WHERE DemandId = @DemandId AND IsActive = 1

-- paging result
--SELECT result.*, demand.Title FROM (
--	SELECT * FROM (
--		SELECT *, ROW_NUMBER() OVER(ORDER BY QuoteId DESC) AS RowNumber 
--		FROM dbo.DemandQuote temp 
--		WHERE temp.DemandId = @DemandId AND temp.IsActive = 1
--	) AS pageResult 
--	WHERE pageResult.RowNumber > (@PageIndex-1) * @PageSize and pageResult.RowNumber <= @PageIndex * @PageSize 
--) AS result
--INNER JOIN dbo.Demand demand ON demand.Id = result.DemandId
--ORDER BY result.LastUpdatedTimestamp DESC

SELECT * FROM DemandQuote WHERE DemandId = @DemandId ORDER BY HandleStatus ASC
SELECT * FROM DemandQuoteHistory history INNER JOIN DemandQuote quote ON quote.QuoteId = history.QuoteId WHERE quote.DemandId = @DemandId

END
GO
