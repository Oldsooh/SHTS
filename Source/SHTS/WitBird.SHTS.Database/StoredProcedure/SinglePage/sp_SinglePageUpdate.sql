CREATE PROCEDURE sp_SinglePageUpdate
	@Id				int,
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
	@ViewCount		int
AS
BEGIN
	update SinglePage set 
	ParentId = @ParentId,
	EntityType = @EntityType,
	Category = @Category,
	Title = @Title,
	Keywords = @Keywords,
	[Description] = @Description,
	ContentStyle = @ContentStyle,
	ContentText = @ContentText,
	ImageUrl = @ImageUrl,
	Link = @Link,
	ViewCount = @ViewCount
	where Id = @Id
END
GO
