CREATE PROCEDURE dbo.sp_WeChatUserRegister
	@Id INT OUTPUT,
    @UserId INT, 
    @OpenId NVARCHAR(50), 
    @NickName NVARCHAR(50), 
    @Sex INT, 
    @Province NVARCHAR(50), 
    @City NVARCHAR(50), 
    @County NVARCHAR(50), 
    @Photo NVARCHAR(50), 
    @AccessToken NVARCHAR(50), 
    @AccessTokenExpired BIT, 
    @AccessTokenExpireTime DATETIME, 
    @HasSubscried BIT, 
	@HasAuthorized BIT,
    @CreatedTime DATETIME 
AS

-- 如果用户取消关注后重新关注，已经存在该用户记录的情况，所以直接更新，保留原用户信息
IF EXISTS(SELECT UserId FROM WeChatUser WHERE OpenId = @OpenId)
BEGIN
	UPDATE dbo.WeChatUser SET
		AccessToken = @AccessToken,
		AccessTokenExpired = @AccessTokenExpired,
		AccessTokenExpireTime = @AccessTokenExpireTime,
		City = @City,
		County = @County,
		--CreatedTime = @CreatedTime,
		NickName = @NickName,
		--OpenId = @OpenId,
		Photo = @Photo,
		Province = @Province,
		Sex = @Sex,
		HasAuthorized = @HasAuthorized,
		HasSubscribed = @HasSubscried
	WHERE OpenId = @OpenId

	SEt @Id = (SELECT Id FROM WeChatUser WHERE OpenId = @OpenId)
END

ELSE
BEGIN
INSERT INTO dbo.WeChatUser 
(
	AccessToken,
	AccessTokenExpired,
	AccessTokenExpireTime,
	City,
	County,
	CreatedTime,
	NickName,
	OpenId,
	Photo,
	Province,
	Sex,
	HasSubscribed,
	HasAuthorized,
	UserId
)
VALUES
(
	@AccessToken,
	@AccessTokenExpired,
	@AccessTokenExpireTime,
	@City,
	@County,
	@CreatedTime,
	@NickName,
	@OpenId,
	@Photo,
	@Province,
	@Sex,
	@HasSubscried,
	@HasAuthorized,
	@UserId
)

SET @Id = @@IDENTITY
END