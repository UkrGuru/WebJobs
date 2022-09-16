BEGIN /***** Init Tables *****/

BEGIN /*** Init SshNet Actions ***/

SET IDENTITY_INSERT [WJbActions] ON 

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 20))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (20, N'SshNet.GetFiles', N'SshNet.GetFilesAction, UkrGuru.WebJobs.Actions.SshNet', N'{
	"sshnet_settings_name": "",
	"remote_path": null,
	"proc_rule": null
}', 0)

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 21))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (21, N'SshNet.PutFiles', N'SshNet.PutFilesAction, UkrGuru.WebJobs.Actions.SshNet', N'{
	"sshnet_settings_name": "",
	"remote_path": null,
	"files": null
}', 0)

SET IDENTITY_INSERT [WJbActions] OFF

END

BEGIN /*** Init SshNet Rules ***/

SET IDENTITY_INSERT [WJbRules] ON

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 20))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (20, N'SshNet.GetFiles Base', 2, 20, N'{
	"sshnet_settings_name": "Sftp1",
	"remote_path": ".",
	"proc_rule": null
}', 0)

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 21))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (21, N'SshNet.PutFiles Base', 2, 21, N'{
	"sshnet_settings_name": "Sftp1",
	"remote_path": ".",
	"files": null
}', 0)

SET IDENTITY_INSERT [WJbRules] OFF

END

BEGIN /*** Init SshNet Settings ***/

IF NOT EXISTS (SELECT * FROM [dbo].[WJbSettings] WHERE [Name] = N'Sftp1')
	INSERT [dbo].[WJbSettings] ([Name], [Value]) VALUES (N'Sftp1', N'{
	"host": "domain.com",
	"port": 22,
	"userName": "test",
	"password": "test",
	"keyFiles": null,
	"fingerPrint": null
}')

END

END