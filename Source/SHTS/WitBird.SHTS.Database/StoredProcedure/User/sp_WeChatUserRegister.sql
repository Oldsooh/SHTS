CREATE PROCEDURE dbo.sp_WeChatUserRegister
	@Id INT OUTPUT,
    @UserId INT,
    @WeChatId NVARCHAR(50), 
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
    @State INT, 
    @CreatedTime DATETIME 
AS

-- 如果用户取消关注后重新关注，已经存在该用户记录的情况，所以直接更新，保留原用户信息
IF EXISTS(SELECT UserId FROM WeChatUser WHERE WeChatId = @WeChatId)
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
		State = @State
		--UserId = @UserId
	WHERE WeChatId = @WeChatId

	SEt @Id = (SELECT Id FROM WeChatUser WHERE WeChatId = @WeChatId)
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
	State,
	UserId,
	WeChatId
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
	@State,
	@UserId,
	@WeChatId
)

SET @Id = @@IDENTITY
END