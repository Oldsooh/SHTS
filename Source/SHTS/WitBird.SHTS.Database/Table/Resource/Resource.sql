CREATE TABLE [dbo].[Resource]
(
    [Id] INT NOT NULL IDENTITY(1,1),
    [ResourceType] INT NOT NULL,
    [Title] NVARCHAR(MAX) NOT NULL,
    [UserId] INT NOT NULL,
    [ProvinceId] NVARCHAR(MAX) NULL,
    [CityId] NVARCHAR(50) NULL,
    [AreaId] NVARCHAR(MAX) NULL,
    [DetailAddress] NVARCHAR(MAX) NULL, --详细地址
    [CanFriendlyLink] BIT NOT NULL,
    [Href] NVARCHAR(MAX) NULL,
    [ShortDesc] NVARCHAR(MAX) NULL, --简短的描述
    [Description] NVARCHAR(MAX) NULL,
    [ImageUrls] NVARCHAR(MAX) NULL,
    [Contract] NVARCHAR(MAX) NULL,
    [StartDate] DATETIME NULL, --预约开始时间
    [EndDate] DATETIME NULL, --预约结束时间
    [QQ] VARCHAR(MAX) NULL,
    [Telephone] NVARCHAR(MAX) NULL, --固定电话
    [Mobile] VARCHAR(MAX) NULL, --手机号码
    [WeChat] VARCHAR(MAX) NULL, --微信账号
    [Email] NVARCHAR(MAX) NULL,
    [CreateTime] DATETIME NOT NULL,
    [LastUpdatedTime] DATETIME NOT NULL,
    [ReadCount] INT NOT NULL, --阅读次数
    [State] INT NOT NULL, --1已经创建，2已经审核，3已经删除

    [ClickCount] INT NOT NULL, --签到次数

    [SpaceTypeId] INT NOT NULL,
    [SpaceFeatureValue] INT NOT NULL,
    [SpaceFacilityValue] INT NOT NULL,
    [SpaceSizeId] INT NOT NULL,
    [SpacePeopleId] INT NOT NULL,
    [SpaceTreat] INT NOT NULL,

    [ActorTypeId] INT NOT NULL,

    [EquipTypeId] INT NOT NULL,

    [OtherTypeId] INT NOT NULL,

    [ActorFromId] INT NOT NULL, --1个人 2团体 3公司
    [ActorSex] INT NOT NULL, --1男 2女

	[ClickTime] DATETIME NOT NULL,-- 最后一次签到时间
    [UserName] NVARCHAR(50) NOT NULL, 
	[Budget] INT NULL, 
    --用户名，与UserId对应

    CONSTRAINT PK_Resource PRIMARY KEY([Id] ASC)
)
