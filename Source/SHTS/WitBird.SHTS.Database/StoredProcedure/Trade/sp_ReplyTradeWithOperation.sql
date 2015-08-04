CREATE PROCEDURE [dbo].[sp_ReplyTradeWithOperation]
	@HistorySubject ntext,
	@HistoryBody ntext,
	@TradeId int,
	@UserId int,
	@UserName nvarchar(100),
	@IsAdminUpdate bit,
	@TradeState int,
	@CreatedTime datetime
AS
	INSERT INTO dbo.TradeHistory 
	(
		HistorySubject,
		HistoryBody,
		TradeId,
		UserId,
		UserName,
		IsAdminUpdate,
		TradeState,
		CreatedTime
	)
	VALUES
	(
		@HistorySubject,
		@HistoryBody,
		@TradeId,
		@UserId,
		@UserName,
		@IsAdminUpdate,
		@TradeState,
		@CreatedTime
	)
	
	SELECT @@IDENTITY
