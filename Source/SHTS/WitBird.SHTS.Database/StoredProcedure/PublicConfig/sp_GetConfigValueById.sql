CREATE PROCEDURE [dbo].[sp_GetConfigValueById]
	@ConfigId int
AS
	SELECT * FROM dbo.PublicConfig WHERE ConfigId = @ConfigId
