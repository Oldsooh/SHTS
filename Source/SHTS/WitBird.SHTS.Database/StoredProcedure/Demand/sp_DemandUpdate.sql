CREATE PROCEDURE sp_DemandUpdate
	@Id				int,
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
	@QQWeixin		nvarchar(50),
	@Email			nvarchar(50),
	@StartTime		datetime,
	@EndTime		datetime,
	@TimeLength		nvarchar(50),
	@PeopleNumber	nvarchar(50),
	@Budget			int,
	@IsActive		bit
AS
	Update Demand
	set
	   [CategoryId]=@CategoryId
      ,[Title]=@Title
      ,[Description]=@Description
      ,[ContentStyle]=@ContentStyle
      ,[ContentText]=@ContentText
      ,[Province]=@Province
      ,[City]=@City
      ,[Area]=@Area
      ,[Address]=@Address
      ,[Phone]=@Phone
	  ,[QQWeixin]=@QQWeixin
	  ,[Email]=@Email
      ,[StartTime]=@StartTime
      ,[EndTime]=@EndTime
      ,[TimeLength]=@TimeLength
      ,[PeopleNumber]=@PeopleNumber
      ,[Budget]=@Budget
	  ,[IsActive]=@IsActive
	where Id = @Id
	  
