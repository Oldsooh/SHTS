CREATE PROCEDURE [dbo].[sp_DemandSelectById]
	@Id		int
AS
BEGIN
	update [Demand] set ViewCount += 1 where Id=@Id
	select D.*,U.UserName as UserName FROM [Demand] as D
	join [User] as U on D.UserId = U.UserId
	where D.Id=@Id and D.IsActive = 1 
END
GO