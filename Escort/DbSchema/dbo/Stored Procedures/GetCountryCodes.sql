CREATE PROCEDURE [dbo].[GetCountryCodes]
  AS
  /*
  GetCountryCodes
  */
  BEGIN
	SELECT CountryCode, CountryName, CountryId
	FROM Country 
  END