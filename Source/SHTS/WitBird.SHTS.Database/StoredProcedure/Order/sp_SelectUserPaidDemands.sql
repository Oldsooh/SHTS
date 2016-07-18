CREATE PROCEDURE [dbo].[sp_SelectUserPaidDemands]
	@UserName nvarchar(100),
	@PageSize int = 10,
	@PageIndex int = 1
AS

BEGIN

select COUNT(1) FROM [TradeOrder] where OrderType = 2 and [State] = 1 and UserName = @UserName
	
	select * from
		(select * from (
			select *, ROW_NUMBER() over(order by LastUpdatedTime desc) as RowNumber 
			FROM [TradeOrder] where OrderType = 2 and [State] = 1 and UserName = @UserName 
		 ) as temp where temp.RowNumber>(@PageIndex-1)*@PageSize and temp.RowNumber<=@PageIndex*@PageSize
		) as tempDemand
END
GO
