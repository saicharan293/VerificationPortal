
document.addEventListener('DOMContentLoaded', function () {

    const config = document.getElementById('collegeMappingConfig');

    if (!config) return;

    const usersElement = document.getElementById('allUsersData');
    const allUsers = JSON.parse(usersElement.textContent);
    const collegesUrl = config.dataset.collegesUrl;

    const alphabet = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';

    const facultySelect = document.getElementById('facultySelect');
    const userSelect = document.getElementById('userSelect');
    const collegeSelect = document.getElementById('collegeSelect');
    const collegeFromSelect = document.getElementById('collegeFromSelect');
    const collegeToSelect = document.getElementById('collegeToSelect');

    const selectAllBtn = document.getElementById('selectAllBtn');
    const clearAllBtn = document.getElementById('clearAllBtn');
    const applyFilterBtn = document.getElementById('applyFilterBtn');
    const selectedCount = document.getElementById('selectedCount');

    // Initialize College To dropdown
    function updateToLetterOptions() {
        const fromLetter = collegeFromSelect.value;
        const fromIndex = alphabet.indexOf(fromLetter);

        collegeToSelect.innerHTML = '';

        alphabet.split('').slice(fromIndex).forEach(letter => {
            const option = document.createElement('option');
            option.value = letter;
            option.textContent = letter;
            collegeToSelect.appendChild(option);
        });

        collegeToSelect.value = 'Z';
    }

    updateToLetterOptions();

    // Update College To when College From changes
    collegeFromSelect.addEventListener('change', updateToLetterOptions);

    // Filter users based on selected faculty
    facultySelect.addEventListener('change', function () {
        const facultyId = parseInt(this.value);

        userSelect.innerHTML = '<option value="0">-- Select User --</option>';

        if (facultyId > 0) {
            const users = allUsers.filter(u => u.facultyId === facultyId);

            users.forEach(u => {
                const option = document.createElement('option');
                option.value = u.userId;
                option.textContent = `${u.name} - ${u.designation || 'N/A'}`;
                userSelect.appendChild(option);
            });

            userSelect.disabled = false;
            applyFilterBtn.disabled = false;
        } else {
            userSelect.disabled = true;
            applyFilterBtn.disabled = true;
        }

        resetColleges();
    });

    function resetColleges() {
        collegeSelect.innerHTML =
            '<option value="" disabled>-- Select Faculty & User First --</option>';

        collegeSelect.disabled = true;
        selectAllBtn.disabled = true;
        clearAllBtn.disabled = true;

        updateCount();
    }

    // Load colleges based on faculty and letter range
    async function loadColleges() {
        const facultyId = parseInt(facultySelect.value);
        const fromLetter = collegeFromSelect.value;
        const toLetter = collegeToSelect.value;

        if (!facultyId || facultyId <= 0) return;

        if (fromLetter > toLetter) {
            alert('College To letter must be same as or after College From letter.');
            return;
        }

        collegeSelect.innerHTML = '<option disabled>Loading colleges...</option>';
        collegeSelect.disabled = true;
        selectAllBtn.disabled = true;
        clearAllBtn.disabled = true;

        try {
            const url = `${collegesUrl}?facultyId=${facultyId}&fromLetter=${encodeURIComponent(fromLetter)}&toLetter=${encodeURIComponent(toLetter)}`;

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error('Unable to load colleges.');
            }

            const colleges = await response.json();

            collegeSelect.innerHTML = '';

            if (!colleges || colleges.length === 0) {
                collegeSelect.innerHTML =
                    '<option value="" disabled>No colleges found for this range</option>';
            } else {
                colleges.forEach(c => {
                    const option = document.createElement('option');
                    option.value = c.code;
                    option.textContent = `${c.name} (${c.town || 'N/A'})`;
                    collegeSelect.appendChild(option);
                });
            }

            collegeSelect.disabled = false;
            selectAllBtn.disabled = !colleges || colleges.length === 0;
            clearAllBtn.disabled = !colleges || colleges.length === 0;

            updateCount();

        } catch (error) {
            console.error('Error loading colleges:', error);

            collegeSelect.innerHTML =
                '<option value="" disabled>Error loading colleges</option>';

            collegeSelect.disabled = true;
            selectAllBtn.disabled = true;
            clearAllBtn.disabled = true;

            updateCount();
        }
    }

    // Apply filter button
    applyFilterBtn.addEventListener('click', loadColleges);

    // Load colleges when user is selected
    userSelect.addEventListener('change', function () {
        if (this.value !== '0') {
            loadColleges();
        } else {
            resetColleges();
        }
    });

    // Apply filter on Enter key
    ['collegeFromSelect', 'collegeToSelect'].forEach(id => {
        document.getElementById(id).addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                loadColleges();
            }
        });
    });

    // Select all colleges
    selectAllBtn.addEventListener('click', function () {
        Array.from(collegeSelect.options).forEach(option => {
            if (!option.disabled && option.value) {
                option.selected = true;
            }
        });

        updateCount();
    });

    // Clear all colleges
    clearAllBtn.addEventListener('click', function () {
        Array.from(collegeSelect.options).forEach(option => {
            option.selected = false;
        });

        updateCount();
    });

    // Update selected college count
    collegeSelect.addEventListener('change', updateCount);

    function updateCount() {
        const count = Array.from(collegeSelect.selectedOptions)
            .filter(option => !option.disabled && option.value)
            .length;

        selectedCount.textContent = count;
    }

    const indexConfig = document.getElementById('collegeMappingIndexConfig');



    if (indexConfig) {

        const urls = {
            updateVerification: indexConfig.dataset.updateVerificationUrl,
            toggleActive: indexConfig.dataset.toggleActiveUrl,
            delete: indexConfig.dataset.deleteUrl,
            getColleges: indexConfig.dataset.getCollegesUrl
        };

        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        const mappingTbody = document.querySelector('#mappingTable tbody');


        if (mappingTbody) {
            new Sortable(mappingTbody, {
                handle: '.drag-handle',
                animation: 150,
                onEnd: async function(evt) {
                    const orderedIds = Array.from(mappingTbody.querySelectorAll('tr[data-id]'))
                        .map(row => parseInt(row.dataset.id));

                    await fetch(urls.updateVerification, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                        },
                        body: JSON.stringify(orderedIds)
                    });

                    location.reload();
                }
            });
        }

        // Toggle Active Status
        document.querySelectorAll('.toggle-btn').forEach(btn => {
            btn.addEventListener('click', async function() {
                const id = this.dataset.id;
                const response = await fetch(`${urls.toggleActive}/${encodeURIComponent(id)}`, {
                    method: 'POST',
                    headers: { 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value }
                });
                const data = await response.json();
                if (data.success) location.reload();
            });
        });

        // Delete
        document.querySelectorAll('.delete-btn').forEach(btn => {
            btn.addEventListener('click', async function() {
                if (!confirm('Are you sure you want to delete this mapping?')) return;
                const id = this.dataset.id;
                const form = document.createElement('form');
                form.method = 'POST';
                form.action = `${urls.delete}?id=${encodeURIComponent(id)}`;
                const token = document.createElement('input');
                token.name = '__RequestVerificationToken';
                token.value = document.querySelector('input[name="__RequestVerificationToken"]').value;
                form.appendChild(token);
                document.body.appendChild(form);
                form.submit();
            });
        });

        document.querySelectorAll('.clickable-range').forEach(element => {
            element.addEventListener('click', function() {
                const mappingId = this.dataset.mappingId;
                const facultyId = this.dataset.facultyId;
                const fromLetter = this.dataset.from;
                const toLetter = this.dataset.to;
                const mappingType = this.dataset.mappingType;

                // Update modal title
                if(mappingType == "Selective"){
                    document.getElementById('modalRangeDisplay').textContent = "(Selected Colleges)";
                }
                else{
                    document.getElementById('modalRangeDisplay').textContent = `(${fromLetter} - ${toLetter})`;
                }

                // Show loading, hide content
                document.getElementById('collegesLoading').style.display = 'block';
                document.getElementById('collegesContent').style.display = 'none';
                document.getElementById('collegesTableBody').innerHTML = '';
                document.getElementById('collegesEmpty').style.display = 'none';
                document.getElementById('collegesError').style.display = 'none';
                document.getElementById('collegesCount').textContent = '';

                // Show modal
                const modal = new bootstrap.Modal(document.getElementById('collegesModal'));
                modal.show();

                // Fetch colleges
                fetch(`${urls.getColleges}?mappingId=${encodeURIComponent(mappingId)}`)
                    .then(response => response.json())
                    .then(colleges => {

                        document.getElementById('collegesLoading').style.display = 'none';
                        document.getElementById('collegesContent').style.display = 'block';

                        if ($.fn.DataTable.isDataTable('#collegesTable')) {
                            $('#collegesTable').DataTable().destroy();
                        }

                        const tbody = document.getElementById('collegesTableBody');
                        tbody.innerHTML = "";

                        if (colleges.length > 0) {

                            colleges.forEach(college => {

                                const row = document.createElement('tr');

                                [college.code, college.name, college.town || "N/A"].forEach(value => {
                                    const td = document.createElement('td');
                                    td.textContent = value ?? '';
                                    row.appendChild(td);
                                });

                                tbody.appendChild(row);

                            });

                            $('#collegesTable').DataTable({

                                pageLength: 10,

                                lengthMenu: [
                                    [10,25,50,100,-1],
                                    [10,25,50,100,"All"]
                                ],

                                ordering: false,

                                searching: true,

                                responsive: true,

                                autoWidth: false,

                                language: {
                                    search: "Search:",
                                    lengthMenu: "Show _MENU_ colleges",
                                    info: "Showing _START_ to _END_ of _TOTAL_ colleges",
                                    zeroRecords: "No matching colleges found"
                                }

                            });

                            document.getElementById('collegesCount').textContent =
                                `${colleges.length} college(s) found`;

                        }
                        else {

                            document.getElementById('collegesEmpty').style.display = 'block';

                            document.getElementById('collegesCount').textContent =
                                '0 college(s) found';

                        }

                    })
                    .catch(error => {
                        console.error('Error loading colleges:', error);
                        document.getElementById('collegesLoading').style.display = 'none';
                        document.getElementById('collegesError').style.display = 'block';
                    });
            });
        });
    }

    

    // View Colleges in Range Modal
});
