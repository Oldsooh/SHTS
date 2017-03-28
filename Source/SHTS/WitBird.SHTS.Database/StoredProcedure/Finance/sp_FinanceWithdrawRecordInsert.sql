CREATE PROCEDURE [dbo].[sp_FinanceWithdrawRecordInsert]
	@UserId int,
	@Amount decimal(18,2),
	@WithdrawStatus nvarchar(50),
	@BankInfo nvarchar(1000),
	@InsertedTimestamp datetime,
	@LastUpdatedTimestamp datetime
AS
	INSERT INTO FinanceWithdrawRecord
	(
		UserId,
		Amount,
		WithdrawStatus,
		BankInfo,
		InsertedTimestamp,
		LastUpdatedTimestamp
	)
	VALUES
	(
		@UserId,
		@Amount,
		@WithdrawStatus,
		@BankInfo,
		@InsertedTimestamp,
		@LastUpdatedTimestamp
	)

GO
