CREATE PROCEDURE [dbo].[sp_SelectOrderByOrderId]
	@OrderId nvarchar(50)
AS
	SET NOCOUNT ON
	SELECT * FROM dbo.TradeOrder WHERE OrderId = @OrderId
