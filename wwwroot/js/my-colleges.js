document.addEventListener("DOMContentLoaded", function () {

    // =====================================================
    // PAGE CONFIGURATION
    // =====================================================

    const page = document.getElementById("myCollegePage");

    if (!page) {
        console.error("myCollegePage element not found.");
        return;
    }

    const statusUrl = page.dataset.statusUrl;

    if (!statusUrl) {
        console.error("Verification status URL not configured.");
        return;
    }

    // =====================================================
    // SEARCH FUNCTIONALITY
    // =====================================================

    const searchInputs = document.querySelectorAll(".college-search");

    searchInputs.forEach(input => {

        input.addEventListener("input", function () {

            const filter = this.value.toUpperCase();

            const container = this.closest(".colleges-container");

            if (!container) return;

            const rows = container.querySelectorAll("tbody tr");

            rows.forEach(row => {

                const match = row.textContent
                    .toUpperCase()
                    .includes(filter);

                row.style.display = match ? "" : "none";
            });
        });
    });

    // =====================================================
    // TOOLTIP INITIALIZATION
    // =====================================================

    if (typeof bootstrap !== "undefined") {

        const tooltipTriggerList = document.querySelectorAll(
            '[data-bs-toggle="tooltip"]'
        );

        tooltipTriggerList.forEach(element => {
            new bootstrap.Tooltip(element);
        });
    }

    // =====================================================
    // LOAD VERIFICATION STATUSES (BULK API)
    // =====================================================

    loadVerificationStatuses();

    async function loadVerificationStatuses() {

        // Group college codes by Faculty ID
        const facultyGroups = {};

        document.querySelectorAll(".verification-status").forEach(element => {

            const facultyId = element.dataset.facultyId;
            const collegeCode = element.dataset.collegeCode;

            if (!facultyId || !collegeCode) {
                return;
            }

            if (!facultyGroups[facultyId]) {
                facultyGroups[facultyId] = [];
            }

            // Avoid duplicate college codes
            if (!facultyGroups[facultyId].includes(collegeCode)) {
                facultyGroups[facultyId].push(collegeCode);
            }
        });

        // One API request per faculty
        for (const facultyId of Object.keys(facultyGroups)) {

            const collegeCodes = facultyGroups[facultyId];

            if (!collegeCodes.length) {
                continue;
            }

            try {

                // Build query parameters
                const params = new URLSearchParams();

                params.append("facultyId", facultyId);

                collegeCodes.forEach(code => {
                    params.append("collegeCodes", code);
                });

                const response = await fetch(`${statusUrl}?${params.toString()}`);

                if (!response.ok) {
                    throw new Error( `Failed to load statuses. HTTP ${response.status}` );
                }

                const result = await response.json();

                if (!result.success || !result.statuses) {
                    throw new Error("Invalid verification status response.");
                }

                updateVerificationStatuses( result.statuses, facultyId );

            }
            catch (error) {

                console.error(
                    `Error loading statuses for faculty ${facultyId}:`,
                    error
                );

                collegeCodes.forEach(collegeCode => {

                    const elements = document.querySelectorAll(
                        `.verification-status[data-college-code="${CSS.escape(collegeCode)}"][data-faculty-id="${CSS.escape(facultyId)}"]`
                    );

                    elements.forEach(element => {

                        element.className = "status-badge pending verification-status";

                        element.innerHTML = `
                            <i class="bi bi-exclamation-circle me-1"></i>
                            <span class="status-text">Unable to Load</span>
                        `;
                    });
                });
            }
        }
    }

    // =====================================================
    // UPDATE VERIFICATION STATUS UI
    // =====================================================

    function updateVerificationStatuses(statuses, facultyId) {

        if (!statuses) return;

        Object.keys(statuses).forEach(collegeCode => {

            const statusData = statuses[collegeCode];

            const elements = document.querySelectorAll(
                `.verification-status[data-college-code="${CSS.escape(collegeCode)}"][data-faculty-id="${CSS.escape(facultyId)}"]`
            );

            elements.forEach(element => {

                const statusText = statusData.status || "Pending";

                let statusClass = "";
                let icon = "";

                switch (statusText.toLowerCase()) {

                    case "accepted":
                    case "completed":
                    case "verified":
                        statusClass = "completed";
                        icon = "bi-check-circle-fill";
                        break;

                    case "rejected":
                        statusClass = "rejected";
                        icon = "bi-x-circle-fill";
                        break;

                    case "not started":
                        statusClass = "pending";
                        icon = "bi-circle";
                        break;

                    case "pending":
                    default:
                        statusClass = "pending";
                        icon = "bi-hourglass-split";
                        break;
                }

                element.className = `status-badge ${statusClass} verification-status`;

                element.innerHTML = `
                    <i class="bi ${icon} me-1"></i>
                    <span class="status-text">${statusText}</span>
                `;

                // Tooltip information
                element.title =
                    `Completed: ${statusData.completedSections ?? 0} / ${statusData.totalSections ?? 0}
                    Pending: ${statusData.pendingSections ?? 0}
                    Rejected: ${statusData.rejectedSections ?? 0}`;
            });
        });
    }

});
