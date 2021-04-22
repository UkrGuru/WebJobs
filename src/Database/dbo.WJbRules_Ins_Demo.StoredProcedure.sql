SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE   PROCEDURE [dbo].[WJbRules_Ins_Demo]
	@Data nvarchar(max) 
AS
INSERT INTO WJbRules (Name, Priority, ActionId, MoreJson)
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(100), Priority tinyint, ActionId int, MoreJson nvarchar(max))

