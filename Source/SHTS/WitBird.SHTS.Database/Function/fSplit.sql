create function [dbo].[fSplit](
    @string nvarchar (max),
    @delimiter nvarchar (10)
)
returns @ValueTable table ([Value] nvarchar(max))

begin 
    declare @NextString nvarchar(max)
    declare @Pos int
    declare @NextPos int
    declare @CommaCheck nvarchar(1)

    set @NextString = ''
    set @CommaCheck = right(@String,1) 
 
    --Check for trailing Comma, if not exists, INSERT
    if (@CommaCheck <> @delimiter )
        begin
            set @string = @string + @delimiter
        end
 
    --Get position of first Comma
    set @Pos = charindex(@delimiter,@string)
    set @NextPos = 1

    -- Loop while there is still a comma in the String of levels
    while (@pos <>  0)  
        begin
            set @NextString = substring(@string,1,@Pos - 1)
 
            insert into @ValueTable ( [Value]) Values (@NextString)
 
            set @string = substring(@string,@pos +1,len(@string))  
            set @NextPos = @Pos
            set @pos  = charindex(@delimiter,@string)
         end
     return
end
go