USE [StudentManagementDB]
GO
/****** Object:  Table [dbo].[Assignments]    Script Date: 3/27/2025 4:56:51 AM ******/
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
PRIMARY KEY CLUSTERED 
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classes]    Script Date: 3/27/2025 4:56:51 AM ******/
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
PRIMARY KEY CLUSTERED 
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClassSchedules]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Exams]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Notifications]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[PasswordResetTokens]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Scores]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Students]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Subjects]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Submissions]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Teachers]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[TeacherSubjects]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 3/27/2025 4:56:51 AM ******/
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
SET IDENTITY_INSERT [dbo].[Classes] ON 

INSERT [dbo].[Classes] ([ClassID], [ClassName], [Grade], [TeacherID], [ClassRoom], [MaxCapacity], [CurrentStudentCount], [AcademicYear], [IsActive]) VALUES (1, N'Sample Class A', N'10th Grade', 1, N'Room 1011', 35, 2, N'2023-2024', 1)
INSERT [dbo].[Classes] ([ClassID], [ClassName], [Grade], [TeacherID], [ClassRoom], [MaxCapacity], [CurrentStudentCount], [AcademicYear], [IsActive]) VALUES (2, N'aaaaaaa', N'1', NULL, N'aaa', 30, 0, N'2025-2026', 0)
SET IDENTITY_INSERT [dbo].[Classes] OFF
GO
SET IDENTITY_INSERT [dbo].[ClassSchedules] ON 

INSERT [dbo].[ClassSchedules] ([ScheduleID], [ClassID], [SubjectID], [TeacherID], [DayOfWeek], [StartTime], [EndTime], [Room]) VALUES (1, 1, 1, 2, N'Monday', CAST(N'08:00:00' AS Time), CAST(N'10:00:00' AS Time), N'AAC')
SET IDENTITY_INSERT [dbo].[ClassSchedules] OFF
GO
SET IDENTITY_INSERT [dbo].[Exams] ON 

INSERT [dbo].[Exams] ([ExamID], [SubjectID], [ClassID], [ExamName], [ExamDate], [Duration], [TotalMarks], [Description]) VALUES (1, 1, 1, N'aaa', CAST(N'2025-03-27T03:07:53.977' AS DateTime), 30, 100, N'aaa')
SET IDENTITY_INSERT [dbo].[Exams] OFF
GO
SET IDENTITY_INSERT [dbo].[Notifications] ON 

