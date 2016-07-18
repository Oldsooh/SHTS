CREATE PROCEDURE [dbo].[sp_SelectTradeHistory]
	@TradeId int
AS
BEGIn
	--SET NOCOUNT ON
	SELECT * FROM dbo.TradeHistory WHERE TradeId = @TradeId
END
GO
