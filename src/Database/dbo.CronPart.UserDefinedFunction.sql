SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*
-- minute, mi, n
-- hour, hh
-- day, dd, d
-- month, mm, m
-- weekday, dw, w

SELECT [dbo].[CronPart]('1 2 3 4 5', 'yy')
*/
CREATE FUNCTION [dbo].[CronPart] (@Expression varchar(100), @CronPart varchar(10))
RETURNS varchar(100)
AS
BEGIN
    DECLARE @Result varchar(100);

    DECLARE @PartPos int = CASE
        WHEN @CronPart in ('minute', 'mi', 'n') THEN 1
        WHEN @CronPart in ('hour', 'hh') THEN 2
        WHEN @CronPart in ('day', 'dd', 'd') THEN 3
        WHEN @CronPart in ('month', 'mm', 'm') THEN 4
        WHEN @CronPart in ('weekday', 'dw', 'w') THEN 5
        ELSE 0 END

    IF @PartPos = 0 GOTO EXIT_PROC

    DECLARE @CurrPos int = 1
    WHILE CHARINDEX(' ', @Expression) > 0 BEGIN
        IF @CurrPos = @PartPos BEGIN
            SET @Result = SUBSTRING(@Expression, 1, CHARINDEX(' ', @Expression) - 1)
            GOTO EXIT_PROC
        END
        SET @Expression = SUBSTRING(@Expression, CHARINDEX(' ', @Expression) + 1, 100)
        SET @CurrPos = @CurrPos + 1
    END

    SET @Result = SUBSTRING(@Expression, 1, 100)

EXIT_PROC:
    RETURN @Result
END

