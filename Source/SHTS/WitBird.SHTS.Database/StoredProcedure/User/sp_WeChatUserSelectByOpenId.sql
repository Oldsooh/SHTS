CREATE PROCEDURE [dbo].[sp_WeChatUserSelectByOpenId]
	@OpenId NVARCHAR(50)
AS
	SELECT * FROM dbo.WeChatUser WHERE OpenId = @OpenId
