CREATE PROCEDURE [dbo].[sp_AddNewOrder]
	@OrderId NVARCHAR(50),
    @Amount DECIMAL(18, 2),
    @Subject NVARCHAR(500), 
    @Body NTEXT, 
    @UserName nvarchar(100), 
    @CreatedTime DATETIME , 
    @LastUpdatedTime DATETIME ,
    @State INT,
	@ResourceUrl NVARCHAR(1000),
	@OrderType int,
	@ResourceId int
AS
BEGIN
INSERT INTO dbo.TradeOrder
(
	OrderId,
	Amount,
	[Subject],
	Body,
	UserName,
	CreatedTime,
	LastUpdatedTime,
	[State],
	ResourceUrl,
	OrderType,
	ResourceId
)
VALUES
(
	@OrderId,
	@Amount,
	@Subject,
	@Body,
	@UserName,
	@CreatedTime,
	@LastUpdatedTime,
	@State,
	@ResourceUrl,
	@OrderType,
	@ResourceId
)
END
GO