
//#region ToDo
async function removeExperienceToDoClick(experienceId) {
    try {
        setAppStatus(appStatusMap[1]);
        await RemoveUserExperienceToDo(experienceId);
        setAppStatus(appStatusMap[2]);
        window.location.reload();
    } catch (error) {
        let errorContainer = document.querySelector("#experience-view #experience-view-error");
        if (!error.Status || error.Status == 500) {
            errorContainer.innerHTML = "I can't add this experience to: ToDo! :(";
        } else if (error.Status == 400) {
            errorContainer.innerHTML = error.ErrorMessage;
        }
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
async function addExperienceToDoClick(experienceId) {
    try {
        setAppStatus(appStatusMap[1]);
        await AddUserExperienceToDo(experienceId);
        setAppStatus(appStatusMap[2]);
        window.location.reload();
    } catch (error) {
        let errorContainer = document.querySelector("#experience-view #experience-view-error");
        if (!error.Status || error.Status == 500) {
            errorContainer.innerHTML = "I can't add this experience to: ToDo! :(";
        } else if (error.Status == 400) {
            errorContainer.innerHTML = error.ErrorMessage;
        }
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
//#endregion


//#region delete
async function deleteExperienceClick() {
    openPopup("experience-delete-confirm-popup").then(() => {
        document.getElementById("experience-delete-confirm-yes-button").focus();
    });
}
async function deleteExperienceSubmit() {
    try {
        setAppStatus(appStatusMap[1]);
        await DeleteExperience(param_experienceId);
        deleteExperienceClose();
        setAppStatus(appStatusMap[2]);

        if (param_redirect) {
            window.location.href = param_redirect;
        } else {
            window.location.href = "/";
        }
    } catch (error) {
        deleteExperienceClose();

        let errorContainer = document.querySelector("#experience-view #experience-view-error");
        if (!error.Status || error.Status == 500) {
            errorContainer.innerHTML = "I can't delete this experience! :(";
        } else if (error.Status == 400) {
            errorContainer.innerHTML = error.ErrorMessage;
        }
        errorContainer.scrollIntoView();
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
function deleteExperienceClose() {
    closePopup("experience-delete-confirm-popup");
}
//#endregion

