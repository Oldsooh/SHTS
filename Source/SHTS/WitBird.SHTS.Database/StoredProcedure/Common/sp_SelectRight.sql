CREATE PROCEDURE sp_SelectRight

AS
begin
	SELECT top 10 Id,ResourceType,Title, InsertTime FROM Demand where IsActive = 1 order by Id desc
	select top 8 Id,ResourceType,Title,CreateTime from Resource where ResourceType = 1 order by Id desc
	select top 8 Id,ResourceType,Title,CreateTime from Resource where ResourceType = 2 order by Id desc
	select top 8 Id,ResourceType,Title,CreateTime from Resource where ResourceType = 3 order by Id desc
	select top 8 Id,ResourceType,Title,CreateTime from Resource where ResourceType = 4 order by Id desc
end
go
