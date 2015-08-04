CREATE PROCEDURE [dbo].[sp_GetUserProfiles]
@userid NVARCHAR(20)
AS
	SELECT * from UserProfile where UserId=@userid;
