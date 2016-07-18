CREATE PROCEDURE [dbo].[sp_UpdateUserVipInfoByUserId]
	@Id int,
	@OrderId nvarchar(50),
	@IdentifyImg nvarchar(max),
	@StartTime datetime,
	@EndTime datetime,
	@Duration int,
	@Amount decimal(18,2),
	@State int,
	@LastUpdatedTime datetime

AS
BEGIN
UPDATE dbo.UserVip SET
OrderId = @OrderId,
IdentifyImg = @IdentifyImg,
StartTime = @StartTime,
EndTime = @EndTime,
Duration = @Duration,
Amount = @Amount,
[State] = @State,
LastUpdatedTime = @LastUpdatedTime
WHERE Id = @Id
END
GO
