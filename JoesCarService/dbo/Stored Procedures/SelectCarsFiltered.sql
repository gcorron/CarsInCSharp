
CREATE PROCEDURE [dbo].[SelectCarsFiltered] @year int, @owner nvarchar(50) 
AS
BEGIN

if @year=0 BEGIN
	if @owner is null or @owner = ''
	begin
		select top 10 * from car
		order by [year],owner
		return
	end
		
	select * from car
	where (owner = @owner)
END

else BEGIN
	if @owner is null or @owner = ''
	begin
		select top 10 * from car
		where ([year]=@year)
		return
	end

	select top 10 * from car
	where (owner like @owner + '%')
	and ([year]=@year)
	order by owner
	return
END 

END