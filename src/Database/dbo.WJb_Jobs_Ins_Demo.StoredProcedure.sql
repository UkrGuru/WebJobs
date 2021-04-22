SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJb_Jobs_Ins_Demo]
AS
INSERT INTO WJbQueue ( RuleId, Priority, MoreJson)
SELECT Id, Priority, N'{ "data": "5" }'
FROM WJbRules
WHERE (Id = 2) AND (Disabled = 0)

INSERT INTO WJbQueue ( RuleId, Priority, MoreJson)
SELECT Id, Priority, N'{ "data": "7" }'
FROM WJbRules
WHERE (Id = 100) AND (Disabled = 0)

