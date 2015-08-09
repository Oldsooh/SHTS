CREATE PROCEDURE [dbo].[sp_WeChatUserDeleteByWeChatId]
	@WeChatId NVARCHAR(50)
AS
	UPDATE dbo.WeChatUser SET State = 1 WHERE WeChatId = @WeChatId
