SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbActions_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbActions (Name, Type, MoreJson)
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(100), Type nvarchar(255), MoreJson nvarchar(max))

