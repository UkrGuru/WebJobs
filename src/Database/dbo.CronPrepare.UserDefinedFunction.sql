SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


CREATE FUNCTION [dbo].[CronPrepare] (@Expression varchar(100)) 
RETURNS varchar(100)
AS BEGIN
	RETURN 
		REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		REPLACE(UPPER(@Expression), 'JAN', '1'),
		'FEB', '2'),'MAR', '3'),'APR', '4'),'MAY', '5'),'JUN', '6'),'JUL', '7'),
		'AUG', '8'),'SEP', '9'),'OCT', '10'),'NOV', '11'),'DEC', '12'),'SUN', '1'),
		'MON', '2'),'TUE', '3'),'WED', '4'),'THU', '5'),'FRI', '6'),'SAT', '7')
END

