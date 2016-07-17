CREATE PROCEDURE [dbo].[sp_SelectDemandQuoteWithHistories]
	@QuoteId int
AS
	-- Update history read status
	UPDATE dbo.DemandQuoteHistory SET HasRead = 1 WHERE QuoteId = @QuoteId;

	-- Select quote
	SELECT TOP 1 quote.*, demand.Title, wechatUser.NickName FROM dbo.DemandQuote quote
	INNER JOIN dbo.Demand demand ON demand.Id = quote.DemandId
	INNER JOIN dbo.WeChatUser wechatUser ON wechatUser.Id = quote.WeChatUserId
	WHERE quote.QuoteId = @QuoteId AND quote.IsActive = 1;

	-- Select quote histories
	SELECT * FROM dbo.DemandQuoteHistory WHERE QuoteId = @QuoteId AND IsActive = 1;
