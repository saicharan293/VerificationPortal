CREATE TABLE TblAllCollegesMapping
(
    Id INT IDENTITY(1,1) PRIMARY KEY,

    SelectedId INT NULL,

    FromCollegeCode NVARCHAR(50) NOT NULL,
    ToCollegeCode NVARCHAR(50) NOT NULL,

    FromLetter NVARCHAR(10) NOT NULL,
    ToLetter NVARCHAR(10) NOT NULL,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedDate DATETIME NULL,
    CreatedBy NVARCHAR(100) NULL,

    ModifiedDate DATETIME NULL,
    ModifiedBy NVARCHAR(100) NULL
);

ALTER TABLE Tbl_CollegeMapping
ADD AllCollegesMappingId INT NULL;


ALTER TABLE [dbo].[Tbl_CollegeMapping]
ADD CONSTRAINT FK_Tbl_CollegeMapping_TblAllCollegesMapping
FOREIGN KEY (AllCollegesMappingId)
REFERENCES TblAllCollegesMapping(Id);

--EXEC sp_helpindex 'Tbl_CollegeMapping';

--DROP INDEX IX_CollegeMapping_Level_Active
--ON Tbl_CollegeMapping;

--ALTER TABLE Tbl_CollegeMapping
--DROP CONSTRAINT DF__Tbl_Colle__ToLet__2799C73C;

--ALTER TABLE Tbl_CollegeMapping
--DROP COLUMN
--    CollegeFrom,
--    CollegeTo,
--    FromLetter,
--    ToLetter;
