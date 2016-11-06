CREATE TABLE [dbo].[ActivityVote]
(
	[VoteId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NULL, 
    [WechatUserOpenId] NVARCHAR(50) NULL, 
	[ActivityId] INT NOT NULL,
    [IsVoted] BIT NOT NULL , 
    [InsertedTimestamp] DATETIME NOT NULL, 
    [LastUpdatedTimestamp] DATETIME NOT NULL
)
