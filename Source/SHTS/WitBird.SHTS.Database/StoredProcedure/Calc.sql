CREATE PROCEDURE [dbo].[Calc]
    @a int,
    @b int
AS
BEGIN
    SELECT @a + @b
END
