CREATE PROCEDURE [dbo].[sp_QueryUsers]
@startRowIndex int,
@PageSize int,
@QueryType int,
@cityid nvarchar(100),
@resourceid int,
@Keyword nvarchar(MAX)
AS
BEGIN
        IF(@cityid ='-1' AND @resourceid =-1 AND @Keyword ='')
		BEGIN
			select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 where U.[State]!=1 AND UserType=@QueryType
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND UserType=@QueryType;
		END
		ELSE IF(@Keyword <>'')
		BEGIN
			select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 where U.[State]!=1 AND UserType=@QueryType and 
				 (Cellphone like'%'+@Keyword+'%' OR UserName like'%'+@Keyword+'%')
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND UserType=@QueryType
				  AND (Cellphone like'%'+@Keyword+'%' OR UserName like'%'+@Keyword+'%');
		END
		ELSE IF(@cityid !='-1')
		BEGIN
			select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 where U.[State]!=1 AND UserType=@QueryType and LocationId like'%'+@cityid+'%'
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND UserType=@QueryType
				  AND (Cellphone like'%'+@Keyword+'%' OR UserName like'%'+@Keyword+'%');
		END
		ELSE IF(@resourceid !='-1')
		BEGIN
			select UT.* from (
			  select u.UserId,ROW_NUMBER() over(Order by U.CreateTime) rownum
				 from [User] u
				 INNER JOIN 
					 (
					 select distinct uTe.UserId
								from [User] uTe 
								INNER JOIN [Resource] r on uTe.UserId=r.UserId
								where uTe.[State]!=1 AND r.ResourceType=@resourceid
								AND uTe.[State]!=1 AND uTe.UserType=@QueryType
					 ) r on r.UserId=u.UserId
			)t,[User] UT
			where t.rownum>=(@startRowIndex-1)*@PageSize+1 
			and t.rownum<=@startRowIndex*@PageSize and UT.UserId=t.UserId;

			SELECT Count(userId) as 'totalCount' from [User]
				 where [State]!=1 AND UserType=@QueryType
				  AND (Cellphone like'%'+@Keyword+'%' OR UserName like'%'+@Keyword+'%');
		END
END
GO