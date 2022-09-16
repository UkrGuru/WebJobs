# UkrGuru.WebJobs
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.WebJobs)](https://www.nuget.org/packages/UkrGuru.WebJobs/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

The UkrGuru.WebJobs package is a Scheduler and N-Workers for any base( or custom) Actions in .NET apps. 
Supports CRON expressions in Rules. Supports polymorphism for Action/Rule/Job parameters and 
transferring the result of the Job to the next Job, based on the results of the current Job. 
Uses UkrGuru.SqlJson to quickly run stored procedures on sql server.

Standard Actions:
- DownloadPage - download the page and save the result to WJbFiles table for further processing.
- FillTemplate - fill the template with your variable values and stored result in a variable for further processing.
- ParseText - parse the text by goals into a variable in json format for further processing.
- ProcItems - processing for each(or selected) item of the file.
- RunApiProc - run sql server stored procedure throught ApiHole and stored result in a variable for further processing.
- RunSqlProc - run sql server stored procedure and stored result in a variable for further processing.
- SendEmail - send email via your smtp settings.
- SSRS.ExportReport - export the SSRS report in any valid format to the WJbFiles table.

ClosedXML Actions:
- ImportFile - import Excel Sheet into WJbItems table.

CsvHelper Actions:
- ImportFile - import CSV file into WJbItems table.

MailKit Actions:
- ReceiveEmails - receive emails from the mailbox in WJbFiles, then creates a WJbQueue record with the result for further processing.

SshNet Actions:
- GetFiles - download files from remote directory to WJbFiles table with SFtpClient.
- PutFiles - upload files from WJbFiles to SFTP remote directory with SFtpClient.
