CREATE PROCEDURE [dbo].[sp_SelectTradeList]
	@PageCount int,
	@PageIndex int,
	@State int
AS
BEGIN
	--SET NOCOUNT ON
	
	
	IF @State <> -1
	BEGIN
	
	select COUNT(1) FROM dbo.Trade WHERE [State] = @State
	select *, UserT1.UserName as CreatedUserName, UserT2.UserName as SellerName, UserT3.UserName as BuyerName from 
	(
		select *, ROW_NUMBER() over(order by dbo.Trade.CreatedTime desc) as RowNumber 
		FROM dbo.Trade
		WHERE [State] = @State
	 ) as temp 
	 left join dbo.[User] as UserT1 on temp.UserId = UserT1.UserId
	 left join dbo.[User] as UserT2 on temp.SellerId = UserT2.UserId
	 left join dbo.[User] as UserT3 on temp.BuyerId = UserT3.UserId
	where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
	END
	ELSE
	BEGIN
	
	select COUNT(1) FROM dbo.Trade
	select *, UserT1.UserName as CreatedUserName, UserT2.UserName as SellerName, UserT3.UserName as BuyerName from 
	(
		select *, ROW_NUMBER() over(order by dbo.Trade.CreatedTime desc) as RowNumber 
		FROM dbo.Trade
	 ) as temp 
	 left join dbo.[User] as UserT1 on temp.UserId = UserT1.UserId
	 left join dbo.[User] as UserT2 on temp.SellerId = UserT2.UserId
	 left join dbo.[User] as UserT3 on temp.BuyerId = UserT3.UserId
	where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
	END
END
GO
