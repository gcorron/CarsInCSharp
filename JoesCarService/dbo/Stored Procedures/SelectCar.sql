CREATE PROCEDURE SelectCar @CarID int 
AS
BEGIN
	SELECT CarID, Make, Model, [Year], Owner
		from car
		where CarID=@CarID
END