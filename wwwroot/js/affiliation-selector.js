document.addEventListener("DOMContentLoaded", function () {

    const openButtons =
        document.querySelectorAll(".affiliation-selector-link");

    const overlay =
        document.getElementById("affiliationSelectorOverlay");

    if (!openButtons.length || !overlay) {
        return;
    }

    openButtons.forEach(function(btn){
        btn.addEventListener("click", function (event) {

            event.preventDefault();

            overlay.classList.remove("is-hidden");

        });
    })

});