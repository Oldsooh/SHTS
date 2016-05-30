CREATE PROCEDURE [dbo].[sp_UpdateDemandStatus]
	@DemandId int,
	@StatusValue int
AS

UPDATE Demand SET [Status] = @StatusValue WHERE Id = @DemandId
