document.addEventListener("DOMContentLoaded", function () {

    const openButton =
        document.getElementById("openAffiliationSelector");

    const overlay =
        document.getElementById("affiliationSelectorOverlay");

    if (!openButton || !overlay) {
        return;
    }

    openButton.addEventListener("click", function (event) {

        event.preventDefault();

        overlay.classList.remove("is-hidden");

    });

});