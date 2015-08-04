CREATE PROCEDURE [dbo].[sp_SelectExistOrderId]
	@OrderId NVARCHAR(50)
AS

SELECT TOP(1) OrderId FROM dbo.TradeOrder WHERE OrderId = @OrderId
