CREATE PROCEDURE sp_SinglePageInsert
	@UserId			int,
	@ParentId		int,
	@EntityType		nvarchar(20),
	@Category		nvarchar(50),
	@Title			nvarchar(100),
	@Keywords		nvarchar(200),
	@Description	nvarchar(300),
	@ContentStyle	text,
	@ContentText	text,
	@ImageUrl		nvarchar(300),
	@Link			nvarchar(300),
	@ViewCount		int,
	@InsertTime		datetime
AS
BEGIN
	insert into SinglePage
	(
		[UserId]
      ,[ParentId]
      ,[EntityType]
      ,[Category]
      ,[Title]
      ,[Keywords]
      ,[Description]
      ,[ContentStyle]
      ,[ContentText]
      ,[ImageUrl]
      ,[Link]
      ,[IsActive]
      ,[ViewCount]
      ,[InsertTime]
	  )
	  values
	  (
	  @UserId,
	  @ParentId,
	  @EntityType,
	  @Category,
	  @Title,
	  @Keywords,
	  @Description,
	  @ContentStyle,
	  @ContentText,
	  @ImageUrl,
	  @Link,
	  1,
	  0,
	  @InsertTime
	  )
END
GO
