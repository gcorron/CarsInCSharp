-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE DeleteCar @CarID int
AS
BEGIN
		delete car
		where CarID=@CarID
END