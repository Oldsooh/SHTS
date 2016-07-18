CREATE PROCEDURE [dbo].[sp_UserSelectById]
	@UserId int
AS
BEGIN
	SELECT * from [User] where [UserId]=@UserId;
END
GO
