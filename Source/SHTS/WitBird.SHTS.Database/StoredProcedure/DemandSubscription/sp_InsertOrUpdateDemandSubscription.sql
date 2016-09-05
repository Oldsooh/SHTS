CREATE PROCEDURE [dbo].[sp_InsertOrUpdateDemandSubscription]
	@SubscriptionId int,
	@WeChatUserId int,
	@IsSubscribed bit,
	@LastPushTimestamp datetime,
	@LastUpdatedTimestamp datetime,
	@InsertedTimestamp datetime,
	@IsEnableEmailSubscription bit,
	@EmailAddress nvarchar(100)
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM dbo.DemandSubscription WHERE WeChatUserId = @WeChatUserId)
	BEGIN
		INSERT INTO dbo.DemandSubscription 
		(WeChatUserId, IsSubscribed,LastPushTimestamp, InsertedTimestamp, LastUpdatedTimestamp, IsEnableEmailSubscription, EmailAddress) 
		VALUES(@WeChatUserId, @IsSubscribed,@LastUpdatedTimestamp, @LastUpdatedTimestamp, @LastUpdatedTimestamp, @IsEnableEmailSubscription, @EmailAddress) 

		SET @SubscriptionId = (SELECT @@IDENTITY)
	END
	ELSE
	BEGIN
		UPDATE dbo.DemandSubscription SET
		IsSubscribed = @IsSubscribed,
		LastPushTimestamp = @LastPushTimestamp,
		LastUpdatedTimestamp = @LastUpdatedTimestamp,
		IsEnableEmailSubscription = @IsEnableEmailSubscription,
		EmailAddress = @EmailAddress
		WHERE SubscriptionId = @SubscriptionId
	END

	SELECT @SubscriptionId
END
GO


