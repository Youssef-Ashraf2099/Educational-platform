// credits.js
document.addEventListener('DOMContentLoaded', function () {
    const listItems = document.querySelectorAll('ul li');
    listItems.forEach(item => {
        item.addEventListener('click', () => {
            alert(`You clicked on ${item.textContent}`);
        });
    });
});
