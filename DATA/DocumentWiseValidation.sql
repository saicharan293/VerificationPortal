



-- ============================================================
-- TABLE: MstDocument
-- PURPOSE:
-- Stores the master list of documents required for verification.
--
-- Each document can be mapped to:
-- 1. A Faculty
-- 2. A Tab
-- 3. Optionally, a Section within that Tab
-- ============================================================

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



-- ============================================================
-- TABLE: DocumentWiseFeedback
-- PURPOSE:
-- Stores document-wise verification feedback provided by
-- authorized RGUHS faculty users for a specific college,
-- faculty, and document.
-- ============================================================


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
        REFERENCES TblRguhsFacultyUser(Id)
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