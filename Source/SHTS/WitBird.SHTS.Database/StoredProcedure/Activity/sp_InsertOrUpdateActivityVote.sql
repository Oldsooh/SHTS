CREATE PROCEDURE [dbo].[sp_InsertOrUpdateActivityVote]
	@VoteId int,
	@UserId int,
	@WechatUserOpenId nvarchar(50),
	@ActivityId int,
	@IsVoted bit,
	@InsertedTimestamp datetime,
	@LastUpdatedTimestamp datetime
AS
BEGIN

	IF EXISTS (SELECT 1 FROM ActivityVote WHERE UserId = @UserId AND ActivityId = @ActivityId)
	BEGIN
		UPDATE ActivityVote SET WechatUserOpenId = @WechatUserOpenId, ActivityId=@ActivityId, IsVoted = @IsVoted, LastUpdatedTimestamp = @LastUpdatedTimestamp 
		WHERE UserId = @UserId AND ActivityId = @ActivityId
	END
	ELSE IF EXISTS(SELECT 1 FROM ActivityVote WHERE WechatUserOpenId = @WechatUserOpenId AND ActivityId = @ActivityId)
	BEGIN
		UPDATE ActivityVote SET UserId = @UserId, ActivityId=@ActivityId, IsVoted = @IsVoted, LastUpdatedTimestamp = @LastUpdatedTimestamp 
		WHERE WechatUserOpenId = @WechatUserOpenId AND ActivityId = @ActivityId
	END
	ELSE
	BEGIN
		INSERT INTO ActivityVote VALUES (@UserId, @WechatUserOpenId, @ActivityId, @IsVoted, @InsertedTimestamp, @LastUpdatedTimestamp)
	END

	SELECT @@ROWCOUNT
END
