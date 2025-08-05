
function showWatchlist() {
    const div1 = document.querySelector('.main-page');
    const div2 = document.querySelector(".watchlist-page");
    div1.style.display = "none";
    div2.style.display = "block";
}

function backToHomepage() {
    const div1 = document.querySelector('.main-page');
    const div2 = document.querySelector(".watchlist-page");
    div1.style.display = "block";
    div2.style.display = "none";
}