CREATE PROCEDURE [dbo].[sp_FinanceRecordSelectByUserId]
	@UserId int
AS
	SELECT * FROM FinanceRecord WHERE UserId = @UserId
	ORDER BY LastUpdatedTimestamp DESC

GO
