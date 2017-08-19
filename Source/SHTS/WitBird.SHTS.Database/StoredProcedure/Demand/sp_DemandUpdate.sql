CREATE PROCEDURE sp_DemandUpdate
	@Id				int,
	@ResourceType	int,
	@ResourceTypeId	int,
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
	@IsActive		bit,
	@ImageUrls		text
AS
BEGIn
	Update Demand
	set
	   ResourceType=@ResourceType
	  ,ResourceTypeId=@ResourceTypeId
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
	  ,[ImageUrls] = @ImageUrls
	where Id = @Id
END
GO
	  
