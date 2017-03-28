CREATE PROCEDURE [dbo].[sp_FinanceRecordInsert]
	@UserId int,
	@FinanceType nvarchar(50),
	@Amount decimal(18,2),
	@Balance decimal(18, 2),
	@Description nvarchar(2000),
	@InsertedTimestamp datetime,
	@LastUpdatedTimestamp datetime
AS
	INSERT INTO FinanceRecord
	(
		UserId,
		FinanceType,
		Amount,
		Balance,
		[Description],
		InsertedTimestamp,
		LastUpdatedTimestamp
	)
	VALUES
	(
		@UserId,
		@FinanceType,
		@Amount,
		@Balance,
		@Description,
		@InsertedTimestamp,
		@LastUpdatedTimestamp
	)

GO
