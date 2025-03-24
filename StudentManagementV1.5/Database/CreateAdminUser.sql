USE StudentManagementDB;
GO

-- Create a new admin account with email and username 'admin' and password '123'
-- This uses SHA2_512 hash without salt, which matches how your AuthenticationService checks admin logins
INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role, IsActive, CreatedDate)
VALUES (
    'admin', 
    HASHBYTES('SHA2_512', '123'), 
    CONVERT(VARBINARY(128), ''), 
    'admin@school.com', 
    'Admin', 
    1, 
    GETDATE()
);
GO

-- If you want to create another admin account with a different username/password
INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role, IsActive, CreatedDate)
VALUES (
    'administrator', 
    HASHBYTES('SHA2_512', 'admin123'), 
    CONVERT(VARBINARY(128), ''), 
    'administrator@school.com', 
    'Admin', 
    1, 
    GETDATE()
);
GO
