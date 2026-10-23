

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