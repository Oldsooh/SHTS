CREATE PROCEDURE [dbo].[sp_WeChatUserSelectByOpenId]
	@OpenId NVARCHAR(50)
AS
BEGIN
	SELECT * FROM dbo.WeChatUser WHERE OpenId = @OpenId
END
GO
