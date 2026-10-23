
USE Admission_Affiliation;
GO

CREATE OR ALTER PROCEDURE [dbo].[verification_SectionWiseFeedback]
(
	@FacultyId INT,
	@CollegeCode NVARCHAR(50),
	@CourseLevel NVARCHAR(50),
	@TabName NVARCHAR(255),
	@SectionName NVARCHAR(255),
	@VerificationStatus NVARCHAR(50),
	@Remarks NVARCHAR(MAX) = NULL,
	@VerifiedBy NVARCHAR(255) = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @TabId INT;
	DECLARE @SectionId INT;
	DECLARE @SaveStatus NVARCHAR(20);

	BEGIN TRY
		BEGIN TRANSACTION;

		SELECT @TabId = TabId
		FROM dbo.MstTabs
		WHERE TabName = @TabName
			AND FacultyId = @FacultyId

		IF @TabId IS NULL
		BEGIN
			THROW 50001, 'Invalid tab specified.', 1;
		END;

		SELECT @SectionId = SectionId
		FROM dbo.MstSections
		WHERE SectionName = @SectionName AND TabId = @TabId;

		IF @SectionId IS NULL
		BEGIN
			THROW 50002, 'Invalid section specified.', 1;
		END;

		IF EXISTS
		(
			SELECT 1
			FROM dbo.SectionWiseFeedback
			WHERE FacultyId = @FacultyId
				AND CollegeCode = @CollegeCode
				AND CourseLevel = @CourseLevel
				AND TabId = @TabId
				AND SectionId = @SectionId
		)
		BEGIN
			
			UPDATE dbo.SectionWiseFeedback
			SET
				VerificationStatus = @VerificationStatus,
				Remarks = @Remarks,
				VerifiedBy = @VerifiedBy,
				VerifiedOn = GETDATE()
			WHERE FacultyId = @FacultyId
				AND CollegeCode = @CollegeCode
				AND CourseLevel = @CourseLevel
				AND TabId = @TabId
				AND SectionId = @SectionId;

			SET @SaveStatus = 'Updated';

		END
		ELSE
		BEGIN
			
			INSERT INTO dbo.SectionWiseFeedback
			( FacultyId, CollegeCode, CourseLevel, TabId, SectionId, VerificationStatus, Remarks, VerifiedBy, VerifiedOn)
			VALUES
			( @FacultyId, @CollegeCode, @CourseLevel, @TabId, @SectionId, @VerificationStatus, @Remarks, @VerifiedBy, GETDATE());

			SET @SaveStatus = 'Inserted';

		END;

		COMMIT TRANSACTION;

		SELECT 
			@SaveStatus AS SaveStatus,
			@TabId AS TabId,
			@SectionId AS SectionId;

	END TRY
	BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
	END CATCH
END;
GO


---------------------------------

SELECT NAME
FROM sys.procedures
WHERE name = 'verification_SectionWiseFeedback'
------------------------------------------------------

select * from MstTabs;

select * from MstSections
where tabid =2


--EXEC dbo.verification_SectionWiseFeedback
--	@FacultyId = 2,
--	@CollegeCode  = 'D038',
--	@CourseLevel = 'UG',
--	@TabName = 'Institution Details',
--	@SectionName = 'Basic Information',
--	@VerificationStatus = 'Accepted',
--	@Remarks = 'Test feedback',
--	@VerifiedBy = 'Test User';

select * from SectionWiseFeedback

--delete from SectionWiseFeedback