CREATE PROCEDURE [dbo].[sp_DeleteDemandQuote]
	@QuoteId int
AS

UPDATE dbo.DemandQuote SET IsActive = 0 WHERE QuoteId = @QuoteId
UPDATE dbo.DemandQuoteHistory SET IsActive = 0 WHERE QuoteId = @QuoteId
