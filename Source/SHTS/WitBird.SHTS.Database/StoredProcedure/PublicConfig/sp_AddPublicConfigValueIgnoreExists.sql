CREATE PROCEDURE [dbo].[sp_AddPublicConfigValueIgnoreExists]
	@ConfigId int,
	@ConfigName nvarchar(100),
	@ConfigValue nvarchar(max),
	@CreatedTime datetime,
	@LastUpdatedTime datetime
AS

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

SELECT @@IDENTITY
