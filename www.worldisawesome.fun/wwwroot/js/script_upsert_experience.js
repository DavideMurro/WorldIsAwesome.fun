//#region events and earth onload
var sunEditor;
window.addEventListener("myOnPageLoad", async () => {

    // debounce 
    document.getElementById("experience-place").addEventListener('input', debounce(() => searchLocation("experience-place-container"), 300));

    // editor textarea 
    sunEditor = SUNEDITOR.create('experience-description', editor_configuration);

    // init method to call
    setAppStatus(appStatusMap[2]);
});
//#endregion


//#region experience
function resetExperiencePopup_errors() {
    document.getElementById("experience-error").innerHTML = "";

    document.getElementById("experience-name-container").classList.remove("error");
    document.getElementById("experience-media-container").classList.remove("error");
    document.getElementById("experience-description-container").classList.remove("error");
    document.getElementById("experience-date-container").classList.remove("error");
    document.getElementById("experience-time-container").classList.remove("error");
    document.getElementById("morningnight-enum-radio-container").classList.remove("error");
    document.getElementById("experience-place-container").classList.remove("error");
    document.getElementById("privacylevel-radio-container").classList.remove("error");
}
function removeExperiencePlace() {
    document.getElementById("experience-place").value = "";
    document.getElementById("experience-place-search-results-container").innerHTML = "";
    document.getElementById("experience-place-remove-button").classList.add("d-none");

    if (document.getElementById("experience-place-id")) document.getElementById("experience-place-id").value = null;

    document.getElementById("experience-place").focus();
}
function experienceMediaChange(input) {
    let fileInput = input.files[0];
    
    if (fileInput) {
        let file = {
            id: null,
            fileName: fileInput.name,
            fileType: fileInput.type
        };
        setExperienceMedia(file, urlCreator.createObjectURL(fileInput));
    } else {
        removeExperienceMedia();
    }
}
function setExperienceMedia(file, fileSrc) {
    let mediaError = document.getElementById("experience-media-error");
    mediaError.innerHTML = "";

    if (file.fileType.startsWith("image/")) {
        document.getElementById("experience-media-file-container").innerHTML = '<img src="' + fileSrc + '" alt="experience media" onerror="onMediaError(this.parentElement)" id="experience-media-img" class="mdi-filecontainer" onclick="document.getElementById(\'experience-media\').click()" />';
    }
    else if (file.fileType.startsWith("video/")) {
        document.getElementById("experience-media-file-container").innerHTML = '<video src="' + fileSrc + '" controls onerror="onMediaError(this.parentElement)" id="experience-media-video"></video>';
    }
    else if (file.fileType.startsWith("audio/")) {
        document.getElementById("experience-media-file-container").innerHTML = '<audio src="' + fileSrc + '" controls onerror="onMediaError(this.parentElement)" id="experience-media-audio"></audio>';
    } else {
        mediaError.innerHTML = "File not supported. Please select <a onclick='document.getElementById(\"experience-media\").click()' title='Another file'>another file</a>";
        document.getElementById("experience-media").value = null;
        document.getElementById("experience-media-file-container").innerHTML = '<img src="/images/icons/nomedia.svg" alt="experience media" id="experience-media-img" />';
    }
    document.getElementById("experience-media-remove-button").classList.remove("d-none");
}
function removeExperienceMedia() {
    document.getElementById("experience-media-file-container").innerHTML = '<img src="/images/icons/media_default.svg" alt="experience media" onerror="onMediaError(this.parentElement)" id="experience-media-img" onclick="document.getElementById(\'experience-media\').click()" />';

    document.getElementById("experience-media-remove-button").classList.add("d-none");
    document.getElementById("experience-media-error").innerHTML = "";

    document.getElementById("experience-media").value = null;
    if (document.getElementById("experience-file-id")) document.getElementById("experience-file-id").value = null;
}
function onMediaError(container) {
    container.innerHTML = '<img src="/images/icons/nomedia.svg" alt="experience media error" class="m-auto" />';
}

function selectMorningnightRadio(input) {
    let searchResultList = document.querySelectorAll("#morningnight-enum-radio-container .morningnight-enum-radio");
    searchResultList.forEach(result => {
        result.classList.remove("active");
    });
    input.parentElement.classList.add("active");

    setMorningNight(morningNightMap[input.value]);
}
function selectPrivacylevelRadio(input) {
    let searchResultList = document.querySelectorAll("#privacylevel-radio-container .privacylevel-radio");
    searchResultList.forEach(result => {
        result.classList.remove("active");
    });
    input.parentElement.classList.add("active");
}

