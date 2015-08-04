CREATE PROCEDURE sp_SinglePageDeleteById
	@Id				int
AS
BEGIN
	update SinglePage set 
	IsActive = 0 
	where Id = @Id
END
GO
