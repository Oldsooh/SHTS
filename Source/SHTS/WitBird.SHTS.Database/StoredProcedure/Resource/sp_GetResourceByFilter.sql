ALTER PROCEDURE [dbo].[sp_GetResourceByFilter]
(
        @state INT,
        @cityId NVARCHAR(50),
        @areaId NVARCHAR(50),
        @resourceType INT,
        @spaceTypeId INT,
        @spaceFeature INT,
        @spaceFacility INT,
        @spaceSizeId INT,
        @spacePeopleId INT,
        @spaceTreat INT,
        @actorTypeId INT,
        @equipTypeId INT,
        @otherTypeId INT,
        @pageIndex INT,
        @pageSize INT,
        @actorFromId INT,
        @actorSex INT,
		@budgetCondition NVARCHAR(200)
)
AS
BEGIN
	DECLARE @sqlText nvarchar(max)
    
	SET @sqlText = N'SELECT COUNT(1) AS TotalCount FROM [dbo].[Resource] Res
    WHERE
        (Res.[State] = 1 OR Res.[State] = 2)
        AND (@cityId IS NULL OR @cityId = N'''' OR Res.[CityId] = @cityId)
        AND (@areaId IS NULL OR @areaId = N'''' OR Res.[AreaId] = @areaId)
        AND (@resourceType = 0 OR Res.[ResourceType] = @resourceType ) 
        AND (@spaceTypeId = 0 OR Res.[SpaceTypeId] = @spaceTypeId)
        AND (@spaceFeature = 0 OR (Res.[SpaceFeatureValue] & @spaceFeature)>0) 
        AND (@spaceFacility = 0 OR (Res.[SpaceFacilityValue] & @spaceFacility)>0)
		AND (@spaceSizeId = 0 OR Res.[SpaceSizeId] = @spaceSizeId)
        AND (@spacePeopleId = 0 OR Res.[SpacePeopleId] = @spacePeopleId)
        AND (@spaceTreat = 0 OR Res.[SpaceTreat] = @spaceTreat)
        AND (@actorTypeId = 0 OR Res.[ActorTypeId] = @actorTypeId)
        AND (@equipTypeId = 0 OR Res.[EquipTypeId] = @equipTypeId)
        AND (@otherTypeId = 0 OR Res.[OtherTypeId] = @otherTypeId)
        AND (@actorFromId = 0 OR Res.[ActorFromId] = @actorFromId)
        AND (@actorSex = 0 OR Res.[ActorSex] = @actorSex) '
	
	IF (@budgetCondition <> '')
	BEGIN
		SET @sqlText += N'
		AND ' + @budgetCondition
	END
	
	SET @sqlText +=N'
    ;WITH T AS
    (SELECT ROW_NUMBER() OVER(order by ClickTime DESC) number,*
    FROM      dbo.Resource AS Res
    WHERE
        (Res.[State] = 1 OR Res.[State] = 2)
        AND (@cityId IS NULL OR @cityId = N'''' OR Res.[CityId] = @cityId)
        AND (@areaId IS NULL OR @areaId = N'''' OR Res.[AreaId] = @areaId)
        AND (@resourceType = 0 OR Res.[ResourceType] = @resourceType )
        AND (@spaceTypeId = 0 OR Res.[SpaceTypeId] = @spaceTypeId)
        AND (@spaceFeature = 0 OR (Res.[SpaceFeatureValue] & @spaceFeature)>0)
        AND (@spaceFacility = 0 OR (Res.[SpaceFacilityValue] & @spaceFacility)>0)
		AND (@spaceSizeId = 0 OR Res.[SpaceSizeId] = @spaceSizeId)
        AND (@spacePeopleId = 0 OR Res.[SpacePeopleId] = @spacePeopleId)
        AND (@spaceTreat = 0 OR Res.[SpaceTreat] = @spaceTreat)
        AND (@actorTypeId = 0 OR Res.[ActorTypeId] = @actorTypeId)
        AND (@equipTypeId = 0 OR Res.[EquipTypeId] = @equipTypeId)
        AND (@otherTypeId = 0 OR Res.[OtherTypeId] = @otherTypeId)
        AND (@actorFromId = 0 OR Res.[ActorFromId] = @actorFromId)
        AND (@actorSex = 0 OR Res.[ActorSex] = @actorSex) '
	IF (@budgetCondition <> '')
	BEGIN
		SET @sqlText += N'
		AND ' + @budgetCondition
	END

	SET @sqlText +=	N')
    SELECT * FROM T
    WHERE T.number between (@pageIndex*@pageSize+1) and (@pageIndex*@pageSize+@pageSize)'


	print @sqlText

	EXEC sp_executesql @sqlText, N'@state INT,
        @cityId NVARCHAR(50),
        @areaId NVARCHAR(50),
        @resourceType INT,
        @spaceTypeId INT,
        @spaceFeature INT,
        @spaceFacility INT,
        @spaceSizeId INT,
        @spacePeopleId INT,
        @spaceTreat INT,
        @actorTypeId INT,
        @equipTypeId INT,
        @otherTypeId INT,
        @pageIndex INT,
        @pageSize INT,
        @actorFromId INT,
        @actorSex INT,
		@budgetCondition NVARCHAR(200)', 
		@state,
        @cityId,
        @areaId,
        @resourceType,
        @spaceTypeId,
        @spaceFeature,
        @spaceFacility,
        @spaceSizeId,
        @spacePeopleId,
        @spaceTreat,
        @actorTypeId,
        @equipTypeId,
        @otherTypeId,
        @pageIndex,
        @pageSize,
        @actorFromId,
        @actorSex,
		@budgetCondition
END
GO