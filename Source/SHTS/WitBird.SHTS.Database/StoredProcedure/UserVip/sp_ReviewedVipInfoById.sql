
CREATE PROCEDURE [dbo].[sp_ReviewedVipInfoById]
@Id int,
@State int
AS
BEGIN
update dbo.UserVip set [State] = @State Where Id = @Id
END
GO