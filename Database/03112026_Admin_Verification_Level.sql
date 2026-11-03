

CREATE TABLE TblVerificationLevel
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    VerificationLevel INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);

-- Prevent assigning the same user to the same level twice
CREATE UNIQUE NONCLUSTERED INDEX UX_VerificationLevel_UserId
ON TblVerificationLevel(UserId);

-- Quickly retrieve active users by verification level
CREATE NONCLUSTERED INDEX IX_VerificationLevel_Level_Active
ON TblVerificationLevel(VerificationLevel, IsActive)
INCLUDE (UserId);



--------------------------------------------

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