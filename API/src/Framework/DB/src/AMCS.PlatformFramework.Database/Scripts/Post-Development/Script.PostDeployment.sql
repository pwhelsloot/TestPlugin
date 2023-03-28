/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
BEGIN TRANSACTION
SET XACT_ABORT ON;
-- VERSIONING
DECLARE @ProductVersion [nvarchar](50)
SET @ProductVersion = '#ProductVersion#'
IF @ProductVersion NOT LIKE '%ProductVersion%'
BEGIN
	IF NOT EXISTS (SELECT NULL FROM DBVersion WHERE Version = @ProductVersion)
	BEGIN
		INSERT INTO DBVersion(Version) VALUES(@ProductVersion)
	END
END
GO

:SETVAR SCRIPTPATH "."

-- This is only to be used for Template training and should be removed
-- before app is used for development
:r $(SCRIPTPATH)\Script.BasicUserTemplateSetUp.sql
PRINT 'Script.BasicUserTemplateSetUp.sql complete.'

GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END
ELSE
COMMIT TRANSACTION;
