SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


CREATE FUNCTION [dbo].[CronExpandAll] (@Expression varchar(100), @Min int, @Max int) 
RETURNS @Values TABLE (Value int)
AS BEGIN
	INSERT @Values (Value) 
	SELECT DISTINCT Value = CONVERT(int, Value) FROM (
		SELECT Value FROM dbo.CronExpandInt(@Expression, @Min, @Max)
		--UNION
		--SELECT DISTINCT Value FROM dbo.CronExpandStar(@Expression, @Min, @Max)
		UNION
		SELECT DISTINCT Value FROM STRING_SPLIT(@Expression, ',')
		WHERE ISNUMERIC(Value) = 1 AND Value BETWEEN @Min AND @Max
		UNION
		SELECT DISTINCT E.Value FROM STRING_SPLIT(@Expression, ',') S
		CROSS APPLY dbo.CronExpandStep(S.value, @Min, @Max) E
		UNION
		SELECT DISTINCT E.Value FROM STRING_SPLIT(@Expression, ',') S
		CROSS APPLY dbo.CronExpandRange(S.Value, @Min, @Max) E
	) AS V
	ORDER BY Value
	
	RETURN
END

