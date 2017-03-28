CREATE PROCEDURE [dbo].[sp_FinanceBalanceInsert]
	@UserId int,
	@AvailableBalance decimal(18,2),
	@FrozenBalance decimal(18,2),
	@InsertedTimestamp datetime,
	@LastUpdatedTimestamp datetime
AS
	INSERT INTO FinanceBalance
	(
		UserId,
		AvailableBalance,
		FrozenBalance,
		InsertedTimestamp,
		LastUpdatedTimestamp
	)
	VALUES
	(
		@UserId,
		@AvailableBalance,
		@FrozenBalance,
		@InsertedTimestamp,
		@LastUpdatedTimestamp
	)

	SELECT @@IDENTITY
