(function () {


    // ====================================================
    // GET EXISTING DOCUMENT FEEDBACK
    // ====================================================

    async function loadDocumentFeedback(documentId, facultyId, collegeCode) {
        const feedbackStatus = document.getElementById("feedbackStatus");

        const feedbackText = document.getElementById("feedbackText");

        if (!documentId || !facultyId || !collegeCode) {
            console.warn("Unable to load feedback. Missing document context.");
            return;
        }

        try {
            const params = new URLSearchParams({
                documentId: documentId,
                facultyId: facultyId,
                collegeCode: collegeCode
            });

            const response = await fetch(`@Url.Action("GetDocumentFeedback","DocumentManager")?${params.toString()}`);

            if (!response.ok) throw new Error("Unable to load document feedback.");

            const result = await response.json();

            if (result.exists) {
                if (feedbackStatus) feedbackStatus.value = result.status || "";

                if (feedbackText) feedbackText.value = result.feedback || "";
            }
        }
        catch (error) {
            console.error("Error loading document feedback: ", error);
        }
    }

    document.addEventListener("click", async function (event) {

        const button =
            event.target.closest(".document-viewer-btn");

        if (!button) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();

        const url =
            button.getAttribute("data-document-url");

        const title =
            button.getAttribute("data-document-title")
            || "Document Viewer";

        const documentId = button.getAttribute("data-document-id");
        const facultyId = button.getAttribute("data-faculty-id");

        const collegeCode = button.getAttribute("data-college-code");

        if (!url) {
            console.error("Document URL is missing.");
            return;
        }

        // -------------------------------------------------
        // VALIDATE DOCUMENT ID
        // -------------------------------------------------

        if (!documentId) {
            console.error("Document ID is missing.");

            return;
        }

        const modalElement =
            document.getElementById("globalDocumentViewerModal");

        const iframe =
            document.getElementById("globalDocumentViewerFrame");

        const titleElement =
            document.getElementById("globalDocumentViewerTitle");

        const loading =
            document.getElementById("documentViewerLoading");

        const error =
            document.getElementById("documentViewerError");


        // -------------------------------------------------
        // GET DOCUMENT FEEDBACK ELEMENTS
        // -------------------------------------------------

        const feedbackDocumentId = document.getElementById("feedbackDocumentId");
        const feedbackFacultyId = document.getElementById("feedbackFacultyId");
        const feedbackCollegeCode = document.getElementById("feedbackCollegeCode");
        const feedbackStatus = document.getElementById("feedbackStatus");
        const feedbackText = document.getElementById("feedbackText");


        console.log("Document viewer clicked");
        console.log("Document URL:", url);
        console.log("Document ID:", documentId);
        console.log("Faculty ID:", facultyId);
        console.log("College Code:", collegeCode);


        // -------------------------------------------------
        // VALIDATE REQUIRED ELEMENTS
        // -------------------------------------------------

        if (!modalElement) {
            console.error(
                "Global document viewer modal not found."
            );
            return;
        }

        if (!iframe) {
            console.error(
                "Global document viewer iframe not found."
            );
            return;
        }


        // -------------------------------------------------
        // RESET
        // -------------------------------------------------

        iframe.style.display = "none";
        iframe.src = "";

        if (loading) {
            loading.classList.remove("d-none");
            loading.classList.add("d-flex");
        }

        if (error) {
            error.classList.remove("d-flex");
            error.classList.add("d-none");
        }


        // -------------------------------------------------
        // TITLE
        // -------------------------------------------------

        if (titleElement) {

            titleElement.innerHTML =
                '<i class="bi bi-file-earmark-text me-2"></i>' +
                title;
        }

        // -------------------------------------------------
        // SET FEEDBACK CONTEXT
        // -------------------------------------------------

        if (feedbackDocumentId) feedbackDocumentId.value = documentId || "";

        if (feedbackFacultyId) feedbackFacultyId.value = facultyId || "";

        if (feedbackCollegeCode) feedbackCollegeCode.value = collegeCode || "";

        if (feedbackStatus) feedbackStatus.value = "";

        if (feedbackText) feedbackText.value = "";



        // -------------------------------------------------
        // SHOW MODAL
        // -------------------------------------------------

        const modal =
            bootstrap.Modal.getOrCreateInstance(
                modalElement
            );

        modal.show();

        // -------------------------------------------------
        // LOAD EXISTING FEEDBACK
        //
        // This runs asynchronously and does not block the
        // document viewer.
        // -------------------------------------------------

        loadDocumentFeedback(
            documentId,
            facultyId,
            collegeCode
        );

        // -------------------------------------------------
        // LOAD DOCUMENT
        // -------------------------------------------------

        iframe.onload = function () {

            if (loading) {
                loading.classList.remove("d-flex");
                loading.classList.add("d-none");
            }

            if (error) {
                error.classList.remove("d-flex");
                error.classList.add("d-none");
            }

            iframe.style.display = "block";
        };


        iframe.onerror = function () {

            if (loading) {
                loading.classList.remove("d-flex");
                loading.classList.add("d-none");
            }

            iframe.style.display = "none";

            if (error) {
                error.classList.remove("d-none");
                error.classList.add("d-flex");
            }
        };


        // IMPORTANT:
        // This causes the browser to request
        // ViewLandBuildingDocument
        iframe.src = url;



    });


    // -----------------------------------------------------
    // CLEAR IFRAME WHEN MODAL CLOSES
    // -----------------------------------------------------

    document.addEventListener(
        "hidden.bs.modal",
        function (event) {

            if (
                event.target.id !==
                "globalDocumentViewerModal"
            ) {
                return;
            }

            const iframe =
                document.getElementById(
                    "globalDocumentViewerFrame"
                );

            if (iframe) {

                iframe.src = "";
                iframe.style.display = "none";

            }


            // FEEDBACK FORM

            const feedbackStatus = document.getElementById("feedbackStatus");
            const feedbackText = document.getElementById("feedbackText");

            const feedbackDocumentId = document.getElementById( "feedbackDocumentId");
            const feedbackFacultyId = document.getElementById("feedbackFacultyId");

            if (feedbackStatus) feedbackStatus.value = "";
            if (feedbackDocumentId) feedbackDocumentId.value = "";
            if (feedbackText) feedbackText.value = "";
            if (feedbackFacultyId) feedbackFacultyId.value = "";


        }
    );

})();