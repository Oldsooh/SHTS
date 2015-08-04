CREATE PROCEDURE [dbo].[sp_CreateOrUpdateProfile]
@id int,
@userid NVARCHAR(20),
@profiletype NVARCHAR(50),
@value NVARCHAR(MAX),
@state int
AS
	DEClare @UpId int
	SET @UpId=0;

	select @UpId=id from UserProfile
	where ProfileType=@profiletype and UserId=@userid;

	if (@UpId = 0)
	BEGIN
		Insert into UserProfile values(@userid,@profiletype,@value,GetDATe(),GETDATE(),0);
		if(@profiletype='UCard')
		BEGIN
		   Update [User] set UCard=@value where UserId=@userid;
		END
		if(@profiletype='IdentiyImgFile')
		BEGIN
		   Update [User] set IdentiyImg=@value where UserId=@userid;
		END
	END
	ELSE
	BEGIN
	   update UserProfile set Value=@value,LastUpdatedTime=GETDATE(),[State]=@state
	   where Id=@id and UserId=@userid;
	    if(@profiletype='UCard')
		BEGIN
		   Update [User] set UCard=@value where UserId=@userid;
		END
		if(@profiletype='IdentiyImgFile')
		BEGIN
		   Update [User] set IdentiyImg=@value where UserId=@userid;
		END
	END
