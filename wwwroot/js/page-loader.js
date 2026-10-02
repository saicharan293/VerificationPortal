
(function () {
    let initialized = false;

    function initializePageLoader() {
        if (initialized) return;

        const loader = document.getElementById("globalPageLoader");

        if (!loader) {
            console.warn("Page loader element #globalPageLoader was not found.");
            return;
        }

        initialized = true;

        window.showPageLoader = function () {
            loader.classList.add("is-visible");
            loader.setAttribute("aria-hidden", "false");
        };

        window.hidePageLoader = function () {
            loader.classList.remove("is-visible");
            loader.setAttribute("aria-hidden", "true");
        };

        // Allow the browser to paint the loader before starting an API call.
        window.showPageLoaderAndPaint = async function () {
            window.showPageLoader();

            await new Promise(resolve => {
                requestAnimationFrame(() => {
                    requestAnimationFrame(resolve);
                });
            });
        };

        // Normal form submissions.
        document.addEventListener("submit", function (event) {
            const form = event.target;

            if (
                form instanceof HTMLFormElement &&
                form.dataset.noLoader !== "true" &&
                !form.dataset.ajax
            ) {
                window.showPageLoader();
            }
        });

        // Internal page navigation.
        document.addEventListener("click", function (event) {
            if (!(event.target instanceof Element)) return;

            const link = event.target.closest("a[href]");

            if (
                !link ||
                event.defaultPrevented ||
                event.button !== 0 ||
                event.ctrlKey ||
                event.shiftKey ||
                event.altKey ||
                event.metaKey ||
                link.target === "_blank" ||
                link.hasAttribute("download") ||
                link.dataset.noLoader === "true"
            ) {
                return;
            }

            const url = new URL(link.href, window.location.href);

            if (
                !["http:", "https:"].includes(url.protocol) ||
                url.origin !== window.location.origin
            ) {
                return;
            }

            if (
                url.pathname === window.location.pathname &&
                url.search === window.location.search
            ) {
                return;
            }

            window.showPageLoader();
        });

        // Restore the correct loader state when returning from browser history.
        window.addEventListener("pageshow", function () {
            window.hidePageLoader();
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            initializePageLoader,
            { once: true }
        );
    } else {
        initializePageLoader();
    }
})();
