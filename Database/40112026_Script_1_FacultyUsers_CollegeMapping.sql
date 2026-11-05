

------------- PRIMARY KEY FOR FACULTY USER -----------
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
