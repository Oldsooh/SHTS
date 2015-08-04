CREATE PROCEDURE [dbo].[sp_DemandSelectById]
	@Id		int
AS
BEGIN
	update [Demand] set ViewCount += 1 where Id=@Id
	select D.*,DC.Name as CategoryName,U.UserName as UserName FROM [Demand] as D 
	join [DemandCategory] as DC on D.CategoryId = DC.Id
	join [User] as U on D.UserId = U.UserId
	where D.Id=@Id and D.IsActive = 1 
END