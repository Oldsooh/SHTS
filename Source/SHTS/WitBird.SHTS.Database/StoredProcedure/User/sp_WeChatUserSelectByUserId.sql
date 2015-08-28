CREATE PROCEDURE [dbo].[sp_WeChatUserSelectByUserId]
	@UserId INT
AS
	SELECT * FROM dbo.WeChatUser WHERE UserId = @UserId
