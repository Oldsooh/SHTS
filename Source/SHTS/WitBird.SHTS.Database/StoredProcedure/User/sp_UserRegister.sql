CREATE PROCEDURE [dbo].[sp_UserRegister]
@UserId int output,
@UserName nvarchar(50),
@EncryptedPassword varchar(MAX),
@UserType int,
@Adress nvarchar(MAX),
@LocationId nvarchar(20),
@Cellphone nvarchar(20),
@Email nvarchar(MAX),
@QQ nvarchar(20),
@UCard nvarchar(MAX),
@SiteUrl nvarchar(MAX),
@LoginIdentiy nvarchar(MAX),
@IdentiyImg nvarchar(MAX),
@Vip int,
@CreateTime datetime,
@LastUpdatedTime datetime

 AS 
BEGIN
	IF exists 
	(Select UserId from [User] Where (UserName=@UserName OR Email=@Email) AND [state] = 0) -- deleted
	BEGIN

	   RAISERROR(50103, 16, 1, 'User identity is conflicted.');

	END
	ELSE
	BEGIN
	IF exists(Select UserId from [User] 
    Where (UserName=@UserName OR Email=@Email) AND [state] = 1) -- deleted
	BEGIN
	SET @UserId = (Select UserId from [User] 
    Where (UserName=@UserName OR Email=@Email) AND [state] = 1) 

	UPDATE [User] SET UserName = @UserName, EncryptedPassword = @EncryptedPassword, UserType = @UserType, Adress = @Adress,
	LocationId = @LocationId, Cellphone = @Cellphone, Email = @Email, QQ=@QQ, UCard = @UCard, SiteUrl = @SiteUrl,
	LoginIdentiy = @LoginIdentiy, IdentiyImg = @IdentiyImg, Vip = @Vip, CreateTime = @CreateTime, LastUpdatedTime = @LastUpdatedTime,
	[State] = 0
	 WHERE UserId = @UserId

	END
	ELSE
	BEGIN
	INSERT INTO [User](
	[UserName],[EncryptedPassword],[UserType],[Adress],[LocationId],[Cellphone],[Email],[QQ],[UCard],[SiteUrl],[LoginIdentiy],[IdentiyImg],[Vip],[CreateTime],[LastUpdatedTime],[State]
	)VALUES(
	@UserName,@EncryptedPassword,@UserType,@Adress,@LocationId,@Cellphone,@Email,@QQ,@UCard,@SiteUrl,@LoginIdentiy,@IdentiyImg,@Vip,@CreateTime,@LastUpdatedTime,0
	)
	SET @UserId = @@IDENTITY
	END
	END
END
GO