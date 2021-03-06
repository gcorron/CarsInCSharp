﻿
CREATE PROCEDURE [dbo].[SelectServicesForCar] @CarID int 
AS
BEGIN

	SELECT [S].[ServiceID], [S].[ServiceDate], [S].[TechName], [S].[LaborCost], [S].[PartsCost], [S].[CarID]
		, [L].[ServiceID], [L].[ServiceLineOrder], [L].[ServiceLineType], [L].[ServiceLineDesc], [L].[ServiceLineCharge] from Service S
		inner join ServiceLine L
		on S.ServiceID=L.ServiceID
		where CarID=@CarID
		order by L.ServiceLineOrder
END