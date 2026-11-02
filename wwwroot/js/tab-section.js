document.addEventListener('DOMContentLoaded', function () {

    // ==========================================
    // FACULTY BASED TAB FILTER
    // ==========================================

    const facultyDropdown = document.getElementById('filterFacultyId');
    const tabDropdown = document.getElementById('filterTabId');

    if (facultyDropdown && tabDropdown) {
        facultyDropdown.addEventListener('change', async function () {
            const facultyId = this.value;
            tabDropdown.innerHTML = '';
            const defaultOption = document.createElement('option');
            defaultOption.value = '';
            defaultOption.textContent = 'All Tabs';
            tabDropdown.appendChild(defaultOption);

            if (!facultyId) {
                return;
            }

            try {
                const url = '@Url.Action("GetTabsByFaculty", "TabAndSectionManagement")';
                const response = await fetch(
                    `${url}?facultyId=${encodeURIComponent(facultyId)}`,
                    {
                        method: 'GET',
                        headers: {
                            'Accept': 'application/json'
                        }
                    }
                );

                if (!response.ok) {
                    throw new Error('Unable to load tabs.');
                }

                const data = await response.json();

                data.forEach(tab => {
                    const option = document.createElement('option');
                    option.value = tab.value;
                    option.textContent = tab.text;
                    tabDropdown.appendChild(option);
                });

            }
            catch (error) {
                console.error('Error loading tabs:', error);
                const errorOption = document.createElement('option');
                errorOption.textContent = 'Unable to load tabs';
                errorOption.disabled = true;
                tabDropdown.appendChild(errorOption);
            }

        });
    }


    // ==========================================
    // COMMON SEARCH AND PAGINATION
    // ==========================================

    function initializeTable(options) {

        let sortColumn = null;
        let sortDirection  = null;
        let currentPage = 1;

        const table = document.getElementById(options.tableId);
        const searchInput = document.getElementById(options.searchId);
        const clearButton = document.getElementById(options.clearId);
        const info = document.getElementById(options.infoId);
        const pagination = document.getElementById(options.paginationId);

        if (!table || !searchInput || !info || !pagination) {
            return;
        }

        const tbody = table.querySelector('tbody');

        const rows = Array.from(
            tbody.querySelectorAll('tr')
        ).filter(row => !row.querySelector('td[colspan]'));

        const pageSize = 10;


        // ======================================
        // NO RESULTS ROW
        // ======================================

        const noResultsRow = document.createElement('tr');

        const noResultsCell = document.createElement('td');

        noResultsCell.colSpan = 5;
        noResultsCell.className = 'ts-search-empty';

        const emptyIcon = document.createElement('i');
        emptyIcon.className = 'bi bi-search';

        const emptyText = document.createTextNode(
            'No matching records found.'
        );

        noResultsCell.appendChild(emptyIcon);
        noResultsCell.appendChild(emptyText);

        noResultsRow.appendChild(noResultsCell);
        noResultsRow.style.display = 'none';

        tbody.appendChild(noResultsRow);
        
        function sortRows(filteredRows){
            if(sortColumn == null) return filteredRows;

            return filteredRows.sort((a, b)=>{
                const valueA = a.cells[sortColumn].textContent.trim();
                const valueB = b.cells[sortColumn].textContent.trim();

                let comparison;

                if(options.tableId == "tabsTable" && sortColumn == 3){
                    comparison = Number(valueA) - Number(valueB);
                }
                else{
                    comparison = valueA.localeCompare(valueB, undefined, {numeric: true, sensitivity : 'base'});
                }
                return comparison * sortDirection;
            });
        }

        // ======================================
        // CREATE PAGINATION BUTTON
        // ======================================

        function createPageButton(content, disabled, callback) {
            const button = document.createElement('button');
            button.type = 'button';
            button.innerHTML = content;
            button.disabled = disabled;
            button.addEventListener('click', callback);
            return button;
        }


        // ======================================
        // RENDER TABLE
        // ======================================

        function renderTable() {

            const searchText = searchInput.value.trim().toLowerCase();

            // Filter records
            let filteredRows = rows.filter(row => {

                return row.textContent
                    .toLowerCase()
                    .includes(searchText);

            });

            filteredRows = sortRows(filteredRows);

            const totalRecords = filteredRows.length;

            const totalPages = Math.ceil(totalRecords / pageSize);

            // Validate current page
            if (totalPages === 0) {
                currentPage = 1;
            }
            else if (currentPage > totalPages) {
                currentPage = totalPages;
            }

            const start = (currentPage - 1) * pageSize;

            const end = Math.min(start + pageSize, totalRecords);

            // Hide all records
            rows.forEach(row => {
                row.style.display = 'none';
            });

            // Display matching records
            filteredRows.forEach((row, index) => {

                if (index >= start && index < end) {
                    row.style.display = '';

                    row.cells[0].textContent = index+1;
                }

            });

            // Empty state
            noResultsRow.style.display = totalRecords === 0 ? '' : 'none';

            // Update information
            if (totalRecords === 0) {
                info.textContent = 'Showing 0 records';
            }
            else {
                info.textContent = `Showing ${start + 1} - ${end} of ${totalRecords} records`;
            }

            // Reset pagination
            pagination.innerHTML = '';

            if (totalPages <= 1) {
                return;
            }

            // Previous button
            const previousButton = createPageButton(
                '<i class="bi bi-chevron-left"></i>',
                currentPage === 1,
                function () {

                    currentPage--;
                    renderTable();

                }
            );

            pagination.appendChild(previousButton);


            // Page numbers
            for (let page = 1; page <= totalPages; page++) {

                const pageButton = createPageButton(
                    page,
                    false,
                    function () {
                        currentPage = page;
                        renderTable();
                    }
                );

                if (page === currentPage) {

                    pageButton.classList.add('active');
                    pageButton.setAttribute('aria-current', 'page' );

                }
                pagination.appendChild(pageButton);
            }


            // Next button
            const nextButton = createPageButton(
                '<i class="bi bi-chevron-right"></i>',
                currentPage === totalPages,
                function () {

                    currentPage++;
                    renderTable();

                }
            );

            pagination.appendChild(nextButton);

        }

        
        const sortButtons = table.querySelectorAll(".ts-sort-button");

        sortButtons.forEach(button => {
            button.addEventListener('click', function(){
                const  columnIndex = Number(this.dataset.sortIndex);
                if(sortColumn == columnIndex){
                    sortDirection *= -1;
                }
                else{
                    sortColumn = columnIndex;
                    sortDirection = 1;
                }

                sortButtons.forEach(btn =>{
                    btn.classList.remove("active");
                    const icon = btn.querySelector("i");
                    icon.className = 'bi bi-arrow-down-up';
                });

                this.classList.add('active');
                
                const icon = this.querySelector('i');

                icon.className = sortDirection == 1 ? 'bi bi-sort-alpha-down' : 'bi bi-sort-alpha-up';

                currentPage = 1;
                renderTable();
            })
        })

        // ======================================
        // SEARCH EVENT
        // ======================================

        searchInput.addEventListener('input', function () {

            currentPage = 1;
            if (clearButton) {
                clearButton.classList.toggle(
                    'visible',
                    this.value.length > 0
                );
            }

            renderTable();

        });


        // ======================================
        // CLEAR SEARCH
        // ======================================

        if (clearButton) {

            clearButton.addEventListener('click', function () {
                searchInput.value = '';
                currentPage = 1;
                this.classList.remove('visible');
                renderTable();
                searchInput.focus();
            });

        }

        // Initial rendering
        renderTable();

    }


    // ==========================================
    // INITIALIZE TAB MANAGEMENT
    // ==========================================

    initializeTable({

        tableId: 'tabsTable',
        searchId: 'tabSearchInput',
        clearId: 'clearTabSearch',
        infoId: 'tabsPageInfo',
        paginationId: 'tabsPagination'

    });


    // ==========================================
    // INITIALIZE SECTION MANAGEMENT
    // ==========================================

    initializeTable({

        tableId: 'sectionsTable',
        searchId: 'sectionSearchInput',
        clearId: 'clearSectionSearch',
        infoId: 'sectionsPageInfo',
        paginationId: 'sectionsPagination'

    });


});