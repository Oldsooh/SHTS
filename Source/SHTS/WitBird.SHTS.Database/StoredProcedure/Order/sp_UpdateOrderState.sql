﻿CREATE PROCEDURE [dbo].[sp_UpdateOrderState]
	@OrderId nvarchar(50),
	@State INT,
	@LastUpdatedTime DATETIME
AS

UPDATE dbo.TradeOrder SET [State] = @State, [LastUpdatedTime] = @LastUpdatedTime WHERE OrderId = @OrderId
