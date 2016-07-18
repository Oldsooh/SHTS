CREATE PROCEDURE [dbo].[sp_UpdateDemandWeixinFeeByDemandId]
	@DemandId int,
	@WeixinBuyFee int
AS
BEGIN
	UPDATE Demand SET WeixinBuyFee = @WeixinBuyFee WHERE Id = @DemandId
END
GO
