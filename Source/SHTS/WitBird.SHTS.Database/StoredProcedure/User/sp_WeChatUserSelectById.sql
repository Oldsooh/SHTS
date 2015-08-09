CREATE PROCEDURE [dbo].[sp_WeChatUserSelectById]
	@Id INT
AS
	SELECT * FROM dbo.WeChatUser WHERE Id = @Id
