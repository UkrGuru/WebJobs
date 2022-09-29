BEGIN /***** Init Tables *****/

BEGIN /*** Init CsvHelper Actions ***/

SET IDENTITY_INSERT [WJbActions] ON 

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 40))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (40, N'CsvHelper.ImportFile', N'CsvHelper.ImportFileAction, UkrGuru.WebJobs.Actions.CsvHelper', N'{
	"file": ""
}', 0)

SET IDENTITY_INSERT [WJbActions] OFF

END

BEGIN /*** Init CsvHelper Rules ***/

SET IDENTITY_INSERT [WJbRules] ON

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 40))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (40, N'CsvHelper.ImportFile Base', 2, 40, NULL, 0)

SET IDENTITY_INSERT [WJbRules] OFF

END

END