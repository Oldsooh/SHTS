CREATE PROCEDURE [dbo].[sp_SetUserToVip]
@userid int,
@Vip int
AS
	Update [User] set Vip=@Vip where UserId=@userid;
