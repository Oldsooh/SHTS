﻿CREATE PROCEDURE [dbo].[sp_WeChatUserDelete]
	@Id INT
AS
	UPDATE dbo.WeChatUser SET State = 1 WHERE Id = @Id
