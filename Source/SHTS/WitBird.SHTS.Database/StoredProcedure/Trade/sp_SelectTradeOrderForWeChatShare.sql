CREATE PROCEDURE [dbo].[sp_SelectTradeOrderForWeChatShare]
	@UserName nvarchar(100),
	@ResourceId int
AS
BEGIN
	SELECT OrderId FROM TradeOrder 
	WHERE UserName = @UserName and ResourceId = @ResourceId AND [State] = 1 AND OrderType = 4 
END
GO

