
CREATE PROCEDURE SelectXsltSheet @id int 
AS
BEGIN

	SELECT [Xml] from dbo.XSLTSheets
	where id=@id 
END