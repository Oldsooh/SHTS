CREATE PROCEDURE [dbo].[sp_UpdateActivityStatu]
	@Id int,
	@State int
AS
BEGIN
	update Activity set [State]=@State,LastUpdatedTime=GETDATE() where Id=@Id
END
GO
