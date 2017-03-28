CREATE PROCEDURE [dbo].[sp_FinanceBalanceUpdate]
	@BalanceId int,
	@AvailableBalance decimal(18, 2),
	@FrozenBalance decimal(18, 2),
	@LastUpdatedTimestamp datetime
AS
	UPDATE FinanceBalance SET AvailableBalance = @AvailableBalance, FrozenBalance = @FrozenBalance, LastUpdatedTimestamp = @LastUpdatedTimestamp
	WHERE Id = @BalanceId
GO
