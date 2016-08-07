CREATE PROCEDURE [dbo].[sp_SelectDemandQuoteByDemandIdAndWeChatUserId]
	@DemandId int,
	@WeChatUserId int
AS
BEGIN
	SELECT * FROM DemandQuote WHERE DemandId = @DemandId AND WeChatUserId = @WeChatUserId AND IsActive = 1
END
GO
