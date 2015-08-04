CREATE PROCEDURE [dbo].[sp_UpdateActivityStatu]
	@Id int,
	@State int
AS
	update Activity set [State]=@State,LastUpdatedTime=GETDATE() where Id=@Id
