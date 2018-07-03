-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateCar] (@carID int, @make nvarchar(50), @model nvarchar(50), @year int, @owner nvarchar(50))

AS
BEGIN
-- performs insert or update depending on @carID, returns a value as ID

	if @carID=0 begin -- pass 0 in ID means insert
		insert car (make,model,[year],owner) values (@make, @model, @year, @owner)
		-- Insert statements for procedure here
		select SCOPE_IDENTITY() as ret
	end

	update car -- pass positive in ID means update
		set make=@make,model=@model, [year]=@year, owner=@owner
	where CarID=@carID
	select @carID as ret
END