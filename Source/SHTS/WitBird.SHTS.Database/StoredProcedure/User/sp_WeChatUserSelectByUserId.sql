CREATE PROCEDURE [dbo].[sp_WeChatUserSelectByUserId]
	@UserId INT
AS
BEGIN
	SELECT * FROM dbo.WeChatUser WHERE UserId = @UserId
END
GO
