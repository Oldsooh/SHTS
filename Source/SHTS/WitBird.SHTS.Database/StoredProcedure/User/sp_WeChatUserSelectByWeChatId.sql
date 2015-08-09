CREATE PROCEDURE [dbo].[sp_WeChatUserSelectByWeChatId]
	@WeChatId NVARCHAR(50)
AS
	SELECT * FROM dbo.WeChatUser WHERE WeChatId = @WeChatId
