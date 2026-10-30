

EXEC sp_rename
    'dbo.TblRguhsFacultyUser',
    'TblRguhsFacultyUser_Old';
GO

CREATE TABLE dbo.TblRguhsFacultyUser
(
    UserId INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_TblRguhsFacultyUser PRIMARY KEY,

    Id INT NULL,

    Password NVARCHAR(256) NULL,
    PasswordHash NVARCHAR(MAX) NULL,
    UserName NVARCHAR(100) NULL,
    IsActive BIT NOT NULL,
    Faculty INT NULL,
    IsFinance BIT NULL,
    FinanceDesignation VARCHAR(10) NULL,
    DesignationDescription VARCHAR(200) NULL,
    IsSection BIT NULL,
    IsAdmin BIT NULL,
    FailedLoginAttempts INT NOT NULL,
    LockoutEndTime DATETIME NULL
);


----------------------------------

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_DEO', 'DentalDEO@2026', '$2y$10$ay08DD3FqFHUyZqw/SE6dOorSppfLzTMZdd4je1kksHf6HxWuKYfW', 1, 2, 0, NULL, 'Data Entry Operator', 0, 0, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_JR', 'DentalJR@2026', '$2y$10$Vb/o0SOyv.X2dWmFZKI7kus/AqBtOOnIfxf5BHCfntsA3NQCvc/.K', 1, 2, 0, NULL, 'Junior Assistant', 0, 0, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_SO', 'DentalSO@2026', '$2y$10$68SypNpAYeJveDDtR9A.ZefTlq4vp4Nm6Smbh/vfGwG1MV0klIU5y', 1, 2, 0, NULL, 'Section Officer', 1, 0, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_AR', 'DentalAR@2026', '$2y$10$EM4KacCJx4VFEgD.88.hO.W0cL15tsr.obOEkwYKjtu7.SmzVJLEu', 1, 2, 0, NULL, 'Assistant Registrar', 0, 0, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_RG', 'DentalRG@2026', '$2y$10$Dvyp7iYXiy3hHfyzrghTtOpq5t9sEL/utd6gSqT7AWuAmqSCDq6oy', 1, 2, 0, NULL, 'Registrar', 0, 0, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_RE', 'DentalRE@2026', '$2y$10$2QhIaIi/BH6RM.RUmKGC4uebNLQKVN42pWSDlVGjCREz2G7qjiIMW', 1, 2, 0, NULL, 'Registrar Evaluation', 0, 0, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_DR', 'DentalDR@2026', '$2y$10$QH2iUDvmoniP7TRBry4GluosxVHrewtD4bH255zf1N6IbjS9cj1xK', 1, 2, 0, NULL, 'Director', 0, 1, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('DENTAL_VC', 'DentalVC@2026', '$2y$10$r2LCi8ut3oHNUYLF7XfDcuJtN1ANhhfCp740xOlhKqDmoZzJLoqVe', 1, 2, 0, NULL, 'Vice Chancellor', 0, 1, 0, NULL);

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('ADMIN', 'Admin@2026', '$2y$10$l6rdWrtRY4G654O3TPIRROQ9M6Y.NsaMbszy1axC8iMWhkkGiBPUe', 1, NULL, 0, NULL, 'System Administrator', 1, 1, 0, NULL);

----------------------------------

INSERT INTO dbo.TblRguhsFacultyUser
(
    Id,
    Password,
    PasswordHash,
    UserName,
    IsActive,
    Faculty,
    IsFinance,
    FinanceDesignation,
    DesignationDescription,
    IsSection,
    IsAdmin,
    FailedLoginAttempts,
    LockoutEndTime
)
SELECT
    Id,
    Password,
    PasswordHash,
    UserName,
    IsActive,
    Faculty,
    IsFinance,
    FinanceDesignation,
    DesignationDescription,
    IsSection,
    IsAdmin,
    FailedLoginAttempts,
    LockoutEndTime
FROM dbo.TblRguhsFacultyUser_Old
ORDER BY Id;

------------------------------------------------

-- ============================================
-- Create College Mapping Table
-- ============================================
USE [Admission_Affiliation];
GO

CREATE TABLE [dbo].[Tbl_CollegeMapping](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UserId] [int] NOT NULL,
    [UserName] [varchar](50) NOT NULL,
    [FacultyCode] int NOT NULL,
    [CollegeFrom] [varchar](5) NOT NULL,   -- Starting letter (e.g., 'A')
    [CollegeTo] [varchar](5) NOT NULL,     -- Ending letter (e.g., 'M')
    [CreatedDate] [datetime] NULL,
    [CreatedBy] [varchar](50) NULL,
    [IsActive] [bit] NULL,
    CONSTRAINT [PK_Tbl_CollegeMapping] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY];
