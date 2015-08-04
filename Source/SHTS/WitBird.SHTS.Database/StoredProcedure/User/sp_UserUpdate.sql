CREATE PROCEDURE [dbo].[sp_UpdateUser]
@UserId int,
@Adress nvarchar(MAX),
@LocationId nvarchar(200),
@Cellphone nvarchar(20),
@Email nvarchar(MAX),
@QQ nvarchar(20),
@UCard nvarchar(MAX),
@SiteUrl nvarchar(MAX),
@Vip int,
@Photo NVARCHAR(250),
@IdentiyImg NVARCHAR(250)

 AS 
	
	Update [User] set Adress=@Adress,
	LocationId=@LocationId,
	Cellphone=@Cellphone,
	Email=@Email,
	QQ=@QQ,
	UCard=@UCard,
	SiteUrl=@SiteUrl,
	Vip=@Vip,
	Photo=@Photo,
	IdentiyImg=@IdentiyImg
	Where UserId=@UserId;