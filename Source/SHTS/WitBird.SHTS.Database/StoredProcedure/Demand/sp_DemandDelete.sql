CREATE PROCEDURE [dbo].[sp_DemandDeleteById]
	@Id		int
AS
BEGIN
	update Demand set IsActive = 0 where Id = @Id
END
GO
