CREATE PROCEDURE [dbo].[sp_UpdateWeChatUserLastRequestTime]
	@OpenId nvarchar(100)
AS
BEGIN
	DECLARE @currentTime datetime = GETDATE()

	UPDATE dbo.WeChatUser SET LastRequestTimestamp = @currentTime WHERE OpenId = @OpenId
END
GO