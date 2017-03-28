CREATE PROCEDURE [dbo].[sp_FinanceWithdrawRecordUpdateStatus]
	@RecordId int,
	@WithdrawStatus nvarchar(50),
	@LastUpdatedTimestamp datetime
AS
	UPDATE FinanceWithdrawRecord SET WithdrawStatus = @WithdrawStatus, LastUpdatedTimestamp = @LastUpdatedTimestamp
	WHERE Id = @RecordId
