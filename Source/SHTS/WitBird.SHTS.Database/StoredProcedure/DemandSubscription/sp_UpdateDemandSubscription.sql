CREATE PROCEDURE [dbo].[sp_UpdateDemandSubscription]
	@SubscriptionId int,
	@IsSubscribed bit,
	@LastRequestTimestamp datetime,
	@LastPushTimestamp datetime,
	@LastUpdatedTimestamp datetime,
	@IsEnableEmailSubscription bit,
	@EmailAddress nvarchar(100)
AS
BEGIN
	UPDATE dbo.DemandSubscription SET
	IsSubscribed = @IsSubscribed,
	LastPushTimestamp = @LastPushTimestamp,
	LastUpdatedTimestamp = @LastUpdatedTimestamp,
	IsEnableEmailSubscription = @IsEnableEmailSubscription,
	EmailAddress = @EmailAddress
	WHERE SubscriptionId = @SubscriptionId
END
GO