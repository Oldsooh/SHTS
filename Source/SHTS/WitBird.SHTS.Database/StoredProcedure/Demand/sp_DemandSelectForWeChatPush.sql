CREATE PROCEDURE [dbo].[sp_DemandSelectForWeChatPush]
	@Locations nvarchar(1000),
	@Categories nvarchar(1000),
	@Keywords nvarchar(1000),
	@InsertTime datetime
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
		CategoryId int
	)
	INSERT INTO #Category
	SELECT Value FROM dbo.fSplit(@Categories, ',')

	CREATE TABLE #Keywords
	(
		Keyword nvarchar(100)
	)
	INSERT INTO #Keywords
	SELECT * FROM dbo.fSplit(@Keywords, ',')

	SELECT TOP 7 demand.* FROM dbo.Demand demand
	INNER JOIN #Category cate ON (cate.CategoryId = demand.CategoryId OR cate.CategoryId IS NULL OR cate.CategoryId = '')
	INNER JOIN #Location loc ON (loc.Province = demand.Province AND (loc.City = demand.City OR loc.City IS NULL OR loc.City = '') AND (loc.Area = demand.Area OR loc.Area IS NULL OR loc.Area = '')) 
	OR ((loc.Province IS NULL OR loc.Province = '') AND (loc.City IS NULL OR loc.City = '') AND (loc.Area IS NULL OR loc.Area = ''))
	INNER JOIN #Keywords words ON (CHARINDEX(words.Keyword, demand.Title, 0) > 0 OR words.Keyword IS NULL OR words.Keyword = '')
	WHERE demand.InsertTime >= @InsertTime AND (demand.Status IS NULL OR demand.Status <> 2)
	ORDER BY demand.Id DESC

END
GO
