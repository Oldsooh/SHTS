CREATE PROCEDURE [dbo].[sp_AccessRecordInsert]
	@UserIP nvarchar(50),
	@AccessUrl nvarchar(1000),
	@TableName nvarchar(50),
	@ColumnName nvarchar(50),
	@PrimaryId nvarchar(50),
	@PrimaryValue nvarchar(50),
	@InsertedTimestamp datetime
AS
INSERT INTO dbo.AccessRecord 
(
	UserIP,
	AccessUrl,
	InsertedTimestamp
)
VALUES
(
	@UserIP,
	@AccessUrl,
	@InsertedTimestamp
)

DECLARE @Sql nvarchar(500)
SET @Sql = N'UPDATE ' + @TableName + 
N' SET ' + @ColumnName + N' = ' + @ColumnName + N' + 1 WHERE ' + 
@PrimaryId + N' = ' + @PrimaryValue

exec(@Sql)
