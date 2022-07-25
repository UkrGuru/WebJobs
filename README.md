# UkrGuru.WebJobs
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.WebJobs)](https://www.nuget.org/packages/UkrGuru.WebJobs/)
[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

The UkrGuru.WebJobs package is the Scheduler and N-Workers for ANY of your background jobs in .NET 6. applications. 
Supports CRON expressions in Rules. Provides extensibility for any custom aActions, polymorphism for Action/Rule/Job parameters 
and transferring the result of the Job to the next Job, based on the results of the current Job. Uses UkrGuru.SqlJson to connect to the database.
Standard Actions:
- DownloadPage - download the page and save the result to WJbFiles table for further processing.
- FillTemplate - fill the template with your variable values and stored result in a variable for further processing.
- ProcItems - processing for each(or selected) item of the file.
- RunApiProc - run sql server stored procedure throught ApiHole and stored result in a variable for further processing.
- RunSqlProc - run sql server stored procedure and stored result in a variable for further processing.
- SendEmail - send email via your smtp settings.

SSRS Actions:
- ExportReport - export SSRS report to WJbFiles table.

SshNet Actions:
- GetFiles - download files from remote directory to WJbFiles table.
- PutFiles - upload files from WJbFiles to remote directory.

MailKit Actions:
- ReceiveEmails - receive emails from the mailbox in WJbFiles and WJbQueue for further processing.

CsvHelper Actions:
- ImportFile - import CSV file into WJbItems table.