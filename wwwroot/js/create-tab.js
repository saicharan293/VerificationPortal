
document.addEventListener('DOMContentLoaded', function () {

    const tabNameInput = document.getElementById('TabName');
    const tabNameCounter = document.getElementById('tabNameCounter');
    const tabForm = document.getElementById('tabForm');

    if (!tabNameInput || !tabNameCounter || !tabForm) {
        return;
    }

    const maxLength = 150;

    // Update character counter
    function updateCharacterCount() {

        const length = tabNameInput.value.length;

        tabNameCounter.textContent = `${length} / ${maxLength}`;

        tabNameCounter.style.color =
            length >= maxLength ? '#dc2626' : '#94a3b8';
    }

    tabNameInput.addEventListener('input', updateCharacterCount);

    // Initial character count
    updateCharacterCount();

    // Prevent whitespace-only tab names
    tabForm.addEventListener('submit', function (event) {

        const tabName = tabNameInput.value.trim();

        if (!tabName) {

            event.preventDefault();

            tabNameInput.value = '';

            tabNameInput.focus();

            tabNameInput.setCustomValidity('Please enter a tab name.');

            tabNameInput.reportValidity();

            return;
        }

        tabNameInput.setCustomValidity('');

    });

    tabNameInput.addEventListener('input', function () {
        this.setCustomValidity('');
    });

});
