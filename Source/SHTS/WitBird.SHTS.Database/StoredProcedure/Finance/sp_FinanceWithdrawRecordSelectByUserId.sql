CREATE PROCEDURE [dbo].[sp_FinanceWithdrawRecordSelectByUserId]
	@UserId int
AS
	SELECT * FROM FinanceWithdrawRecord 
	WHERE UserId = @UserId
	ORDER BY LastUpdatedTimestamp DESC
GO
