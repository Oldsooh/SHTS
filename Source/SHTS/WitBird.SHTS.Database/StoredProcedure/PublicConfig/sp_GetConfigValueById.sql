CREATE PROCEDURE [dbo].[sp_GetConfigValueById]
	@ConfigId int
AS
BEGIN
	SELECT * FROM dbo.PublicConfig WHERE ConfigId = @ConfigId
END
GO