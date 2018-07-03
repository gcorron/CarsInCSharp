-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateService] @ServiceID int, @ServiceDate smalldatetime,@TechName varchar(50), @LaborCost money, @PartsCost money, @CarID int
	
AS
BEGIN

	-- performs insert or update depending on @ServiceID, returns a value as ID

	if @ServiceID=0 begin -- pass 0 in ID means insert
		insert service (ServiceDate,TechName,LaborCost, PartsCost, CarID)
			values (@ServiceDate,@TechName,@LaborCost,@PartsCost,@CarID)
		select SCOPE_IDENTITY() as ret
	end

	update Service
		set ServiceDate=@ServiceDate,
			TechName=@TechName,
			LaborCost=@LaborCost,
			PartsCost=@PartsCost,
			CarID=@CarID
	where ServiceID=@ServiceID
	
	select -1 as ret -- means updated


END