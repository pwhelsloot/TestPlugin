CREATE FUNCTION [dbo].[fn_GlossaryTranslation] (@input NVARCHAR(200), @languageCode NVARCHAR(30))
RETURNS NVARCHAR(200)
AS
BEGIN
	BEGINNING:
	DECLARE @output NVARCHAR(200);

	SELECT	TOP 1 @output = [Translated]
	FROM	[dbo].[GlossaryInternalCache]
	WHERE	[dbo].[GlossaryInternalCache].[Original] = @input 
		AND [LanguageCode] = @languageCode;

	IF(@output IS NOT NULL)
		RETURN @output;

	DECLARE @hyphenIndex INT =  CHARINDEX('-', REVERSE(@languageCode));

	IF (@hyphenIndex = 0)
		RETURN @input;

	DECLARE @lastIndex INT = (LEN(@languageCode)) - @hyphenIndex;
	
	SET @languageCode = SUBSTRING(@languageCode, 0, @lastIndex + 1);
	GOTO BEGINNING;

	RETURN @output
END