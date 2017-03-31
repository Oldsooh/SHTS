CREATE PROCEDURE [dbo].[sp_CheckOrderForDemandBonusByDemandId]
	@DemandId int
AS
	IF EXISTS (SELECT 1 FROM TradeOrder WHERE OrderType = 3 AND ResourceId = @DemandId)
	BEGIN
		SELECT 1
	END
	ELSE
	BEGIN
		SELECT 0
	END
GO
