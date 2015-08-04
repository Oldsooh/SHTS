CREATE PROCEDURE sp_CreateOrUpdateActivity
@Id int,
@UserId int,
@ActivityType nvarchar(20),
@LocationId nvarchar(100),
@Adress nvarchar(200),
@StartTime datetime,
@EndTime datetime,
@HoldBy nvarchar(200),
@Title nvarchar(100),
@Keywords nvarchar(200),
@Jingdu nvarchar(100),
@Weidu nvarchar(100),
@Description nvarchar(300),
@ContentStyle text,
@ContentText text,
@ImageUrl nvarchar(300),
@Link nvarchar(300),
@State int,
@ViewCount int,
@CreatedTime datetime,
@LastUpdatedTime datetime,
@IsFromMobile bit
 AS 
    IF (@StartTime IS NULL)
		SET @StartTime=GETDATE();
	IF (@EndTime IS NULL)
		SET @EndTime=GETDATE();
	IF (@IsFromMobile IS NULL)
		SET @IsFromMobile=0;

    IF (@Id=0)
	BEGIN
		INSERT INTO [Activity](
	[UserId],[ActivityType],[Adress],[LocationId],[StartTime],[EndTime],[HoldBy],
	[Title],[Keywords],[Jingdu],[Weidu],
	[Description],[ContentStyle],
	[ContentText],[ImageUrl],[Link],
	[State],[ViewCount],[CreatedTime],[LastUpdatedTime],IsFromMobile
	)VALUES(
	@UserId,@ActivityType,@Adress,@LocationId,@StartTime,@EndTime
	,@HoldBy,@Title,@Keywords,@Jingdu,@Weidu,@Description,
	@ContentStyle,@ContentText,@ImageUrl,@Link,
	@State,@ViewCount,@CreatedTime,@LastUpdatedTime,@IsFromMobile
	)
	END
	ELSE
	BEGIN
		EXEC sp_UpdateActivity 	@Id,@UserId,@ActivityType,@Adress,@LocationId,@StartTime,@EndTime
			,@HoldBy,@Title,@Keywords,@Jingdu,@Weidu,@Description,
			@ContentStyle,@ContentText,@ImageUrl,@Link,
			@State,@ViewCount,@CreatedTime,@LastUpdatedTime;
	END
