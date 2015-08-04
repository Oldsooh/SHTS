CREATE PROCEDURE [dbo].[sp_GetConfigValueByConfigName]
	@ConfigName nvarchar(100)
AS
	SELECT * FROM dbo.PublicConfig WHERE ConfigName = @ConfigName
