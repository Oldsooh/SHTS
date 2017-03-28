CREATE PROCEDURE [dbo].[sp_FinanceWithdrawRecordSelectByRecordId]
	@RecordId int
AS
	SELECT TOP 1 * FROM FinanceWithdrawRecord WHERE Id = @RecordId
GO
