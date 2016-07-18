CREATE PROCEDURE [dbo].[sp_WeChatUserDeleteByOpenId]
	@OpenId NVARCHAR(50)
AS
BEGIN
	UPDATE dbo.WeChatUser SET HasSubscribed = 0, HasAuthorized = 0 WHERE OpenId = @OpenId
END
GO
