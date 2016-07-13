CREATE PROCEDURE [dbo].[sp_SelectDemandSubscriptionByWeChatUserId]
	@WeChatUserId int
AS
	DECLARE @SubscriptionId int
	SELECT @SubscriptionId = SubscriptionId FROM DemandSubscription WHERE WeChatUserId = @WeChatUserId

	SELECT * FROM DemandSubscription WHERE SubscriptionId = @SubscriptionId
	SELECT * FROM DemandSubscriptionDetail WHERe SubscriptionId = @SubscriptionId
