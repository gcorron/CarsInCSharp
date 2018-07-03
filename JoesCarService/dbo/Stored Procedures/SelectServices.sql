-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectServices] (@begindate smalldatetime)
	
AS
BEGIN

	select [a].[ServiceID], [a].[ServiceDate], [a].[TechName], [a].[LaborCost], [a].[PartsCost], [a].[CarID]
		, [b].[ServiceID], [b].[ServiceLineOrder], [b].[ServiceLineType], [b].[ServiceLineDesc], [b].[ServiceLineCharge] from service a
		left outer join ServiceLine b
		on a.ServiceID=b.ServiceID

		where ServiceDate>=@begindate

END