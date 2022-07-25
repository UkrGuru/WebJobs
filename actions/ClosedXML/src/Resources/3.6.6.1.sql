BEGIN /***** Init Tables *****/

BEGIN /*** Init ClosedXML Actions ***/

SET IDENTITY_INSERT [WJbActions] ON 

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 50))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (50, N'ClosedXML.ImportFile', N'ClosedXML.ImportFileAction, UkrGuru.WebJobs.Actions.ClosedXML', N'{
	"file": ""
}', 0)

SET IDENTITY_INSERT [WJbActions] OFF

END

BEGIN /*** Init ClosedXML Rules ***/

SET IDENTITY_INSERT [WJbRules] ON

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 50))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (50, N'ClosedXML.ImportFile Base', 2, 50, NULL, 0)

SET IDENTITY_INSERT [WJbRules] OFF

END

END