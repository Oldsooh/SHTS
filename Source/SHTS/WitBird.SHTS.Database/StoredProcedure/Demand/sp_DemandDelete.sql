CREATE PROCEDURE [dbo].[sp_DemandDeleteById]
	@Id		int
AS
	update Demand set IsActive = 0 where Id = @Id
