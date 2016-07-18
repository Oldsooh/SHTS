CREATE PROCEDURE [dbo].[sp_VerifyAccountInfo]
@columnname nvarchar(50),
@value nvarchar(MAX)
AS
BEGIN
	declare @SQL nvarchar(200);
	set @SQL='Select count(UserId) from [user] where [state]!=1 and '+@columnname+' = '''+@value+'''';
	EXECUTE(@SQL)
END
GO