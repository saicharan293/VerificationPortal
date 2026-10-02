

document.addEventListener("DOMContentLoaded", function () {
    // Add search/filter functionality for college tables
    const searchInputs = document.querySelectorAll('.college-search');

    searchInputs.forEach(input => {
        input.addEventListener('keyup', function () {
            const filter = this.value.toUpperCase();
            const table = this.closest('.colleges-container').querySelector('table');
            const rows = table.querySelectorAll('tbody tr');

            rows.forEach(row => {
                const cells = row.querySelectorAll('td');
                let match = false;
                cells.forEach(cell => {
                    if (cell.textContent.toUpperCase().includes(filter)) {
                        match = true;
                    }
                });
                row.style.display = match ? '' : 'none';
            });
        });
    });

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // =====================================================
    // LOAD VERIFICATION STATUSES
    // =====================================================
    const page = document.getElementById("myCollegesPage");
    const statusUrl = page.dataset.statusUrl;

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

            // Prevent duplicate college codes
            if (!facultyGroups[facultyId].includes(collegeCode)) {
                facultyGroups[facultyId].push(collegeCode);
            }
        });

        // Call API separately for each faculty
        for (const facultyId in facultyGroups) {

            const collegeCodes = facultyGroups[facultyId];

            if (!collegeCodes || collegeCodes.length === 0) {
                continue;
            }

            try {

                // Build query string
                const params = new URLSearchParams();

                params.append("facultyId", facultyId);

                collegeCodes.forEach(code => {
                    params.append("collegeCodes", code);
                });

                const response = await fetch(`${statusUrl}?${params.toString()}`);

                if (!response.ok) {
                    throw new Error("Failed to load verification statuses.");
                }

                const statuses = await response.json();

                updateVerificationStatuses(statuses);

            }
            catch (error) {

                console.error(
                    "Error loading verification statuses:",
                    error
                );

                // Show error state
                collegeCodes.forEach(collegeCode => {

                    const elements = document.querySelectorAll(
                        `.verification-status[data-college-code="${collegeCode}"][data-faculty-id="${facultyId}"]`
                    );

                    elements.forEach(element => {

                        element.className =
                            "status-badge pending verification-status";

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
    // UPDATE STATUS UI
    // =====================================================

    function updateVerificationStatuses(statuses) {

        if (!statuses) {
            return;
        }

        Object.keys(statuses).forEach(collegeCode => {

            const statusData = statuses[collegeCode];

            const elements = document.querySelectorAll(
                `.verification-status[data-college-code="${collegeCode}"]`
            );

            elements.forEach(element => {

                let statusClass = "";
                let icon = "";
                let statusText = statusData.status || "Pending";

                switch (statusText.toLowerCase()) {

                    case "completed":
                    case "verified":
                        statusClass = "completed";
                        icon = "bi-check-circle-fill";
                        break;

                    case "rejected":
                        statusClass = "rejected";
                        icon = "bi-x-circle-fill";
                        break;

                    case "pending":
                    default:
                        statusClass = "pending";
                        icon = "bi-hourglass-split";
                        statusText = "Pending";
                        break;
                }

                element.className =
                    "status-badge " +
                    statusClass +
                    " verification-status";

                element.innerHTML = `
                        <i class="bi ${icon} me-1"></i>
                        <span class="status-text">${statusText}</span>
                    `;

                // Optional tooltip with section counts
                element.title =
                    `Completed: ${statusData.completedSections ?? 0} / ${statusData.totalSections ?? 0}
                    Pending: ${statusData.pendingSections ?? 0}
                    Rejected: ${statusData.rejectedSections ?? 0}`;
            });
        });
    }


    // =====================================================
    // SEARCH FUNCTIONALITY
    // =====================================================

    // const searchInputs =
    //     document.querySelectorAll('.college-search');

    searchInputs.forEach(input => {

        input.addEventListener('keyup', function() {

            const filter =
                this.value.toUpperCase();

            const table =
                this.closest('.colleges-container')
                    .querySelector('table');

            const rows =
                table.querySelectorAll('tbody tr');

            rows.forEach(row => {

                const cells =
                    row.querySelectorAll('td');

                let match = false;

                cells.forEach(cell => {

                    if (
                        cell.textContent
                            .toUpperCase()
                            .includes(filter)
                    ) {
                        match = true;
                    }
                });

                row.style.display =
                    match ? '' : 'none';
            });
        });
    });


    // =====================================================
    // TOOLTIP INITIALIZATION
    // =====================================================

    // const tooltipTriggerList =
    //     [].slice.call(
    //         document.querySelectorAll(
    //             '[data-bs-toggle="tooltip"]'
    //         )
    //     );

    tooltipTriggerList.map(function(tooltipTriggerEl) {

        return new bootstrap.Tooltip(
            tooltipTriggerEl
        );
    });
});