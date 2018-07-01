-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateServiceLine] @ServiceID int, @ServiceLineOrder tinyint, @ServiceLineType tinyint, @ServiceLineDesc nvarchar(50), @ServiceLineCharge money
	
AS
BEGIN
	if @ServiceLineOrder=0 begin
		delete ServiceLine where ServiceID=@ServiceID -- going to replace all the detail lines
	end
	insert ServiceLine values(@ServiceID, @ServiceLineOrder, @ServiceLineType, @ServiceLineDesc, @ServiceLineCharge) 
END