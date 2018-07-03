-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectCars]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT CarID, Make, Model, [Year], Owner,
		case when exists(select CarID from Service where service.CarID=car.CarID) then cast(1 as bit) else cast(0 as bit) end as HasService
		from Car
END