CREATE PROCEDURE [dbo].[sp_SelectDemandQuoteWithHistories]
	@QuoteId int
AS
BEGIN
	-- Update history read status
	UPDATE dbo.DemandQuoteHistory SET HasRead = 1 WHERE QuoteId = @QuoteId;

	-- Select quote
	SELECT TOP 1 quote.*, demand.Title, wechatUser.NickName, demand.UserId AS DemandUserId, demand.ResourceType AS DemandResourceType,
     demand.ResourceTypeId AS DemandResourceTypeId, demand.Province AS DemandProvince, demand.City AS DemandCity, demand.Area AS DemandArea,
     demand.Address AS DemandAddress, demand.Phone AS DemandPhone, demand.StartTime AS DemandStartTime, demand.EndTime AS DemandEndTime,
     demand.TimeLength AS DemandTimeLength, demand.PeopleNumber AS DemandPeopleNumber, demand.Budget AS DemandBudget, demand.QQWeixin AS DemandQQWeixin,
     demand.Email AS DemandEmail, demand.IsActive AS DemandIsActive, demand.InsertTime AS DemandInsertTime, demand.WeixinBuyFee AS DemandWeixinBuyFee,
     demand.Status AS DemandStatus, demand.ImageUrls AS DemandImageUrls 
	FROM dbo.DemandQuote quote
	INNER JOIN dbo.Demand demand ON demand.Id = quote.DemandId
	INNER JOIN dbo.WeChatUser wechatUser ON wechatUser.Id = quote.WeChatUserId
	WHERE quote.QuoteId = @QuoteId AND quote.IsActive = 1;

	-- Select quote histories
	SELECT * FROM dbo.DemandQuoteHistory WHERE QuoteId = @QuoteId AND IsActive = 1;
END
GO
