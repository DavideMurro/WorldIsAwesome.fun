
//#region events onload
window.addEventListener("load", async () => {
    let dropInputContainerList = document.getElementsByClassName("mdi-container");
    if (dropInputContainerList) {
        Array.from(dropInputContainerList).forEach((dropInputContainer) => {
            let dropInput = dropInputContainer.getElementsByClassName("mdi-fileinput")[0];

            dropInputContainer.addEventListener("drop", (e) => {
                e.preventDefault();
                e.stopPropagation();

                // If you want to use some of the dropped files
                let dT = new DataTransfer();
                dT.items.add(e.dataTransfer.files[0]);
                dropInput.files = dT.files;
                dropInput.onchange();

                dropInputContainer.classList.add("dropped");
                dropInputContainer.classList.remove("dragover");
            });
            dropInputContainer.addEventListener("dragover", (e) => {
                e.preventDefault();
                e.stopPropagation();
                dropInputContainer.classList.add("dragover");
            });
            dropInputContainer.addEventListener("dragleave", (e) => {
                e.preventDefault();
                e.stopPropagation();
                dropInputContainer.classList.remove("dragover");
            });
        });
    }
});
//#endregion

