CREATE PROCEDURE [dbo].[sp_FinanceRecordSelectByUserId]
	@UserId int
AS
	SELECT * FROM FinanceRecord WHERE UserId = @UserId
	ORDER BY InsertedTimestamp DESC

GO
