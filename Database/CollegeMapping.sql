-- ============================================
-- Insert Faculty Users for DENTAL (Faculty = 2)
-- ============================================
-- Password: "Dental@2026" for all users
-- PasswordHash: Use BCrypt to generate actual hash

BEGIN TRANSACTION;

----- INSERT QUERY FOR COMMITTEE NAMES FOR DENTAL FACULTY ----------

INSERT INTO CA_MST_Med_CommitteeNames
(
    CommitteeName,
    FacultyCode,
    SubFacultyCode,
    CourseLevel
)
VALUES
('Academic council details',        2, NULL, 'ALL'),
('Anti-ragging committee',          2, NULL, 'ALL'),
('Gender harassment committee',     2, NULL, 'ALL'),
('Institutional ethical committee', 2, NULL, 'ALL'),
('POSH Committee',                  2, NULL, 'ALL'),
('Pharmacovigilance Committee',     2, NULL, 'UG'),
('Curriculum Committee',            2, NULL, 'UG'),
('IACE (Institutional Animal Ethics Committee)', 2, NULL, 'ALL');

-- 1. Data Entry Operator (DEO)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (1, 'DENTAL_DEO', 'DentalDEO@2026', '$2y$10$ay08DD3FqFHUyZqw/SE6dOorSppfLzTMZdd4je1kksHf6HxWuKYfW', 
     1, 2, 0, NULL, 'Data Entry Operator', 0, 0, 0, NULL);

-- 2. Junior Assistant (JR)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (2, 'DENTAL_JR', 'DentalJR@2026', '$2y$10$Vb/o0SOyv.X2dWmFZKI7kus/AqBtOOnIfxf5BHCfntsA3NQCvc/.K', 
     1, 2, 0, NULL, 'Junior Assistant', 0, 0, 0, NULL);

-- 3. Section Officer (SO)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (3, 'DENTAL_SO', 'DentalSO@2026', '$2y$10$68SypNpAYeJveDDtR9A.ZefTlq4vp4Nm6Smbh/vfGwG1MV0klIU5y', 
     1, 2, 0, NULL, 'Section Officer', 1, 0, 0, NULL);

-- 4. Assistant Registrar (AR)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (4, 'DENTAL_AR', 'DentalAR@2026', '$2y$10$EM4KacCJx4VFEgD.88.hO.W0cL15tsr.obOEkwYKjtu7.SmzVJLEu', 
     1, 2, 0, NULL, 'Assistant Registrar', 0, 0, 0, NULL);

-- 5. Registrar (RG)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (5, 'DENTAL_RG', 'DentalRG@2026', '$2y$10$Dvyp7iYXiy3hHfyzrghTtOpq5t9sEL/utd6gSqT7AWuAmqSCDq6oy', 
     1, 2, 0, NULL, 'Registrar', 0, 0, 0, NULL);

-- 6. Registrar Evaluation (RE)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (6, 'DENTAL_RE', 'DentalRE@2026', '$2y$10$2QhIaIi/BH6RM.RUmKGC4uebNLQKVN42pWSDlVGjCREz2G7qjiIMW', 
     1, 2, 0, NULL, 'Registrar Evaluation', 0, 0, 0, NULL);

-- 7. Director (DR)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (7, 'DENTAL_DR', 'DentalDR@2026', '$2y$10$QH2iUDvmoniP7TRBry4GluosxVHrewtD4bH255zf1N6IbjS9cj1xK', 
     1, 2, 0, NULL, 'Director', 0, 1, 0, NULL);

-- 8. Vice Chancellor (VC)
INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (8, 'DENTAL_VC', 'DentalVC@2026', '$2y$10$r2LCi8ut3oHNUYLF7XfDcuJtN1ANhhfCp740xOlhKqDmoZzJLoqVe', 
     1, 2, 0, NULL, 'Vice Chancellor', 0, 1, 0, NULL);



-- ============================================
-- Insert ADMIN User
-- ============================================
-- This admin user has NO faculty assignment
-- and has full system access (IsAdmin = 1)

INSERT INTO TblRguhsFacultyUser 
    (UserId, UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, 
     FinanceDesignation, DesignationDescription, IsSection, IsAdmin, 
     FailedLoginAttempts, LockoutEndTime)
VALUES 
    (0, 'ADMIN', 'Admin@2026', '$2y$10$l6rdWrtRY4G654O3TPIRROQ9M6Y.NsaMbszy1axC8iMWhkkGiBPUe', 
     1, NULL, 0, NULL, 'System Administrator', 1, 1, 0, NULL);


COMMIT;

-- ============================================
-- Verify Inserted Data
-- ============================================
SELECT 
    Id, UserId, UserName, IsActive, Faculty, 
    DesignationDescription, IsSection, IsAdmin
FROM TblRguhsFacultyUser 
WHERE Faculty = 2 
ORDER BY UserId;

SELECT * FROM [dbo].[TblRguhsFacultyUser]
  where Faculty=2


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


SELECT * FROM Affiliation_College_Master
WHERE FacultyCode=2 order by CollegeName desc

