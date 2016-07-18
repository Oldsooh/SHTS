CREATE PROCEDURE [dbo].[sp_SetUserToVip]
@userid int,
@Vip int
AS
BEGIN
	Update [User] set Vip=@Vip where UserId=@userid;
END
GO
