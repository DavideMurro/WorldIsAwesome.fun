//#region events and earth onload
window.addEventListener("load", () => {
    setAppStatus(appStatusMap[1]);

    // debounce 
    document.getElementById("user-residence").addEventListener('input', debounce(searchLocation_user, 300));

    // editor textarea 
    sunEditor = SUNEDITOR.create('user-description', editor_configuration);
    
    setAppStatus(appStatusMap[2]);
});
//#endregion


//#region signup
function cancelProfileUserPopupClick() {
    openPopup("insert-cancel-confirm-popup").then(() => {
        document.getElementById("insert-cancel-confirm-yes-button").focus();
    });
}

function resetUserProfileFormPopup_errors() {
    document.getElementById("user-error").innerHTML = "";

    document.getElementById("user-nickname-container").classList.remove("error");
    document.getElementById("user-name-container").classList.remove("error");
    document.getElementById("user-surname-container").classList.remove("error");
    document.getElementById("user-picture-container").classList.remove("error");
    document.getElementById("user-description-container").classList.remove("error");
    document.getElementById("user-birthdate-container").classList.remove("error");
    document.getElementById("user-residence-container").classList.remove("error");

    document.getElementById("user-mail-container").classList.remove("error");
    document.getElementById("user-password-container").classList.remove("error");
    document.getElementById("user-passwordrepeat-container").classList.remove("error");
    document.getElementById("user-privacypolicyterms-container").classList.remove("error");
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
function removeUserResidence() {
    document.getElementById("user-residence").value = "";
    document.getElementById("user-residence-search-results-container").innerHTML = "";
    document.getElementById("user-residence-place-id").value = null;
    document.getElementById("user-residence-remove-button").classList.add("d-none");
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
    if (await isUserProfileValid()) {
        openPopup("signup-confirm-popup").then(() => {
            document.getElementById("signup-confirm-yes-button").focus();
        });
    }
}
async function userProfileSubmit() {
    closePopup("signup-confirm-popup");
    setAppStatus(appStatusMap[1], "It may take a while...");
    
    // API CALL
    if (await isUserProfileValid()) {
        try {
            let nicknameInput = document.getElementById("user-nickname");
            let nameInput = document.getElementById("user-name");
            let surnameInput = document.getElementById("user-surname");
            let profilePhotoFileInput = document.getElementById("user-picture");
            let descriptionValue = sunEditor.getContents() == "<p><br></p>" ? null : sunEditor.getContents();
            let birthdateInput = document.getElementById("user-birthdate");
            let residencePlaceList = document.querySelectorAll('#user-residence-search-results-container input[name="search-result-location"]');
            let residencePlaceRadio = document.querySelector('#user-residence-search-results-container input[name="search-result-location"]:checked');
            //if (!userId) {
            let mailInput = document.getElementById("user-mail");
            let passwordInput = document.getElementById("user-password");
            //}
        
            // autoselect place if there is only one on search container
            if (residencePlaceList.length == 1) {
                residencePlaceList[0].checked = true;
                selectResultLocation(residencePlaceList[0], "user-residence-container");
                residencePlaceRadio = residencePlaceList[0];
            }

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

            // Insert
            await InsertUser(nicknameInput.value, nameInput.value, surnameInput.value, profilePhotoFile, descriptionValue, birthdateInput.value, residencePlaceName, residencePlaceDescription, residencePlaceLatitude, residencePlaceLongitude, mailInput.value, passwordInput.value);

            openPopup("signup-success-popup");

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
async function isUserProfileValid() {
    resetUserProfileFormPopup_errors();
    let isFormValid = false;

    let userId = document.getElementById("user-id").value;
    let profilePhotoFileId = document.getElementById("user-picture-file-id").value;
    let residencePlaceId = document.getElementById("user-residence-place-id").value;

    let nicknameInput = document.getElementById("user-nickname");
    let nicknameInputContainer = document.getElementById("user-nickname-container");
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
    let mailInput = document.getElementById("user-mail");
    let mailInputContainer = document.getElementById("user-mail-container");
    let passwordInput = document.getElementById("user-password");
    let passwordInputContainer = document.getElementById("user-password-container");
    let passwordrepeatInput = document.getElementById("user-passwordrepeat");
    let passwordrepeatInputContainer = document.getElementById("user-passwordrepeat-container");
    let privacypolicytermsInput = document.getElementById("user-privacypolicyterms");
    let privacypolicytermsInputContainer = document.getElementById("user-privacypolicyterms-container");
    //}

    // autoselect place if there is only one on search container
    if (residencePlaceList.length == 1) {
        residencePlaceList[0].checked = true;
        selectResultLocation(residencePlaceList[0], "user-residence-container");
        residencePlaceRadio = residencePlaceList[0];
    }

    // Check all clients errors
    try {

        if (!(nicknameInput.value && nicknameInput.value.length >= 3)) {
            nicknameInput.focus();
            nicknameInput.select();
            nicknameInputContainer.classList.add("error");
            throw "Nickname doesn t set or shorter then 3 chars";
        }
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
        if (!descriptionValue) {
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
        if (!userId) {
            if (!(mailInput.value && (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mailInput.value)))) {
                mailInput.focus();
                mailInputContainer.classList.add("error");
                throw "Mail doesn t set or is an invalid email address";
            }
            if (!(passwordInput.value && passwordInput.value.length >= 8)) {
                passwordInput.focus();
                passwordInputContainer.classList.add("error");
                throw "Password doesn t set or is shorter then 8 chars";
            }
            if (!(passwordrepeatInput.value && passwordrepeatInput.value == passwordInput.value)) {
                passwordrepeatInput.focus();
                passwordrepeatInputContainer.classList.add("error");
                throw "Password doesn t set or doesn t match with the password above";
            }
            if (!(privacypolicytermsInput.checked)) {
                privacypolicytermsInput.focus();
                privacypolicytermsInputContainer.classList.add("error");
                throw "Privacy policy terms doesn t set";
            }
        }
        isFormValid = true;
    } catch (error) {
        isFormValid = false;
    }

    return isFormValid;
}
//#endregion


//#region search location 
async function searchLocation_user() {
    searchLocation("user-residence-container");
}
//#endregion

