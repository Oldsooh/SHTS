CREATE PROCEDURE [dbo].[sp_SelectActivityTotalVotesCount]
	@ActivityId int
AS
BEGIN
	SELECT COUNT(1) FROM ActivityVote WHERE ActivityId = @ActivityId
END
