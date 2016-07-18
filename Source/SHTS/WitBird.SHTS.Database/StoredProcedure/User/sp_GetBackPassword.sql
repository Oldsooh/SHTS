CREATE PROCEDURE [dbo].[sp_GetBackPassword]
	@EncryptedPassword nvarchar(max),
	@UserId nvarchar(20)
AS
BEGIN
	Update [User] set EncryptedPassword=@EncryptedPassword,LastUpdatedTime=GETDATE() where UserId = @UserId;
END
GO
