CREATE PROCEDURE [dbo].[sp_GetUserProfiles]
@userid NVARCHAR(20)
AS
BEGIN
	SELECT * from UserProfile where UserId=@userid;
END
GO
