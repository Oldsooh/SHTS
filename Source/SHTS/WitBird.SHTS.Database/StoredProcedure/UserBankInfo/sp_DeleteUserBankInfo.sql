CREATE PROCEDURE [dbo].[sp_DeleteUserBankInfo]
	@BankId int
AS
BEGIN
DELETE FROM dbo.UserBankInfo WHERE BankId = @BankId

SELECT @@IDENTITY
END
GO
