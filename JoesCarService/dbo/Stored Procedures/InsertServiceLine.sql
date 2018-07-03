-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE InsertServiceLine @ServiceID int, @ServiceLineOrder tinyint, @ServiceLineType tinyint, @ServiceLineDesc varchar(50), @ServiceLineCharge money
	
AS
BEGIN

insert ServiceLine (ServiceID, ServiceLineOrder, ServiceLineType, ServiceLineDesc, ServiceLineCharge)
	values(@ServiceID, @ServiceLineOrder, @ServiceLineType, @ServiceLineDesc, @ServiceLineCharge)


END