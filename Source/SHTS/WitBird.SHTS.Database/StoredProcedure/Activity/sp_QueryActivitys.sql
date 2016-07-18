CREATE PROCEDURE [dbo].[sp_QueryActivitys]
@startRowIndex int,
@PageSize int,
@ActivityType int,
@QueryType int,
@cityid nvarchar(100),
@StartTime datetime,
@EndTime datetime,
@UserId int,
@Status nvarchar(100),
@LastUpdatedTime datetime,
@keywors nvarchar(100)
AS
BEGIN
		IF(@QueryType=-1)
		BEGIN
		    /* 普通搜索*/
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where U.[State]=3 or U.[State]=0
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id 
			and AT.UserId=ur.UserId;

			SELECT Count(Id) as 'totalCount' from [Activity]
				 where [State]=3 or [State]=0;
		END
		ELSE IF(@QueryType=0)
		BEGIN
		    /* 类型搜索*/
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where (U.[State]=3 or U.[State]=0) and u.ActivityType=@ActivityType
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id and AT.UserId=ur.UserId;

			SELECT Count(userId) as 'totalCount' from [Activity] u
				 where ([State]=3 or [State]=0) and u.ActivityType=@ActivityType;
		END
		ELSE IF(@QueryType=1)
		BEGIN
		    /* 类型+地点搜索*/
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where (U.[State]=3 or U.[State]=0) and u.ActivityType=@ActivityType 
				 and u.LocationId=@cityid
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id and AT.UserId=ur.UserId;

			SELECT Count(userId) as 'totalCount' from [Activity] u
				 where ([State]=3 or [State]=0) and u.ActivityType=@ActivityType 
				 and u.LocationId=@cityid;
		END
		ELSE IF(@QueryType=2)
		BEGIN
		    /* 时间区间搜索*/
			IF(@StartTime is null)
			BEGIN 
			    set @StartTime='1/1/1900'
			END
			IF(@EndTime is null)
			BEGIN 
			    set @EndTime='12/31/9998'
			END
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where ([State]=3 or [State]=0) and u.StartTime>@StartTime and EndTime>@EndTime
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id and AT.UserId=ur.UserId;

			SELECT Count(userId) as 'totalCount' from [Activity] u
				 where ([State]=3 or [State]=0) and u.StartTime>=@StartTime and EndTime<=@EndTime;
		END
		ELSE IF(@QueryType=3)
		BEGIN
		    /*用户相关搜索*/
			select AT.[Id],[UserId],[ActivityType],[Adress],[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',[ImageUrl],
			[Link],[State],[ViewCount],[CreatedTime],[LastUpdatedTime],LocationId,'' as 'UserName'
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where U.[State]!=1 and U.UserId=@UserId
			)t,[Activity] AT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id;

			SELECT Count(Id) as 'totalCount' from [Activity]
				 where [State]!=1 and UserId=@UserId;
		END
		ELSE IF(@QueryType=4)
		BEGIN
		    /* 各个类别搜索*/
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(PARTITION BY U.ActivityType Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where [State]=3 or [State]=0
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id 
			and AT.UserId=ur.UserId;

			SELECT Count(Id) as 'totalCount' from [Activity]
				 where [State]=3 or [State]=0;
		END
		ELSE IF(@QueryType=5)
		BEGIN
		    /* 后台搜索*/
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where U.[State] in (select * from SplitStringToIds(@Status))
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id 
			and AT.UserId=ur.UserId;

			SELECT Count(Id) as 'totalCount' from [Activity]
				 where [State] in (select * from SplitStringToIds(@Status));
		END
		ELSE IF(@QueryType=6)
		BEGIN
		    /* 后台搜索*/
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],ContentStyle,ContentText,AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where (U.[State]=3 or U.[State]=0) and LastUpdatedTime>@LastUpdatedTime
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id 
			and AT.UserId=ur.UserId;

			SELECT Count(Id) as 'totalCount' from [Activity]
				 where ([State]=3 or [State]=0) and LastUpdatedTime>@LastUpdatedTime;
		END
		IF(@QueryType=7)
		BEGIN
		    /* 后台统计*/
			IF(@StartTime is null)
			BEGIN 
			    set @StartTime='1/1/1900'
			END
			IF(@EndTime is null)
			BEGIN 
			    set @EndTime=GETDATE()
			END
			select AT.[Id],AT.[UserId],[ActivityType],AT.[Adress],AT.[StartTime],
			[EndTime],[HoldBy],[Title],[Keywords],[Jingdu],[Weidu],[IsFromMobile],
			[Description],NULL as 'ContentStyle',NULL as 'ContentText',AT.[ImageUrl],
			AT.[Link],AT.[State],AT.[ViewCount],AT.[CreatedTime],AT.[LastUpdatedTime],
			AT.LocationId,ur.UserName
			from (
			  select u.Id,ROW_NUMBER() over(Order by U.[LastUpdatedTime] desc) rownum
				 from [Activity] u
				 where (U.[State] in(0,2,3)) 
				 AND u.StartTime>@StartTime 
				 AND EndTime<=@EndTime
				 AND U.Title LIKE'%'+@keywors+'%'
			)t,[Activity] AT,[User] ur
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and AT.Id=t.Id 
			and AT.UserId=ur.UserId;

			SELECT Count(Id) as 'totalCount' from [Activity] U
				 where (U.[State] in(0,2,3)) 
				 AND u.StartTime>@StartTime 
				 AND EndTime<=@EndTime
				 AND U.Title LIKE'%'+@keywors+'%';
		END
	
END
GO