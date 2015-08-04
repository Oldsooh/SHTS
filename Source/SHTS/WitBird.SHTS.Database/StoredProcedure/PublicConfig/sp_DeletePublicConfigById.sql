CREATE PROCEDURE [dbo].[sp_DeletePublicConfigById]
	@ConfigId int
AS
	DELETE FROM dbo.PublicConfig WHERE ConfigId= @ConfigId

SELECT @@IDENTITY
