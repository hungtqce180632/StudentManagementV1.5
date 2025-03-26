USE StudentManagementDB;
GO

-- Create Assignments table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Assignments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Assignments](
        [AssignmentID] [int] IDENTITY(1,1) NOT NULL,
        [Title] [nvarchar](100) NOT NULL,
        [Description] [nvarchar](max) NULL,
        [CreatedDate] [datetime] NOT NULL,
        [DueDate] [datetime] NOT NULL,
        [MaxPoints] [int] NOT NULL,
        [TeacherID] [int] NOT NULL,
        [SubjectID] [int] NOT NULL,
        [ClassID] [int] NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        CONSTRAINT [PK_Assignments] PRIMARY KEY CLUSTERED ([AssignmentID] ASC),
        CONSTRAINT [FK_Assignments_Teachers] FOREIGN KEY([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]),
        CONSTRAINT [FK_Assignments_Subjects] FOREIGN KEY([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID]),
        CONSTRAINT [FK_Assignments_Classes] FOREIGN KEY([ClassID]) REFERENCES [dbo].[Classes] ([ClassID])
    );

    PRINT 'Assignments table created successfully.';
END
ELSE
BEGIN
    PRINT 'Assignments table already exists.';
END

-- Create Submissions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Submissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Submissions](
        [SubmissionID] [int] IDENTITY(1,1) NOT NULL,
        [AssignmentID] [int] NOT NULL,
        [StudentID] [int] NOT NULL,
        [Content] [nvarchar](max) NOT NULL,
        [AttachmentPath] [nvarchar](255) NULL,
        [SubmissionDate] [datetime] NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [Score] [int] NULL,
        [Feedback] [nvarchar](max) NULL,
        CONSTRAINT [PK_Submissions] PRIMARY KEY CLUSTERED ([SubmissionID] ASC),
        CONSTRAINT [FK_Submissions_Assignments] FOREIGN KEY([AssignmentID]) REFERENCES [dbo].[Assignments] ([AssignmentID]),
        CONSTRAINT [FK_Submissions_Students] FOREIGN KEY([StudentID]) REFERENCES [dbo].[Students] ([StudentID])
    );

    PRINT 'Submissions table created successfully.';
END
ELSE
BEGIN
    PRINT 'Submissions table already exists.';
END

-- Add sample data to Assignments table if it's empty
IF NOT EXISTS (SELECT TOP 1 * FROM Assignments)
BEGIN
    -- Get a teacher ID
    DECLARE @TeacherID int;
    SELECT TOP 1 @TeacherID = TeacherID FROM Teachers;
    
    -- Get a subject ID
    DECLARE @SubjectID int;
    SELECT TOP 1 @SubjectID = SubjectID FROM Subjects;
    
    -- Get a class ID
    DECLARE @ClassID int;
    SELECT TOP 1 @ClassID = ClassID FROM Classes;
    
    -- Insert sample assignments if we have the necessary IDs
    IF @TeacherID IS NOT NULL AND @SubjectID IS NOT NULL AND @ClassID IS NOT NULL
    BEGIN
        INSERT INTO Assignments (Title, Description, CreatedDate, DueDate, MaxPoints, TeacherID, SubjectID, ClassID, Status)
        VALUES 
        ('Midterm Assignment', 'Complete exercises from chapter 5-8', GETDATE(), DATEADD(day, 14, GETDATE()), 100, @TeacherID, @SubjectID, @ClassID, 'Published'),
        ('Research Paper', 'Write a 5-page research paper on a topic of your choice', GETDATE(), DATEADD(day, 21, GETDATE()), 150, @TeacherID, @SubjectID, @ClassID, 'Published'),
        ('Group Project', 'Work in groups of 3-4 to create a presentation', GETDATE(), DATEADD(day, 7, GETDATE()), 80, @TeacherID, @SubjectID, @ClassID, 'Draft');
        
        PRINT 'Sample assignments have been added.';
    END
    ELSE
    BEGIN
        PRINT 'Could not add sample assignments. Missing teacher, subject, or class data.';
    END
END
ELSE
BEGIN
    PRINT 'Assignments table already contains data.';
END

-- Add sample data to Submissions table if it's empty
IF NOT EXISTS (SELECT TOP 1 * FROM Submissions)
BEGIN
    -- Get an assignment ID
    DECLARE @AssignmentID int;
    SELECT TOP 1 @AssignmentID = AssignmentID FROM Assignments;
    
    -- Get a student ID
    DECLARE @StudentID int;
    SELECT TOP 1 @StudentID = StudentID FROM Students;
    
    -- Insert sample submissions if we have the necessary IDs
    IF @AssignmentID IS NOT NULL AND @StudentID IS NOT NULL
    BEGIN
        INSERT INTO Submissions (AssignmentID, StudentID, Content, AttachmentPath, SubmissionDate, Status, Score, Feedback)
        VALUES 
        (@AssignmentID, @StudentID, 'Here is my completed assignment. I covered all the required topics.', 'homework1.pdf', GETDATE(), 'Submitted', NULL, NULL),
        (@AssignmentID, @StudentID, 'This is my research paper on climate change.', 'research_paper.docx', DATEADD(day, -2, GETDATE()), 'Graded', 85, 'Good work, but needs more citations.');
        
        PRINT 'Sample submissions have been added.';
    END
    ELSE
    BEGIN
        PRINT 'Could not add sample submissions. Missing assignment or student data.';
    END
END
ELSE
BEGIN
    PRINT 'Submissions table already contains data.';
END

PRINT 'Database setup for assignments and submissions completed successfully.';
GO
