-- SQL Server Database Schema for Student Management System

USE master;
GO

-- Create and use the database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StudentManagementDB')
BEGIN
    CREATE DATABASE StudentManagementDB;
END
GO

USE StudentManagementDB;
GO

-- Users Table (for authentication)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARBINARY(64) NOT NULL,
    PasswordSalt VARBINARY(128) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Teacher', 'Student')),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastLoginDate DATETIME NULL
);

-- Classes Table - Updated with additional columns
CREATE TABLE Classes (
    ClassID INT PRIMARY KEY IDENTITY(1,1),
    ClassName NVARCHAR(50) NOT NULL,
    Grade NVARCHAR(20) NULL,
    TeacherID INT NULL,
    ClassRoom NVARCHAR(30) NULL,
    MaxCapacity INT DEFAULT 30,
    CurrentStudentCount INT DEFAULT 0,
    AcademicYear NVARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- Students Table
CREATE TABLE Students (
    StudentID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT UNIQUE FOREIGN KEY REFERENCES Users(UserID),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    Address NVARCHAR(200),
    PhoneNumber NVARCHAR(20),
    ClassID INT FOREIGN KEY REFERENCES Classes(ClassID),
    EnrollmentDate DATE DEFAULT GETDATE()
);

-- Teachers Table
CREATE TABLE Teachers (
    TeacherID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT UNIQUE FOREIGN KEY REFERENCES Users(UserID),
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(10),
    Address NVARCHAR(200),
    PhoneNumber NVARCHAR(20),
    HireDate DATE DEFAULT GETDATE(),
    Specialization NVARCHAR(100)
);

-- Add foreign key constraint after both tables exist
ALTER TABLE Classes
ADD CONSTRAINT FK_Classes_Teachers FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID);

-- Subjects Table
CREATE TABLE Subjects (
    SubjectID INT PRIMARY KEY IDENTITY(1,1),
    SubjectName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Credits INT,
    IsActive BIT DEFAULT 1
);

-- TeacherSubjects Table (Many-to-Many relationship)
CREATE TABLE TeacherSubjects (
    TeacherSubjectID INT PRIMARY KEY IDENTITY(1,1),
    TeacherID INT FOREIGN KEY REFERENCES Teachers(TeacherID),
    SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID),
    ClassID INT FOREIGN KEY REFERENCES Classes(ClassID),
    AcademicYear NVARCHAR(20) NOT NULL,
    CONSTRAINT UQ_TeacherSubject UNIQUE (TeacherID, SubjectID, ClassID, AcademicYear)
);

-- Exams Table
CREATE TABLE Exams (
    ExamID INT PRIMARY KEY IDENTITY(1,1),
    SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID),
    ClassID INT FOREIGN KEY REFERENCES Classes(ClassID),
    ExamName NVARCHAR(100) NOT NULL,
    ExamDate DATETIME NOT NULL,
    Duration INT, -- In minutes
    TotalMarks INT NOT NULL,
    Description NVARCHAR(500)
);

-- Scores Table
CREATE TABLE Scores (
    ScoreID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT FOREIGN KEY REFERENCES Students(StudentID),
    ExamID INT FOREIGN KEY REFERENCES Exams(ExamID),
    MarksObtained DECIMAL(5,2) NOT NULL,
    Remarks NVARCHAR(200),
    RecordedBy INT FOREIGN KEY REFERENCES Users(UserID),
    RecordedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_StudentExam UNIQUE (StudentID, ExamID)
);

-- ClassSchedules Table
CREATE TABLE ClassSchedules (
    ScheduleID INT PRIMARY KEY IDENTITY(1,1),
    ClassID INT FOREIGN KEY REFERENCES Classes(ClassID),
    SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID),
    TeacherID INT FOREIGN KEY REFERENCES Teachers(TeacherID),
    DayOfWeek NVARCHAR(10) NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    Room NVARCHAR(20),
    CONSTRAINT CHK_Times CHECK (EndTime > StartTime)
);

-- Notifications Table
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Message NVARCHAR(1000) NOT NULL,
    SenderID INT FOREIGN KEY REFERENCES Users(UserID),
    RecipientType NVARCHAR(20) NOT NULL, -- 'All', 'Class', 'Teacher', 'Student'
    RecipientID INT NULL, -- Can be ClassID, TeacherID, or StudentID based on RecipientType
    IsRead BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ExpiryDate DATETIME NULL
);

-- Assignments Table
CREATE TABLE Assignments (
    AssignmentID INT PRIMARY KEY IDENTITY(1,1),
    SubjectID INT FOREIGN KEY REFERENCES Subjects(SubjectID),
    ClassID INT FOREIGN KEY REFERENCES Classes(ClassID),
    TeacherID INT FOREIGN KEY REFERENCES Teachers(TeacherID),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000),
    AssignedDate DATETIME DEFAULT GETDATE(),
    Deadline DATETIME NOT NULL,
    TotalMarks INT NOT NULL,
    AttachmentPath NVARCHAR(500)
);

-- Submissions Table
CREATE TABLE Submissions (
    SubmissionID INT PRIMARY KEY IDENTITY(1,1),
    AssignmentID INT FOREIGN KEY REFERENCES Assignments(AssignmentID),
    StudentID INT FOREIGN KEY REFERENCES Students(StudentID),
    SubmissionDate DATETIME DEFAULT GETDATE(),
    Content NVARCHAR(1000),
    AttachmentPath NVARCHAR(500),
    MarksObtained DECIMAL(5,2) NULL,
    Feedback NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Submitted', -- 'Draft', 'Submitted', 'Graded', 'Late'
    CONSTRAINT UQ_StudentAssignment UNIQUE (StudentID, AssignmentID)
);

-- Password Reset OTP Table
CREATE TABLE PasswordResetTokens (
    TokenID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    Token NVARCHAR(6) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    ExpiresAt DATETIME NOT NULL,
    IsUsed BIT DEFAULT 0
);

-- Insert default admin account with encrypted password (default password: 123)
-- In real code we'll encrypt the password properly
INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, Role)
VALUES ('admin', HASHBYTES('SHA2_512', CONVERT(NVARCHAR(50), '123')), CONVERT(VARBINARY(128), ''), 'admin@school.com', 'Admin');
GO

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