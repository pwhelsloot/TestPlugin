CREATE FUNCTION [dbo].[fn_ColumnsForBulkInsert] (@TableName NVARCHAR(500), @SchemaName NVARCHAR(500))
RETURNS TABLE AS RETURN
(
	SELECT
		col.name as ColumnName
	FROM
		sysobjects obj
		INNER JOIN syscolumns col ON obj.id = col.id
		INNER JOIN sys.tables sysTable ON sysTable.object_id = obj.id
	WHERE
		obj.name = @TableName
		AND SCHEMA_NAME(sysTable.Schema_id) = @SchemaName
		AND col.iscomputed = 0
)
GO