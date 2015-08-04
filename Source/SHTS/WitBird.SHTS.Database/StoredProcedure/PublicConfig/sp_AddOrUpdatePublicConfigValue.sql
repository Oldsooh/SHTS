CREATE PROCEDURE [dbo].[sp_AddOrUpdatePublicConfigValue]
	@ConfigId int,
	@ConfigName nvarchar(100),
	@ConfigValue nvarchar(max),
	@CreatedTime datetime,
	@LastUpdatedTime datetime
AS

IF EXISTS (SELECT 1 FROM dbo.PublicConfig WHERE ConfigName = @ConfigName)
BEGIN
	UPDATE dbo.PublicConfig SET 
		ConfigValue = @ConfigValue,
		LastUpdatedTime = @LastUpdatedTime
	WHERE ConfigName = @ConfigName OR ConfigId = @ConfigId
END

ELSE
BEGIN

INSERT INTO dbo.PublicConfig
(
	ConfigName, 
	ConfigValue, 
	CreatedTime, 
	LastUpdatedTime
) VALUES
(
	@ConfigName,
	@ConfigValue,
	@CreatedTime,
	@LastUpdatedTime
)
END

SELECT @@IDENTITY
