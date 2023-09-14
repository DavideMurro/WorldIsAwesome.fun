//#region events and earth onload
window.addEventListener("earthjsload", () => {
    myearth = new Earth(document.getElementById('myearth'), {
        location: { lat: 16, lng: 16 },
        light: 'none',
        mapLandColor: '#557755',
        mapBorderColor: '#668866',
        mapSeaColor: '#bbeeee',
        mapBorderWidth: 0.3,
        quality: (window.innerWidth < 1000) ? 3 : 4,
        autoRotate: true,
        scale: 3
    });


    myearth.addEventListener("ready", async () => {

        // add photo overlay
        mediaOverlay = myearth.addOverlay({
            content: '<div id="photo" onclick="photoClick(); event.stopPropagation();"><div id="close-photo" onclick="closePhoto(); event.stopPropagation();"></div><h3 id="photo-title"></h3></div>',
            visible: false,
            elementScale: 1,
            depthScale: 0.5
        });

        // init method to call
        try {
            await CheckLogin();
            await updateMarkers();
        } catch (error) {
            // TODO: da togliere?
            openPopup("login-popup").then(() => {
                let usernameInput = document.getElementById("username");
                usernameInput.focus();
                usernameInput.select();
            });
        }

        // set app to ready
        setAppStatus(appStatusMap[2]);
    });


    // close photo overlay when navigating away
    /*myearth.addEventListener("change", () => {
        if (!currentMarker || autoRotate) return;

        if (Earth.getAngle(myearth.location, currentMarker.location) > 45) {
            closePhoto();
        }
    });*/
});
//#endregion

//#region markers
async function updateMarkers() {
    let placeMediaList = await GetPlaceMediaList(lightNight ? lightNightMap.indexOf(lightNight) : 0);
    setMarkers(placeMediaList);
}

function setMarkers(placeMediaList) {
    closePhoto();

    // resetto la lista
    let experienceListContainer = document.getElementById("experience-list-container");
    experienceListContainer.innerHTML = "";

    // Resetto i markers
    markerList.forEach((marker) => {
        marker.removeEventListener('click');
        marker.animate('scale', 0, { duration: 140 });
        marker.visible = false;
        marker.remove();
    });
    markerList = [];

    // add photo pins
    for (let i = 0; i < placeMediaList.length; i++) {

        let marker = myearth.addMarker({
            mesh: "Marker",
            color: '#bb4444',
            location: { lat: placeMediaList[i].placeLatitude, lng: placeMediaList[i].placeLongitude },
            scale: 0.01,
            offset: 1.6,
            visible: false,
            transparent: true,
            hotspotRadius: 0.6,
            hotspotHeight: 1.25,

            // custom property
            photoInfo: placeMediaList[i]
        });

        marker.addEventListener('click', () => {
            openPhoto(marker);
        });

        // animate marker
        marker.visible = true;
        setTimeout(() => { 
            marker.animate('scale', 1, { duration: 140 });
            marker.animate('offset', 0, { duration: 1100, easing: 'bounce' });
        }, 120 * i);

        markerList.push(marker);

        // set the list
        let experience = document.createElement("div");
        experience.classList.add("experience-item");
        experience.dataset.placeid = placeMediaList[i].placeId;
        experience.innerHTML += "<div>" + placeMediaList[i].placeName + "</div>";
        experience.innerHTML += "<div>Number of experience: " + placeMediaList[i].experience_Count + "</div>";
        experience.addEventListener('click', () => {
            openPhoto(marker);
        });
        experienceListContainer.append(experience);
    }
}
//#endregion


