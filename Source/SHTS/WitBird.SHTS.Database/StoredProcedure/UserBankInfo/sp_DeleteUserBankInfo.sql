CREATE PROCEDURE [dbo].[sp_DeleteUserBankInfo]
	@BankId int
AS

DELETE FROM dbo.UserBankInfo WHERE BankId = @BankId

SELECT @@IDENTITY
