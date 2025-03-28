USE [StudentManagementDB]
GO
/****** Object:  Table [dbo].[Assignments]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Assignments](
	[AssignmentID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectID] [int] NULL,
	[ClassID] [int] NULL,
	[TeacherID] [int] NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[AssignedDate] [datetime] NULL,
	[Deadline] [datetime] NOT NULL,
	[TotalMarks] [int] NOT NULL,
	[AttachmentPath] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classes]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classes](
	[ClassID] [int] IDENTITY(1,1) NOT NULL,
	[ClassName] [nvarchar](50) NOT NULL,
	[Grade] [nvarchar](20) NULL,
	[TeacherID] [int] NULL,
	[ClassRoom] [nvarchar](30) NULL,
	[MaxCapacity] [int] NULL,
	[CurrentStudentCount] [int] NULL,
	[AcademicYear] [nvarchar](20) NOT NULL,
	[IsActive] [bit] NULL,
	[SemesterID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClassSchedules]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassSchedules](
	[ScheduleID] [int] IDENTITY(1,1) NOT NULL,
	[ClassID] [int] NULL,
	[SubjectID] [int] NULL,
	[TeacherID] [int] NULL,
	[DayOfWeek] [nvarchar](10) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[Room] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ScheduleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Exams]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exams](
	[ExamID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectID] [int] NULL,
	[ClassID] [int] NULL,
	[ExamName] [nvarchar](100) NOT NULL,
	[ExamDate] [datetime] NOT NULL,
	[Duration] [int] NULL,
	[TotalMarks] [int] NOT NULL,
	[Description] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[ExamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[NotificationID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Message] [nvarchar](1000) NOT NULL,
	[SenderID] [int] NULL,
	[RecipientType] [nvarchar](20) NOT NULL,
	[RecipientID] [int] NULL,
	[IsRead] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[ExpiryDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PasswordResetTokens]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordResetTokens](
	[TokenID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[Token] [nvarchar](6) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[ExpiresAt] [datetime] NOT NULL,
	[IsUsed] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[TokenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Scores]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Scores](
	[ScoreID] [int] IDENTITY(1,1) NOT NULL,
	[StudentID] [int] NULL,
	[ExamID] [int] NULL,
	[MarksObtained] [decimal](5, 2) NOT NULL,
	[Remarks] [nvarchar](200) NULL,
	[RecordedBy] [int] NULL,
	[RecordedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ScoreID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Semesters]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Semesters](
	[SemesterID] [int] IDENTITY(1,1) NOT NULL,
	[SemesterName] [nvarchar](50) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SemesterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[StudentID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[DateOfBirth] [date] NULL,
	[Gender] [nvarchar](10) NULL,
	[Address] [nvarchar](200) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[ClassID] [int] NULL,
	[EnrollmentDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[StudentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subjects]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subjects](
	[SubjectID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[Credits] [int] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[SubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Submissions]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Submissions](
	[SubmissionID] [int] IDENTITY(1,1) NOT NULL,
	[AssignmentID] [int] NULL,
	[StudentID] [int] NULL,
	[SubmissionDate] [datetime] NULL,
	[Content] [nvarchar](1000) NULL,
	[AttachmentPath] [nvarchar](500) NULL,
	[MarksObtained] [decimal](5, 2) NULL,
	[Feedback] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[SubmissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teachers]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teachers](
	[TeacherID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[DateOfBirth] [date] NULL,
	[Gender] [nvarchar](10) NULL,
	[Address] [nvarchar](200) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[HireDate] [date] NULL,
	[Specialization] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[TeacherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeacherSubjects]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeacherSubjects](
	[TeacherSubjectID] [int] IDENTITY(1,1) NOT NULL,
	[TeacherID] [int] NULL,
	[SubjectID] [int] NULL,
	[ClassID] [int] NULL,
	[AcademicYear] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TeacherSubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 3/27/2025 4:13:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [varbinary](64) NOT NULL,
	[PasswordSalt] [varbinary](128) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Role] [nvarchar](20) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[LastLoginDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Assignments] ON 

INSERT [dbo].[Assignments] ([AssignmentID], [SubjectID], [ClassID], [TeacherID], [Title], [Description], [AssignedDate], [Deadline], [TotalMarks], [AttachmentPath], [Status]) VALUES (1, 1, 3, 1, N'Algebra Practice Set 1', N'Complete problems 1-15 from chapter 2.', CAST(N'2025-03-27T15:32:31.900' AS DateTime), CAST(N'2025-04-03T15:32:31.900' AS DateTime), 100, N'/assignments/math10/alg_set1.pdf', N'Published')
INSERT [dbo].[Assignments] ([AssignmentID], [SubjectID], [ClassID], [TeacherID], [Title], [Description], [AssignedDate], [Deadline], [TotalMarks], [AttachmentPath], [Status]) VALUES (2, 3, 4, 2, N'Essay: The Great Gatsby', N'Analyze the symbolism in The Great Gatsby (1500 words).', CAST(N'2025-03-27T15:32:31.900' AS DateTime), CAST(N'2025-04-10T15:32:31.900' AS DateTime), 100, NULL, N'Published')
INSERT [dbo].[Assignments] ([AssignmentID], [SubjectID], [ClassID], [TeacherID], [Title], [Description], [AssignedDate], [Deadline], [TotalMarks], [AttachmentPath], [Status]) VALUES (3, 2, 3, 2, N'Science Project Proposal', N'Submit your proposal for the semester science project.', CAST(N'2025-03-22T15:32:31.900' AS DateTime), CAST(N'2025-04-06T15:32:31.900' AS DateTime), 20, NULL, N'Published')
INSERT [dbo].[Assignments] ([AssignmentID], [SubjectID], [ClassID], [TeacherID], [Title], [Description], [AssignedDate], [Deadline], [TotalMarks], [AttachmentPath], [Status]) VALUES (4, 4, 4, 3, N'History Research Paper Outline', N'Submit a detailed outline for your research paper on the Vietnam War.', CAST(N'2025-03-27T15:38:16.847' AS DateTime), CAST(N'2025-04-10T15:38:16.847' AS DateTime), 30, NULL, N'Published')
INSERT [dbo].[Assignments] ([AssignmentID], [SubjectID], [ClassID], [TeacherID], [Title], [Description], [AssignedDate], [Deadline], [TotalMarks], [AttachmentPath], [Status]) VALUES (5, 4, 4, 3, N'History Research Paper Outline', N'Submit a detailed outline for your research paper on the Vietnam War.', CAST(N'2025-03-27T15:40:46.637' AS DateTime), CAST(N'2025-04-10T15:40:46.637' AS DateTime), 30, NULL, N'Published')
INSERT [dbo].[Assignments] ([AssignmentID], [SubjectID], [ClassID], [TeacherID], [Title], [Description], [AssignedDate], [Deadline], [TotalMarks], [AttachmentPath], [Status]) VALUES (6, 4, 4, 3, N'aaa', N'aaa', CAST(N'2025-03-27T15:49:05.547' AS DateTime), CAST(N'2025-04-03T15:49:05.547' AS DateTime), 100, NULL, N'Published')
INSERT [dbo].[Assignments] ([AssignmentID], [SubjectID], [ClassID], [TeacherID], [Title], [Description], [AssignedDate], [Deadline], [TotalMarks], [AttachmentPath], [Status]) VALUES (7, 4, 4, 3, N'aaa', N'aaa', CAST(N'2025-03-27T15:49:19.373' AS DateTime), CAST(N'2025-04-03T15:49:19.373' AS DateTime), 100, NULL, N'Published')
SET IDENTITY_INSERT [dbo].[Assignments] OFF
GO
SET IDENTITY_INSERT [dbo].[Classes] ON 

INSERT [dbo].[Classes] ([ClassID], [ClassName], [Grade], [TeacherID], [ClassRoom], [MaxCapacity], [CurrentStudentCount], [AcademicYear], [IsActive], [SemesterID]) VALUES (1, N'Sample Class A', N'10th Grade', 1, N'Room 101', 35, 0, N'2024-2025', 0, 1)
INSERT [dbo].[Classes] ([ClassID], [ClassName], [Grade], [TeacherID], [ClassRoom], [MaxCapacity], [CurrentStudentCount], [AcademicYear], [IsActive], [SemesterID]) VALUES (3, N'10B', N'10', 1, N'Room 102', 30, 2, N'2024-2025', 1, 2)
INSERT [dbo].[Classes] ([ClassID], [ClassName], [Grade], [TeacherID], [ClassRoom], [MaxCapacity], [CurrentStudentCount], [AcademicYear], [IsActive], [SemesterID]) VALUES (4, N'11A', N'11', 2, N'Room 201', 30, 1, N'2024-2025', 1, 2)
SET IDENTITY_INSERT [dbo].[Classes] OFF
GO
SET IDENTITY_INSERT [dbo].[ClassSchedules] ON 

INSERT [dbo].[ClassSchedules] ([ScheduleID], [ClassID], [SubjectID], [TeacherID], [DayOfWeek], [StartTime], [EndTime], [Room]) VALUES (2, 3, 1, 1, N'Tuesday', CAST(N'08:30:00' AS Time), CAST(N'10:00:00' AS Time), N'Room 102')
INSERT [dbo].[ClassSchedules] ([ScheduleID], [ClassID], [SubjectID], [TeacherID], [DayOfWeek], [StartTime], [EndTime], [Room]) VALUES (3, 3, 2, 2, N'Wednesday', CAST(N'10:15:00' AS Time), CAST(N'11:45:00' AS Time), N'Lab B')
INSERT [dbo].[ClassSchedules] ([ScheduleID], [ClassID], [SubjectID], [TeacherID], [DayOfWeek], [StartTime], [EndTime], [Room]) VALUES (4, 4, 3, 2, N'Tuesday', CAST(N'13:00:00' AS Time), CAST(N'14:30:00' AS Time), N'Room 201')
INSERT [dbo].[ClassSchedules] ([ScheduleID], [ClassID], [SubjectID], [TeacherID], [DayOfWeek], [StartTime], [EndTime], [Room]) VALUES (5, 4, 4, 3, N'Thursday', CAST(N'08:30:00' AS Time), CAST(N'10:00:00' AS Time), N'Room 201')
SET IDENTITY_INSERT [dbo].[ClassSchedules] OFF
GO
SET IDENTITY_INSERT [dbo].[Exams] ON 

INSERT [dbo].[Exams] ([ExamID], [SubjectID], [ClassID], [ExamName], [ExamDate], [Duration], [TotalMarks], [Description]) VALUES (1, 1, 3, N'Midterm Math 10 Exam', CAST(N'2025-04-27T15:32:31.910' AS DateTime), 90, 100, N'Covers first half of Math 10 curriculum.')
INSERT [dbo].[Exams] ([ExamID], [SubjectID], [ClassID], [ExamName], [ExamDate], [Duration], [TotalMarks], [Description]) VALUES (2, 3, 4, N'Midterm English 11 Exam', CAST(N'2025-05-03T15:32:31.910' AS DateTime), 120, 100, N'Focus on literature analysis.')
INSERT [dbo].[Exams] ([ExamID], [SubjectID], [ClassID], [ExamName], [ExamDate], [Duration], [TotalMarks], [Description]) VALUES (3, 1, 3, N'Math 10 Quiz 1', CAST(N'2025-03-13T15:32:31.910' AS DateTime), 30, 25, N'Quiz on Chapter 1.')
INSERT [dbo].[Exams] ([ExamID], [SubjectID], [ClassID], [ExamName], [ExamDate], [Duration], [TotalMarks], [Description]) VALUES (4, 4, 4, N'History 11 Quiz 2', CAST(N'2025-03-24T15:38:16.847' AS DateTime), 45, 50, N'Quiz covering post-WWII events.')
INSERT [dbo].[Exams] ([ExamID], [SubjectID], [ClassID], [ExamName], [ExamDate], [Duration], [TotalMarks], [Description]) VALUES (5, 4, 4, N'History 11 Quiz 2', CAST(N'2025-03-24T15:40:46.637' AS DateTime), 45, 50, N'Quiz covering post-WWII events.')
SET IDENTITY_INSERT [dbo].[Exams] OFF
GO
SET IDENTITY_INSERT [dbo].[Notifications] ON 

INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (2, N'System Maintenance', N'The system will undergo maintenance tonight from 10 PM to 11 PM.', 2, N'All', NULL, 0, CAST(N'2025-03-27T03:31:22.730' AS DateTime), CAST(N'2025-03-28T03:31:22.730' AS DateTime))
INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (3, N'Welcome Spring 2025!', N'Welcome to the new semester!', 1, N'All', NULL, 0, CAST(N'2025-03-27T15:32:31.923' AS DateTime), CAST(N'2025-04-10T15:32:31.923' AS DateTime))
INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (4, N'Math Assignment Reminder', N'Remember Algebra Practice Set 1 is due soon!', 3, N'Class', 3, 0, CAST(N'2025-03-27T15:32:31.923' AS DateTime), CAST(N'2025-04-04T15:32:31.923' AS DateTime))
INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (5, N'Check Feedback', N'Feedback for your Science Project Proposal is available.', 4, N'Student', 1, 0, CAST(N'2025-03-27T15:32:31.923' AS DateTime), NULL)
INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (6, N'History Paper Reminder', N'Don''t forget the research paper outline is due soon!', 8, N'Class', 4, 0, CAST(N'2025-03-27T15:38:16.847' AS DateTime), CAST(N'2025-04-10T15:38:16.847' AS DateTime))
INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (7, N'Upcoming Holiday', N'Reminder: School will be closed next Monday for the national holiday.', 9, N'All', NULL, 0, CAST(N'2025-03-27T15:38:16.873' AS DateTime), CAST(N'2025-04-04T15:38:16.873' AS DateTime))
INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (8, N'History Paper Reminder', N'Don''t forget the research paper outline is due soon!', 8, N'Class', 4, 0, CAST(N'2025-03-27T15:40:46.637' AS DateTime), CAST(N'2025-04-10T15:40:46.637' AS DateTime))
INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (9, N'Upcoming Holiday', N'Reminder: School will be closed next Monday for the national holiday.', 9, N'All', NULL, 0, CAST(N'2025-03-27T15:40:46.640' AS DateTime), CAST(N'2025-04-04T15:40:46.640' AS DateTime))
SET IDENTITY_INSERT [dbo].[Notifications] OFF
GO
SET IDENTITY_INSERT [dbo].[PasswordResetTokens] ON 

INSERT [dbo].[PasswordResetTokens] ([TokenID], [UserID], [Token], [CreatedAt], [ExpiresAt], [IsUsed]) VALUES (1, 7, N'112233', CAST(N'2025-03-27T15:32:31.927' AS DateTime), CAST(N'2025-03-27T16:32:31.927' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[PasswordResetTokens] OFF
GO
SET IDENTITY_INSERT [dbo].[Scores] ON 

INSERT [dbo].[Scores] ([ScoreID], [StudentID], [ExamID], [MarksObtained], [Remarks], [RecordedBy], [RecordedDate]) VALUES (1, 1, 3, CAST(22.50 AS Decimal(5, 2)), N'Excellent work!', 3, CAST(N'2025-03-17T15:32:31.917' AS DateTime))
INSERT [dbo].[Scores] ([ScoreID], [StudentID], [ExamID], [MarksObtained], [Remarks], [RecordedBy], [RecordedDate]) VALUES (2, 2, 3, CAST(19.00 AS Decimal(5, 2)), N'Good understanding.', 3, CAST(N'2025-03-17T15:32:31.917' AS DateTime))
INSERT [dbo].[Scores] ([ScoreID], [StudentID], [ExamID], [MarksObtained], [Remarks], [RecordedBy], [RecordedDate]) VALUES (3, 3, 2, CAST(75.50 AS Decimal(5, 2)), N'Good effort, review grammar rules.', 4, CAST(N'2025-03-27T15:40:46.637' AS DateTime))
INSERT [dbo].[Scores] ([ScoreID], [StudentID], [ExamID], [MarksObtained], [Remarks], [RecordedBy], [RecordedDate]) VALUES (4, 3, 5, CAST(42.00 AS Decimal(5, 2)), N'Solid understanding of key events.', 8, CAST(N'2025-03-27T15:40:46.637' AS DateTime))
SET IDENTITY_INSERT [dbo].[Scores] OFF
GO
SET IDENTITY_INSERT [dbo].[Semesters] ON 

INSERT [dbo].[Semesters] ([SemesterID], [SemesterName], [StartDate], [EndDate], [IsActive]) VALUES (1, N'Fall 2024', CAST(N'2024-09-02' AS Date), CAST(N'2024-12-20' AS Date), 0)
INSERT [dbo].[Semesters] ([SemesterID], [SemesterName], [StartDate], [EndDate], [IsActive]) VALUES (2, N'Spring 2025', CAST(N'2025-01-13' AS Date), CAST(N'2025-05-23' AS Date), 1)
INSERT [dbo].[Semesters] ([SemesterID], [SemesterName], [StartDate], [EndDate], [IsActive]) VALUES (3, N'Fall 2025', CAST(N'2025-09-01' AS Date), CAST(N'2025-12-19' AS Date), 0)
SET IDENTITY_INSERT [dbo].[Semesters] OFF
GO
SET IDENTITY_INSERT [dbo].[Students] ON 

INSERT [dbo].[Students] ([StudentID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [ClassID], [EnrollmentDate]) VALUES (1, 5, N'Michael', N'Brown', CAST(N'2008-03-12' AS Date), N'Male', N'1 Student St', N'555-111-2222', 3, CAST(N'2025-01-10' AS Date))
INSERT [dbo].[Students] ([StudentID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [ClassID], [EnrollmentDate]) VALUES (2, 6, N'Emily', N'Davis', CAST(N'2008-07-25' AS Date), N'Female', N'2 Student St', N'555-333-4444', 3, CAST(N'2025-01-10' AS Date))
INSERT [dbo].[Students] ([StudentID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [ClassID], [EnrollmentDate]) VALUES (3, 7, N'Hung', N'', CAST(N'2007-05-18' AS Date), N'Male', N'3 Student St', N'555-555-6666', 4, CAST(N'2025-01-10' AS Date))
SET IDENTITY_INSERT [dbo].[Students] OFF
GO
SET IDENTITY_INSERT [dbo].[Subjects] ON 

INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [Description], [Credits], [IsActive]) VALUES (1, N'Mathematics 10', N'Core concepts for 10th Grade Math', 4, 1)
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [Description], [Credits], [IsActive]) VALUES (2, N'Science 10', N'Introduction to Physics and Chemistry', 4, 1)
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [Description], [Credits], [IsActive]) VALUES (3, N'English 11', N'Literature and Composition for 11th Grade', 3, 1)
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [Description], [Credits], [IsActive]) VALUES (4, N'History 11', N'US History', 3, 1)
SET IDENTITY_INSERT [dbo].[Subjects] OFF
GO
SET IDENTITY_INSERT [dbo].[Submissions] ON 

INSERT [dbo].[Submissions] ([SubmissionID], [AssignmentID], [StudentID], [SubmissionDate], [Content], [AttachmentPath], [MarksObtained], [Feedback], [Status]) VALUES (1, 1, 1, CAST(N'2025-04-01T15:32:31.907' AS DateTime), N'My submission for Algebra Practice Set 1.', N'/submissions/s1/alg_set1_mbrown.docx', NULL, NULL, N'Submitted')
INSERT [dbo].[Submissions] ([SubmissionID], [AssignmentID], [StudentID], [SubmissionDate], [Content], [AttachmentPath], [MarksObtained], [Feedback], [Status]) VALUES (2, 1, 2, CAST(N'2025-04-02T15:32:31.907' AS DateTime), N'Attached homework.', N'/submissions/s2/algebra_set1.pdf', NULL, NULL, N'Submitted')
INSERT [dbo].[Submissions] ([SubmissionID], [AssignmentID], [StudentID], [SubmissionDate], [Content], [AttachmentPath], [MarksObtained], [Feedback], [Status]) VALUES (3, 3, 1, CAST(N'2025-04-05T15:32:31.907' AS DateTime), N'Science project proposal attached.', N'/submissions/s1/science_proposal_mbrown.docx', CAST(18.50 AS Decimal(5, 2)), N'Good topic, refine methodology.', N'Graded')
INSERT [dbo].[Submissions] ([SubmissionID], [AssignmentID], [StudentID], [SubmissionDate], [Content], [AttachmentPath], [MarksObtained], [Feedback], [Status]) VALUES (4, 2, 3, CAST(N'2025-04-06T15:40:46.637' AS DateTime), N'My essay on The Great Gatsby.', N'/submissions/s3/gatsby_essay_hung.docx', NULL, NULL, N'Submitted')
INSERT [dbo].[Submissions] ([SubmissionID], [AssignmentID], [StudentID], [SubmissionDate], [Content], [AttachmentPath], [MarksObtained], [Feedback], [Status]) VALUES (5, 5, 3, CAST(N'2025-04-08T15:40:46.637' AS DateTime), N'Research paper outline attached.', N'/submissions/s3/history_outline_hung.pdf', NULL, NULL, N'Submitted')
SET IDENTITY_INSERT [dbo].[Submissions] OFF
GO
SET IDENTITY_INSERT [dbo].[Teachers] ON 

INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [HireDate], [Specialization]) VALUES (1, 3, N'John', N'Smith', CAST(N'1980-05-15' AS Date), N'Male', NULL, N'555-123-4567', CAST(N'2025-03-25' AS Date), N'Mathematics')
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [HireDate], [Specialization]) VALUES (2, 4, N'Sarah', N'Johnson', CAST(N'1985-08-22' AS Date), N'Female', NULL, N'555-234-5678', CAST(N'2025-03-25' AS Date), N'Science')
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [HireDate], [Specialization]) VALUES (3, 8, N'Hung', N'A', CAST(N'1982-01-20' AS Date), N'Male', N'789 Teacher Blvd', N'555-987-6543', CAST(N'2021-07-20' AS Date), N'History')
SET IDENTITY_INSERT [dbo].[Teachers] OFF
GO
SET IDENTITY_INSERT [dbo].[TeacherSubjects] ON 

INSERT [dbo].[TeacherSubjects] ([TeacherSubjectID], [TeacherID], [SubjectID], [ClassID], [AcademicYear]) VALUES (1, 1, 1, 3, N'2024-2025')
INSERT [dbo].[TeacherSubjects] ([TeacherSubjectID], [TeacherID], [SubjectID], [ClassID], [AcademicYear]) VALUES (2, 2, 2, 3, N'2024-2025')
INSERT [dbo].[TeacherSubjects] ([TeacherSubjectID], [TeacherID], [SubjectID], [ClassID], [AcademicYear]) VALUES (3, 2, 3, 4, N'2024-2025')
INSERT [dbo].[TeacherSubjects] ([TeacherSubjectID], [TeacherID], [SubjectID], [ClassID], [AcademicYear]) VALUES (4, 3, 4, 4, N'2024-2025')
SET IDENTITY_INSERT [dbo].[TeacherSubjects] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (1, N'admin', 0xCD8C29B8DEED323FE1538CFBDB46FC2A2EA61CFD67807F0629708EA2A6E13A2919DEF3C837C4E7F2C8E0067568E3236827DEFB05C9346E476B6E954440A908A7, 0x, N'admin@school.com', N'Admin', 0, CAST(N'2025-03-25T02:40:48.420' AS DateTime), NULL)
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (2, N'administrator', 0x7FCF4BA391C48784EDDE599889D6E3F1E47A27DB36ECC050CC92F259BFAC38AFAD2C68A1AE804D77075E8FB722503F3ECA2B2C1006EE6F6C7B7628CB45FFFD1D, 0x, N'administrator@school.com', N'Admin', 1, CAST(N'2025-03-25T02:40:48.423' AS DateTime), CAST(N'2025-03-27T15:55:48.510' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (3, N'teacher1', 0x492F7C4ECF9BC4C038A2842C87D88C290AF7A65AE359D59F31D543C8561E30DE44004F9E01793E806F9AD82ED7CD424D83DEC8E670A8EA324324B9F54D956FF4, 0x2BE14E01660BF048F0154E43A0F09C884BC4F8745BCEC9E409556C085085988489B1F9441BD0A57D65F193C31599A1A8DBAECC214AD8CC2A1BAB24A0CAEA2CE532D89C54B339AB5E7E87D283D25C5B8DD53FD36B6CB009A3C0DF842B9CA8638EC0366FB26FD0513B183CF798183A13C521573E925C3D942BFD3D411E8B6010B4, N'teacher1@school.com', N'Teacher', 1, CAST(N'2025-03-25T04:55:39.587' AS DateTime), CAST(N'2025-03-27T15:56:17.017' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (4, N'teacher2', 0x2227B9A850EA63850C16F8348F2AB971B94F3BAB2DC42B2554DF848E58FF252072EC269A8C22B9B5B5F7D6F4A091F5749FF4B6AB2C6FCA39139FDB13F84E6A24, 0xC6F505F49E8F66A7109AE4617671E89B3E5471B877AE74898D29E7A3E67439B578C44A67D23AB08B76D939489584479E95C5DF1616DAC419387A536E068D17DEF8B93A50E70E9D4F509B00EFA1C21831CC444D88D5E9F3676F26E30E68D902C48E383055789F5E9277CDDC94F88817D36D95AC2832B672C53F238E55694D2FA7, N'teacher2@school.com', N'Teacher', 1, CAST(N'2025-03-25T04:55:39.597' AS DateTime), CAST(N'2025-03-25T05:10:04.707' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (5, N'student1', 0x4963AAC737CCC27622805921E1933650262F3FAAFF39C0A38098ACC762B65BEC8DBD13D46E566EB81DEC8CC3125E8B29B38426F13211E1DAA33EEB7D63CFBAB2, 0x8E8462F47993CC084CB92FA78C46A80C5350E199A2D0B2C487FC133E7C64B2852C5DF3ACC2E8CD52233C0C74722B4556F4CBA5B299B75E491E26D24E519BE4A5A21D0AF73C77FDD0CFC68154C9DAB3D032694A5D05FD2C76A2E73B454D9B7B13A3F2C4B2F4BBFC0C2FE2A3B5167B3711123444133DFBD8C8CA9E4E2E80F8365A, N'student1@school.com', N'Student', 1, CAST(N'2025-03-25T04:55:39.597' AS DateTime), CAST(N'2025-03-27T16:12:20.403' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (6, N'student2', 0x70E4F547FA7BECB294D19CC52EAAD4A9DAC0F6647F270FB5BCEA7FB948FC9A940C62660B7C901C3B229D74999F86BD6F779C33504A6EDA3479842FCA00418362, 0xC64CEC27F335810F921F525577947CF0E80D7D22C8B6D0EFFE63BBC833333A7AE954AD95159009C922EB948BF65537E0F6AEDF8BF9C71067760A9E4637CD188E63C0866B97E8C433B1E2D99DBACFEB5CD50794C50A0F711C27A038C3F8190777A30E178B96E1C118D494B2337F994616F1DF2A0CBAADC100352773DC30C4BE0B, N'student2@school.com', N'Student', 1, CAST(N'2025-03-25T04:55:39.600' AS DateTime), NULL)
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (7, N'hung', 0x20D13DCB53B8012965DF8EB49568814DA93C3EE62D149B3C89A370BAB6AA9099529C886AAA53074797CEFB6CC24C3CE654DA8BCE602CAA6C244C97956B741EA9, 0x516BAE044A41EA80FCED5C27E7C8102129243AF178CDA889D66F68586CB428E134202FF4B38C01115CCEC7B8DCFB98018B0F6339B3F1C8AD305AD04C7CC74D76273DF94B3CA72382A89728BD6FF7593055595872EE644E765EE9BBCEB0890EEE0C9DD2FBA8601230DF6BF3E3958CFFA6D09D36EB6BA9DEDCC2DDD1B97FD2977C, N'hung@gmail.com', N'Student', 1, CAST(N'2025-03-27T01:20:43.213' AS DateTime), CAST(N'2025-03-27T03:36:47.823' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (8, N'hungA', 0x91126899D79CF026BFAB3D0932C6D3000CB661E5E2B92069DB1B2C948C9B5368E398A4409F2841AB55530427356FF618F4B2D2830B14871D2DAB9146E19637AF, 0x50D2F61BCAD6B1B00753FD03E792FB5CDAD3BE75CEFE6D9972D6BE83F1A1135309C4B577D145814186BBDD93B258998F7FAFCB38275CA874283AAF2BE265CF201749A2C6C35DA9FAF576542368DF61A3650D4B8477759389038CBEADB839B138323481F69F9945C24D427DAEC54CDA9AFCF08EFBBD6E63314A51F4696836235C, N'hungA@gmail.com', N'Teacher', 1, CAST(N'2025-03-27T03:37:48.567' AS DateTime), CAST(N'2025-03-27T15:41:20.483' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (9, N'hungB', 0x33C17B543B26A1670E2B7CC770FCB7280125B030F0348197DA5399340E6CF14C23C66C4AC7AAEF27658AF4B2E3A42F2A2EAB04428A0D13BA727F6BCEC851702B, 0xE53F45E375E15913405E434E494A6B20C67E39BBF6622FDCC7E64139C711BEB91B6093FC355BB955CC348A6957DAA9CF8CA8D909CE3835416C75A4251E6938763C554C28364DECCCF5BB2F7396421E96675CB7B0F6461A8CBFA9A9473CF4063A1B261B38561380420C93FDDD82F9661DC174F9BDEF233621AFAA261708DD62AF, N'hungB@gmail.com', N'Admin', 1, CAST(N'2025-03-27T03:38:08.517' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [UQ_StudentExam]    Script Date: 3/27/2025 4:13:52 PM ******/
ALTER TABLE [dbo].[Scores] ADD  CONSTRAINT [UQ_StudentExam] UNIQUE NONCLUSTERED 
(
	[StudentID] ASC,
	[ExamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Students__1788CCAD747D6F03]    Script Date: 3/27/2025 4:13:52 PM ******/
ALTER TABLE [dbo].[Students] ADD UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_StudentAssignment]    Script Date: 3/27/2025 4:13:52 PM ******/
ALTER TABLE [dbo].[Submissions] ADD  CONSTRAINT [UQ_StudentAssignment] UNIQUE NONCLUSTERED 
(
	[StudentID] ASC,
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Teachers__1788CCAD4DE2BA40]    Script Date: 3/27/2025 4:13:52 PM ******/
ALTER TABLE [dbo].[Teachers] ADD UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_TeacherSubject]    Script Date: 3/27/2025 4:13:52 PM ******/
ALTER TABLE [dbo].[TeacherSubjects] ADD  CONSTRAINT [UQ_TeacherSubject] UNIQUE NONCLUSTERED 
(
	[TeacherID] ASC,
	[SubjectID] ASC,
	[ClassID] ASC,
	[AcademicYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E49195C94B]    Script Date: 3/27/2025 4:13:52 PM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D10534A4BC6FA6]    Script Date: 3/27/2025 4:13:52 PM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Assignments] ADD  DEFAULT (getdate()) FOR [AssignedDate]
