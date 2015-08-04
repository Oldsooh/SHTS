CREATE PROCEDURE [dbo].[sp_AdminLogin]
@UserName nvarchar(50),
@EncryptedPassword varchar(MAX)
AS
	SELECT * 
	from [User] where UserName=@UserName and EncryptedPassword=@EncryptedPassword;