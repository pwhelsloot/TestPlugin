IF NOT EXISTS (SELECT NULL FROM [dbo].[User]
    WHERE UserName = 'ADMIN')
BEGIN
INSERT INTO [dbo].[User] ([UserName], [Password], [EmailAddress])
  VALUES('Admin', 'crV5xhsD1NCrs9Y2BL19OrwwKuA=', 'admin@amcsgroup.com')
END
GO