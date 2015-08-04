CREATE PROCEDURE [dbo].[sp_GetUserBankInfos]
	@UserId int
AS
	SELECT * FROM dbo.UserBankInfo WHERE UserId = @UserId
