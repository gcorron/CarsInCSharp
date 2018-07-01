-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE DeleteService @ServiceID int
AS
BEGIN
		delete ServiceLine
			where ServiceID=@ServiceID --delete all lines first
		delete Service
			where ServiceID=@ServiceID
END