INSERT [dbo].[Notifications] ([NotificationID], [Title], [Message], [SenderID], [RecipientType], [RecipientID], [IsRead], [CreatedDate], [ExpiryDate]) VALUES (2, N'aaaa', N'aaaaa', 2, N'All', NULL, 0, CAST(N'2025-03-27T03:31:22.730' AS DateTime), CAST(N'2025-04-03T23:59:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[Notifications] OFF
GO
SET IDENTITY_INSERT [dbo].[Students] ON 

INSERT [dbo].[Students] ([StudentID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [ClassID], [EnrollmentDate]) VALUES (1, 5, N'Michael', N'Brown', CAST(N'2005-03-12' AS Date), N'Male', NULL, NULL, 1, CAST(N'2025-03-25' AS Date))
INSERT [dbo].[Students] ([StudentID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [ClassID], [EnrollmentDate]) VALUES (2, 6, N'Emily', N'Davis1', CAST(N'2006-07-25' AS Date), N'Female', NULL, NULL, 1, CAST(N'2025-03-25' AS Date))
SET IDENTITY_INSERT [dbo].[Students] OFF
GO
SET IDENTITY_INSERT [dbo].[Subjects] ON 

INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [Description], [Credits], [IsActive]) VALUES (1, N'aaa', N'aaa', 100, 1)
SET IDENTITY_INSERT [dbo].[Subjects] OFF
GO
SET IDENTITY_INSERT [dbo].[Teachers] ON 

INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [HireDate], [Specialization]) VALUES (1, 3, N'John', N'Smith', CAST(N'1980-05-15' AS Date), N'Male', NULL, N'555-123-4567', CAST(N'2025-03-25' AS Date), N'Mathematics')
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FirstName], [LastName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [HireDate], [Specialization]) VALUES (2, 4, N'Sarah', N'Johnson', CAST(N'1985-08-22' AS Date), N'Female', NULL, N'555-234-5678', CAST(N'2025-03-25' AS Date), N'Science')
SET IDENTITY_INSERT [dbo].[Teachers] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (1, N'admin', 0xCD8C29B8DEED323FE1538CFBDB46FC2A2EA61CFD67807F0629708EA2A6E13A2919DEF3C837C4E7F2C8E0067568E3236827DEFB05C9346E476B6E954440A908A7, 0x, N'admin@school.com', N'Admin', 0, CAST(N'2025-03-25T02:40:48.420' AS DateTime), NULL)
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (2, N'administrator', 0x7FCF4BA391C48784EDDE599889D6E3F1E47A27DB36ECC050CC92F259BFAC38AFAD2C68A1AE804D77075E8FB722503F3ECA2B2C1006EE6F6C7B7628CB45FFFD1D, 0x, N'administrator@school.com', N'Admin', 1, CAST(N'2025-03-25T02:40:48.423' AS DateTime), CAST(N'2025-03-27T03:37:17.570' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (3, N'teacher1', 0xB0A8F9F878F9D6A2F4866094B979461841D00E780C909557EBFEA85E1B73DFB2AEE0EB5819E2060C3C18BF99AB545AEAD5776A92ECB7FDB311A55E767A49CFDB, 0x, N'teacher1@school.com', N'Teacher', 1, CAST(N'2025-03-25T04:55:39.587' AS DateTime), CAST(N'2025-03-25T05:05:32.863' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (4, N'teacher2', 0xB0A8F9F878F9D6A2F4866094B979461841D00E780C909557EBFEA85E1B73DFB2AEE0EB5819E2060C3C18BF99AB545AEAD5776A92ECB7FDB311A55E767A49CFDB, 0x, N'teacher2@school.com', N'Teacher', 1, CAST(N'2025-03-25T04:55:39.597' AS DateTime), CAST(N'2025-03-25T05:10:04.707' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (5, N'student1', 0x6ABC9C01CFB743A16053BB0F02046D3AA89BB6A548FEEA85DD674D0F7A04171927F5F485AEA6C7B7A2EDF9E29AEBDB10054BF1CA3F49FF2CE13BDE54B3C526C7, 0x, N'student1@school.com', N'Student', 1, CAST(N'2025-03-25T04:55:39.597' AS DateTime), CAST(N'2025-03-25T05:11:55.340' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (6, N'student2', 0x6ABC9C01CFB743A16053BB0F02046D3AA89BB6A548FEEA85DD674D0F7A04171927F5F485AEA6C7B7A2EDF9E29AEBDB10054BF1CA3F49FF2CE13BDE54B3C526C7, 0x, N'student2@school.com', N'Student', 1, CAST(N'2025-03-25T04:55:39.600' AS DateTime), NULL)
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (7, N'hung', 0x20D13DCB53B8012965DF8EB49568814DA93C3EE62D149B3C89A370BAB6AA9099529C886AAA53074797CEFB6CC24C3CE654DA8BCE602CAA6C244C97956B741EA9, 0x516BAE044A41EA80FCED5C27E7C8102129243AF178CDA889D66F68586CB428E134202FF4B38C01115CCEC7B8DCFB98018B0F6339B3F1C8AD305AD04C7CC74D76273DF94B3CA72382A89728BD6FF7593055595872EE644E765EE9BBCEB0890EEE0C9DD2FBA8601230DF6BF3E3958CFFA6D09D36EB6BA9DEDCC2DDD1B97FD2977C, N'hung@gmail.com', N'Student', 1, CAST(N'2025-03-27T01:20:43.213' AS DateTime), CAST(N'2025-03-27T03:36:47.823' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (8, N'hungA', 0x91126899D79CF026BFAB3D0932C6D3000CB661E5E2B92069DB1B2C948C9B5368E398A4409F2841AB55530427356FF618F4B2D2830B14871D2DAB9146E19637AF, 0x50D2F61BCAD6B1B00753FD03E792FB5CDAD3BE75CEFE6D9972D6BE83F1A1135309C4B577D145814186BBDD93B258998F7FAFCB38275CA874283AAF2BE265CF201749A2C6C35DA9FAF576542368DF61A3650D4B8477759389038CBEADB839B138323481F69F9945C24D427DAEC54CDA9AFCF08EFBBD6E63314A51F4696836235C, N'hungA@gmail.com', N'Teacher', 1, CAST(N'2025-03-27T03:37:48.567' AS DateTime), CAST(N'2025-03-27T04:15:19.990' AS DateTime))
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [PasswordSalt], [Email], [Role], [IsActive], [CreatedDate], [LastLoginDate]) VALUES (9, N'hungB', 0x33C17B543B26A1670E2B7CC770FCB7280125B030F0348197DA5399340E6CF14C23C66C4AC7AAEF27658AF4B2E3A42F2A2EAB04428A0D13BA727F6BCEC851702B, 0xE53F45E375E15913405E434E494A6B20C67E39BBF6622FDCC7E64139C711BEB91B6093FC355BB955CC348A6957DAA9CF8CA8D909CE3835416C75A4251E6938763C554C28364DECCCF5BB2F7396421E96675CB7B0F6461A8CBFA9A9473CF4063A1B261B38561380420C93FDDD82F9661DC174F9BDEF233621AFAA261708DD62AF, N'hungB@gmail.com', N'Admin', 1, CAST(N'2025-03-27T03:38:08.517' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [UQ_StudentExam]    Script Date: 3/27/2025 4:56:51 AM ******/
ALTER TABLE [dbo].[Scores] ADD  CONSTRAINT [UQ_StudentExam] UNIQUE NONCLUSTERED 
(
	[StudentID] ASC,
	[ExamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Students__1788CCADA384046B]    Script Date: 3/27/2025 4:56:51 AM ******/
ALTER TABLE [dbo].[Students] ADD UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_StudentAssignment]    Script Date: 3/27/2025 4:56:51 AM ******/
ALTER TABLE [dbo].[Submissions] ADD  CONSTRAINT [UQ_StudentAssignment] UNIQUE NONCLUSTERED 
(
	[StudentID] ASC,
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Teachers__1788CCADC1F53F22]    Script Date: 3/27/2025 4:56:51 AM ******/
ALTER TABLE [dbo].[Teachers] ADD UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_TeacherSubject]    Script Date: 3/27/2025 4:56:51 AM ******/
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
/****** Object:  Index [UQ__Users__536C85E4727B2562]    Script Date: 3/27/2025 4:56:51 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D1053460FD7FD6]    Script Date: 3/27/2025 4:56:51 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Assignments] ADD  DEFAULT (getdate()) FOR [AssignedDate]
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