GO

-- Add foreign key relationship
ALTER TABLE [dbo].[Tbl_CollegeMapping] WITH CHECK 
ADD CONSTRAINT [FK_Tbl_CollegeMapping_Faculty] 
FOREIGN KEY([FacultyCode])
REFERENCES [dbo].[Faculty] ([FacultyId]);
GO

-- Add index for faster lookups
CREATE NONCLUSTERED INDEX [IX_Tbl_CollegeMapping_UserId] 
ON [dbo].[Tbl_CollegeMapping] ([UserId] ASC);
GO


CREATE NONCLUSTERED INDEX [IX_Tbl_CollegeMapping_FacultyCode] 
ON [dbo].[Tbl_CollegeMapping] ([FacultyCode] ASC);
GO

-- Verify table created
SELECT * FROM [dbo].[Tbl_CollegeMapping];

ALTER TABLE Tbl_CollegeMapping
ADD FromLetter NVARCHAR(1) NOT NULL DEFAULT('A'),
    ToLetter NVARCHAR(1) NOT NULL DEFAULT('Z');

-- Update existing NULL values first
UPDATE Tbl_CollegeMapping
SET IsActive = 1
WHERE IsActive IS NULL;

-- Make the column NOT NULL
ALTER TABLE Tbl_CollegeMapping
ALTER COLUMN IsActive BIT NOT NULL;

ALTER TABLE Tbl_CollegeMapping
ADD CONSTRAINT DF_TblCollegeMapping_IsActive
DEFAULT (1) FOR IsActive;
------------------------------------

/* ============================================================
   1. MstTabs
   ============================================================ */

CREATE TABLE dbo.MstTabs
(
    TabId INT IDENTITY(1,1) NOT NULL,
    FacultyId INT NOT NULL,
    TabName NVARCHAR(200) NOT NULL,

    CONSTRAINT PK_MstTabs
        PRIMARY KEY (TabId),

    CONSTRAINT FK_MstTabs_Faculty
        FOREIGN KEY (FacultyId)
        REFERENCES dbo.Faculty(FacultyId)
);
GO

/* ============================================================
   2. MstSections
   ============================================================ */

CREATE TABLE dbo.MstSections
(
    SectionId INT IDENTITY(1,1) NOT NULL,
    FacultyId INT NOT NULL,
    TabId INT NOT NULL,
    SectionName NVARCHAR(200) NOT NULL,

    CONSTRAINT PK_MstSections
        PRIMARY KEY (SectionId),

    CONSTRAINT FK_MstSections_Faculty
        FOREIGN KEY (FacultyId)
        REFERENCES dbo.Faculty(FacultyId),

    CONSTRAINT FK_MstSections_MstTabs
        FOREIGN KEY (TabId)
        REFERENCES dbo.MstTabs(TabId)
);
GO

-------------------------- MST TABS INSERT QUERIES ----------------

INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Institution Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Trust Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Trust Member Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Dean / Director Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Principal Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'UG Course Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'PG Course Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Courses & Intake');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Land And Building Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Classroom And Laboratory');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Chair Distribution');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Bed Distribution');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Hostel Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Department Offices And Educational Unit');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Equipment List');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Vehicle Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'UG Academic Matters');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'PG Academic Matters');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Staff Pay Scale');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Staff Other Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Finance Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Library Services');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Research & Publications');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Library Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Faculty Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Teaching Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Clinical Facilities');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Workshop Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Animal House Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Field Practice Area');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Library Expenditure and Services');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Library Staff Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Library User Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Payment Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Action Taken Report');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Faculty Repository');

---------------------------------- INSERT QUERIES MST SECTION ----------------------------

INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Basic Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Address Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Contact Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Institutional Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Head of Institution');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Nodal Officer Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Principal Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Dean / Director Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Trust Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 2, 'Trust Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 3, 'Trust Members');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 4, 'Dean / Director Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 4, 'Teaching Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 4, 'Administrative Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 5, 'Principal Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 5, 'Teaching Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 5, 'Administrative Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 6, 'Present Intake');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 6, 'Previous Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 6, 'Particulars of Permission');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 6, 'Year of Obtaining EC & FC from Government of Karnataka');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 6, 'Year of Last Affiliation Granted by RGUHS');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 6, 'Previous Inspection by LIC');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 7, 'PG Course Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 9, 'Land Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 9, 'Building Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 10, 'Facility Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 11, 'Chair Distribution Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 12, 'Beds Distribution');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 13, 'Hostel Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 13, 'Student Accommodation');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 13, 'Common Room Facilities');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 13, 'Hostel Facilities');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 14, 'Department Office Infrastructure');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 14, 'Dental Education Unit');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 15, 'Equipment Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 16, 'Vehicle Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 17, 'UG Academic Performance');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 18, 'PG Academic Performance');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 19, 'Staff Pay Scale');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 20, 'Staff Other Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 21, 'Authority & Account Details - UG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 21, 'Annual Financial Details - UG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 21, 'Account & Audit Details - UG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 21, 'Authority & Account Details - PG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 21, 'Annual Financial Details - PG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 21, 'Account & Audit Details - PG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 22, 'Department Library Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 23, 'Research & Publication Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 24, 'General Library Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 24, 'Library Items');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 24, 'Library Building');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 24, 'Technical Processes');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 24, 'Library Equipment');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 24, 'Library Finance');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 25, 'Faculty Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Clinical Hospital Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Clinical Statistics');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Discipline Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Nursing, Paramedical, Technical & Allied Services');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Engineering & Allied Services');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Ward-wise Bed Distribution in Attached Hospital');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Administrative Section');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Managing Other Health Science Colleges');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 28, 'Workshop Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 29, 'Animal House Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 30, 'Rural Field Practice Area');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 30, 'Urban Field Practice Area');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 31, 'Library services availability');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 31, 'Library Expenditure');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 32, 'Library Staff Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 33, 'Library User Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 34, 'Payment Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 34, 'Payment Receipt Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 35, 'Action Taken Report');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 36, 'Faculty Repository');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Certificates and Future Development Documents');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Anatomy Act Registration');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 27, 'Hospital Tie up');



--------------------- SECTION WISE FEEDBACK TABLE -------------------

/* ============================================================
   3. SectionWiseFeedback
   ============================================================ */

CREATE TABLE dbo.SectionWiseFeedback
(
    SectionWiseFeedbackId INT IDENTITY(1,1) NOT NULL,

    FacultyId INT NOT NULL,
    CollegeCode NVARCHAR(100) NOT NULL,
    CourseLevel VARCHAR(10) NULL,
    TabId INT NOT NULL,
    SectionId INT NOT NULL,
    VerificationStatus NVARCHAR(50) NULL,
    Remarks NVARCHAR(max) NULL,

    VerifiedBy NVARCHAR(200) NULL,
    VerifiedOn DATETIME2 NULL,

    CONSTRAINT PK_SectionWiseFeedback
        PRIMARY KEY (SectionWiseFeedbackId),

    CONSTRAINT FK_SectionWiseFeedback_Faculty
        FOREIGN KEY (FacultyId)
        REFERENCES dbo.Faculty(FacultyId),

    CONSTRAINT FK_SectionWiseFeedback_College
        FOREIGN KEY (CollegeCode)
        REFERENCES [dbo].[Affiliation_College_Master](CollegeCode),

    CONSTRAINT FK_SectionWiseFeedback_Tab
        FOREIGN KEY (TabId)
        REFERENCES dbo.MstTabs(TabId),

    CONSTRAINT FK_SectionWiseFeedback_Section
        FOREIGN KEY (SectionId)
        REFERENCES dbo.MstSections(SectionId)
);


------------------- INDEXES ---------------------


/* ============================================================
   5. Useful indexes
   ============================================================ */

CREATE INDEX IX_MstTabs_FacultyId
ON dbo.MstTabs(FacultyId);
GO

CREATE INDEX IX_MstSections_FacultyId
ON dbo.MstSections(FacultyId);
GO

CREATE INDEX IX_MstSections_TabId
ON dbo.MstSections(TabId);
GO

CREATE INDEX IX_SectionWiseFeedback_FacultyId
ON dbo.SectionWiseFeedback(FacultyId);
GO

CREATE INDEX IX_SectionWiseFeedback_CollegeCode
ON dbo.SectionWiseFeedback(CollegeCode);
GO

CREATE INDEX IX_SectionWiseFeedback_TabId
ON dbo.SectionWiseFeedback(TabId);
GO

CREATE INDEX IX_SectionWiseFeedback_SectionId
ON dbo.SectionWiseFeedback(SectionId);
GO

----------------------- MST DOCUMENTS --------------------------

