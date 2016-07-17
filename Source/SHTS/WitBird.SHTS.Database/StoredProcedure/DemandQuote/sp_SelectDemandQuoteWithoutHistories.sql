CREATE PROCEDURE [dbo].[sp_SelectDemandQuoteWithoutHistories]
	@QuoteId int
AS
	SELECT TOP 1 quote.*, demand.Title, wechatUser.NickName FROM dbo.DemandQuote quote
	INNER JOIN dbo.Demand demand ON demand.Id = quote.DemandId
	INNER JOIN dbo.WeChatUser wechatUser ON wechatUser.Id = quote.WeChatUserId
	WHERE quote.QuoteId = @QuoteId AND quote.IsActive = 1;
