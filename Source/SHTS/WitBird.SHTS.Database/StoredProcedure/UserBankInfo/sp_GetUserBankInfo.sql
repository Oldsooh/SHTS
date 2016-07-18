CREATE PROCEDURE [dbo].[sp_GetUserBankInfo]
	@BankId int
AS
BEGIN
	SELECT * FROM dbo.UserBankInfo WHERE BankId = @BankId
END
GO
