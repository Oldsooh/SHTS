CREATE PROCEDURE [dbo].[sp_QueryAccessAnalytics]
	@startRowIndex int,
	@PageSize int,
	@FormTime DateTime,
	@ToTime DateTime,
	@QueryType int
AS
		IF(@QueryType=-1)
		BEGIN
			select UT.*,U.UserName from (
			  select u.Id,ROW_NUMBER() over(Order by U.CreateTime desc) rownum
				 from [AccessAnalytics] u
				 where U.CreateTime>=@FormTime and U.CreateTime<@ToTime
			)t,[AccessAnalytics] UT
			LEFT JOIN [User] U on UT.UserId=U.UserId
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.Id=t.Id;

			SELECT Count(Id) as 'totalCount' from [AccessAnalytics]
				 where CreateTime>=@FormTime and CreateTime<@ToTime
		END
		ELSE if(@QueryType=1)
		BEGIN
		   /*会员*/
		   select UT.*,U.UserName from (
			  select u.Id,ROW_NUMBER() over(Order by U.CreateTime desc) rownum
				 from [AccessAnalytics] u
				 where U.CreateTime>=@FormTime and U.CreateTime<@ToTime
				 AND U.UserId is not null
			)t,[AccessAnalytics] UT
			LEFT JOIN [User] U on UT.UserId=U.UserId
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.Id=t.Id;

			SELECT Count(Id) as 'totalCount' from [AccessAnalytics]
				 where CreateTime>=@FormTime and CreateTime<@ToTime AND UserId is not null;
		END
		ELSE if(@QueryType = 2)
		BEGIN
		   /*游客*/
		   select UT.*,U.UserName  from (
			  select u.Id,ROW_NUMBER() over(Order by U.CreateTime desc) rownum
				 from [AccessAnalytics] u
				 where U.CreateTime>=@FormTime and U.CreateTime<@ToTime
				 AND U.UserId is null
			)t,[AccessAnalytics] UT
			LEFT JOIN [User] U on UT.UserId=U.UserId
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.Id=t.Id;

			SELECT Count(Id) as 'totalCount' from [AccessAnalytics]
				 where CreateTime>=@FormTime and CreateTime<@ToTime AND UserId is null;
		END
		ELSE if(@QueryType = 3)
		BEGIN
		   /*登录*/
		   select UT.*,U.UserName  from (
			  select u.Id,ROW_NUMBER() over(Order by U.CreateTime desc) rownum
				 from [AccessAnalytics] u
				 where U.CreateTime>=@FormTime and U.CreateTime<@ToTime
				 AND U.Operation='login'
			)t,[AccessAnalytics] UT
			LEFT JOIN [User] U on UT.UserId=U.UserId
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.Id=t.Id;

			SELECT Count(Id) as 'totalCount' from [AccessAnalytics]
				 where CreateTime>=@FormTime and CreateTime<@ToTime AND Operation='login';
		END
		ELSE if(@QueryType = 4)
		BEGIN
		   /*z最大*/
		   select UT.*,'' as 'UserName'  from (
			  select u.Id,ROW_NUMBER() over(Order by U.maxcount desc) rownum
				 from (
				   select COUNT(PageTitle) as maxcount,min(A.Id) as Id from 
				   (
			       select * from [AccessAnalytics]
				   where CreateTime>=@FormTime and CreateTime<@ToTime
			       ) A
				   group by A.PageTitle
				 ) u
			)t,[AccessAnalytics] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.Id=t.Id;

			SELECT Count(NA.Id) as 'totalCount' from (
			   select min(A.Id) as Id 
			   from (
			       select * from [AccessAnalytics]
				   where CreateTime>=@FormTime and CreateTime<@ToTime
			   ) A
				   group by A.PageTitle
		    ) NA
		END