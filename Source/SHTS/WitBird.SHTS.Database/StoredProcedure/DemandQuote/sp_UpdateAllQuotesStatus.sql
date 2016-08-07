CREATE PROCEDURE [dbo].[sp_UpdateAllQuotesStatus]
	@DemandId int,
	@QuoteId int
AS
BEGIN
	UPDATE DemandQuote SET HandleStatus = 1, AcceptStatus = 'Accept' WHERE QuoteId = @QuoteId
	UPDATE DemandQuote SET HandleStatus = 1, AcceptStatus = 'Denied' WHERE DemandId = @DemandId AND QuoteId <> @QuoteId
END
GO
