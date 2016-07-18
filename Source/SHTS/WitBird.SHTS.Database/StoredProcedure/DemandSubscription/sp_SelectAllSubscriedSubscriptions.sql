CREATE PROCEDURE [dbo].[sp_SelectAllSubscriedSubscriptions]
AS
BEGIN
	CREATE TABLE #SubscriptionIds (SubscriptionId int)
	INSERT INTO #SubscriptionIds
	SELECT SubscriptionId FROM DemandSubscription WHERE IsSubscribed = 1

	SELECT subscription.* FROM DemandSubscription subscription 
	INNER JOIN #SubscriptionIds ON #SubscriptionIds.SubscriptionId = subscription.SubscriptionId

	SELECT detail.* FROM DemandSubscriptionDetail detail
	INNER JOIN #SubscriptionIds ON #SubscriptionIds.SubscriptionId = detail.SubscriptionId
END
GO
