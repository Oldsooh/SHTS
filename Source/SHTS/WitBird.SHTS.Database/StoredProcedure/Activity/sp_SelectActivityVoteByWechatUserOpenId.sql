CREATE PROCEDURE [dbo].[sp_SelectActivityVoteByWechatUserOpenId]
	@WechatUserOpenId nvarchar(50),
	@ActivityId int
AS
BEGIN
	
	SELECT * FROM ActivityVote WHERE WechatUserOpenId = @WechatUserOpenId AND ActivityId = @ActivityId

	SELECT COUNT(1) FROM ActivityVote WHERE ActivityId = @ActivityId AND IsVoted = 1
END