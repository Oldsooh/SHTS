CREATE PROCEDURE [dbo].[sp_InsertOrUpdateDemandSubscription]
	@SubscriptionId int,
	@WeChatUserId int,
	@IsSubscribed bit,
	@LastRequestTimestamp datetime,
	@LastPushTimestamp datetime,
	@LastUpdatedTimestamp datetime,
	@InsertedTimestamp datetime
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM dbo.DemandSubscription WHERE WeChatUserId = @WeChatUserId)
	BEGIN
		INSERT INTO dbo.DemandSubscription 
		(WeChatUserId, IsSubscribed,LastRequestTimestamp,LastPushTimestamp, InsertedTimestamp, LastUpdatedTimestamp) 
		VALUES(@WeChatUserId, @IsSubscribed,@LastUpdatedTimestamp,@LastUpdatedTimestamp, @LastUpdatedTimestamp, @LastUpdatedTimestamp) 

		SET @SubscriptionId = (SELECT @@IDENTITY)
	END
	ELSE
	BEGIN
		UPDATE dbo.DemandSubscription SET
		IsSubscribed = @IsSubscribed,
		LastPushTimestamp = @LastPushTimestamp,
		LastRequestTimestamp= @LastRequestTimestamp,
		LastUpdatedTimestamp = @LastUpdatedTimestamp
		WHERE SubscriptionId = @SubscriptionId
	END

	SELECT @SubscriptionId
END
GO


