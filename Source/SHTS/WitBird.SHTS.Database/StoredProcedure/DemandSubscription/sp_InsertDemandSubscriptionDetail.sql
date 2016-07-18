CREATE PROCEDURE [dbo].[sp_InsertDemandSubscriptionDetail]
	@SubscriptionId int,
	@SubscriptionType nvarchar(50),
	@SubscriptionValue nvarchar(500),
	@InsertedTimestamp datetime
AS
BEGIN
	INSERT INTO dbo.DemandSubscriptionDetail
	(SubscriptionId, SubscriptionType, SubscriptionValue, InsertedTimestamp)
	VALUES (@SubscriptionId, @SubscriptionType, @SubscriptionValue, @InsertedTimestamp)
END
GO
