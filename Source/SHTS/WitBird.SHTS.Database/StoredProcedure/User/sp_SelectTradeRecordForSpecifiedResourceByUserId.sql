CREATE PROC sp_SelectTradeRecordForSpecifiedResourceByUserId
(
	@UserId int,
	@ResourceId int,
	@RecordCount int output
)
AS

SET @RecordCount = 
(
	SELECT COUNT(1) FROM dbo.Trade 
	WHERE 
		BuyerId = @UserId AND
		ResourceUrl IS NOT NULL AND
		SUBSTRING
		(
			ResourceUrl, 
			(LEN(ResourceUrl) - CHARINDEX('/', REVERSE(ResourceUrl)) + 1) + 1, -- the last index of '/'
			LEN(ResourceUrl) - (LEN(ResourceUrl) - CHARINDEX('/', REVERSE(ResourceUrl)) + 1) -- length
		)
		
		like CAST(@ResourceId as varchar) +'%'
)

SELECT @RecordCount