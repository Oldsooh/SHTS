CREATE PROCEDURE [dbo].[sp_QuersyUsersByCityAndResource]
@startRowIndex int,
@PageSize int,
@QueryType int,
@cityid nvarchar(100),
@resourceid int,
@Keyword nvarchar(MAX)
AS
BEGIN

		IF(@QueryType =-1 AND @cityid ='-1' AND @resourceid =-1)
		BEGIN
			select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 where U.[State]!=1 AND u.UserName like'%'+@Keyword+'%'
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND UserName like'%'+@Keyword+'%';
		END
		ELSE IF(@QueryType =-1 AND @cityid !='-1' AND @resourceid =-1)
		BEGIN
		   select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 where U.[State]!=1 AND u.LocationId like'%'+@cityid+'%'
				 AND u.UserName like'%'+@Keyword+'%'
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND LocationId like'%'+@cityid+'%'
				 AND UserName like'%'+@Keyword+'%';
		END
		ELSE IF(@QueryType =-1 AND @cityid !='-1' AND @resourceid !=-1)
		BEGIN
			select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u 
				 INNER JOIN [Resource] r on r.UserId=u.UserId
				 where U.[State]!=1 AND u.LocationId like'%'+@cityid+'%'
				 AND r.ResourceType=@resourceid
				 AND u.UserName like'%'+@Keyword+'%'
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(u.userId) as 'totalCount' from [User] u 
				 Left JOIN [Resource] r on r.UserId=u.UserId
				 where U.[State]!=1 AND u.LocationId like'%'+@cityid+'%'
				 AND r.ResourceType=@resourceid AND u.UserName like'%'+@Keyword+'%';
		END
		ELSE IF(@QueryType =-1 AND @cityid ='-1' AND @resourceid !=-1)
		BEGIN
		   select UT.* from 
		      (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u 
				 INNER JOIN 
					 (
					 select distinct uTe.UserId
								from [User] uTe 
								INNER JOIN [Resource] r on uTe.UserId=r.UserId
								where uTe.[State]!=1 AND r.ResourceType=@resourceid
								AND uTe.UserName like'%'+@Keyword+'%'
					 ) r on r.UserId=u.UserId
				) t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(t.userId) as 'totalCount' from 
			(select distinct uTe.UserId
				 from [User] uTe 
				 INNER JOIN [Resource] r on uTe.UserId=r.UserId
				 where uTe.[State]!=1 AND r.ResourceType=@resourceid
				 AND uTe.UserName like'%'+@Keyword+'%'
			) t;
		END
		ELSE IF(@QueryType !=-1 AND @cityid !='-1' AND @resourceid =-1)
		BEGIN
		   select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 where U.[State]!=1 AND UserType=@QueryType 
				 AND u.LocationId like'%'+@cityid+'%'
				 AND u.UserName like'%'+@Keyword+'%'
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND UserType=@QueryType AND LocationId like'%'+@cityid+'%'
				 AND UserName like'%'+@Keyword+'%';
		END
		ELSE IF(@QueryType !=-1 AND @cityid !='-1' AND @resourceid !=-1)
		BEGIN
		   select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 INNER JOIN
				 (	select distinct r.UserId  
					from [Resource] r,[User] UTemp where r.UserId=UTemp.UserId
					 AND UTemp.[State]!=1 AND UTemp.UserType=@QueryType 
					 AND UTemp.LocationId like'%'+@cityid+'%' AND r.ResourceType=@resourceid
					 AND UTemp.UserName like'%'+@Keyword+'%'
				 )TR on TR.UserId=u.UserId
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from 
			(	select distinct r.UserId  
					from [Resource] r,[User] UTemp where r.UserId=UTemp.UserId
					 AND UTemp.[State]!=1 AND UTemp.UserType=@QueryType 
					 AND UTemp.LocationId like'%'+@cityid+'%' 
					 AND r.ResourceType=@resourceid
					 AND UTemp.UserName like'%'+@Keyword+'%'
		    ) t;
		END
		ELSE IF(@QueryType !=-1 AND @cityid ='-1' AND @resourceid =-1)
		BEGIN
		   select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 where U.[State]!=1 AND UserType=@QueryType
				 AND u.UserName like'%'+@Keyword+'%'
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND UserType=@QueryType AND UserName like'%'+@Keyword+'%';
		END
		ELSE IF(@QueryType !=-1 AND @cityid ='-1' AND @resourceid !=-1)
		BEGIN
		   select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
			     INNER JOIN 
				 (	
				     SELECT distinct r.UserId 
					 from [Resource] r ,[User] UTemp 
					 where r.UserId=UTemp.UserId 
					 AND r.ResourceType=@resourceid
					 AND UTemp.[State]!=1 AND UTemp.UserType=@QueryType
					 AND UTemp.UserName like'%'+@Keyword+'%'
				 ) TR on TR.UserId=u.UserId
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(TR.userId) as 'totalCount' from 
			(	
				     SELECT distinct r.UserId 
					 from [Resource] r ,[User] UTemp 
					 where r.UserId=UTemp.UserId 
					 AND r.ResourceType=@resourceid
					 AND UTemp.[State]!=1 AND UTemp.UserType=@QueryType
					 AND UTemp.UserName like'%'+@Keyword+'%'
			) TR;
		END
END