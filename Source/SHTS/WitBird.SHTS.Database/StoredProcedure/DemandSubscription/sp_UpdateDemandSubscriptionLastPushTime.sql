CREATE PROCEDURE [dbo].[sp_UpdateDemandSubscriptionLastPushTime]
	@WeChatUserId int
AS
BEGIN
	DECLARE @currentTime datetime = GETDATE()

	UPDATE dbo.DemandSubscription SET LastPushTimestamp = @currentTime WHERE WeChatUserId = @WeChatUserId
END
GO