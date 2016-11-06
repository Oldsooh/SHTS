CREATE PROCEDURE [dbo].[sp_SelectActivityVoteByUserId]
	@UserId int,
	@ActivityId int
AS
BEGIN
	
	SELECT * FROM ActivityVote WHERE UserId = @UserId AND ActivityId = @ActivityId

	SELECT COUNT(1) FROM ActivityVote WHERE ActivityId = @ActivityId AND IsVoted = 1
END
