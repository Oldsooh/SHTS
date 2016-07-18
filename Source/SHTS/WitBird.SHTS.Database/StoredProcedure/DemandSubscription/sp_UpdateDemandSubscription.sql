CREATE PROCEDURE [dbo].[sp_UpdateDemandSubscription]
	@SubscriptionId int,
	@IsSubscribed bit,
	@LastRequestTimestamp datetime,
	@LastPushTimestamp datetime,
	@LastUpdatedTimestamp datetime
AS
BEGIN
	UPDATE dbo.DemandSubscription SET
	IsSubscribed = @IsSubscribed,
	LastPushTimestamp = @LastPushTimestamp,
	LastRequestTimestamp= @LastRequestTimestamp,
	LastUpdatedTimestamp = @LastUpdatedTimestamp
	WHERE SubscriptionId = @SubscriptionId
END
GO