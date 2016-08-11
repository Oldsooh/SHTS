CREATE PROCEDURE [dbo].[sp_WeChatUserSelectOnlySubscried]
AS
BEGIN

SELECT * FROM dbo.WeChatUser WHERE HasSubscribed = 1 AND LastRequestTimestamp IS NOT NULL;

END
GO
