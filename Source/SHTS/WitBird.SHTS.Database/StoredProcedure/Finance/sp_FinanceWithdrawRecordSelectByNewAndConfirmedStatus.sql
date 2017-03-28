CREATE PROCEDURE [dbo].[sp_FinanceWithdrawRecordSelectByNewAndConfirmedStatus]
AS
	SELECT record.*, u.UserName, balance.AvailableBalance FROM FinanceWithdrawRecord record
	LEFT JOIN [User] u ON record.UserId = u.UserId
	INNER JOIN FinanceBalance balance ON balance.UserId = u.UserId
	WHERE record.WithdrawStatus = 'New' OR record.WithdrawStatus = 'Confirmed'
	ORDER BY record.InsertedTimestamp DESC
