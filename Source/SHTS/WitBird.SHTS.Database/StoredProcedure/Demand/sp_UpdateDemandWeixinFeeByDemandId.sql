CREATE PROCEDURE [dbo].[sp_UpdateDemandWeixinFeeByDemandId]
	@DemandId int,
	@WeixinBuyFee int
AS
	UPDATE Demand SET WeixinBuyFee = @WeixinBuyFee WHERE Id = @DemandId
