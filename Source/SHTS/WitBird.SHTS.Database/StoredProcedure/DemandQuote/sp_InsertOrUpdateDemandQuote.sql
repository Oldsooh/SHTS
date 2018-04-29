CREATE PROCEDURE [dbo].[sp_InsertOrUpdateDemandQuote]
	@QuoteId int,
	@WeChatUserId int,
	@DemandId int,
	@ResourceId int,
	@ContactName nvarchar(50),
	@ContactPhoneNumber nvarchar(50),
	@QuotePrice decimal(18, 2),
	@HandleStatus bit,
	@AcceptStatus nvarchar(50),
	@InsertedTimestamp datetime,
	@LastUpdatedTimestamp datetime,
	@IsActive bit
AS
BEGIN
	-- Update exist record
	IF EXISTS (SELECT 1 FROM dbo.DemandQuote WHERE QuoteId = @QuoteId)
	BEGIN
		UPDATE dbo.DemandQuote SET
			ContactName = @ContactName,
			ContactPhoneNumber = @ContactPhoneNumber,
			QuotePrice = @QuotePrice,
			HandleStatus = @HandleStatus,
			AcceptStatus = @AcceptStatus,
			LastUpdatedTimestamp = @LastUpdatedTimestamp,
			IsActive = @IsActive,
			ResourceId = @ResourceId
		WHERE QuoteId = @QuoteId
	END
	-- Insert new record
	ELSE
	BEGIN
		INSERT INTO dbo.DemandQuote 
		(AcceptStatus, ContactName, ContactPhoneNumber, DemandId, ResourceId, HandleStatus, InsertedTimestamp, IsActive, LastUpdatedTimestamp, QuotePrice, WeChatUserId)
		VALUES (@AcceptStatus, @ContactName, @ContactPhoneNumber, @DemandId, @ResourceId, @HandleStatus, @InsertedTimestamp, @IsActive, @LastUpdatedTimestamp, @QuotePrice, @WeChatUserId)

		SET @QuoteId = (SELECT @@IDENTITY)
	END

	SELECT @QuoteId
END
Go
