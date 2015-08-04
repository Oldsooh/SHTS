CREATE FUNCTION [dbo].[SplitStringToIds]
(	
@string nvarchar(20)
)
RETURNS @Ids TABLE(id int)
AS
begin
	DECLARE @srtid nvarchar(20);

	declare   @i int
	set @i=LEN(@string)
	while   @i>0  
	begin
		select 
		@srtid=substring(@string,@i,1);   
		set @i=@i-2;
		insert into @Ids select @srtid
	end
	return;
end
