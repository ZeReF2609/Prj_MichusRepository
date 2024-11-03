document.addEventListener("DOMContentLoaded", function () {
    const conteiner = document.querySelector(".conteiner");
    const btnSignIn = document.getElementById("btn-sign-in");
    const btnSignUp = document.getElementById("btn-sign-up");

    if (conteiner && btnSignIn && btnSignUp) {
        btnSignIn.addEventListener("click", () => {
            conteiner.classList.remove("toggle");
        });

        btnSignUp.addEventListener("click", () => {
            conteiner.classList.add("toggle");
        });
    } else {
        console.warn("One or more elements (conteiner, btnSignIn, btnSignUp) were not found in the document.");
    }
});
