CREATE PROCEDURE [dbo].[sp_FinanceBalanceSelectByUserId]
	@UserId int
AS
	SELECT * from FinanceBalance WHERE UserId = @UserId
GO
