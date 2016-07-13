CREATE PROCEDURE [dbo].[sp_DeleteDemandSubscriptionDetailsBySubscriptionId]
	@SubscriptionId int
AS
	DELETE FROM dbo.DemandSubscriptionDetail WHERE SubscriptionId = @SubscriptionId
