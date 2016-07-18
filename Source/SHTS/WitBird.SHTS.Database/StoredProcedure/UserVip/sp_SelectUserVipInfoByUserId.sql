CREATE PROCEDURE [dbo].[sp_SelectUserVipInfoByUserId]
	@UserId int
AS
BEGIN
	IF NOT EXISTS (SELECT 1 FROM dbo.UserVip  WHERE UserId = @UserId)

	BEGIN
		IF EXISTS (SELECT 1 FROM dbo.[User] WHERE UserId = @UserId)
		BEGIN
			DECLARE @date datetime
			SET @date = GETDATE()

			INSERT INTO dbo.UserVip
			(
				UserId,
				OrderId,
				IdentifyImg,
				StartTime,
				EndTime,
				Duration,
				Amount,
				[State],
				CreatedTime,
				LastUpdatedTime
			)
			VALUES
			(
				@UserId,
				'',
				'',
				null,
				null,
				0,
				0,
				0,
				@date,
				@date

			)
		END
	END

	SELECT * FROM dbo.UserVip WHERE UserId = @UserId
END
GO