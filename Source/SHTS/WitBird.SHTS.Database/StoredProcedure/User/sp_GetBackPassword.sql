CREATE PROCEDURE [dbo].[sp_GetBackPassword]
	@EncryptedPassword nvarchar(max),
	@Cellphone nvarchar(20)
AS
	Update [User] set EncryptedPassword=@EncryptedPassword,LastUpdatedTime=GETDATE() where Cellphone=@Cellphone;
