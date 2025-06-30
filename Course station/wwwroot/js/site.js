// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    const toggleSwitch = document.querySelector('.theme-switch input[type="checkbox"]');
    const currentTheme = localStorage.getItem('theme');

    if (currentTheme) {
        document.body.classList.add(currentTheme);

        if (currentTheme === 'dark-theme') {
            toggleSwitch.checked = true;
        @ViewData["IsDarkTheme"] = true;
        }
    }
   

    toggleSwitch.addEventListener('change', function (e) {
        if (e.target.checked) {
            document.body.classList.add('dark-theme');
            localStorage.setItem('theme', 'dark-theme');
        @ViewData["IsDarkTheme"] = true;
        } else {
            document.body.classList.remove('dark-theme');
            localStorage.setItem('theme', 'light-theme');
        @ViewData["IsDarkTheme"] = false;
        }
    });

    const form = document.querySelector("form");
    const contactContainer = document.querySelector(".contact-container");

    form.addEventListener("submit", function (event) {
        event.preventDefault(); // Prevent the default form submission

        // Hide the form
        form.style.display = "none";

        // Create and display the thank you message
        const thankYouMessage = document.createElement("p");
        thankYouMessage.textContent = "Thank you, we will be in touch";
        contactContainer.appendChild(thankYouMessage);
    });
});