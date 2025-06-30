document.addEventListener('DOMContentLoaded', function () {
    const darkModeToggle = document.getElementById('dark-mode-toggle');
    const darkModeCss = document.getElementById('dark-mode-css');

    // Check if dark mode is enabled in local storage
    if (localStorage.getItem('darkMode') === 'enabled') {
        enableDarkMode();
    } else {
        disableDarkMode();
    }

    darkModeToggle.addEventListener('click', function () {
        if (document.body.classList.contains('dark-theme')) {
            disableDarkMode();
        } else {
            enableDarkMode();
        }
    });

    function enableDarkMode() {
        document.body.classList.add('dark-theme');
        darkModeCss.disabled = false;
        darkModeToggle.textContent = 'Toggle Light Mode';
        localStorage.setItem('darkMode', 'enabled');
    }

    function disableDarkMode() {
        document.body.classList.remove('dark-theme');
        darkModeCss.disabled = true;
        darkModeToggle.textContent = 'Toggle Dark Mode';
        localStorage.setItem('darkMode', 'disabled');
    }
});