//#region photo overlay
async function setPhoto(photoInfo) {
    try {
        // get the media and visualize
        let media = await GetDefaultMediaPlace(photoInfo.placeId);
        let imageUrl = urlCreator.createObjectURL(media);
        document.getElementById('photo').style.backgroundImage = "url(" + imageUrl + ")";
        document.getElementById('photo-title').innerHTML = photoInfo.placeName;
    } catch (error) {
        // TODO: mettere un messaggio di errore
        document.getElementById('photo').style.backgroundImage = "url('icons/media.svg')";
    }

}
function openPhoto(marker) {
    if(marker != currentMarker) {
        // close current photo
        if (currentMarker) {
            closePhoto();
        }

        setPhoto(marker.photoInfo);

        // animate marker
        marker.animate('scale', 0.001, { duration: 150 });

        // animate overlay
        mediaOverlay.location = marker.location;
        mediaOverlay.visible = true;
        setTimeout(() => { 
            document.getElementById('photo').className = 'photo-appear'; 
        }, 120);

        currentMarker = marker;
        document.querySelector("#experience-list-container .experience-item[data-placeid='" + marker.photoInfo.placeId + "']").classList.add("active");
    }

    // rotate earth if needed
    myearth.goTo(marker.location, { relativeDuration: 100, approachAngle: 12 });
    //myearth.animate( 'zoom', 2, { relativeDuration: 100 } );
    myearth.autoRotate = false;
}
function closePhoto() {
    if (!currentMarker) return;

    // animate overlay
    document.getElementById('photo').className = '';
    document.getElementById('photo').style.backgroundImage = 'none';
    //setTimeout(() => { 
        mediaOverlay.visible = false;
    //}, 120);

    // animate earth
    //myearth.animate( 'zoom', 1, { relativeDuration: 100 } );
    myearth.autoRotate = true;

    // animate marker
    currentMarker.animate('scale', 1, { duration: 150 });
    currentMarker.opacity = 0.7;
    currentMarker = false;
    document.querySelector("#experience-list-container .experience-item.active").classList.remove("active");
}
function photoClick() {
    openExperienceList(currentMarker.photoInfo.placeId);
}
//#endregion


//#region ui event 
async function userClick() {
    if (userStatus == userStatusMap[2]) {
        openPopup("login-popup").then(() => {
            let usernameInput = document.getElementById("username");
            usernameInput.focus();
            usernameInput.select();
        });
    }
}
async function lampClick() {
    try {
        let placeMediaList = [];
        let newLightNight;

        switch (lightNight) {
            case lightNightMap[1]:
                newLightNight = lightNightMap[2];
                break;
            case lightNightMap[2]:
                newLightNight = lightNightMap[3];
                break;
            case lightNightMap[3]:
                newLightNight = lightNightMap[0];
                break;

            default:
                newLightNight = lightNightMap[1];
                break;
        }

        placeMediaList = await GetPlaceMediaList(newLightNight);
        setMarkers(placeMediaList);

        setLightNight(newLightNight);
    } catch (error) {
        openPopupError(error);
    }
}

async function loginSubmit(event) {
    event.preventDefault();

    document.getElementById("login-error").innerHTML = "";
    document.getElementById("login-confirm-button").disabled = true;
    document.getElementById("login-confirm-button").classList.add("loading");

    let usernameInput = document.getElementById("username");
    let passwordInput = document.getElementById("password");
    try {
        await Login(usernameInput.value, passwordInput.value);
        document.getElementById("login-error").innerHTML = "";
        document.getElementById("login-form").reset();
        closePopup("login-popup");

        await updateMarkers();
    } catch (error) {
        console.error(error);
        usernameInput.focus();
        usernameInput.select();
        document.getElementById("login-error").innerHTML = "Hey! The login is wrong or I m drunk?";
    }

    document.getElementById("login-confirm-button").classList.remove("loading");
    document.getElementById("login-confirm-button").disabled = false;
}

