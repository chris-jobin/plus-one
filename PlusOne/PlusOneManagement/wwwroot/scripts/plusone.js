
function modalOpen(id) {
    let modal = document.getElementById(id);
    modal.style.display = "flex";
    modal.showModal();
}

function modalClose(id) {
    let modal = document.getElementById(id);
    modal.style.display = "";
    modal.close();
}