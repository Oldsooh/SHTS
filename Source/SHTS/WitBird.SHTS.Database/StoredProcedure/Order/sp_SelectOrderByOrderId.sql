CREATE PROCEDURE [dbo].[sp_SelectOrderByOrderId]
	@OrderId nvarchar(50)
AS
BEGIN
	--SET NOCOUNT ON
	SELECT * FROM dbo.TradeOrder WHERE OrderId = @OrderId
END
GO