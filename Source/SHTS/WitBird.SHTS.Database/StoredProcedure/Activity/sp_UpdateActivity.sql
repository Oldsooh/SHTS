CREATE PROCEDURE sp_UpdateActivity
@Id int,
@UserId int,
@ActivityType nvarchar(20),
@Adress nvarchar(200),
@LocationId nvarchar(100),
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
@LastUpdatedTime datetime
 AS 
 BEGIN
	UPDATE [Activity] SET 
	[UserId] = @UserId,[ActivityType] = @ActivityType,[Adress] = @Adress,[LocationId] = @LocationId,[StartTime] = @StartTime,[EndTime] = @EndTime,[HoldBy] = @HoldBy,[Title] = @Title,[Keywords] = @Keywords,[Jingdu] = @Jingdu,[Weidu] = @Weidu,[Description] = @Description,[ContentStyle] = @ContentStyle,[ContentText] = @ContentText,[ImageUrl] = @ImageUrl,[Link] = @Link,[State] = @State,[ViewCount] = @ViewCount,[CreatedTime] = @CreatedTime,[LastUpdatedTime] = @LastUpdatedTime
	WHERE Id=@Id
END 
GO
