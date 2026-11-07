



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


----------- MEDICAL FACULTY USERS ----------

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_DEO', 'MedicalDEO@2026', '$2a$12$0V4Ha7UK.raNCyKjbeKxhe2y6PcF.r3ow/REahmsXnuu35WjYp0tC', 1, 1, 0, NULL, 'Data Entry Operator', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_JR', 'MedicalJR@2026', '$2a$12$yWy1Py26URsmB4iNINtSC.OW3.1aK.XEPwGOk8MqaMzeX.B9Lscze', 1, 1, 0, NULL, 'Junior Assistant', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_SO', 'MedicalSO@2026', '$2a$12$AsPHU4NBYz3Lh6gxcjpbX.VXeUYa7l5GQIOa.ke1h8x8f59kO3ebC', 1, 1, 0, NULL, 'Section Officer', 1, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_AR', 'MedicalAR@2026', '$2a$12$auLUmk5SyrUt052d5JlAzOsbzpIjtTlACSA5WUtIq0o716USIzoWa', 1, 1, 0, NULL, 'Assistant Registrar', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_RG', 'MedicalRG@2026', '$2a$12$K1eNGE8J0yjtyZo21ylBf.Uz.Ms2VNw799IuVfNPNCi9zSj.Uk8MO', 1, 1, 0, NULL, 'Registrar', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_RE', 'MedicalRE@2026', '$2a$12$tjqzdNW26Iw9fGe.xQUVp.39I5qRJe1drjnro/y7oB7bzjzklKhAq', 1, 1, 0, NULL, 'Registrar Evaluation', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_DR', 'MedicalDR@2026', '$2a$12$ea5YUfN5lXdLyjs/nhu04uhk28kB5hZLmjV9nLbQtERjXoZYSnEz.', 1, 1, 0, NULL, 'Director', 0, 1, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_VC', 'MedicalVC@2026', '$2a$12$WBsxyo4qvt8DVjG4gbYAe.Zj08II5LZsXH7Lcfpbc90cJKq6rgFi6', 1, 1, 0, NULL, 'Vice Chancellor', 0, 1, 0, NULL);


---------------- TBL COLLEGE MAPPING ----------------

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

ALTER TABLE Tbl_CollegeMapping
ADD
    VerificationLevel INT NOT NULL
        CONSTRAINT DF_CollegeMapping_VerificationLevel DEFAULT 1,
    VerifiedBy INT NULL,
    VerifiedDate DATETIME NULL;


    -- Quickly retrieve colleges awaiting verification at a particular level
CREATE NONCLUSTERED INDEX IX_CollegeMapping_Level_Active
ON Tbl_CollegeMapping(VerificationLevel, IsActive)
INCLUDE (UserId, FacultyCode, CollegeFrom, CollegeTo);

-- Quickly find mappings belonging to a particular user
CREATE NONCLUSTERED INDEX IX_CollegeMapping_UserId
ON Tbl_CollegeMapping(UserId);




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

------------------------------------- MST TABS INSERT QUERIES ------------------------

INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Institution Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Trust Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Trust Member Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Dean or Director Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Principal Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'UG Course Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Land And Building Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Classroom And Laboratory');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Bed Distribution');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Equipment List');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Department Offices And Educational Unit');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Hostel Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Vehicle Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'UG Academic Matters');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Finance Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Staff Pay Scale');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Staff Other Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Library Services');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Research and Publications');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Library Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Non Teaching Staff Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Payment Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Geo Tagged Photos');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Associated Institution');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Institution Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Trust Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Trust Member Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Dean or Director Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Principal Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'UG Course Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'PG Course Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Courses and Intake');
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
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Research and Publications');
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
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (2, 'Non Teaching Staff Details');

------------------------------- MST SECTION INSERT QUERIES --------------------------

