CREATE PROCEDURE [dbo].[sp_SelectTradeByTradeId]
	@TradeId int
AS
	SET NOCOUNT ON
	
	declare @SellerId int
	declare @BuyerId int
	declare @CreateUserId int

	set @CreateUserId = (select Userid from dbo.Trade where TradeId = @TradeId)
	set @SellerId = (select SellerId from dbo.Trade where TradeId = @TradeId)
	set @BuyerId = (select BuyerId from dbo.Trade where TradeId = @TradeId)
	
	select * from dbo.Trade where TradeId = @TradeId
	select * from dbo.[User] where UserId = @SellerId
	select * from dbo.[User] where UserId = @BuyerId

	--如果创建者是卖家，那么需要查询出买家的银行信息；反之，则需要查询卖家的银行信息
	if @CreateUserId = @SellerId
	select * from dbo.UserBankInfo where UserId = @BuyerId
	
	else

	select * from dbo.UserBankInfo where UserId = @SellerId
GO

