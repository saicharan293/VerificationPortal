// ==================== THEME MANAGEMENT ====================
(function () {
    'use strict';

    const THEME_KEY = 'verification-portal-theme';
    const html = document.documentElement;

    // Get saved theme or default to LIGHT
    function getTheme() {
        const saved = localStorage.getItem(THEME_KEY);

        // If user has already selected a theme, use it
        if (saved === 'dark' || saved === 'light') {
            return saved;
        }

        // Default theme is always LIGHT
        return 'light';
    }

    // Apply theme
    function applyTheme(theme) {
        html.setAttribute('data-theme', theme);
        localStorage.setItem(THEME_KEY, theme);

        // Update meta theme-color
        const metaThemeColor =
            document.querySelector('meta[name="theme-color"]');

        if (metaThemeColor) {
            console.log('theme', metaThemeColor)
            metaThemeColor.setAttribute(
                'content',
                theme === 'dark' ? '#0f172a' : '#1e40af'
            );
        }
    }

    // Toggle theme
    function toggleTheme() {
        const current =
            html.getAttribute('data-theme') || 'light';

        const next =
            current === 'light' ? 'dark' : 'light';

        applyTheme(next);
    }

    // Apply theme immediately
    applyTheme(getTheme());

    // Setup toggle button
    document.addEventListener('DOMContentLoaded', function () {

        const toggleButtons =
            document.querySelectorAll('#themeToggle');

        toggleButtons.forEach(function (toggleBtn) {
            toggleBtn.addEventListener('click', toggleTheme);
        });

    });

})();