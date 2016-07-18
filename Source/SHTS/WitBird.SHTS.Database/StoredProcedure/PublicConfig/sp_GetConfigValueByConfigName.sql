CREATE PROCEDURE [dbo].[sp_GetConfigValueByConfigName]
	@ConfigName nvarchar(100)
AS
BEGIN
	SELECT * FROM dbo.PublicConfig WHERE ConfigName = @ConfigName
	END
GO
