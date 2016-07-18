CREATE PROCEDURE [dbo].[sp_DeleteDemandSubscriptionDetailsBySubscriptionId]
	@SubscriptionId int
AS
BEGIN
	DELETE FROM dbo.DemandSubscriptionDetail WHERE SubscriptionId = @SubscriptionId
END
GO