/*
    MstSections data migration using FacultyId + TabName mapping.

    IMPORTANT:
    1. Populate MstTabs first.
    2. Clear existing MstSections data before running this script, as planned.
    3. This script does NOT use the old TabId values from the source data.
       It resolves the current TabId from MstTabs using FacultyId + TabName.
    4. Review the missing-tab validation output before inserting.
*/

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @SectionSeed TABLE
    (
        FacultyId   INT NOT NULL,
        TabName     NVARCHAR(150) NOT NULL,
        SectionName NVARCHAR(250) NOT NULL
    );

    INSERT INTO @SectionSeed (FacultyId, TabName, SectionName)
    VALUES
    (2, N'Institution Details', N'Basic Information'),
    (2, N'Institution Details', N'Address Details'),
    (2, N'Institution Details', N'Contact Information'),
    (2, N'Institution Details', N'Institutional Details'),
    (2, N'Institution Details', N'Head of Institution'),
    (2, N'Institution Details', N'Nodal Officer Details'),
    (2, N'Institution Details', N'Principal Details'),
    (2, N'Institution Details', N'Dean / Director Details'),
    (2, N'Institution Details', N'Trust Details'),
    (2, N'Trust Details', N'Trust Information'),
    (2, N'Trust Member Details', N'Trust Members'),
    (2, N'Dean or Director Details', N'Dean / Director Information'),
    (2, N'Dean or Director Details', N'Teaching Experience'),
    (2, N'Dean or Director Details', N'Administrative Experience'),
    (2, N'Principal Details', N'Principal Information'),
    (2, N'Principal Details', N'Teaching Experience'),
    (2, N'Principal Details', N'Administrative Experience'),
    (2, N'UG Course Details', N'Present Intake'),
    (2, N'UG Course Details', N'Previous Details'),
    (2, N'UG Course Details', N'Particulars of Permission'),
    (2, N'UG Course Details', N'Year of Obtaining EC & FC from Government of Karnataka'),
    (2, N'UG Course Details', N'Year of Last Affiliation Granted by RGUHS'),
    (2, N'UG Course Details', N'Previous Inspection by LIC'),
    (2, N'PG Course Details', N'PG Course Details'),
    (2, N'Land And Building Details', N'Land Details'),
    (2, N'Land And Building Details', N'Building Details'),
    (2, N'Classroom And Laboratory', N'Facility Requirements'),
    (2, N'Chair Distribution', N'Chair Distribution Details'),
    (2, N'Bed Distribution', N'Beds Distribution'),
    (2, N'Hostel Details', N'Hostel Information'),
    (2, N'Hostel Details', N'Student Accommodation'),
    (2, N'Hostel Details', N'Common Room Facilities'),
    (2, N'Hostel Details', N'Hostel Facilities'),
    (2, N'Department Offices And Educational Unit', N'Department Office Infrastructure'),
    (2, N'Department Offices And Educational Unit', N'Dental Education Unit'),
    (2, N'Equipment List', N'Equipment Details'),
    (2, N'Vehicle Details', N'Vehicle Information'),
    (2, N'UG Academic Matters', N'UG Academic Performance'),
    (2, N'PG Academic Matters', N'PG Academic Performance'),
    (2, N'Staff Pay Scale', N'Staff Pay Scale'),
    (2, N'Staff Other Details', N'Staff Other Details'),
    (2, N'Finance Details', N'Authority & Account Details - UG'),
    (2, N'Finance Details', N'Annual Financial Details - UG'),
    (2, N'Finance Details', N'Account & Audit Details - UG'),
    (2, N'Finance Details', N'Authority & Account Details - PG'),
    (2, N'Finance Details', N'Annual Financial Details - PG'),
    (2, N'Finance Details', N'Account & Audit Details - PG'),
    (2, N'Library Services', N'Department Library Details'),
    (2, N'Research and Publications', N'Research and Publication Details'),
    (2, N'Library Details', N'General Library Details'),
    (2, N'Library Details', N'Library Items'),
    (2, N'Library Details', N'Library Building'),
    (2, N'Library Details', N'Technical Processes'),
    (2, N'Library Details', N'Library Equipment'),
    (2, N'Library Details', N'Library Finance'),
    (2, N'Faculty Details', N'Faculty Details'),
    (2, N'Clinical Facilities', N'Clinical Hospital Details'),
    (2, N'Clinical Facilities', N'Clinical Statistics'),
    (2, N'Clinical Facilities', N'Discipline Details'),
    (2, N'Clinical Facilities', N'Nursing, Paramedical, Technical & Allied Services'),
    (2, N'Clinical Facilities', N'Engineering & Allied Services'),
    (2, N'Clinical Facilities', N'Ward-wise Bed Distribution in Attached Hospital'),
    (2, N'Institution Details', N'Administrative Section'),
    (2, N'Institution Details', N'Managing Other Health Science Colleges'),
    (2, N'Workshop Details', N'Workshop Details'),
    (2, N'Animal House Details', N'Animal House Details'),
    (2, N'Field Practice Area', N'Rural Field Practice Area'),
    (2, N'Field Practice Area', N'Urban Field Practice Area'),
    (2, N'Library Expenditure and Services', N'Library services availability'),
    (2, N'Library Expenditure and Services', N'Library Expenditure'),
    (2, N'Library Staff Details', N'Library Staff Details'),
    (2, N'Library User Details', N'Library User Details'),
    (2, N'Payment Details', N'Payment Details'),
    (2, N'Payment Details', N'Payment Receipt Details'),
    (2, N'Action Taken Report', N'Action Taken Report'),
    (2, N'Faculty Repository', N'Faculty Repository'),
    (2, N'Clinical Facilities', N'Certificates and Future Development Documents'),
    (2, N'Clinical Facilities', N'Anatomy Act Registration'),
    (2, N'Clinical Facilities', N'Hospital Tie up'),
    (1, N'Institution Details', N'Basic Information'),
    (1, N'Institution Details', N'Address Details'),
    (1, N'Institution Details', N'Contact Information'),
    (1, N'Institution Details', N'Institutional Details'),
    (1, N'Institution Details', N'Head of Institution'),
    (1, N'Institution Details', N'Nodal Officer Details'),
    (1, N'Institution Details', N'Principal Details'),
    (1, N'Institution Details', N'Dean / Director Details'),
    (1, N'Institution Details', N'Trust Details'),
    (1, N'Trust Details', N'Trust Information'),
    (1, N'Trust Member Details', N'Trust Members'),
    (1, N'Dean or Director Details', N'Dean / Director Information'),
    (1, N'Dean or Director Details', N'Teaching Experience'),
    (1, N'Dean or Director Details', N'Administrative Experience'),
    (1, N'Principal Details', N'Principal Information'),
    (1, N'Principal Details', N'Teaching Experience'),
    (1, N'Principal Details', N'Administrative Experience'),
    (1, N'UG Course Details', N'Present Intake'),
    (1, N'UG Course Details', N'Previous Details'),
    (1, N'UG Course Details', N'Particulars of Permission'),
    (1, N'UG Course Details', N'Year of Obtaining EC & FC from Government of Karnataka'),
    (1, N'UG Course Details', N'Year of Last Affiliation Granted by RGUHS'),
    (1, N'UG Course Details', N'Previous Inspection by LIC'),
    (1, N'Land And Building Details', N'Land Details'),
    (1, N'Land And Building Details', N'Building Details'),
    (1, N'Land And Building Details', N'Area Details'),
    (1, N'Land And Building Details', N'Facilities Availability'),
    (1, N'Land And Building Details', N'Staff Quarters'),
    (1, N'Land And Building Details', N'Workshop Details'),
    (1, N'Land And Building Details', N'Animal House Details'),
    (1, N'Land And Building Details', N'Small Group Teaching General'),
    (1, N'Land And Building Details', N'Teaching Rooms'),
    (1, N'Land And Building Details', N'Laboratory Availability'),
    (1, N'Land And Building Details', N'Museum Availability'),
    (1, N'Classroom And Laboratory', N'Facility Requirements'),
    (1, N'Bed Distribution', N'Beds Distribution'),
    (1, N'Equipment List', N'Equipment Details'),
    (1, N'Department Offices And Educational Unit', N'Department Office Infrastructure'),
    (1, N'Department Offices And Educational Unit', N'Medical Education Unit'),
    (1, N'Hostel Details', N'Hostel Information'),
    (1, N'Hostel Details', N'Student Accommodation'),
    (1, N'Hostel Details', N'Common Room Facilities'),
    (1, N'Hostel Details', N'Hostel Facilities'),
    (1, N'Vehicle Details', N'Vehicle Information'),
    (1, N'UG Academic Matters', N'UG Academic Performance'),
    (1, N'Finance Details', N'Authority & Account Details - UG'),
    (1, N'Finance Details', N'Annual Financial Details - UG'),
    (1, N'Finance Details', N'Account & Audit Details - UG'),
    (1, N'Staff Pay Scale', N'Staff Pay Scale'),
    (1, N'Staff Other Details', N'Staff Other Details'),
    (1, N'Library Services', N'Department Library Details'),
    (1, N'Library Services', N'Library Services'),
    (1, N'Library Services', N'Library Staff Details'),
    (1, N'Library Services', N'Digital Valuation Centre'),
    (1, N'Library Services', N'Special Features'),
    (1, N'Research and Publications', N'Research and Publication Details'),
    (1, N'Research and Publications', N'Department wise Publications'),
    (1, N'Library Details', N'General Library Details'),
    (1, N'Library Details', N'Library Items'),
    (1, N'Library Details', N'Library Building'),
    (1, N'Library Details', N'Technical Processes'),
    (1, N'Library Details', N'Library Equipment'),
    (1, N'Library Details', N'Library Finance'),
    (1, N'Non Teaching Staff Details', N'Non Teaching Staff Details'),
    (2, N'Non Teaching Staff Details', N'Non Teaching Staff Details'),
    (1, N'Payment Details', N'Payment Details'),
    (1, N'Geo Tagged Photos', N'Blood Bank'),
    (1, N'Geo Tagged Photos', N'Classrooms Photo'),
    (1, N'Geo Tagged Photos', N'Clinical Wards'),
    (1, N'Geo Tagged Photos', N'College Building Photo'),
    (1, N'Geo Tagged Photos', N'Faculty Pics'),
    (1, N'Geo Tagged Photos', N'Attached Hospital'),
    (1, N'Geo Tagged Photos', N'ICU'),
    (1, N'Geo Tagged Photos', N'Labs'),
    (1, N'Associated Institution', N'Associated Institution Details');

    -- Validate that every FacultyId + TabName exists in MstTabs.
    IF EXISTS
    (
        SELECT 1
        FROM @SectionSeed AS s
        LEFT JOIN [Admission_Affiliation].[dbo].[MstTabs] AS t
            ON t.FacultyId = s.FacultyId
           AND LTRIM(RTRIM(t.TabName)) = LTRIM(RTRIM(s.TabName))
        WHERE t.TabId IS NULL
    )
    BEGIN
        SELECT DISTINCT
            s.FacultyId,
            s.TabName AS MissingTabName
        FROM @SectionSeed AS s
        LEFT JOIN [Admission_Affiliation].[dbo].[MstTabs] AS t
            ON t.FacultyId = s.FacultyId
           AND LTRIM(RTRIM(t.TabName)) = LTRIM(RTRIM(s.TabName))
        WHERE t.TabId IS NULL;

        THROW 50001, 'One or more tab names were not found in MstTabs. No sections were inserted.', 1;
    END;

    -- Insert sections using the current TabId resolved by FacultyId + TabName.
    INSERT INTO [Admission_Affiliation].[dbo].[MstSections]
    (
        FacultyId,
        TabId,
        SectionName
    )
    SELECT
        s.FacultyId,
        t.TabId,
        s.SectionName
    FROM @SectionSeed AS s
    INNER JOIN [Admission_Affiliation].[dbo].[MstTabs] AS t
        ON t.FacultyId = s.FacultyId
       AND LTRIM(RTRIM(t.TabName)) = LTRIM(RTRIM(s.TabName));

    DECLARE @InsertedRows INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @InsertedRows AS InsertedSectionCount;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;


