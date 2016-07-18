CREATE PROCEDURE dbo.sp_WeChatUserUpdate
	@Id INT,
    @UserId INT,
    @OpenId NVARCHAR(50), 
    @NickName NVARCHAR(50), 
    @Sex INT, 
    @Province NVARCHAR(50), 
    @City NVARCHAR(50), 
    @County NVARCHAR(50), 
    @Photo NVARCHAR(500), 
    @AccessToken NVARCHAR(50), 
    @AccessTokenExpired BIT, 
    @AccessTokenExpireTime DATETIME, 
    @HasSubscribed BIT,
	@HasAuthorized BIT
AS
BEGIN
UPDATE dbo.WeChatUser SET
	AccessToken = @AccessToken,
	AccessTokenExpired = @AccessTokenExpired,
	AccessTokenExpireTime = @AccessTokenExpireTime,
	City = @City,
	County = @County,
	NickName = @NickName,
	OpenId = @OpenId,
	Photo = @Photo,
	Province = @Province,
	Sex = @Sex,
	HasSubscribed = @HasSubscribed,
	HasAuthorized = @HasAuthorized,
	UserId = @UserId
WHERE Id = @Id
END
GO