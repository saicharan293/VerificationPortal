
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
});
