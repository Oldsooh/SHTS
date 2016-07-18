CREATE PROCEDURE [dbo].[sp_AdminLogin]
@UserName nvarchar(50),
@EncryptedPassword varchar(MAX)
AS
BEGIN
	SELECT * 
	from [User] where UserName=@UserName and EncryptedPassword=@EncryptedPassword;
END
GO