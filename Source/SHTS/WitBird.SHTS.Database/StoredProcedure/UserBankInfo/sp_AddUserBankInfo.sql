CREATE PROCEDURE [dbo].[sp_AddUserBankInfo]
	@UserId int,
	@BankName nvarchar(200),
	@BankAccount nvarchar(200),
	@BankUserName nvarchar(50),
	@BankAddress nvarchar(max),
	@IsDefault bit,
	@CreatedTime datetime,
	@LastUpdatedTime datetime
AS
BEGIN
INSERT INTO dbo.UserBankInfo 
(
	UserId,
	BankName,
	BankAccount,
	BankUserName,
	BankAddress,
	IsDefault,
	CreatedTime,
	LastUpdatedTime
)
VALUES
(
	@UserId,
	@BankName,
	@BankAccount,
	@BankUserName,
	@BankAddress,
	@IsDefault,
	@CreatedTime,
	@LastUpdatedTime
)

SELECT @@IDENTITY
END
GO