CREATE PROCEDURE [dbo].[sp_AddSMSRecord]
	@Cellphone NVARCHAR(50),
	@Provider NVARCHAR(50),
	@State INT
AS
BEGIN
	insert into ShortMessage values(@Cellphone,@Provider,GETDATE(),@State);
END
GO
