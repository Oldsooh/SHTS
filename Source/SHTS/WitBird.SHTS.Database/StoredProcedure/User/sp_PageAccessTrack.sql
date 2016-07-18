CREATE PROCEDURE [dbo].[sp_PageAccessTrack]
    @UserId INT , 
    @AccessUrl NVARCHAR(MAX),
	@ReferrerUrl NVARCHAR(MAX),
	@Operation NVARCHAR(50),   
    @PageTitle NVARCHAR(50), 
    @IP NVARCHAR(50)
AS
BEGIN
	insert into [AccessAnalytics] values(@UserId,@AccessUrl,
	@ReferrerUrl,@Operation,@PageTitle,@IP,GETDATE(),NULL,NULL);
END
GO