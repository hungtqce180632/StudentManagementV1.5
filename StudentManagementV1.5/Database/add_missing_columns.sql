USE StudentManagementDB;
GO

-- Step 1: Add the missing columns to the Classes table
ALTER TABLE Classes
ADD Grade NVARCHAR(20) NULL,
    TeacherID INT NULL,
    ClassRoom NVARCHAR(30) NULL,
    MaxCapacity INT DEFAULT 30,
    CurrentStudentCount INT DEFAULT 0;
GO

-- Step 2: Add foreign key constraint for TeacherID
ALTER TABLE Classes
ADD CONSTRAINT FK_Classes_Teachers
FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID);
GO

-- Step 3: Add a sample class with all the new fields for testing
INSERT INTO Classes (ClassName, AcademicYear, Grade, ClassRoom, MaxCapacity, CurrentStudentCount, IsActive)
VALUES ('Sample Class A', '2023-2024', '10th Grade', 'Room 101', 35, 0, 1);
GO

-- If you have any teachers in the database, you can associate a teacher with the class:
-- UPDATE Classes SET TeacherID = 1 WHERE ClassID = 1;
