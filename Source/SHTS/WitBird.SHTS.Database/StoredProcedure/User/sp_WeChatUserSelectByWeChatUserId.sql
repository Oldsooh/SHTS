CREATE PROCEDURE [dbo].[sp_WeChatUserSelectByWeChatUserId]
	@WeChatUserId INT
AS
BEGIN
	SELECT * FROM dbo.WeChatUser WHERE Id = @WeChatUserId
END
GO