select * from [dbo].[AFF_InstitutionsDetails]
where facultycode = 2 and CollegeCode='d038'

ALTER TABLE AFF_InstitutionsDetails
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG (Registrar)
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE (Registrar Evaluation / Relevant Role)
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR (Director)
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC (Vice Chancellor)
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL;


ALTER TABLE InstitutionBasicDetails
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,a

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;




ALTER TABLE CONTINUATION_TrustMemberDetails
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


ALTER TABLE [Aff_DeanOrDirectorDetails]
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


ALTER TABLE Aff_PrincipalDetails
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


ALTER TABLE [dbo].[Affiliation_CourseDetails]
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


ALTER TABLE [dbo].[Affiliation_PgSsCourseDetails]
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


ALTER TABLE [dbo].[DentalInfrastructure]
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


ALTER TABLE [dbo].[DentalCollegeLandBuildingDetail]
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


ALTER TABLE [dbo].[DentalChairs]
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;



ALTER TABLE [dbo].[Medical_UGBedDistribution]
ADD
    -- DEO
    IsDeoVerified BIT NULL,
    DeoRemarks NVARCHAR(1000) NULL,
    DeoVerifiedDate DATETIME NULL,
    DeoName NVARCHAR(200) NULL,

    -- JR
    IsJrVerified BIT NULL,
    JrRemarks NVARCHAR(1000) NULL,
    JrVerifiedDate DATETIME NULL,
    JrName NVARCHAR(200) NULL,

    -- SO
    IsSoVerified BIT NULL,
    SoRemarks NVARCHAR(1000) NULL,
    SoVerifiedDate DATETIME NULL,
    SoName NVARCHAR(200) NULL,

    -- AR
    IsArVerified BIT NULL,
    ArRemarks NVARCHAR(1000) NULL,
    ArVerifiedDate DATETIME NULL,
    ArName NVARCHAR(200) NULL,

    -- RG
    IsRgVerified BIT NULL,
    RgRemarks NVARCHAR(1000) NULL,
    RgVerifiedDate DATETIME NULL,
    RgName NVARCHAR(200) NULL,

    -- RE
    IsReVerified BIT NULL,
    ReRemarks NVARCHAR(1000) NULL,
    ReVerifiedDate DATETIME NULL,
    ReName NVARCHAR(200) NULL,

    -- DR
    IsDrVerified BIT NULL,
    DrRemarks NVARCHAR(1000) NULL,
    DrVerifiedDate DATETIME NULL,
    DrName NVARCHAR(200) NULL,

    -- VC
    IsVcVerified BIT NULL,
    VcRemarks NVARCHAR(1000) NULL,
    VcVerifiedDate DATETIME NULL,
    VcName NVARCHAR(200) NULL,

    -- Overall Workflow
    CurrentVerificationLevel NVARCHAR(50) NULL,
    OverallStatus NVARCHAR(30) NULL,
    LastUpdatedDate DATETIME NULL;


select * from TblRguhsFacultyUser;

SELECT * FROM [dbo].[AFF_InstitutionsDetails] WHERE CollegeCode='D008';

SELECT * FROM Affiliation_College_Master
WHERE FacultyCode=2
WHERE CollegeCode = 'D008';

SELECT * FROM CollegeCourseIntakeDetails
WHERE FacultyCode=2;

SELECT * FROM AcademicIntake
WHERE FacultyCode=2 and CollegeCode = 'D008';

SELECT * FROM InstitutionBasicDetails
WHERE CollegeCode = 'DO38';


select * from Aff_DeanOrDirectorDetails
where CollegeCode = 'd038';

SELECT * FROM Aff_PrincipalDetails
WHERE CollegeCode = 'D038';

select * from [dbo].[Affiliation_CourseDetails]
where facultycode = 2 and Collegecode = 'd038';

SELECT * FROM [dbo].[CA_Progress]
WHERE CollegeCode = 'd007' and StepKey like '%veh%' and courselevel like '%UG%'

SELECT * FROM [dbo].[CA_Med_LibraryEquipments]
WHERE FacultyCode = 2 AND CollegeCode = 'D008'

SELECT * FROM CA_MST_Med_LibraryEquipments;

SELECT * FROM [dbo].[CA_MST_Med_CommitteeNames]

SELECT * FROM CA_ExaminationScheme

SELECT * FROM HospitalDetailsForAffiliation
where FacultyCode= 2 and CollegeCode = 'd037';


SELECT* FROM DentalWardBedDistribution
WHERE CollegeCode = 'D038'

select * from Medical_UGBedDistribution
where collegecode = 'd038'

SELECT * FROM MstDentalBedDistribution
where SeatSlab = 50;

SELECT * FROM DentalInfrastructure;

select * from MstDentalInfrastructure;

SELECT * FROM Medical_DepartmentOfficesMeu
WHERE FacultyCode=2 AND CollegeCode='D038';


SELECT * FROM DesignationMaster
WHERE FacultyCode = 2

SELECT * FROM [dbo].[FacultyDetails]
where FacultyCode = 2 and CollegeCode='d038'

SELECT * FROM DepartmentMaster
WHERE FacultyCode=2