function resetExperiencePopup() {
    let popup = document.getElementById("experience-form-popup");
    popup.classList.remove("insert");
    popup.classList.remove("update");

    document.getElementById("experience-form").reset();
    document.getElementById("experience-media-img").src = "icons/media.svg";
    document.getElementById("experience-morningnight").value = lightNightMap.indexOf(lightNight) > 0 ? lightNightMap.indexOf(lightNight) : 1;
    document.getElementById("search-results-location").innerHTML = "";

    document.getElementById("experience-view-error").innerHTML = "";
}
async function newExperienceClick() {
    resetExperiencePopup();
    document.getElementById("experience-form-popup").classList.add("insert");

    openPopup("experience-form-popup").then(() => {
        let nameInput = document.getElementById("experience-name");
        nameInput.focus();
        nameInput.select();
    });
}
async function openExperienceList(placeId) {
    // get Experiences
    try {
        currentExperienceList = await GetExperienceListByPlace(placeId);
        await setExperienceIndex(0);
    } catch (error) {
        console.error(error);
        openPopupError(error);
        throw error;
    }
}
async function setExperienceIndex(experienceIndex) {
    currentExperienceIndex = experienceIndex;
    checkExperienceListArrows();
    let experience = currentExperienceList[currentExperienceIndex];

    if (experience) {
        // set values
        document.getElementById("experience-view-name").innerHTML = experience.name;
        document.getElementById("experience-view-place").innerHTML = experience.placeName;
        document.getElementById("experience-view-datetime").innerHTML = "<time datetime='" + experience.dateTime + "'>" + new Date(experience.dateTime).toLocaleString() + "</time>";
        document.getElementById("experience-view-description").innerHTML = experience.description;
        document.getElementById("experience-view-share-fb").href = "https://www.facebook.com/sharer/sharer.php?u=http://davidemurro-001-site1.htempurl.com/share/Facebook?ExperienceId=" + experience.id;

        
        // open popup
        if (!isPopupOpen("experience-view-popup")) {
            openPopup("experience-view-popup");
        }

        // in the end get media
        try {
            document.getElementById("experience-view-media").src = urlCreator.createObjectURL(await GetMedia(experience.fileId));
        } catch (error) {
            console.error(error);
            document.getElementById("experience-view-media").src = 'icons/media.svg';
        }
    }
}
function checkExperienceListArrows() {
    if (currentExperienceIndex == 0) {
        document.getElementById("experience-view-prev-button").style.visibility = "hidden";
    } else {
        document.getElementById("experience-view-prev-button").style.visibility = "visible";
    }
    if (!currentExperienceList[currentExperienceIndex + 1]) {
        document.getElementById("experience-view-next-button").style.visibility = "hidden";
    } else {
        document.getElementById("experience-view-next-button").style.visibility = "visible";
    }
}
async function closeExperienceList() {
    currentExperienceList = null;
    closePopup("experience-view-popup");
}
async function editExperienceClick() {
    let experience;

    try {
        resetExperiencePopup();
        document.getElementById("experience-form-popup").classList.add("update");

        //experience = currentExperienceList.find(x => x.id == experienceId);
        experience = currentExperienceList[currentExperienceIndex];

        // set values
        let experienceDateTime = new Date(experience.dateTime);
        // TODO: davvero devo traformare in UTC??
        let experienceDateTimeUTC = new Date(Date.UTC(experienceDateTime.getFullYear(), experienceDateTime.getMonth(), experienceDateTime.getDate(), experienceDateTime.getHours(), experienceDateTime.getMinutes(), 0));

        // set popup
        document.getElementById("experience-id").value = experience.id;
        document.getElementById("experience-place-id").value = experience.placeId;
        document.getElementById("experience-file-id").value = experience.fileId;

        document.getElementById("experience-name").value = experience.name;
        document.getElementById("experience-description").value = experience.description;
        document.getElementById("experience-date").valueAsDate = experienceDateTimeUTC;
        document.getElementById("experience-time").valueAsDate = experienceDateTimeUTC;
        document.getElementById("experience-morningnight").value = experience.morningNightEnum;
        document.getElementById("autocomplete-place").value = experience.placeName;

        openPopup("experience-form-popup").then(() => {
            let nameInput = document.getElementById("experience-name");
            nameInput.focus();
            nameInput.select();
        });
    } catch (error) {
        console.error(error);
        openPopupError(error);
    }

    // in the end get media
    try {
        document.getElementById("experience-media-img").src = urlCreator.createObjectURL(await GetMedia(experience.fileId));
    } catch (error) {
        console.error(error);
        document.getElementById("experience-view-media").src = 'icons/media.svg';
    }
}
async function deleteExperienceClick() {
    openPopup("experience-delete-confirm-popup").then(() => {
        document.getElementById("experience-delete-confirm-yes-button").focus();
    });
}

