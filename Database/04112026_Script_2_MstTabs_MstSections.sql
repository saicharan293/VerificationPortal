

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


/* =======================================================
    MST TAB INSERT QUERIES
    ====================================================== */



--    SELECT
--    'INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ' +
--    '([FacultyId], [TabName]) VALUES (' +
--    CAST([FacultyId] AS VARCHAR(20)) + ', ' +
--    '''' + REPLACE([TabName], '''', '''''') + ''');'
--    AS InsertQuery
--FROM [Admission_Affiliation].[dbo].[MstTabs]
--ORDER BY FacultyId, TabId;

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
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Clinical Facilities');
INSERT INTO [Admission_Affiliation].[dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Associated Institutions');
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

/* =======================================================
    MST SECTION INSERT QUERIES
    ====================================================== */

--    SELECT
--    'INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ' +
--    '([FacultyId], [TabId], [SectionName]) VALUES (' +
--    CAST([FacultyId] AS VARCHAR(20)) + ', ' +
--    CAST([TabId] AS VARCHAR(20)) + ', ' +
--    '''' + REPLACE([SectionName], '''', '''''') + '''' +
--    ');' AS InsertQuery
--FROM [Admission_Affiliation].[dbo].[MstSections]
--ORDER BY [SectionId];

INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Basic Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Address Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Contact Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Institutional Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Head of Institution');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Nodal Officer Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Principal Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Dean or Director Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 1, 'Trust Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 2, 'Trust Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 3, 'Trust Members');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 4, 'Dean or Director Information');
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
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 23, 'Research and Publication Details');
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
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Basic Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Address Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Contact Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Institutional Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Head of Institution');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Nodal Officer Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Principal Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Dean or Director Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Trust Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 38, 'Trust Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 39, 'Trust Members');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 40, 'Dean or Director Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 40, 'Teaching Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 40, 'Administrative Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 41, 'Principal Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 41, 'Teaching Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 41, 'Administrative Experience');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Present Intake');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Previous Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Particulars of Permission');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Year of Obtaining EC & FC from Government of Karnataka');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Year of Last Affiliation Granted by RGUHS');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Previous Inspection by LIC');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Land Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Building Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Area Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Facilities Availability');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Staff Quarters');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Workshop Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Animal House Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Small Group Teaching General');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Teaching Rooms');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Laboratory Availability');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 43, 'Museum Availability');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 44, 'Facility Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 45, 'Beds Distribution');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 46, 'Equipment Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 47, 'Department Office Infrastructure');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 47, 'Medical Education Unit');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 48, 'Hostel Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 48, 'Student Accommodation');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 48, 'Common Room Facilities');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 48, 'Hostel Facilities');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 49, 'Vehicle Information');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 50, 'UG Academic Performance');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 51, 'Authority & Account Details - UG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 51, 'Annual Financial Details - UG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 51, 'Account & Audit Details - UG');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 52, 'Staff Pay Scale');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 53, 'Staff Other Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 54, 'Department Library Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 54, 'Library Services');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 54, 'Library Staff Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 54, 'Digital Valuation Centre');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 54, 'Special Features');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 55, 'Research and Publication Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 55, 'Department wise Publications');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 56, 'General Library Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 56, 'Library Items');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 56, 'Library Building');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 56, 'Technical Processes');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 56, 'Library Equipment');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 56, 'Library Finance');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 58, 'Non Teaching Staff Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (2, 57, 'Non Teaching Staff Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 59, 'Payment Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'Blood Bank');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'Classrooms Photo');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'Clinical Wards');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'College Building Photo');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'Faculty Pics');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'Attached Hospital');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'ICU');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 60, 'Labs');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Indoor Department Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Operation Theater Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Casualty Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'CSSD and Laundry Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Radio-Diagnosis Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Anaesthesiology Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Central Laboratory Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Blood Bank Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Yoga Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Radiation Oncology Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Art Center Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Pharmacy Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Utilities Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Indoor Beds Units Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Out Patient Area Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Clinical Hospital Details');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Clinical Statistics');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Affiliated Hospital Documents');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'OT Requirements');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Supervision in FPA');
INSERT INTO [Admission_Affiliation].[dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 61, 'Indoor Beds Requirements');





--  SELECT
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


INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (2, 1, 1, 'Government Autonomous Certificate', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 37, 80, 'Government Autonomous Certificate', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (2, 1, 8, 'Appointment Order of Dean', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 37, 87, 'Appointment Order of Dean', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (2, 1, 63, 'Members of Governing Body or Council', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (2, 6, 19, 'Previous Year Notification', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (2, 6, 21, 'GOK Order', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (2, 6, 22, 'RGUHS Notification', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 42, 98, 'Previous Year Notification', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 42, 100, 'GOK Order', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 42, 101, 'RGUHS Notification', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 43, 103, 'Land Records', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 43, 104, 'Approved Building Plan', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 47, 118, 'MEU Members List', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (2, 14, 35, 'DEU Members List', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 48, 119, 'Possession Proof', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 51, 127, 'Governing Council Document', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 51, 127, 'Account Summary', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 51, 127, 'Audited Statement', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 53, 129, 'Form 16 Details', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 53, 129, 'AEBAS – Last Three Months', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 53, 129, 'AEBAS – Inspection Day', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 53, 129, 'Provident Fund', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 53, 129, 'ESI', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 135, 'Publications Document', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 135, 'Clinical Trials Document', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 135, 'Students Projects Document', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 135, 'Faculty Projects Document', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Anatomy', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Anesthesiology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Biochemistry', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Community Medicine', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Dentistry', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Dermatology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Forensic Medicine and Toxicology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - General Medicine', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - General Surgery', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Microbiology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Obstetrics and Gynecology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Ophthalmology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Orthopedics', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Otorhinolaryngology (ENT)', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Pathology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Pediatrics', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Pharmacology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Physiology', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Psychiatry', 0, NULL, 1);
INSERT INTO [Admission_Affiliation].[dbo].[MstDocument] ( [FacultyId], [TabId], [SectionId], [DocumentName], [IsMandatory], [DisplayOrder], [IsActive]) VALUES (1, 55, 136, 'Publication - Radiodiagnosis', 0, NULL, 1);

