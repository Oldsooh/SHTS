CREATE PROCEDURE [dbo].[sp_UpdateTradeState]
	@TradeId int,
	@TradeState int,
	@LastUpdatedTime datetime
AS
	UPDATE dbo.Trade SET [State] = @TradeState, LastUpdatedTime=@LastUpdatedTime WHERE TradeId = @TradeId
