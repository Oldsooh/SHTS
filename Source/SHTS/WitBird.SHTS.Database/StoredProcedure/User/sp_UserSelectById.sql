CREATE PROCEDURE [dbo].[sp_UserSelectById]
	@UserId int
AS
	SELECT * from [User] where [UserId]=@UserId;
