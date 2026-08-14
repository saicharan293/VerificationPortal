(function () {

    document.addEventListener("click", function (event) {

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

        if (!url) {
            console.error("Document URL is missing.");
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


        // -------------------------------------------------
        // SHOW MODAL
        // -------------------------------------------------

        const modal =
            bootstrap.Modal.getOrCreateInstance(
                modalElement
            );

        modal.show();

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

        }
    );

})();