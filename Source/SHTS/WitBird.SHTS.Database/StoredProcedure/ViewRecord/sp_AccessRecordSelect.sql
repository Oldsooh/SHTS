CREATE PROCEDURE [dbo].[sp_AccessRecordSelect]
	@UserIP nvarchar(50),
	@AccessUrl nvarchar(1000)
AS
SELECT TOP 1 * FROM dbo.AccessRecord WHERE UserIP = @UserIP AND AccessUrl = @AccessUrl