--DELETE FROM MstSections

--delete from mstdocument

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


------------- TAB WISE FEEDBACK -------------

CREATE TABLE TabWiseFeedback
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FacultyId INT NOT NULL,
    CollegeCode NVARCHAR(100) NOT NULL,
    CourseLevel VARCHAR(50) NULL,
    TabId INT NOT NULL,
    VerificationStatus VARCHAR(50) NULL,
    Remarks NVARCHAR(MAX) NULL,
    VerifiedBy VARCHAR(100) NULL,
    VerifiedOn DATETIME NULL,

    CONSTRAINT FK_TabWiseFeedback_Faculty
        FOREIGN KEY (FacultyId)
        REFERENCES Faculty(FacultyId),

    CONSTRAINT FK_TabWiseFeedback_College
        FOREIGN KEY (CollegeCode)
        REFERENCES [dbo].[Affiliation_College_Master](CollegeCode),

    CONSTRAINT FK_TabWiseFeedback_Tab
        FOREIGN KEY (TabId)
        REFERENCES MstTabs(TabId)
);


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



--SELECT
--    'INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ' +
--    '([DocumentId], [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (' +
--    CAST([DocumentId] AS VARCHAR(20)) + ', ' +
--    CAST([FacultyId] AS VARCHAR(20)) + ', ' +
--    CAST([TabId] AS VARCHAR(20)) + ', ' +
--    CAST([SectionId] AS VARCHAR(20)) + ', ' +
--    CASE
--        WHEN [DocumentName] IS NULL THEN 'NULL'
--        ELSE '''' + REPLACE([DocumentName], '''', '''''') + ''''
--    END + ', ' +
--    CASE
--        WHEN [IsMandatory] IS NULL THEN 'NULL'
--        WHEN [IsMandatory] = 1 THEN '1'
--        ELSE '0'
--    END + ', ' +
--    COALESCE(CAST([DisplayOrder] AS VARCHAR(20)), 'NULL') + ', ' +
--    CASE
--        WHEN [IsActive] IS NULL THEN 'NULL'
--        WHEN [IsActive] = 1 THEN '1'
--        ELSE '0'
--    END +
--    ');' AS InsertQuery
--FROM [Admission_Affiliation].[dbo].[MstDocument]
--ORDER BY [DocumentId];

