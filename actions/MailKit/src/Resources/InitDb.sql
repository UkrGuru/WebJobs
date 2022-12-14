BEGIN /***** Init Tables *****/

BEGIN /*** Init MailKit Actions ***/

SET IDENTITY_INSERT [WJbActions] ON 

IF NOT EXISTS (SELECT 1 FROM [WJbActions] WHERE (ActionId = 30))
	INSERT [WJbActions] ([ActionId], [ActionName], [ActionType], [ActionMore], [Disabled]) 
	VALUES (30, N'MailKit.ReceiveEmails', N'MailKit.ReceiveEmailsAction, UkrGuru.WebJobs.Actions.MailKit', N'{
	"pop3_settings_name": "",
	"proc_rule": null
}', 0)

SET IDENTITY_INSERT [WJbActions] OFF

END

BEGIN /*** Init MailKit Rules ***/

SET IDENTITY_INSERT [WJbRules] ON

IF NOT EXISTS (SELECT 1 FROM [WJbRules] WHERE (RuleId = 30))
	INSERT [WJbRules] ([RuleId], [RuleName], [RulePriority], [ActionId], [RuleMore], [Disabled]) 
	VALUES (30, N'MailKit.ReceiveEmails Base', 2, 30, N'{
	"pop3_settings_name": "Mailbox1",
	"proc_rule": null
}', 0)

SET IDENTITY_INSERT [WJbRules] OFF

END

BEGIN /*** Init MailKit Settings ***/

IF NOT EXISTS (SELECT * FROM [dbo].[WJbSettings] WHERE [Name] = 'Mailbox1')
	INSERT [dbo].[WJbSettings] ([Name], [Value]) VALUES (N'Mailbox1', N'{
	"host": "domain.com",
	"port": 995,
	"useSsl": true,
	"userName": "test",
	"password": "test"
}')

END

END