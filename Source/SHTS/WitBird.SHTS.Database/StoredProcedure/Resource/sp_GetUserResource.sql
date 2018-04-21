CREATE PROCEDURE [dbo].[sp_GetUserResource]
(
        @userId INT,
        @pageIndex INT,
        @pageSize INT
)
AS
BEGIN
    SELECT COUNT(1) AS TotalCount FROM [dbo].[Resource] Res
    WHERE (Res.[UserId] = @userId) AND (Res.[State] = 1 OR Res.[State] = 2)


    ;WITH T AS
    (SELECT ROW_NUMBER() OVER(order by id DESC) number,*
    FROM      dbo.Resource AS Res
    WHERE (Res.[UserId] = @userId) AND (Res.[State] = 1 OR Res.[State] = 2))
    SELECT * FROM T
    WHERE T.number between (@pageIndex*@pageSize+1) and (@pageIndex*@pageSize+@pageSize)

END
GO