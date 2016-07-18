CREATE PROCEDURE [dbo].[sp_SelectTradeListByUserId]
	@UserId int,
	@PageCount int,
	@PageIndex int
AS
BEGIN
	--SET NOCOUNT ON
	
	select COUNT(1) FROM dbo.Trade where SellerId = @UserId OR BuyerId = @UserId
	
	select *, UserT1.UserName as CreatedUserName, UserT2.UserName as SellerName, UserT3.UserName as BuyerName from 
	(
		select *, ROW_NUMBER() over(order by dbo.Trade.CreatedTime desc) as RowNumber 
		FROM dbo.Trade where BuyerId = @UserId OR SellerId = @UserId
	 ) as temp 
	 left join dbo.[User] as UserT1 on temp.UserId = UserT1.UserId
	 left join dbo.[User] as UserT2 on temp.SellerId = UserT2.UserId
	 left join dbo.[User] as UserT3 on temp.BuyerId = UserT3.UserId
	where temp.RowNumber>(@PageIndex-1)*@PageCount and temp.RowNumber<=@PageIndex*@PageCount
END
GO
