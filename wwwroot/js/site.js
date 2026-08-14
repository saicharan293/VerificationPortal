// Prevent theme flash on page load
(function () {
    const theme = localStorage.getItem('verification-portal-theme') || 'light';
    document.documentElement.setAttribute('data-theme', theme);
})();


// Initialize AOS Animations
document.addEventListener('DOMContentLoaded', function () {

    // Initialize AOS
    if (typeof AOS !== 'undefined') {
        AOS.init({
            duration: 800,
            once: true,
            offset: 50,
            easing: 'ease-out-cubic'
        });
    }

    // ==================== SEARCH TABS ====================
    const searchTabs = document.querySelectorAll('.search-tab');
    const searchInput = document.getElementById('searchInput');

    const placeholders = {
        'college': 'Enter college name to verify...',
        'code': 'Enter college code (e.g., MED-2024-001)...',
        'district': 'Select or enter district name...',
        'faculty': 'Choose a faculty type...'
    };

    searchTabs.forEach(tab => {
        tab.addEventListener('click', function () {
            searchTabs.forEach(t => t.classList.remove('active'));
            this.classList.add('active');

            const searchType = this.dataset.searchType;
            if (searchInput && placeholders[searchType]) {
                searchInput.placeholder = placeholders[searchType];
                searchInput.focus();
            }
        });
    });

    // ==================== ANIMATED COUNTER ====================
    const counters = document.querySelectorAll('.stat-number');

    const animateCounter = (counter) => {
        const target = parseInt(counter.getAttribute('data-target'));
        const duration = 2000; // 2 seconds
        const increment = target / (duration / 16); // 60fps
        let current = 0;

        const updateCounter = () => {
            current += increment;
            if (current < target) {
                counter.innerText = Math.ceil(current).toLocaleString();
                requestAnimationFrame(updateCounter);
            } else {
                counter.innerText = target.toLocaleString();
            }
        };
        updateCounter();
    };

    // Intersection Observer for Counter
    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting && entry.target.innerText === '0') {
                animateCounter(entry.target);
            }
        });
    }, { threshold: 0.5 });

    counters.forEach(counter => counterObserver.observe(counter));

    // ==================== LANGUAGE SWITCHER ====================
    const langBtns = document.querySelectorAll('.lang-btn');
    langBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            langBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            // Add your language switching logic here
            console.log('Language switched to:', this.dataset.lang);
        });
    });

    // ==================== SMOOTH SCROLL FOR NAV LINKS ====================

    document.querySelectorAll('a[href^="#"]').forEach(anchor => {

        anchor.addEventListener('click', function (e) {

            const href = this.getAttribute('href');

            // Ignore empty "#"
            if (!href || href === '#') {
                return;
            }

            // Only process actual hash links
            if (!href.startsWith('#')) {
                return;
            }

            e.preventDefault();

            const target = document.getElementById(href.substring(1));

            if (target) {

                const offsetTop =
                    target.getBoundingClientRect().top +
                    window.pageYOffset -
                    80;

                window.scrollTo({
                    top: offsetTop,
                    behavior: 'smooth'
                });
            }

        });

    });

    // ==================== NAVBAR SCROLL EFFECT ====================
    const navbar = document.querySelector('.custom-navbar');
    if (navbar) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 100) {
                navbar.style.boxShadow = '0 4px 20px rgba(0,0,0,0.1)';
                navbar.style.padding = '10px 0';
            } else {
                navbar.style.boxShadow = '0 1px 3px rgba(0,0,0,0.08)';
                navbar.style.padding = '15px 0';
            }
        });
    }

    // ==================== PARALLAX EFFECT FOR HERO ====================
    const heroSection = document.querySelector('.hero-section');
    if (heroSection) {
        window.addEventListener('scroll', () => {
            const scrolled = window.pageYOffset;
            const heroContent = heroSection.querySelector('.hero-content');
            if (heroContent && scrolled < 600) {
                heroContent.style.transform = `translateY(${scrolled * 0.3}px)`;
                heroContent.style.opacity = 1 - (scrolled / 600);
            }
        });
    }

    // ==================== SEARCH BUTTON CLICK ====================
    const searchBtn = document.querySelector('.search-btn');
    if (searchBtn && searchInput) {
        searchBtn.addEventListener('click', function () {
            const query = searchInput.value.trim();
            if (query) {
                console.log('Searching for:', query);
                // Add your search logic here
                searchBtn.innerHTML = '<span class="spinner-border spinner-border-sm"></span> Searching...';
                setTimeout(() => {
                    searchBtn.innerHTML = 'Verify <i class="bi bi-arrow-right-circle"></i>';
                }, 1500);
            } else {
                searchInput.focus();
                searchInput.classList.add('shake');
                setTimeout(() => searchInput.classList.remove('shake'), 500);
            }
        });

        searchInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                searchBtn.click();
            }
        });
    }

    // ==================== RIPPLE EFFECT ON BUTTONS ====================
    document.querySelectorAll('.btn, .action-card, .btn-login, .search-btn').forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.cssText = `
                position: absolute;
                width: ${size}px;
                height: ${size}px;
                left: ${x}px;
                top: ${y}px;
                background: rgba(255,255,255,0.4);
                border-radius: 50%;
                transform: scale(0);
                animation: ripple 0.6s ease-out;
                pointer-events: none;
            `;

            this.style.position = 'relative';
            this.style.overflow = 'hidden';
            this.appendChild(ripple);

            setTimeout(() => ripple.remove(), 600);
        });
    });

    // Add ripple animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes ripple {
            to { transform: scale(4); opacity: 0; }
        }
        @keyframes shake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-10px); }
            75% { transform: translateX(10px); }
        }
        .shake { animation: shake 0.5s ease !important; border-color: var(--danger) !important; }
    `;
    document.head.appendChild(style);
});
