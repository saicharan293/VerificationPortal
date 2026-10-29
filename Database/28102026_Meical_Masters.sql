

------------------- MEDICAL USERS ---------------

INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_DEO', 'MedicalDEO@2026', '$2a$12$0V4Ha7UK.raNCyKjbeKxhe2y6PcF.r3ow/REahmsXnuu35WjYp0tC', 1, 1, 0, NULL, 'Data Entry Operator', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_JR', 'MedicalJR@2026', '$2a$12$yWy1Py26URsmB4iNINtSC.OW3.1aK.XEPwGOk8MqaMzeX.B9Lscze', 1, 1, 0, NULL, 'Junior Assistant', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_SO', 'MedicalSO@2026', '$2a$12$AsPHU4NBYz3Lh6gxcjpbX.VXeUYa7l5GQIOa.ke1h8x8f59kO3ebC', 1, 1, 0, NULL, 'Section Officer', 1, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_AR', 'MedicalAR@2026', '$2a$12$auLUmk5SyrUt052d5JlAzOsbzpIjtTlACSA5WUtIq0o716USIzoWa', 1, 1, 0, NULL, 'Assistant Registrar', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_RG', 'MedicalRG@2026', '$2a$12$K1eNGE8J0yjtyZo21ylBf.Uz.Ms2VNw799IuVfNPNCi9zSj.Uk8MO', 1, 1, 0, NULL, 'Registrar', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_RE', 'MedicalRE@2026', '$2a$12$tjqzdNW26Iw9fGe.xQUVp.39I5qRJe1drjnro/y7oB7bzjzklKhAq', 1, 1, 0, NULL, 'Registrar Evaluation', 0, 0, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_DR', 'MedicalDR@2026', '$2a$12$ea5YUfN5lXdLyjs/nhu04uhk28kB5hZLmjV9nLbQtERjXoZYSnEz.', 1, 1, 0, NULL, 'Director', 0, 1, 0, NULL);
																																																				   
INSERT INTO dbo.TblRguhsFacultyUser (UserName, Password, PasswordHash, IsActive, Faculty, IsFinance, FinanceDesignation, DesignationDescription, IsSection, IsAdmin, FailedLoginAttempts, LockoutEndTime) VALUES ('MEDICAL_VC', 'MedicalVC@2026', '$2a$12$WBsxyo4qvt8DVjG4gbYAe.Zj08II5LZsXH7Lcfpbc90cJKq6rgFi6', 1, 1, 0, NULL, 'Vice Chancellor', 0, 1, 0, NULL);


----------------------------------

----------------------------MST MEDICAL TABS ---------------------------
INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Institution Details');
INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Trust Details');
INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Trust Member Details');
INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Dean / Director Details');
INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Principal Details');
INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'UG Course Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'PG Course Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Courses & Intake');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Land & Building Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Classroom & Laboratory');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Chair Distribution');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Bed Distribution');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Hostel Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Department Offices And Educational Unit');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Equipment List');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Vehicle Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'UG Academic Matters');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'PG Academic Matters');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Staff Pay Scale');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Staff Other Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Finance Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Library Services');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Research & Publications');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Library Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Faculty Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Teaching Experience');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Clinical Facilities');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Workshop Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Animal House Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Field Practice Area');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Library Expenditure and Services');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Library Staff Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Library User Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Payment Details');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Action Taken Report');
--INSERT INTO [dbo].[MstTabs] ([FacultyId], [TabName]) VALUES (1, 'Faculty Repository');


---------------- MST MEDICAL SECTIONS --------------------------

INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Basic Information');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Address Details');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Contact Information');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Institutional Details');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Head of Institution');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Nodal Officer Details');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Principal Details');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Dean / Director Details');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 37, 'Trust Details');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 38, 'Trust Information');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 39, 'Trust Members');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 40, 'Dean / Director Information');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 40, 'Teaching Experience');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 40, 'Administrative Experience');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 41, 'Principal Information');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 41, 'Teaching Experience');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 41, 'Administrative Experience');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Present Intake');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Previous Details');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Particulars of Permission');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Year of Obtaining EC & FC from Government of Karnataka');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Year of Last Affiliation Granted by RGUHS');
INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 42, 'Previous Inspection by LIC');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 7, 'PG Course Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 9, 'Land Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 9, 'Building Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 10, 'Facility Requirements');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 11, 'Chair Distribution Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 12, 'Beds Distribution');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 13, 'Hostel Information');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 13, 'Student Accommodation');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 13, 'Common Room Facilities');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 13, 'Hostel Facilities');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 14, 'Department Office Infrastructure');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 14, 'Dental Education Unit');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 15, 'Equipment Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 16, 'Vehicle Information');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 17, 'UG Academic Performance');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 18, 'PG Academic Performance');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 19, 'Staff Pay Scale');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 20, 'Staff Other Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 21, 'Authority & Account Details - UG');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 21, 'Annual Financial Details - UG');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 21, 'Account & Audit Details - UG');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 21, 'Authority & Account Details - PG');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 21, 'Annual Financial Details - PG');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 21, 'Account & Audit Details - PG');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 22, 'Department Library Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 23, 'Research & Publication Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 24, 'General Library Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 24, 'Library Items');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 24, 'Library Building');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 24, 'Technical Processes');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 24, 'Library Equipment');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 24, 'Library Finance');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 25, 'Faculty Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Clinical Hospital Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Clinical Statistics');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Discipline Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Nursing, Paramedical, Technical & Allied Services');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Engineering & Allied Services');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Ward-wise Bed Distribution in Attached Hospital');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 1, 'Administrative Section');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 1, 'Managing Other Health Science Colleges');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 28, 'Workshop Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 29, 'Animal House Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 30, 'Rural Field Practice Area');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 30, 'Urban Field Practice Area');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 31, 'Library services availability');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 31, 'Library Expenditure');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 32, 'Library Staff Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 33, 'Library User Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 34, 'Payment Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 34, 'Payment Receipt Details');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 35, 'Action Taken Report');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 36, 'Faculty Repository');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Certificates and Future Development Documents');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Anatomy Act Registration');
--INSERT INTO [dbo].[MstSections] ([FacultyId], [TabId], [SectionName]) VALUES (1, 27, 'Hospital Tie up');