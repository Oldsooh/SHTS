CREATE PROCEDURE [dbo].[sp_GetUserByUserName]

@username NVARCHAR(50)

AS

SELECT * from [User] where (UserName=@username OR Email=@username OR Cellphone=@username);