CREATE TABLE MstDocument
(
    -- Primary key for the document
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,

    -- Faculty to which this document belongs
    -- Example: Medical, Dental, Nursing, etc.
    FacultyId INT NOT NULL,

    -- Tab under which the document should appear
    -- Example: Infrastructure, Institution Details, Faculty Details
    TabId INT NOT NULL,

    -- Optional section under the selected Tab
    -- Can be NULL if the document belongs directly to the Tab
    SectionId INT NULL,

    -- Name of the required document
    -- Example: Fire Safety Certificate
    DocumentName VARCHAR(MAX) NOT NULL,

    -- Indicates whether the document is mandatory
    -- 1 = Mandatory
    -- 0 = Optional
    IsMandatory BIT NOT NULL DEFAULT 1,

    -- Controls the order in which documents are displayed
    -- Example: 1, 2, 3, etc.
    DisplayOrder INT NULL,

    -- Indicates whether the document is currently active
    -- 1 = Active
    -- 0 = Inactive
    IsActive BIT NOT NULL DEFAULT 1,

    -- Foreign key relationship with Faculty table
    CONSTRAINT FK_MstDocument_Faculty
        FOREIGN KEY (FacultyId)
        REFERENCES Faculty(FacultyId),

    -- Foreign key relationship with MstTab table
    CONSTRAINT FK_MstDocument_MstTab
        FOREIGN KEY (TabId)
        REFERENCES MstTabs(TabId),

    -- Foreign key relationship with MstSection table
    -- SectionId is optional, so it can contain NULL
    CONSTRAINT FK_MstDocument_MstSection
        FOREIGN KEY (SectionId)
        REFERENCES MstSections(SectionId)
);


--------------------- Document Wise Feedback ---------------------

CREATE TABLE DocumentWiseFeedback
(
    -- Primary key
    DocumentWiseFeedbackId INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_DocumentWiseFeedback
        PRIMARY KEY,

    -- Faculty to which the verification belongs
    FacultyId INT NOT NULL,

    -- College whose document is being verified
    CollegeCode NVARCHAR(100) NOT NULL,

    -- Master document being verified
    DocumentId INT NOT NULL,

    -- RGUHS user/verifier providing the feedback
    UserId INT NOT NULL,

    -- Verification feedback / remarks
    Feedback NVARCHAR(MAX) NULL,

    -- Verification status
    -- Example: Pending / Approved / Rejected / ClarificationRequired
    Status NVARCHAR(50) NOT NULL,

    -- Indicates whether the feedback record is active
    IsActive BIT NOT NULL
        CONSTRAINT DF_DocumentWiseFeedback_IsActive
        DEFAULT (1),

    -- Audit fields
    CreatedOn DATETIME NOT NULL
        CONSTRAINT DF_DocumentWiseFeedback_CreatedOn
        DEFAULT (GETDATE()),

    ModifiedOn DATETIME NULL,

    -- Foreign key to Faculty
    CONSTRAINT FK_DocumentWiseFeedback_Faculty
        FOREIGN KEY (FacultyId)
        REFERENCES Faculty(FacultyId),

    -- Foreign key to College
    CONSTRAINT FK_DocumentWiseFeedback_College
        FOREIGN KEY (CollegeCode)
        REFERENCES Affiliation_College_Master(CollegeCode),

    -- Foreign key to Document
    CONSTRAINT FK_DocumentWiseFeedback_Document
        FOREIGN KEY (DocumentId)
        REFERENCES MstDocument(DocumentId),

    -- Foreign key to RGUHS Faculty User
    CONSTRAINT FK_DocumentWiseFeedback_User
        FOREIGN KEY (UserId)
        REFERENCES TblRguhsFacultyUser(UserId)
);

ALTER TABLE DocumentWiseFeedback
ADD CONSTRAINT UQ_DocumentWiseFeedback
UNIQUE
(
    FacultyId,
    CollegeCode,
    DocumentId,
    UserId
);

--------------------------------------------

--DocumentName
--GOK Order Document
--Trust PAN
--Bank Statement
--Registration Certificate
--Audit Statement
--Members of Governing Body or Council
--Appointment Order of Dean
--Government Autonomous Certificate
--Sale Deed
--Encumbrance Certificate
--Land Use Certificate
--Approved Layout Plan
--Land Sketch
--Distance Certificate
--Completion Certificate
--Structural Stability Certificate
--Fire Safety NOC
--Lift License
--Electrical Safety Certificate
--Previous Year Notification
--GOK Order
--RGUHS Notification
--Possession Proof
--DEU Members List
--Rural Field Practice Area - Staff List
--Urban Field Practice Area - Staff List
--Examiners List
--Form 16 Details
--Form 16 Details Upload 2
--Form 16 Details Upload 3
--Form 16 Details Upload 4
--Form 16 Details Upload 5
--AEBAS � Last Three Months
--AEBAS � Inspection Day
--Provident Fund
--ESI
--Transaction Receipt
--Relevant Report
--KPME Certificate
--Pollution Control Board Certificate
--Bio-Medical Waste Certificate
--Drug Free Campus Certification
--Proposed Plans for Future Developments
--Anatomy Act Registration
--Hospital Tie up Document

--------------------------------------------------