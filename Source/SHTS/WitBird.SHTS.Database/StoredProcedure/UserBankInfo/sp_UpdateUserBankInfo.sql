CREATE PROCEDURE [dbo].[sp_UpdateUserBankInfo]
	@BankId int,
	@UserId int,
	@BankName nvarchar(200),
	@BankAccount nvarchar(200),
	@BankUserName nvarchar(50),
	@BankAddress nvarchar(max),
	@IsDefault bit,
	@LastUpdatedTime datetime
AS
BEGIN
IF @IsDefault = 1
BEGIN
	UPDATE dbo.UserBankInfo SET IsDefault = 0 WHERE UserId = @UserId
END

UPDATE dbo.UserBankInfo SET
	BankName = @BankName,
	BankAccount = @BankAccount,
	BankUserName = @BankUserName,
	BankAddress = @BankAddress,
	IsDefault = @IsDefault,
	LastUpdatedTime = @LastUpdatedTime
WHERE BankId = @BankId

SELECT @@IDENTITY
END
GO