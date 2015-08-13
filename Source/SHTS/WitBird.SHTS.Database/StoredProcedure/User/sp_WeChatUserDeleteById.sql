CREATE PROCEDURE [dbo].[sp_WeChatUserDeleteById]
	@Id INT
AS
	UPDATE dbo.WeChatUser SET HasSubscribed = 0, HasAuthorized = 0 WHERE Id = @Id