GO
ALTER TABLE [dbo].[Assignments] ADD  DEFAULT ('Published') FOR [Status]
GO
ALTER TABLE [dbo].[Classes] ADD  DEFAULT ((30)) FOR [MaxCapacity]
GO
ALTER TABLE [dbo].[Classes] ADD  DEFAULT ((0)) FOR [CurrentStudentCount]
GO
ALTER TABLE [dbo].[Classes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT ((0)) FOR [IsRead]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PasswordResetTokens] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[PasswordResetTokens] ADD  DEFAULT ((0)) FOR [IsUsed]
GO
ALTER TABLE [dbo].[Scores] ADD  DEFAULT (getdate()) FOR [RecordedDate]
GO
ALTER TABLE [dbo].[Semesters] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT (getdate()) FOR [EnrollmentDate]
GO
ALTER TABLE [dbo].[Subjects] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Submissions] ADD  DEFAULT (getdate()) FOR [SubmissionDate]
GO
ALTER TABLE [dbo].[Submissions] ADD  DEFAULT ('Submitted') FOR [Status]
GO
ALTER TABLE [dbo].[Teachers] ADD  DEFAULT (getdate()) FOR [HireDate]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[Assignments]  WITH CHECK ADD FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[Classes]  WITH CHECK ADD FOREIGN KEY([SemesterID])
REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[Classes]  WITH CHECK ADD  CONSTRAINT [FK_Classes_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[Classes] CHECK CONSTRAINT [FK_Classes_Teachers]
GO
ALTER TABLE [dbo].[ClassSchedules]  WITH CHECK ADD FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[ClassSchedules]  WITH CHECK ADD FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[ClassSchedules]  WITH CHECK ADD FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[Exams]  WITH CHECK ADD FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[Exams]  WITH CHECK ADD FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[Notifications]  WITH CHECK ADD FOREIGN KEY([SenderID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[PasswordResetTokens]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Scores]  WITH CHECK ADD FOREIGN KEY([ExamID])
REFERENCES [dbo].[Exams] ([ExamID])
GO
ALTER TABLE [dbo].[Scores]  WITH CHECK ADD FOREIGN KEY([RecordedBy])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Scores]  WITH CHECK ADD FOREIGN KEY([StudentID])
REFERENCES [dbo].[Students] ([StudentID])
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Submissions]  WITH CHECK ADD FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[Assignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[Submissions]  WITH CHECK ADD FOREIGN KEY([StudentID])
REFERENCES [dbo].[Students] ([StudentID])
GO
ALTER TABLE [dbo].[Teachers]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[TeacherSubjects]  WITH CHECK ADD FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[TeacherSubjects]  WITH CHECK ADD FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[TeacherSubjects]  WITH CHECK ADD FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[ClassSchedules]  WITH CHECK ADD  CONSTRAINT [CHK_Times] CHECK  (([EndTime]>[StartTime]))
GO
ALTER TABLE [dbo].[ClassSchedules] CHECK CONSTRAINT [CHK_Times]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  (([Role]='Student' OR [Role]='Teacher' OR [Role]='Admin'))
GO