--------------------- MST DOCUMENTS ---------------------

/*
    MstDocument migration script
    Source: user-provided MstDocument INSERT list (48 records).
    TabId and SectionId are resolved by FacultyId + TabName + SectionName.
    DocumentId is generated by the target database (not copied).
    DisplayOrder NULL, IsMandatory 0, IsActive 1 are retained from source.
*/

USE [Admission_Affiliation];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @DocumentSeed TABLE
(
    FacultyId INT NOT NULL,
    TabName NVARCHAR(150) NOT NULL,
    SectionName NVARCHAR(250) NOT NULL,
    DocumentName NVARCHAR(500) NOT NULL,
    IsMandatory BIT NULL,
    DisplayOrder INT NULL,
    IsActive BIT NULL
);

INSERT INTO @DocumentSeed
(
    FacultyId, TabName, SectionName, DocumentName,
    IsMandatory, DisplayOrder, IsActive
)
VALUES
    (2, N'Institution Details', N'Basic Information', N'Government Autonomous Certificate', 0, NULL, 1),
    (1, N'Institution Details', N'Basic Information', N'Government Autonomous Certificate', 0, NULL, 1),
    (2, N'Institution Details', N'Dean / Director Details', N'Appointment Order of Dean', 0, NULL, 1),
    (1, N'Institution Details', N'Dean / Director Details', N'Appointment Order of Dean', 0, NULL, 1),
    (2, N'Institution Details', N'Administrative Section', N'Members of Governing Body or Council', 0, NULL, 1),
    (2, N'UG Course Details', N'Previous Details', N'Previous Year Notification', 0, NULL, 1),
    (2, N'UG Course Details', N'Year of Obtaining EC & FC from Government of Karnataka', N'GOK Order', 0, NULL, 1),
    (2, N'UG Course Details', N'Year of Last Affiliation Granted by RGUHS', N'RGUHS Notification', 0, NULL, 1),
    (1, N'UG Course Details', N'Previous Details', N'Previous Year Notification', 0, NULL, 1),
    (1, N'UG Course Details', N'Year of Obtaining EC & FC from Government of Karnataka', N'GOK Order', 0, NULL, 1),
    (1, N'UG Course Details', N'Year of Last Affiliation Granted by RGUHS', N'RGUHS Notification', 0, NULL, 1),
    (1, N'Land And Building Details', N'Land Details', N'Land Records', 0, NULL, 1),
    (1, N'Land And Building Details', N'Building Details', N'Approved Building Plan', 0, NULL, 1),
    (1, N'Department Offices and Educational Unit', N'Medical Education Unit', N'MEU Members List', 0, NULL, 1),
    (2, N'Department Offices and Educational Unit', N'Dental Education Unit', N'DEU Members List', 0, NULL, 1),
    (1, N'Hostel Details', N'Hostel Information', N'Possession Proof', 0, NULL, 1),
    (1, N'Finance Details', N'Account & Audit Details - UG', N'Governing Council Document', 0, NULL, 1),
    (1, N'Finance Details', N'Account & Audit Details - UG', N'Account Summary', 0, NULL, 1),
    (1, N'Finance Details', N'Account & Audit Details - UG', N'Audited Statement', 0, NULL, 1),
    (1, N'Staff Other Details', N'Staff Other Details', N'Form 16 Details', 0, NULL, 1),
    (1, N'Staff Other Details', N'Staff Other Details', N'AEBAS – Last Three Months', 0, NULL, 1),
    (1, N'Staff Other Details', N'Staff Other Details', N'AEBAS – Inspection Day', 0, NULL, 1),
    (1, N'Staff Other Details', N'Staff Other Details', N'Provident Fund', 0, NULL, 1),
    (1, N'Staff Other Details', N'Staff Other Details', N'ESI', 0, NULL, 1),
    (1, N'Research and Publications', N'Research and Publication Details', N'Publications Document', 0, NULL, 1),
    (1, N'Research and Publications', N'Research and Publication Details', N'Clinical Trials Document', 0, NULL, 1),
    (1, N'Research and Publications', N'Research and Publication Details', N'Students Projects Document', 0, NULL, 1),
    (1, N'Research and Publications', N'Research and Publication Details', N'Faculty Projects Document', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Anatomy', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Anesthesiology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Biochemistry', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Community Medicine', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Dentistry', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Dermatology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Forensic Medicine and Toxicology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - General Medicine', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - General Surgery', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Microbiology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Obstetrics and Gynecology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Ophthalmology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Orthopedics', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Otorhinolaryngology (ENT)', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Pathology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Pediatrics', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Pharmacology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Physiology', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Psychiatry', 0, NULL, 1),
    (1, N'Research and Publications', N'Department wise Publications', N'Publication - Radiodiagnosis', 0, NULL, 1);

-- Validate that every source row resolves to exactly one tab and one section.
IF EXISTS
(
    SELECT 1
    FROM @DocumentSeed s
    LEFT JOIN dbo.MstTabs t
        ON t.FacultyId = s.FacultyId
       AND LTRIM(RTRIM(t.TabName)) = LTRIM(RTRIM(s.TabName))
    LEFT JOIN dbo.MstSections sec
        ON sec.FacultyId = s.FacultyId
       AND sec.TabId = t.TabId
       AND LTRIM(RTRIM(sec.SectionName)) = LTRIM(RTRIM(s.SectionName))
    GROUP BY s.FacultyId, s.TabName, s.SectionName, s.DocumentName,
             s.IsMandatory, s.DisplayOrder, s.IsActive
    HAVING COUNT(DISTINCT t.TabId) <> 1
        OR COUNT(DISTINCT sec.SectionId) <> 1
)
BEGIN
    SELECT
        s.FacultyId,
        s.TabName,
        s.SectionName,
        s.DocumentName,
        COUNT(DISTINCT t.TabId) AS MatchingTabs,
        COUNT(DISTINCT sec.SectionId) AS MatchingSections
    FROM @DocumentSeed s
    LEFT JOIN dbo.MstTabs t
        ON t.FacultyId = s.FacultyId
       AND LTRIM(RTRIM(t.TabName)) = LTRIM(RTRIM(s.TabName))
    LEFT JOIN dbo.MstSections sec
        ON sec.FacultyId = s.FacultyId
       AND sec.TabId = t.TabId
       AND LTRIM(RTRIM(sec.SectionName)) = LTRIM(RTRIM(s.SectionName))
    GROUP BY s.FacultyId, s.TabName, s.SectionName, s.DocumentName
    HAVING COUNT(DISTINCT t.TabId) <> 1
        OR COUNT(DISTINCT sec.SectionId) <> 1;

    THROW 50001, 'MstDocument migration stopped: one or more Tab/Section mappings are missing or ambiguous. No data was deleted.', 1;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Remove existing documents as requested.
    -- If another table has a foreign key to MstDocument, SQL Server will stop
    -- the delete and the transaction will roll back.
    DELETE FROM dbo.MstDocument;

    INSERT INTO dbo.MstDocument
    (
        FacultyId,
        TabId,
        SectionId,
        DocumentName,
        IsMandatory,
        DisplayOrder,
        IsActive
    )
    SELECT
        s.FacultyId,
        t.TabId,
        sec.SectionId,
        s.DocumentName,
        s.IsMandatory,
        s.DisplayOrder,
        s.IsActive
    FROM @DocumentSeed s
    INNER JOIN dbo.MstTabs t
        ON t.FacultyId = s.FacultyId
       AND LTRIM(RTRIM(t.TabName)) = LTRIM(RTRIM(s.TabName))
    INNER JOIN dbo.MstSections sec
        ON sec.FacultyId = s.FacultyId
       AND sec.TabId = t.TabId
       AND LTRIM(RTRIM(sec.SectionName)) = LTRIM(RTRIM(s.SectionName));

    DECLARE @InsertedCount INT = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @InsertedCount AS InsertedDocumentCount;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;

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