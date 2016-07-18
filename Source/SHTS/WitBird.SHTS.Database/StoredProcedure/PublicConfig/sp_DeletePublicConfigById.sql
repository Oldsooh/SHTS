CREATE PROCEDURE [dbo].[sp_DeletePublicConfigById]
	@ConfigId int
AS
BEGIN
	DELETE FROM dbo.PublicConfig WHERE ConfigId= @ConfigId

SELECT @@IDENTITY
END
GO
