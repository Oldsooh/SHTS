CREATE PROCEDURE [dbo].[sp_DeleteUser]
	@UserId int
AS
BEGIN
	Update [User] set [State]=1 where [UserId]=@UserId;
END
GO
