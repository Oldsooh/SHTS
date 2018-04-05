CREATE PROCEDURE [dbo].[sp_SelectAllSubscriedSubscriptions]
AS
BEGIN

	SELECT subscription.*, wechatUser.OpenId, wechatUser.UserId FROM DemandSubscription subscription 
	LEFT JOIN WeChatUser wechatUser ON subscription.WeChatUserId = wechatUser.Id
	WHERE subscription.IsSubscribed = 1 AND wechatUser.HasSubscribed = 1

	SELECT detail.* FROM DemandSubscriptionDetail detail
	INNER JOIN DemandSubscription subscription ON subscription.SubscriptionId = detail.SubscriptionId
	LEFT JOIN WeChatUser wechatUser ON subscription.WeChatUserId = wechatUser.Id
	WHERE subscription.IsSubscribed = 1 AND wechatUser.HasSubscribed = 1
END
GO