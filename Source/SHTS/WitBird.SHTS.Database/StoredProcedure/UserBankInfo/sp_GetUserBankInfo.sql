CREATE PROCEDURE [dbo].[sp_GetUserBankInfo]
	@BankId int
AS
	SELECT * FROM dbo.UserBankInfo WHERE BankId = @BankId
