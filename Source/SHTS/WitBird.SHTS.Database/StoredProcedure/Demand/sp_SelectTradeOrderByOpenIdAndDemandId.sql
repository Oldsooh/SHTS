CREATE PROCEDURE [dbo].[sp_SelectTradeOrderByOpenIdAndDemandId]
	@UserName nvarchar(100),
	@ResourceId int
AS
	SELECT OrderId FROM TradeOrder WHERE UserName = @UserName and ResourceId = @ResourceId AND [State] = 1 AND OrderType = 2 

