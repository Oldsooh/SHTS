CREATE PROCEDURE [dbo].[sp_AddNewTrade]
	@TradeId int output,
	@UserId int,
	@UserQQ nvarchar(20),
	@UserCellPhone nvarchar(20),
	@UserEmail nvarchar(MAX),
	@UserBankInfo nvarchar(MAX),
	@UserAddress nvarchar(MAX),
	@SellerId int,
	@BuyerId int,
	@TradeAmount decimal(18,2),
	@TradeSubject ntext,
	@TradeBody ntext,
	@Payer int,
	@PayCommission decimal(18,2),
	@PayCommissionPercent float,
	@CreatedTime datetime,
	@LastUpdatedTime datetime,
	@State int,
	@SellerGet decimal(18,2),
	@BuyerPay decimal(18,2),
	@ViewCount int,
	@ResourceUrl nvarchar(max),
	@IsBuyerPaid bit,
	@TradeOrderId int
AS
BEGIN
INSERT INTO dbo.Trade
(
	UserId,
	UserQQ,
	UserCellPhone,
	UserEmail,
	UserBankInfo,
	UserAddress,
	SellerId,
	BuyerId,
	TradeAmount,
	TradeSubject,
	TradeBody,
	Payer,
	PayCommission,
	PayCommissionPercent,
	CreatedTime,
	LastUpdatedTime,
	[State],
	SellerGet,
	BuyerPay,
	ViewCount,
	ResourceUrl,
	IsBuyerPaid,
	TradeOrderId
)
VALUES
(
	@UserId,
	@UserQQ,
	@UserCellPhone,
	@UserEmail,
	@UserBankInfo,
	@UserAddress,
	@SellerId,
	@BuyerId,
	@TradeAmount,
	@TradeSubject,
	@TradeBody,
	@Payer,
	@PayCommission,
	@PayCommissionPercent,
	@CreatedTime,
	@LastUpdatedTime,
	@State,
	@SellerGet,
	@BuyerPay,
	@ViewCount,
	@ResourceUrl,
	@IsBuyerPaid,
	@TradeOrderId
)

SET @TradeId = @@IDENTITY
END
GO