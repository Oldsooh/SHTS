CREATE PROCEDURE [dbo].[sp_DemandSelectForWeChatPush]
	@Locations nvarchar(1000),
	@Categories nvarchar(1000),
	@Keywords nvarchar(1000),
	@InsertTime datetime,
	@PageCount int
AS
BEGIN

	-- Locations is like: zhixiashi_beijing_dongchengqu, sichuan_chengdu_xxx
	CREATE TABLE #Location
	(
		Province nvarchar(50),
		City nvarchar(50),
		Area nvarchar(50)
	)
	INSERT INTO #Location
	SELECT 
		(SELECT t.Value FROM (SELECT ROW_NUMBER() OVER(ORDER BY GETDATE()) AS number, Value FROM fsplit (tempLocation.loc, '_')) AS t WHERE number=1) AS Province,
		(SELECT t.Value FROM (SELECT ROW_NUMBER() OVER(ORDER BY GETDATE()) AS number, Value FROM fsplit (tempLocation.loc, '_')) AS t WHERE number=2) AS City,
		(SELECT t.Value FROM (SELECT ROW_NUMBER() OVER(ORDER BY GETDATE()) AS number, Value FROM fsplit (tempLocation.loc, '_')) AS t WHERE number=3) AS Area
	FROM (SELECT Value AS loc FROM dbo.fSplit(@Locations, ',')) AS tempLocation

	CREATE TABLE #Category
	(
		TypeId int,
		SubTypeId int
	)
	INSERT INTO #Category
	SELECT 
		(SELECT t.Value FROM (SELECT ROW_NUMBER() OVER(ORDER BY GETDATE()) AS number, Value FROM fsplit (tempCategory.Value, '_')) AS t WHERE number=1) AS TypeId,
		(SELECT t.Value FROM (SELECT ROW_NUMBER() OVER(ORDER BY GETDATE()) AS number, Value FROM fsplit (tempCategory.Value, '_')) AS t WHERE number=2) AS SubTypeId
	FROM (SELECT Value FROM dbo.fSplit(@Categories, ',')) AS tempCategory

	CREATE TABLE #Keyword
	(
		Keyword nvarchar(100)
	)
	INSERT INTO #Keyword
	SELECT * FROM dbo.fSplit(@Keywords, ',')

	IF (@PageCount > 0)
	BEGIN
		SELECT TOP (@PageCount) demand.* FROM dbo.Demand demand
		INNER JOIN #Category cate ON (cate.TypeId = demand.ResourceType AND (cate.SubTypeId IS NULL OR cate.SubTypeId = -1 OR cate.SubTypeId = demand.ResourceTypeId))
		INNER JOIN #Location loc ON (loc.Province = demand.Province AND (loc.City = demand.City OR loc.City IS NULL OR loc.City = '') AND (loc.Area = demand.Area OR loc.Area IS NULL OR loc.Area = '')) 
		OR ((loc.Province IS NULL OR loc.Province = '') AND (loc.City IS NULL OR loc.City = '') AND (loc.Area IS NULL OR loc.Area = ''))
		INNER JOIN #Keyword words ON (CHARINDEX(words.Keyword, demand.Title, 0) > 0 OR words.Keyword IS NULL OR words.Keyword = '')
		WHERE demand.InsertTime >= @InsertTime AND (demand.Status IS NULL OR demand.Status <> 2) AND demand.IsActive = 1
		ORDER BY demand.Id DESC
	END
	ELSE
	BEGIN
		SELECT demand.* FROM dbo.Demand demand
		INNER JOIN #Category cate ON (cate.TypeId = demand.ResourceType AND (cate.SubTypeId IS NULL OR cate.SubTypeId = -1 OR cate.SubTypeId = demand.ResourceTypeId))
		INNER JOIN #Location loc ON (loc.Province = demand.Province AND (loc.City = demand.City OR loc.City IS NULL OR loc.City = '') AND (loc.Area = demand.Area OR loc.Area IS NULL OR loc.Area = '')) 
		OR ((loc.Province IS NULL OR loc.Province = '') AND (loc.City IS NULL OR loc.City = '') AND (loc.Area IS NULL OR loc.Area = ''))
		INNER JOIN #Keyword words ON (CHARINDEX(words.Keyword, demand.Title, 0) > 0 OR words.Keyword IS NULL OR words.Keyword = '')
		WHERE demand.InsertTime >= @InsertTime AND (demand.Status IS NULL OR demand.Status <> 2) AND demand.IsActive = 1
		ORDER BY demand.Id DESC
	END
END
GO
