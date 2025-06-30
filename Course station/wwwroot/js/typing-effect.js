const text = "Welcome to Course Station!";
const typingEffect = document.getElementById("typing-effect");

let isDeleting = false;
let charIndex = 0;
let typingSpeed = 150;
let deletingSpeed = 30;
let pauseAfterTyping = 2000; // Pause for 2 seconds after typing the full text
let pauseAfterDeleting = 500; // Pause for 0.5 seconds after deleting the text

function type() {
    const currentText = text.slice(0, charIndex);
    typingEffect.textContent = currentText;

    if (isDeleting) {
        charIndex--;
        if (charIndex === 0) {
            isDeleting = false;
            setTimeout(type, pauseAfterDeleting); // Pause before starting to type again
            return;
        }
    } else {
        charIndex++;
        if (charIndex === text.length) {
            isDeleting = true;
            setTimeout(type, pauseAfterTyping); // Pause before starting to delete
            return;
        }
    }

    setTimeout(type, isDeleting ? deletingSpeed : typingSpeed);
}

type();
