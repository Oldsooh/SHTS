CREATE PROCEDURE sp_DemandCategory_Select
AS
BEGIN
	SELECT * FROM [DemandCategory]
    order by DisplayOrder
END
GO
