CREATE PROCEDURE [dbo].[sp_InsertDemandQuoteHistory]
	@QuoteId int,
	@WeChatUserId int,
	@Comments nvarchar(1000),
	@HasRead bit,
	@InsertedTimestamp datetime,
	@IsActive bit
AS
BEGIN
	INSERT INTO dbo.DemandQuoteHistory
	(Comments, HasRead,InsertedTimestamp, IsActive, QuoteId, WeChatUserId)
	VALUES (@Comments, @HasRead,@InsertedTimestamp, @IsActive, @QuoteId, @WeChatUserId)
END
GO
