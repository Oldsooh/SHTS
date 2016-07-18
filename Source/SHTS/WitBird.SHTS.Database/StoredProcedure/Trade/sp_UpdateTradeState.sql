CREATE PROCEDURE [dbo].[sp_UpdateTradeState]
	@TradeId int,
	@TradeState int,
	@LastUpdatedTime datetime
AS
BEGIN
	UPDATE dbo.Trade SET [State] = @TradeState, LastUpdatedTime=@LastUpdatedTime WHERE TradeId = @TradeId
END
GO
