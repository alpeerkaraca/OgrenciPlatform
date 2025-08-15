
// Sidebar Menü

var hamburger = document.querySelector("#toggle-btn");
var menuBtn = document.querySelector(".bx-menu");

hamburger.addEventListener("click",
    function() {
        document.querySelector("#sidebar").classList.toggle("expand");
        menuBtn.classList.add("bx-spin");
    })

menuBtn.addEventListener("animationend",
    function() {
        menuBtn.classList.remove("bx-spin");
    });

if (document.querySelector("input[type=number]")) {
    document.querySelector("input[type=number]").addEventListener("keypress",
        function(evt) {
            if (evt.which < 48 || evt.which > 57) {
                evt.preventDefault();
            }
        });
}