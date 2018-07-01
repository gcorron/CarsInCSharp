
CREATE PROCEDURE [dbo].[SelectServicesForCar] @CarID int 
AS
BEGIN

	SELECT * from Service S
		inner join ServiceLine L
		on S.ServiceID=L.ServiceID
		where CarID=@CarID
		order by L.ServiceLineOrder
END