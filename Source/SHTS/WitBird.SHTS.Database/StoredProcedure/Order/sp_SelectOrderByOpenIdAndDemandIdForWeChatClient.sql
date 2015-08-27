CREATE PROCEDURE [dbo].[sp_SelectOrderByOpenIdAndDemandIdForWeChatClient]
	@UserName nvarchar(100),
	@ResourceId int
AS
	SELECT * FROM TradeOrder WHERE UserName = @UserName and ResourceId = @ResourceId AND OrderType = 2
