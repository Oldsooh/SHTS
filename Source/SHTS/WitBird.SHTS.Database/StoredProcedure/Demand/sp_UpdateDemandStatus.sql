CREATE PROCEDURE [dbo].[sp_UpdateDemandStatus]
	@DemandId int,
	@StatusValue int
AS
BEGIN
UPDATE Demand SET [Status] = @StatusValue WHERE Id = @DemandId
END
GO
