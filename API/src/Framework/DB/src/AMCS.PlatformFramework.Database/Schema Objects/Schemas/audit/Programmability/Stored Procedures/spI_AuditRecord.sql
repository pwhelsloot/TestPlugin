CREATE PROCEDURE [audit].[spI_AuditRecord] (@SysUserId int, @TableName nvarchar(256), @KeyName nvarchar(256), @KeyId int, @ChangeType char(1))
AS
BEGIN
  DECLARE @columns nvarchar(max), @postsql nvarchar(max), @parameters nvarchar(max)
           
  IF EXISTS(SELECT 1 FROM dbo.AuditTable WHERE TableName = @TableName and AuditOn = 1)
  BEGIN      
    SELECT 
      @columns = COALESCE(@columns+',' ,'') + '[' + C.name + ']'
    FROM 
      sys.objects  o
      JOIN sys.schemas s on s.schema_id = o.schema_id
      JOIN sys.objects ao ON ao.name = '_' + o.name
      JOIN sys.columns c on c.object_id = o.object_id
      JOIN sys.columns ac on ac.object_id = ao.object_id and ac.name = c.name
    WHERE
      s.name = N'dbo'
      and o.name = @TableName

    SET @postsql =
      N'INSERT INTO audit._' + @TableName + '([_ChangedDate], [_ChangedById], [_ChangeType], ' + @columns + ') ' +
      'SELECT SYSDATETIMEOFFSET(), @SysUserId, @ChangeType, ' + REPLACE(@columns,'[RowVersion]','CONVERT(binary(8),[RowVersion])') + ' ' +
      'FROM dbo.' + @TableName + ' ' +
      'WHERE ' + @KeyName + ' = @KeyId'
      SET @parameters = N'@SysUserId [int], @ChangeType [char](1), @KeyId [int]';

    BEGIN TRY
      EXECUTE sp_executesql @postsql, @parameters, @SysUserId = @SysUserId, @ChangeType = @ChangeType, @KeyId = @KeyId;
    END TRY
    BEGIN CATCH
      INSERT INTO audit.RecordException (PostSQL, TableName, KeyName, ErrorMessage) VALUES (@postsql, @TableName, @KeyName, ERROR_MESSAGE())
    END CATCH
  END
  RETURN 0
END