async function experienceSubmit(event) {
    event.preventDefault();

    document.getElementById("experience-error").innerHTML = "";
    document.getElementById("experience-confirm-button").disabled = true;
    document.getElementById("experience-confirm-button").classList.add("loading");
    let isFormValid = false;

    let experienceId = document.getElementById("experience-id").value;
    let fileId = document.getElementById("experience-file-id").value;
    let placeId = document.getElementById("experience-place-id").value;

    // let formElementList = event.target.elements;
    let nameInput = document.getElementById("experience-name");
    let fileInput = document.getElementById("experience-media");
    let descriptionInput = document.getElementById("experience-description");
    let dateInput = document.getElementById("experience-date");
    let timeInput = document.getElementById("experience-time");
    let morningNightInput = document.getElementById("experience-morningnight");
    let placeInput = document.getElementById("autocomplete-place");
    let placeList = document.querySelector('#search-results-location input[name="search-result-location"]');
    let placeRadio = document.querySelector('#search-results-location input[name="search-result-location"]:checked');

    // Check all clients errors
    //nameInput.classList.remove("error");
    //fileInput.classList.remove("error");
    //descriptionInput.classList.remove("error");
    //morningNightInput.classList.remove("error");
    //placeInput.classList.remove("error");

    try {
        if (!nameInput.value) {
            nameInput.focus();
            nameInput.select();
            //nameInput.classList.add("error");
            throw "Name doesn t set";
        }
        if (!fileInput.files[0] && !fileId) {
            fileInput.focus();
            //fileInput.classList.add("error");
            throw "File doesn t set";
        }
        if (!descriptionInput.value || descriptionInput.value.length < 200) {
            descriptionInput.focus();
            descriptionInput.select();
            //descriptionInput.classList.add("error");
            throw "Description doesn t set";
        }
        if (!morningNightInput.value) {
            morningNightInput.focus();
            //morningNightInput.classList.add("error");
            throw "Morning or night doesn t set";
        }
        if (!dateInput) {
            dateInput.focus();
            //dateInput.classList.add("error");
            throw "Date doesn t set";
        }
        if (!timeInput) {
            timeInput.focus();
            //timeInput.classList.add("error");
            throw "Time doesn t set";
        }
        if ((placeList && !placeRadio) || (!placeList && !placeRadio && !placeId)) {
            placeInput.focus();
            placeInput.select();
            //placeInput.classList.add("error");
            throw "Place doesn t set";
        }
        isFormValid = true;
    } catch (error) {
        console.error(error);
        isFormValid = false;
    }

    // API CALL
    if (isFormValid) {
        try {
            // set datetime
            let dateTime = new Date(dateInput.value + " " + timeInput.value);
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
            await UpsertExperience(experienceId, dateTime, nameInput.value, descriptionInput.value, morningNightInput.value, fileId, file, placeId, placeName, placeDescription, placeLatitude, placeLongitude);

            resetExperiencePopup();
            closePopup("experience-form-popup");
            closeExperienceList("experience-view-popup");

            await updateMarkers();
        } catch (error) {
            nameInput.focus();
            nameInput.select();
            document.getElementById("experience-error").innerHTML = "Hey! The experience is wrong or I m drunk?";
        }
    }

    document.getElementById("experience-confirm-button").classList.remove("loading");
    document.getElementById("experience-confirm-button").disabled = false;
}
async function deleteExperienceSubmit() {
    try {
        experience = currentExperienceList[currentExperienceIndex];
        await DeleteExperience(experience.id);
        closeExperienceList("experience-view-popup");
        updateMarkers();
    } catch (error) {
        console.error(error);
        document.getElementById("experience-view-error").innerHTML = "I can't delete this experience! :(";
    }
    closePopup("experience-delete-confirm-popup");
}

async function searchLocation(event) {
    let resultContainer = document.getElementById("search-results-location");
    resultContainer.innerHTML = "<p>Loading...</p>";

    try {
        let placeAutocomplete = document.getElementById("autocomplete-place");

        let response = await fetch("https://api.teleport.org/api/cities/?search=" + placeAutocomplete.value + "&limit=5");
        data = await response.json();

        if (data._embedded["city:search-results"].length <= 0) {
            resultContainer.innerHTML = "<p>No results! Where did you go??</p>";
        } else {
            resultContainer.innerHTML = "";
            data._embedded["city:search-results"].forEach((result, index) => {
                let item = document.createElement("label");
                item.setAttribute("for", "search-result-location" + index);
                item.className = "search-result";
                item.innerHTML = "<input type='radio' name='search-result-location' id='search-result-location" + index + "' value='" + result._links["city:item"].href + "' onchange='selectResultLocation(this)'>" + result.matching_full_name;
                //item.innerHTML += "<label for='search-result-location" + index + "'>" + result.matching_full_name;
                resultContainer.appendChild(item);
            });
        }
    } catch (error) {
        resultContainer.innerHTML = "<p>There s an error! I don t know why</p>";
        console.error(error);
    }
}
async function selectResultLocation(input) {
    let searchResultList = document.querySelectorAll("#search-results-location .search-result");
    searchResultList.forEach(result => {
        result.classList.remove("active");
    });
    input.parentElement.classList.add("active");
}
//#endregion