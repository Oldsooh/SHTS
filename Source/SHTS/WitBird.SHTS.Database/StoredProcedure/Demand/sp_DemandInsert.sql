CREATE PROCEDURE sp_DemandInsert
	@UserId			int,
	@CategoryId		int,
	@Title			nvarchar(100),
	@Description	nvarchar(300),
	@ContentStyle	text,
	@ContentText	text,
	@Province		nvarchar(50),
	@City			nvarchar(50),
	@Area			nvarchar(50),
	@Address		nvarchar(50),
	@Phone			nvarchar(50),
	@StartTime		datetime,
	@EndTime		datetime,
	@TimeLength		nvarchar(50),
	@PeopleNumber	nvarchar(50),
	@Budget			int,
	@InsertTime		datetime
AS
	Insert into Demand
	([UserId]
      ,[CategoryId]
      ,[Title]
      ,[Description]
      ,[ContentStyle]
      ,[ContentText]
      ,[Province]
      ,[City]
      ,[Area]
      ,[Address]
      ,[Phone]
      ,[StartTime]
      ,[EndTime]
      ,[TimeLength]
      ,[PeopleNumber]
      ,[Budget]
      ,[ViewCount]
      ,[IsActive]
      ,[InsertTime]
	  )
	  values
	  (
	  @UserId,
	  @CategoryId,
	  @Title,
	  @Description,
	  @ContentStyle,
	  @ContentText,
	  @Province,
	  @City,
	  @Area,
	  @Address,
	  @Phone,
	  @StartTime,
	  @EndTime,
	  @TimeLength,
	  @PeopleNumber,
	  @Budget,
	  0,
	  1,
	  @InsertTime
	  )