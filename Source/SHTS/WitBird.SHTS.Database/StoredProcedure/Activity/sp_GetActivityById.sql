CREATE PROCEDURE sp_GetActivityById
@Id int
AS 
BEGIN
	SELECT 
	Id,AT.UserId,ActivityType,AT.Adress,StartTime,EndTime,HoldBy,Title,Keywords,
	Jingdu,Weidu,[Description],ContentStyle,ContentText,ImageUrl,
	Link,AT.[State],ViewCount,AT.CreatedTime,AT.LastUpdatedTime,AT.LocationId
	,U.UserName,AT.IsFromMobile
	 FROM [Activity] AT 
	 INNER JOIN [User] U ON U.UserId=AT.UserId
	 WHERE Id=@Id
	 and AT.[State] !=1

	 Update [Activity] set ViewCount=ViewCount+1 WHERE Id=@Id 
END
GO