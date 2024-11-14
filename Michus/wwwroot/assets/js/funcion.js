const conteiner = document.querySelector(".conteiner");
const btnSignIn = document.getElementById("btn-sign-in");
const btnSignUp = document.getElementById("btn-sign-up");

btnSignIn.addEventListener("click", () => {
    conteiner.classList.remove("toggle");
});

btnSignUp.addEventListener("click", () => {
    conteiner.classList.add("toggle");

});