async function experienceSubmitClick(isDraft = false) {
    resetExperiencePopup_errors();

    setAppStatus(appStatusMap[1], "It may take a while...");
    let isFormValid = false;

    let experienceId = document.getElementById("experience-id") ? document.getElementById("experience-id").value : null;
    let fileId = document.getElementById("experience-file-id") ? document.getElementById("experience-file-id").value : null;
    let placeId = document.getElementById("experience-place-id") ? document.getElementById("experience-place-id").value : null;

    let nameInput = document.getElementById("experience-name");
    let nameInputContainer = document.getElementById("experience-name-container");
    let fileInput = document.getElementById("experience-media");
    let fileInputContainer = document.getElementById("experience-media-container");
    let descriptionInput = document.getElementById("experience-description");
    let descriptionInputContainer = document.getElementById("experience-description-container");
    let descriptionValue = sunEditor.getContents() == "<p><br></p>" ? null : sunEditor.getContents();
    let dateInput = document.getElementById("experience-date");
    let dateInputContainer = document.getElementById("experience-date-container");
    let timeInput = document.getElementById("experience-time");
    let timeInputContainer = document.getElementById("experience-time-container");
    let morningNightInput = document.forms["experience-form"]["experience-morningnight-enum"];
    let morningNightInputContainer = document.getElementById("morningnight-enum-radio-container");
    let placeInput = document.getElementById("experience-place");
    let placeInputContainer = document.getElementById("experience-place-container");
    let placeList = document.querySelectorAll('#experience-place-search-results-container input[name="search-result-location"]');
    let placeRadio = document.querySelector('#experience-place-search-results-container input[name="search-result-location"]:checked');
    let privacyLevelInput = document.forms["experience-form"]["experience-privacylevel"];
    let privacyLevelInputContainer = document.getElementById("privacylevel-radio-container");

    // autoselect place if there is only one on search container
    if (placeList.length == 1) {
        placeList[0].checked = true;
        selectResultLocation(placeList[0], "experience-place-container");
        placeRadio = placeList[0];
    }

    // Check all clients errors
    try {
        if (!nameInput.value) {
            nameInput.focus();
            nameInput.select();
            nameInputContainer.classList.add("error");
            throw "Name doesn t set";
        }
        if (!isDraft) {
            if ((!fileInput.files[0] && !fileId) || (fileInput.files[0] && fileInput.files[0].size > 20971520)) {
                fileInputContainer.scrollIntoView();
                fileInputContainer.classList.add("error");
                throw "File doesn t set or too big. Max 20MB";
            }
            // if (!descriptionInput.value/* || descriptionInput.value.length < 200*/) {
            if(!descriptionValue) {
                descriptionInput.focus();
                descriptionInput.select();
                descriptionInputContainer.classList.add("error");
                descriptionInputContainer.scrollIntoView();
                throw "Description doesn t set";
            }
            if (!morningNightInput.value) {
                // morningNightInput.focus();
                morningNightInputContainer.scrollIntoView();
                morningNightInputContainer.classList.add("error");
                throw "Morning or night doesn t set";
            }
            if (!dateInput.value) {
                dateInput.focus();
                dateInputContainer.classList.add("error");
                throw "Date doesn t set";
            }
            /*
            if (!timeInput.value) {
                timeInput.focus();
                timeInputContainer.classList.add("error");
                throw "Time doesn t set";
            }
            */
            if ((placeList.length && !placeRadio) || (!placeList.length && !placeRadio && !placeId)) {
                placeInput.focus();
                placeInput.select();
                placeInputContainer.classList.add("error");
                throw "Place doesn t set";
            }
            if (!privacyLevelInput.value) {
                //privacyLevelInput.focus();
                privacyLevelInputContainer.scrollIntoView();
                privacyLevelInputContainer.classList.add("error");
                throw "Privacy level doesn t set";
            }
        }
        isFormValid = true;
    } catch (error) {
        isFormValid = false;
    }

    // API CALL
    if (isFormValid) {
        try {
            let date = dateInput.value;
            let time = timeInput.value ? timeInput.value : null;
            let morningNightValue = morningNightInput.value ? morningNightInput.value : null;
            let privacyLevelValue = privacyLevelInput.value ? privacyLevelInput.value : null;
            let placeName;
            let placeDescription;
            let placeLatitude;
            let placeLongitude;
            let file;

            // check place
            if (placeRadio && placeRadio.value) {
                let response = await fetch(placeRadio.value);
                placeDetail = await response.json();

                placeName = placeDetail.full_name;
                placeDescription = "";
                placeLatitude = placeDetail.location.latlon.latitude;
                placeLongitude = placeDetail.location.latlon.longitude;
            }

            // check file
            if (fileInput.files[0]) {
                file = fileInput.files[0];
            }

            // Insert / Update
            await UpsertExperience(experienceId, date, time, nameInput.value, descriptionValue, morningNightValue, privacyLevelValue, fileId, file, placeId, placeName, placeDescription, placeLatitude, placeLongitude, isDraft);

            setAppStatus(appStatusMap[2]);

            if (param_experienceId) {
                if (param_redirect) {
                    window.location.href = param_redirect;
                } else {
                    window.location.href = "/";
                }
            } else {
                //document.getElementById("success-okexperience-button").href = "/View/Experience?ExperienceId=" + ;
                openPopup("insert-success-popup");
            }
        } catch (error) {
            let errorContainer = document.getElementById("experience-error");
            if (!error.Status || error.Status == 500) {
                errorContainer.innerHTML = "Hey! There's something wrong! Try in a few minutes";
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

function cancelExperiencePopupClick() {
    openPopup("insert-cancel-confirm-popup").then(() => {
        document.getElementById("insert-cancel-confirm-yes-button").focus();
    });
}
//#endregion
