CREATE PROCEDURE [dbo].[sp_UpdateDemandSubscriptionDateTimeField]
	@WeChatUserId int,
	@FieldName nvarchar(50)
AS
BEGIN
	DECLARE @currentTime datetime = GETDATE()

	UPDATE dbo.DemandSubscription SET @FieldName = @currentTime WHERE WeChatUserId = @WeChatUserId
END
GO