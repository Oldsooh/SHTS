CREATE PROCEDURE [dbo].[sp_SelectTradeHistory]
	@TradeId int
AS
	SET NOCOUNT ON
	SELECT * FROM dbo.TradeHistory WHERE TradeId = @TradeId

