//#region events and onload
window.addEventListener("myOnPageLoad", async() => {
    // debounce 
    document.getElementById("user-residence").addEventListener('input', debounce(() => { searchLocation("user-residence-container") }, 300));

    // editor textarea 
    sunEditor = SUNEDITOR.create('user-description', editor_configuration);

    setAppStatus(appStatusMap[2]);
});
//#endregion


//#region form
function cancelProfileUserPopup() {
    openPopup("insert-cancel-confirm-popup").then(() => {
        document.getElementById("insert-cancel-confirm-yes-button").focus();
    });
}
function resetUserProfileFormPopup() {
    document.getElementById("user-profile-form-popup").classList.remove("insert");
    document.getElementById("user-profile-form-popup").classList.remove("update");
    document.getElementById("user-residence-search-results-container").innerHTML = "";
    document.getElementById("user-profile-form").reset();
    //document.getElementById("user-id").value = null;
    document.getElementById("user-picture-file-id").value = null;
    document.getElementById("user-residence-place-id").value = null;

    document.getElementById("user-profile-form").scrollTop = 0;

    removeUserResidence();
    removeUserPicture();
    resetUserProfileFormPopup_errors();
}
function resetUserProfileFormPopup_errors() {
    document.getElementById("user-error").innerHTML = "";

    // document.getElementById("user-nickname-container").classList.remove("error");
    document.getElementById("user-name-container").classList.remove("error");
    document.getElementById("user-surname-container").classList.remove("error");
    document.getElementById("user-picture-container").classList.remove("error");
    document.getElementById("user-description-container").classList.remove("error");
    document.getElementById("user-birthdate-container").classList.remove("error");
    document.getElementById("user-residence-container").classList.remove("error");
}
function removeUserPicture() {
    document.getElementById('user-picture-file-container').style.backgroundImage = null;
    document.getElementById("user-picture-img").src = "/images/icons/user_avatar_default.svg";
    document.getElementById("user-picture-img").classList.remove("d-none");

    document.getElementById("user-picture-remove-button").classList.add("d-none");
    document.getElementById("user-picture-error").innerHTML = "";

    document.getElementById("user-picture").value = null;
    document.getElementById("user-picture-file-id").value = null;
}
function userPictureChange(input) {
    if (input.files[0]) {
        let file = input.files[0];

        document.getElementById('user-picture-img').type = file.type;
        document.getElementById('user-picture-img').src = urlCreator.createObjectURL(file);
        document.getElementById("user-picture-img").classList.remove("d-none");

        document.getElementById("user-picture-remove-button").classList.remove("d-none");
    } else {
        removeUserPicture();
    }
}
async function userProfileSubmitClick() {
    setAppStatus(appStatusMap[1], "It may take a while...");
    resetUserProfileFormPopup_errors();
    let isFormValid = false;

    let userId = document.getElementById("user-id").value;
    let profilePhotoFileId = document.getElementById("user-picture-file-id").value;
    let residencePlaceId = document.getElementById("user-residence-place-id").value;

    // let nicknameInput = document.getElementById("user-nickname");
    // let nicknameInputContainer = document.getElementById("user-nickname-container");
    let nameInput = document.getElementById("user-name");
    let nameInputContainer = document.getElementById("user-name-container");
    let surnameInput = document.getElementById("user-surname");
    let surnameInputContainer = document.getElementById("user-surname-container");
    let profilePhotoFileInput = document.getElementById("user-picture");
    let profilePhotoFileInputContainer = document.getElementById("user-picture-container");
    let descriptionInput = document.getElementById("user-description");
    let descriptionInputContainer = document.getElementById("user-description-container");
    let descriptionValue = sunEditor.getContents() == "<p><br></p>" ? null : sunEditor.getContents();
    let birthdateInput = document.getElementById("user-birthdate");
    let birthdateInputContainer = document.getElementById("user-birthdate-container");
    let residencePlaceInput = document.getElementById("user-residence");
    let residencePlaceInputContainer = document.getElementById("user-residence-container");
    let residencePlaceList = document.querySelectorAll('#user-residence-search-results-container input[name="search-result-location"]');
    let residencePlaceRadio = document.querySelector('#user-residence-search-results-container input[name="search-result-location"]:checked');
    //if (!userId) {
    /*
let mailInput = document.getElementById("user-mail");
let mailInputContainer = document.getElementById("user-mail-container");
let passwordInput = document.getElementById("user-password");
let passwordInputContainer = document.getElementById("user-password-container");
let passwordrepeatInput = document.getElementById("user-passwordrepeat");
let passwordrepeatInputContainer = document.getElementById("user-passwordrepeat-container");
let privacypolicytermsInput = document.getElementById("user-privacypolicyterms");
let privacypolicytermsInputContainer = document.getElementById("user-privacypolicyterms-container");
*/
    //}

    if (descriptionValue)

    // autoselect place if there is only one on search container
    if (residencePlaceList.length == 1) {
        residencePlaceList[0].checked = true;
        selectResultLocation(residencePlaceList[0], "user-residence-container");
        residencePlaceRadio = residencePlaceList[0];
    }

    // Check all clients errors
    try {
        if (!nameInput.value) {
            nameInput.focus();
            nameInput.select();
            nameInputContainer.classList.add("error");
            throw "Name doesn t set";
        }
        if (!surnameInput.value) {
            surnameInput.focus();
            surnameInput.select();
            surnameInputContainer.classList.add("error");
            throw "Surname doesn t set";
        }
        if (/*(!profilePhotoFileInput.files[0] && !pictureFileId) ||*/ (profilePhotoFileInput.files[0] && profilePhotoFileInput.files[0].size > 20971520)) {
            profilePhotoFileInputContainer.scrollIntoView();
            profilePhotoFileInputContainer.classList.add("error");
            throw "File too big. Max 20MB";
        }
        /*
        if (!descriptionValue.getContents()) {
            descriptionInput.focus();
            descriptionInput.select();
            descriptionInputContainer.classList.add("error");
            descriptionInputContainer.scrollIntoView();
            throw "Description doesn t set";
        }*/
        /*if (!birthdateInput.value) {
            birthdateInput.focus();
            birthdateInputContainer.classList.add("error");
            throw "Date doesn t set";
        }*/
        /*if ((residenceList.length && !residenceRadio) || (!residenceList.length && !residenceRadio && !residencePlaceId)) {
            residenceInput.focus();
            residenceInput.select();
            residenceInputContainer.classList.add("error");
            throw "Residence doesn t set";
        }*/
        isFormValid = true;
    } catch (error) {
        isFormValid = false;
    }


    // API CALL
    if (isFormValid) {
        try {
            let residencePlaceName;
            let residencePlaceDescription;
            let residencePlaceLatitude;
            let residencePlaceLongitude;
            let profilePhotoFile;

            // check residence
            if (residencePlaceRadio && residencePlaceRadio.value) {
                let response = await fetch(residencePlaceRadio.value);
                residenceDetail = await response.json();

                residencePlaceName = residenceDetail.full_name;
                residencePlaceDescription = "";
                residencePlaceLatitude = residenceDetail.location.latlon.latitude;
                residencePlaceLongitude = residenceDetail.location.latlon.longitude;
            }

            // check file
            if (profilePhotoFileInput.files[0]) {
                profilePhotoFile = profilePhotoFileInput.files[0];
            }

            // Insert / Update
            if (userId) {
                await UpdateUser_PersonalInformations(userId, nameInput.value, surnameInput.value, profilePhotoFileId, profilePhotoFile, descriptionValue, birthdateInput.value, residencePlaceId, residencePlaceName, residencePlaceDescription, residencePlaceLatitude, residencePlaceLongitude);

                window.location.href = viewUri + "User?UserId=" + userId;
            } else {
                throw "User doesn t set";
            }

            setAppStatus(appStatusMap[2]);
        } catch (error) {
            let errorContainer = document.getElementById("user-error");
            if (!error.Status || error.Status == 500) {
                errorContainer.innerHTML = "Hey! something is wrong, maybe I m drunk. Try in a few minutes";
            } else if (error.Status == 400) {
                errorContainer.innerHTML = error.ErrorMessage;
            }
            errorContainer.scrollIntoView();

            if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
            throw error;
        }
    }

    if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
}

function removeUserResidence() {
    document.getElementById("user-residence").value = "";
    document.getElementById("user-residence-search-results-container").innerHTML = "";
    document.getElementById("user-residence-place-id").value = null;
    document.getElementById("user-residence-remove-button").classList.add("d-none");
}
//#endregion

