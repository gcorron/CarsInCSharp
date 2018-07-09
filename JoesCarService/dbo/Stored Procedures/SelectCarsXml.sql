CREATE PROCEDURE [dbo].[SelectCarsXml]
AS
	SELECT * from Car
	For XML raw, ROOT
