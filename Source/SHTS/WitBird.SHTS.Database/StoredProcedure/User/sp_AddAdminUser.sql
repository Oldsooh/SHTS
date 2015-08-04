CREATE PROCEDURE [dbo].[sp_AddAdminUser]
@AdminId int output,
@UserName nvarchar(50),
@EncryptedPassword nvarchar(MAX),
@Role int,
@CreateTime datetime,
@LastUpdatedTime datetime,
@State int

 AS 
	INSERT INTO [AdminUser](
	[UserName],[EncryptedPassword],[Role],[CreateTime],[LastUpdatedTime],[State]
	)VALUES(
	@UserName,@EncryptedPassword,@Role,@CreateTime,@LastUpdatedTime,@State
	)
	SET @AdminId = @@IDENTITY
