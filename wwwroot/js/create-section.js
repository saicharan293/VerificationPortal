
document.addEventListener('DOMContentLoaded', function () {

    const sectionForm = document.getElementById('sectionForm');
    const facultyDropdown = document.getElementById('FacultyId');
    const tabDropdown = document.getElementById('TabId');

    const sectionNameInput = document.getElementById('SectionName');
    const sectionNameCounter = document.getElementById('sectionNameCounter');

    const tabHelpText = document.getElementById('tabHelpText');

    if (!sectionForm || !facultyDropdown || !tabDropdown) {
        return;
    }

    const tabsUrl = sectionForm.dataset.tabsUrl;

    // ======================================
    // LOAD TABS BASED ON FACULTY
    // ======================================

    async function loadTabs(facultyId, selectedTabId = '') {

        tabDropdown.innerHTML = '';

        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = facultyId
            ? 'Loading tabs...'
            : 'Select Faculty First';

        tabDropdown.appendChild(defaultOption);

        tabDropdown.disabled = true;

        if (tabHelpText) {
            tabHelpText.textContent = facultyId
                ? 'Loading available tabs...'
                : 'Select a faculty first to view its tabs.';
        }

        if (!facultyId) {
            return;
        }

        try {

            const response = await fetch(
                `${tabsUrl}?facultyId=${encodeURIComponent(facultyId)}`,
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

            const tabs = await response.json();

            tabDropdown.innerHTML = '';

            const placeholder = document.createElement('option');
            placeholder.value = '';
            placeholder.textContent = 'Select Tab';

            tabDropdown.appendChild(placeholder);

            tabs.forEach(tab => {

                const option = document.createElement('option');

                option.value = tab.value;
                option.textContent = tab.text;

                if (String(tab.value) === String(selectedTabId)) {
                    option.selected = true;
                }

                tabDropdown.appendChild(option);

            });

            tabDropdown.disabled = false;

            if (tabHelpText) {
                tabHelpText.textContent = tabs.length > 0
                    ? 'Select the tab under which this section belongs.'
                    : 'No tabs are available for this faculty.';
            }

        }
        catch (error) {

            console.error('Error loading tabs:', error);

            tabDropdown.innerHTML = '';

            const errorOption = document.createElement('option');
            errorOption.value = '';
            errorOption.textContent = 'Unable to load tabs';

            tabDropdown.appendChild(errorOption);

            if (tabHelpText) {
                tabHelpText.textContent = 'Please try selecting the faculty again.';
            }

        }
    }

    // ======================================
    // FACULTY CHANGE EVENT
    // ======================================

    facultyDropdown.addEventListener('change', function () {

        // Clear previous tab selection
        loadTabs(this.value);

    });

    // ======================================
    // INITIAL TAB STATE
    // ======================================

    // Existing options are populated by the controller.
    // This preserves the selected tab when editing.
    if (!facultyDropdown.value) {

        tabDropdown.disabled = true;

    }
    else {

        tabDropdown.disabled = false;

        if (tabHelpText) {
            tabHelpText.textContent =
                'Select the tab under which this section belongs.';
        }

    }

    // ======================================
    // SECTION NAME CHARACTER COUNTER
    // ======================================

    if (sectionNameInput && sectionNameCounter) {

        function updateCharacterCount() {

            const length = sectionNameInput.value.length;

            sectionNameCounter.textContent = `${length} / 150`;

            sectionNameCounter.style.color =
                length >= 150 ? '#dc2626' : '#94a3b8';
        }

        sectionNameInput.addEventListener('input', updateCharacterCount);

        updateCharacterCount();
    }

    // ======================================
    // FORM VALIDATION
    // ======================================

    sectionForm.addEventListener('submit', function (event) {

        const sectionName = sectionNameInput
            ? sectionNameInput.value.trim()
            : '';

        if (!facultyDropdown.value) {

            event.preventDefault();
            facultyDropdown.focus();
            facultyDropdown.reportValidity();

            return;
        }

        if (!tabDropdown.value) {

            event.preventDefault();
            tabDropdown.focus();
            tabDropdown.reportValidity();

            return;
        }

        if (!sectionName) {

            event.preventDefault();

            sectionNameInput.value = '';
            sectionNameInput.setCustomValidity(
                'Please enter a section name.'
            );

            sectionNameInput.focus();
            sectionNameInput.reportValidity();

            return;
        }

        if (sectionNameInput) {
            sectionNameInput.setCustomValidity('');
        }

    });

    if (sectionNameInput) {
        sectionNameInput.addEventListener('input', function () {
            this.setCustomValidity('');
        });
    }

});
