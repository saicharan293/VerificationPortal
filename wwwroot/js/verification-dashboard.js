// verification-dashboard.js - JavaScript for the verification dashboard


document.addEventListener('DOMContentLoaded', function () {

    AOS.init({
        duration: 600,
        once: true
    });

    const status = document.getElementById("status");
    const remarks = document.getElementById("remarks");
    const form = document.querySelector(".feedback-form");

    function toggleRemarksValidation() {

        if (status.value === "reject" || status.value === "pending") {
            remarks.required = true;
        } else {
            remarks.required = false;
            remarks.setCustomValidity("");
        }
    }

    if (status) {
        status.addEventListener("change", toggleRemarksValidation);
        toggleRemarksValidation();
    }

    form.addEventListener("submit", function (e) {

        toggleRemarksValidation();

        if (!form.checkValidity()) {
            e.preventDefault();
            e.stopPropagation();
        }

        form.classList.add("was-validated");
    });

    // Elements
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const mobileSidebarToggle = document.getElementById('mobileSidebarToggle');

    // Toggle sidebar on desktop
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('expanded');
            localStorage.setItem('sidebarCollapsed', sidebar.classList.contains('collapsed'));
        });
    }

    // Toggle sidebar on mobile
    if (mobileSidebarToggle) {
        mobileSidebarToggle.addEventListener('click', function () {
            sidebar.classList.add('active');
            sidebarOverlay.classList.add('active');
            document.body.style.overflow = 'hidden';
        });
    }

    // Close sidebar on overlay click (mobile)
    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', function () {
            sidebar.classList.remove('active');
            sidebarOverlay.classList.remove('active');
            document.body.style.overflow = '';
        });
    }

    // Restore sidebar state from localStorage
    const sidebarCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (sidebarCollapsed) {
        sidebar.classList.add('collapsed');
        mainContent.classList.add('expanded');
    }

    // Active tab highlighting based on URL
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href && currentPath.includes(href.toLowerCase().split('/').pop())) {
            link.classList.add('active');
        }
    });

    // Form validation enhancement
    const forms = document.querySelectorAll('form.needs-validation');
    forms.forEach(form => {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });

    // Auto-save draft functionality
    const formInputs = document.querySelectorAll('.verification-card-body input, .verification-card-body select, .verification-card-body textarea');
    let autoSaveTimer;

    formInputs.forEach(input => {
        input.addEventListener('change', function () {
            clearTimeout(autoSaveTimer);
            autoSaveTimer = setTimeout(() => {
                saveDraft();
            }, 2000);
        });
    });

    function saveDraft() {
        // Collect form data
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            const formData = new FormData(form);
            const data = Object.fromEntries(formData);
            const key = `draft_${form.id || form.action}`;
            localStorage.setItem(key, JSON.stringify(data));
        });
        showToast('Draft saved automatically', 'info');
    }

    // Load draft on page load
    function loadDrafts() {
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            const key = `draft_${form.id || form.action}`;
            const draft = localStorage.getItem(key);
            if (draft) {
                try {
                    const data = JSON.parse(draft);
                    Object.keys(data).forEach(fieldName => {
                        const input = form.querySelector(`[name="${fieldName}"]`);
                        if (input && !input.value) {
                            input.value = data[fieldName];
                        }
                    });
                } catch (e) {
                    console.warn('Failed to load draft:', e);
                }
            }
        });
    }

    loadDrafts();

    // Clear draft on successful submit
    document.addEventListener('submit', function (event) {
        const form = event.target;
        if (form.tagName === 'FORM') {
            const key = `draft_${form.id || form.action}`;
            localStorage.removeItem(key);
        }
    });

    // Keyboard shortcuts
    document.addEventListener('keydown', function (event) {
        // Ctrl/Cmd + S to save
        if ((event.ctrlKey || event.metaKey) && event.key === 's') {
            event.preventDefault();
            const submitBtn = document.querySelector('button[type="submit"], .btn-primary[type="button"]');
            if (submitBtn) {
                submitBtn.click();
            }
        }

        // Ctrl/Cmd + B to toggle sidebar
        if ((event.ctrlKey || event.metaKey) && event.key === 'b') {
            event.preventDefault();
            if (sidebarToggle) {
                sidebarToggle.click();
            }
        }

        // Escape to close sidebar on mobile
        if (event.key === 'Escape' && sidebar.classList.contains('active')) {
            sidebar.classList.remove('active');
            sidebarOverlay.classList.remove('active');
            document.body.style.overflow = '';
        }
    });

    // Tooltip initialization
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Popover initialization
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Table search/filter functionality
    const searchInputs = document.querySelectorAll('.table-search');
    searchInputs.forEach(input => {
        input.addEventListener('keyup', function () {
            const filter = this.value.toUpperCase();
            const table = document.querySelector(this.dataset.tableSelector);
            if (!table) return;

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

    // Export summary functionality
    const exportBtn = document.getElementById('exportSummaryBtn');
    if (exportBtn) {
        exportBtn.addEventListener('click', function () {
            exportVerificationSummary();
        });
    }

    function exportVerificationSummary() {
        // Create a summary report
        const summary = {
            institution: document.querySelector('.institution-name')?.textContent?.trim() || '',
            collegeCode: document.querySelector('.institution-code')?.textContent?.replace('Code: ', '').trim() || '',
            faculty: document.querySelector('.institution-faculty')?.textContent?.trim() || '',
            status: document.querySelector('.status-badge')?.textContent?.trim() || '',
            exportDate: new Date().toISOString(),
            sections: []
        };

        // Collect section progress
        document.querySelectorAll('.summary-card').forEach(card => {
            const title = card.querySelector('h6')?.textContent?.trim() || '';
            const progress = card.querySelector('.progress-bar')?.style?.width || '0%';
            const percent = card.querySelector('.text-muted')?.textContent?.trim() || '';

            summary.sections.push({ title, progress, percent });
        });

        // Create downloadable JSON
        const blob = new Blob([JSON.stringify(summary, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `verification-summary-${summary.collegeCode}-${new Date().toISOString().split('T')[0]}.json`;
        a.click();
        URL.revokeObjectURL(url);

        showToast('Summary exported successfully', 'success');
    }

    // Print functionality
    const printBtn = document.getElementById('printReportBtn');
    if (printBtn) {
        printBtn.addEventListener('click', function () {
            window.print();
        });
    }

    // Confirmation dialogs
    document.querySelectorAll('[data-confirm]').forEach(element => {
        element.addEventListener('click', function (event) {
            const message = this.dataset.confirm || 'Are you sure?';
            if (!confirm(message)) {
                event.preventDefault();
            }
        });
    });

    // Status change handlers
    document.querySelectorAll('.status-select').forEach(select => {
        select.addEventListener('change', function () {
            const newStatus = this.value;
            const badge = this.closest('tr')?.querySelector('.badge') ||
                         document.querySelector('.status-badge');
            if (badge) {
                badge.textContent = newStatus.charAt(0).toUpperCase() + newStatus.slice(1);
                badge.className = 'badge status-badge ' + getStatusBadgeClass(newStatus);
            }
        });
    });

    function getStatusBadgeClass(status) {
        const classes = {
            'pending': 'pending',
            'in-progress': 'in-progress',
            'completed': 'completed',
            'rejected': 'rejected',
            'under-review': 'in-progress',
            'approved': 'completed'
        };
        return classes[status.toLowerCase()] || 'secondary';
    }

    // Character counter for textareas
    document.querySelectorAll('textarea[maxlength]').forEach(textarea => {
        const maxLength = parseInt(textarea.getAttribute('maxlength'));
        const counter = document.createElement('div');
        counter.className = 'form-text text-end';
        counter.innerHTML = `<span class="current-length">${textarea.value.length}</span> / ${maxLength}`;
        textarea.parentNode.appendChild(counter);

        textarea.addEventListener('input', function () {
            counter.querySelector('.current-length').textContent = this.value.length;
            if (this.value.length > maxLength * 0.9) {
                counter.classList.add('text-warning');
            } else {
                counter.classList.remove('text-warning');
            }
        });
    });

    // Tab persistence
    const tabLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    tabLinks.forEach(link => {
        link.addEventListener('click', function () {
            localStorage.setItem('activeVerificationTab', this.getAttribute('href'));
        });
    });

    // Restore active tab
    const savedTab = localStorage.getItem('activeVerificationTab');
    if (savedTab) {
        const tabLink = document.querySelector(`.sidebar-nav .nav-link[href="${savedTab}"]`);
        if (tabLink) {
            tabLinks.forEach(l => l.classList.remove('active'));
            tabLink.classList.add('active');
        }
    }

    // Window resize handler
    let resizeTimer;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => {
            if (window.innerWidth >= 992) {
                sidebar.classList.remove('active');
                sidebarOverlay.classList.remove('active');
                document.body.style.overflow = '';
            }
        }, 250);
    });

    // Toast notification function
    function showToast(message, type = 'info') {
        // Remove existing toasts
        document.querySelectorAll('.toast-container .toast').forEach(toast => toast.remove());

        const toastContainer = getOrCreateToastContainer();
        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white bg-${type} border-0`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;

        toastContainer.appendChild(toast);
        const bsToast = new bootstrap.Toast(toast, { delay: 3000 });
        bsToast.show();
    }

    function getOrCreateToastContainer() {
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            container.style.zIndex = '1080';
            document.body.appendChild(container);
        }
        return container;
    }

    const editBtn = document.getElementById("btnEditVerification");
    const cancelBtn = document.getElementById("btnCancelEdit");

    const display = document.getElementById("verificationDisplay");
    const editform = document.getElementById("verificationEditForm");

    if (editBtn) {

        editBtn.addEventListener("click", function () {

            display.style.display = "none";
            editform.style.display = "block";

        });

    }

    if (cancelBtn) {

        cancelBtn.addEventListener("click", function () {

            editform.style.display = "none";
            display.style.display = "block";

        });

    }

    // Expose functions globally for use in views
    window.VerificationDashboard = {
        showToast,
        saveDraft,
        loadDrafts,
        exportVerificationSummary
    };
});