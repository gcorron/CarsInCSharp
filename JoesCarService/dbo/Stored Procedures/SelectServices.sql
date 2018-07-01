-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectServices] (@begindate smalldatetime)
	
AS
BEGIN

	select * from service a
		left outer join ServiceLine b
		on a.ServiceID=b.ServiceID

		where ServiceDate>=@begindate

END