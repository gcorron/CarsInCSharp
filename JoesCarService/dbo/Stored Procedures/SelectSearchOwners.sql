-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectSearchOwners] @search as varchar(50)
AS
BEGIN
	SELECT distinct top 5 Owner
	from car
	where owner like @search + '%'
	order by owner
END