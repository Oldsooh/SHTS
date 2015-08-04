CREATE PROCEDURE [dbo].[sp_DeleteUser]
	@UserId int
AS
	Update [User] set [State]=1 where [UserId]=@UserId;
