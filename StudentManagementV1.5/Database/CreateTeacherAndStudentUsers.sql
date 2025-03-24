USE StudentManagementDB;
GO

-- Create teacher accounts
-- Note: For testing purposes, we're using the same SHA2_512 approach as admin accounts
-- In production, account creation should happen through the application for proper HMACSHA512 with salt

-- Create Teacher User 1
DECLARE @TeacherUserID1 INT;
INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role, IsActive, CreatedDate)
VALUES (
    'teacher1', 
    HASHBYTES('SHA2_512', 'teacher123'), 
    CONVERT(VARBINARY(128), ''), 
    'teacher1@school.com', 
    'Teacher', 
    1, 
    GETDATE()
);
SET @TeacherUserID1 = SCOPE_IDENTITY();

-- Create Teacher record linked to User 1
INSERT INTO Teachers (UserID, FirstName, LastName, DateOfBirth, Gender, PhoneNumber, Specialization)
VALUES (
    @TeacherUserID1,
    'John',
    'Smith',
    '1980-05-15',
    'Male',
    '555-123-4567',
    'Mathematics'
);

-- Create Teacher User 2
DECLARE @TeacherUserID2 INT;
INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role, IsActive, CreatedDate)
VALUES (
    'teacher2', 
    HASHBYTES('SHA2_512', 'teacher123'), 
    CONVERT(VARBINARY(128), ''), 
    'teacher2@school.com', 
    'Teacher', 
    1, 
    GETDATE()
);
SET @TeacherUserID2 = SCOPE_IDENTITY();

-- Create Teacher record linked to User 2
INSERT INTO Teachers (UserID, FirstName, LastName, DateOfBirth, Gender, PhoneNumber, Specialization)
VALUES (
    @TeacherUserID2,
    'Sarah',
    'Johnson',
    '1985-08-22',
    'Female',
    '555-234-5678',
    'Science'
);

-- Create student accounts
-- First, make sure we have at least one class for students to be assigned to
DECLARE @ClassID INT;
SELECT TOP 1 @ClassID = ClassID FROM Classes WHERE IsActive = 1;

-- If no active class exists, create one
IF @ClassID IS NULL
BEGIN
    INSERT INTO Classes (ClassName, AcademicYear, Grade, IsActive)
    VALUES ('Default Class', '2023-2024', '10th Grade', 1);
    SET @ClassID = SCOPE_IDENTITY();
END

-- Create Student User 1
DECLARE @StudentUserID1 INT;
INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role, IsActive, CreatedDate)
VALUES (
    'student1', 
    HASHBYTES('SHA2_512', 'student123'), 
    CONVERT(VARBINARY(128), ''), 
    'student1@school.com', 
    'Student', 
    1, 
    GETDATE()
);
SET @StudentUserID1 = SCOPE_IDENTITY();

-- Create Student record linked to User 1
INSERT INTO Students (UserID, FirstName, LastName, DateOfBirth, Gender, ClassID, EnrollmentDate)
VALUES (
    @StudentUserID1,
    'Michael',
    'Brown',
    '2005-03-12',
    'Male',
    @ClassID,
    GETDATE()
);

-- Create Student User 2
DECLARE @StudentUserID2 INT;
INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role, IsActive, CreatedDate)
VALUES (
    'student2', 
    HASHBYTES('SHA2_512', 'student123'), 
    CONVERT(VARBINARY(128), ''), 
    'student2@school.com', 
    'Student', 
    1, 
    GETDATE()
);
SET @StudentUserID2 = SCOPE_IDENTITY();

-- Create Student record linked to User 2
INSERT INTO Students (UserID, FirstName, LastName, DateOfBirth, Gender, ClassID, EnrollmentDate)
VALUES (
    @StudentUserID2,
    'Emily',
    'Davis',
    '2006-07-25',
    'Female',
    @ClassID,
    GETDATE()
);

-- Update the CurrentStudentCount for the class
UPDATE Classes
SET CurrentStudentCount = (SELECT COUNT(*) FROM Students WHERE ClassID = @ClassID)
WHERE ClassID = @ClassID;

-- Optional: Associate a teacher with the class
DECLARE @TeacherID INT;
SELECT TOP 1 @TeacherID = TeacherID FROM Teachers;

IF @TeacherID IS NOT NULL
BEGIN
    UPDATE Classes
    SET TeacherID = @TeacherID
    WHERE ClassID = @ClassID;
END
GO

PRINT 'Teacher and Student accounts have been created successfully.'
GO
