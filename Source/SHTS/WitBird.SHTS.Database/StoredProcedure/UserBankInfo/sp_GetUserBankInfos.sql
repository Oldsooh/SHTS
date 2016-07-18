CREATE PROCEDURE [dbo].[sp_GetUserBankInfos]
	@UserId int
AS
BEGIN
	SELECT * FROM dbo.UserBankInfo WHERE UserId = @UserId
END
GO