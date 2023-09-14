//#region events and earth onload
window.addEventListener("earthjsload", () => {
    myearth = new Earth(document.getElementById('myearth'), {
        location: { lat: 16, lng: 16 },
        light: 'none',
        lightAmbience: 0.2,

        mapLandColor: '#afd078',
        mapBorderColor: '#abcc74',
        mapSeaColor: '#34b9c4',
        mapBorderWidth: 0.3,
        mapStyles: "#GR, #ES, #PT, #PK, #MX, #PE, #AR, #BF, #ET, #GN, #SL, #LR, #CI, #GH, #TG, #BJ, #NG, #ZM, #AO, #RW, #BI, #TZ, #MW, #MZ { fill: #79b73a; stroke: #75b336; } \
        #GL, #IS, #AQ { fill: #e5e8ee; stroke: #e1e4ea; } \
        #SE, #NO, #FI, #MY, #ID, #JP, #CA, #GT, #HN, #SV, #NI, #GF, #SR, #GY, #SS, #CM, #GQ, #CF, #GA, #CG, #CD, #UG, #BR { fill: #69b73b; stroke: #65b337; } \
        #AU, #KE, #MG, #ZW, #TR, #IR, #KZ, #TJ, #KG, #AF, #CL, #MA, #ER, #DJ, #SO, #GM, #SN, #NA, #LS, #BW, #SZ, #ZA { fill: #97c75b; stroke: #93c357; } \
        #SY, #JO, #LB, #IL, #KW, #SA, #AE, #QA, #YE, #OM, #IQ, #UZ, #TM, #DZ, #TN, #EG, #SD, #ML, #LY, #EH, #MR, #NE, #TD { fill: #e6e18b; stroke: #e2dd87; } \ ",
        quality: 4,
        autoRotate: true,
        scale: 3,

        zoom: 0.7,
        zoomable: false,
        zoomMin: 0.5,
        zoomMax: 2.0
    });

    
    // on earth change / move / zoom
    myearth.addEventListener("change", () => {
        try {
            // scale marker
            let markerScaleNew = getMarkerScale();

            if (markerScale != markerScaleNew) {
                markerList.forEach((marker) => {
                    marker.scale = markerScaleNew;
                });
                markerScale = markerScaleNew;
            }

            // stop start autorotate
            if (myearth.zoom >= 1.5) {
                myearth.autoRotate = false;
            } else {
                // myearth.autoRotate = true;
            }

            //if (!currentMarker || autoRotate) return;

            // overlay size fix
            if (myearth.zoom >= 1.5) {
                if (window.innerWidth > screenWidthLimitMax) {
                    mediaOverlay.elementScale = 2;
                } else {
                    mediaOverlay.elementScale = 1;
                }
            } else {
                if (window.innerWidth > screenWidthLimitMax) {
                    mediaOverlay.elementScale = 1.5;
                } else {
                    mediaOverlay.elementScale = 0.5;
                }
            }

            // TODO: drag movement adaptation (reduce sensibility)

            if (!myearth.isOpeningPhoto) {
                if (!myearth.isPreviewMode) {
                    // auto close photo
                    if (currentMarker && Earth.getAngle(myearth.location, currentMarker.location) > 25) {
                        closePhoto();
                    }
                } else {
                    // previewMode 
                    if (!myearth.isClosingPhoto && currentMarker && Earth.getAngle(myearth.location, currentMarker.location) > 50) {
                        closePhoto();
                    }
                    if (!currentMarker) {
                        let markerClose = markerList.find(x => Earth.getAngle(myearth.location, x.location) <= 25);
                        if (markerClose) {
                            openPhoto(markerClose, markerClose.isToDo, markerClose.type, true).then(() => {
                                myearth.autoRotate = true;
                            });
                        }
                    }

                }
            }
        } catch (error) {
            console.error(error);
        }
    });


    // earth load
    myearth.addEventListener("ready", async () => {
        // animate world
        myearth.animate("lightAmbience", 1, { duration: 1500 });
        myearth.animate("zoom", 1, { duration: 1000, complete: () => {
            myearth.zoomable = true;
        } });

        // set fixed size
        setHomeWorldViewSize();

        // marker scale
        markerScale = getMarkerScale();

        // add photo overlay
        mediaOverlay = myearth.addOverlay({
            content: '<div id="photo" onclick="photoClick(); event.stopPropagation();"><div id="photo-header"><h3 id="photo-title"></h3><img src="/images/icons/close.svg" alt="close" id="close-photo" onclick="closePhotoClick(event);" /></div><div id="photo-media-container"></div></div>',
            visible: false,
            elementScale: 1,
            depthScale: 0.5
        });
    });
});

// page load
window.addEventListener("myOnPageLoad", async () => {
    // debounce 
    document.getElementById("experience-search").addEventListener('input', debounce(updateMarkers, 300));
    window.addEventListener('click', debounce(startPreviewMode, 5000));
    window.addEventListener('touchstart', debounce(startPreviewMode, 5000));
    window.addEventListener('startPreviewMode', debounce(startPreviewMode, 5000));
    window.addEventListener('openingPhoto', debounce(finishOpeningPhoto, 2000));
    window.addEventListener('closingPhoto', debounce(finishClosingPhoto, 2000));

    //global search
    /*if(!param_userId && document.getElementById("global-search")) {
        document.getElementById("global-search").addEventListener('input', debounce(() => searchLocation("global-search-container", globalSearch), 300));
        document.getElementById("global-search").onfocus = globalSearchFocus;
        document.getElementById("global-search").onblur = globalSearchBlur;
    }*/

    // add hammerjs
    // TODO: da fare meglio
    let bcHammer = new Hammer(document.getElementById("global-list-container"));
    bcHammer.on("swipeleft", (ev) => {
        closeExperiencePlacesClick();
    });
    bcHammer.on("swiperight", (ev) => {
        openExperiencePlacesClick();
    });

    // init method to call on all is ready
    if (myearth && myearth.ready) {
        init();
    } else if (myearth) {
        myearth.addEventListener("ready", init);
    } else {
        window.addEventListener("earthjsload", () => {
            myearth.addEventListener("ready", init);
        });
    }
});
/*
window.addEventListener("resize", () => {
    //setHomeWorldViewSize();
    // world home view
    if (document.body.classList.contains("home-view")) {
        startHomeView();
    } else if (document.body.classList.contains("world-view")) {
        startWorldView();
    }
});
*/
window.addEventListener("scroll", () => {
    if (isHomeWorldViewScrollEnable) {
        let scrollPosition = window.scrollY;
        let homeViewOffset = document.body.offsetTop;
        let worldViewOffset = document.getElementById("world-body-container").offsetTop;

        if (scrollPosition >= (worldViewOffset - (worldViewOffset/2))) {
            if(!isFirstWorldView) {
                setWorldView();
            } else {
                if (isInitCompleted) {
                    startWorldView();
                } else {
                    window.addEventListener("myOnInitCompleted", startWorldView);
                }
            }
        } else {
            if(!isFirstHomeView) {
                setHomeView();
            } else {
                if (isInitCompleted) {
                    startHomeView();
                } else {
                    window.addEventListener("myOnInitCompleted", startHomeView);
                }
            }
        }
    }
});
//#endregion


//#region GLOBAL
async function init() {
    try {
        // init markers
        if (!param_userId) {
            updateMarkers();
            isHomeWorldViewScrollEnable = false;
        }

        // set app to ready
        isInitCompleted = true;
        window.dispatchEvent(myInitCompletedEvent);
        setAppStatus(appStatusMap[2]);
    } catch (error) {
        setAppStatus(appStatusMap[3]);
        throw error;
    }
}

function getMarkerScale() {
    return ((((myearth.zoomMax) - myearth.zoom) + 0.3) / 2).toFixed(2);
    // return ((myearth.zoomMax - myearth.zoom) + 0.2).toFixed(2);
}

async function startPreviewMode() {
    myearth.autoRotate = true;
    if (window.innerWidth > screenWidthLimitMax || (window.innerWidth <= screenWidthLimitMax && !document.body.classList.contains("place-list-open"))) {
        myearth.isPreviewMode = true;
        document.getElementById("myearth").addEventListener("click", stopPreviewMode, false);
        document.getElementById("myearth").addEventListener("touchstart", stopPreviewMode, false);
    }
}
function stopPreviewMode() {
    myearth.autoRotate = false;
    myearth.isPreviewMode = false;
    document.getElementById("myearth").removeEventListener("click", stopPreviewMode, false);
    document.getElementById("myearth").removeEventListener("touchstart", stopPreviewMode, false);
}

function startOpeningPhoto() {
    myearth.isOpeningPhoto = true;
    window.dispatchEvent(new CustomEvent('openingPhoto'));
}
function finishOpeningPhoto() {
    myearth.isOpeningPhoto = false;
}
function startClosingPhoto() {
    myearth.isClosingPhoto = true;
    window.dispatchEvent(new CustomEvent('closingPhoto'));
}
function finishClosingPhoto() {
    myearth.isClosingPhoto = false;
}

async function changeMorningNight() {
    let newMorningNight;

    switch (morningNight) {
        case morningNightMap[1]:
            newMorningNight = morningNightMap[2];
            break;
        case morningNightMap[2]:
            newMorningNight = morningNightMap[3];
            break;
        case morningNightMap[3]:
            newMorningNight = morningNightMap[0];
            break;

        default:
            newMorningNight = morningNightMap[1];
            break;
    }

    setMorningNight(newMorningNight);

    updateMarkers();
}
async function changeView() {
    /*vtIndex = viewTypeMap.findIndex(x => x == viewType);
    if (vtIndex >= viewTypeMap.length - 1) {
        setView(viewTypeMap[1]);
    } else {
        setView(viewTypeMap[vtIndex + 1]);
    }
    */
    let newViewType;

    switch (viewType) {
        case viewTypeMap[1]:
            newViewType = viewTypeMap[3];
            break;
        case viewTypeMap[3]:
            newViewType = viewTypeMap[1];
            break;

        default:
            newViewType = viewTypeMap[1];
            break;
    }

    setView(newViewType);

    await updateMarkers();
}

async function newExperienceClick() {
    window.location.href = "/InsertExperience" + (morningNight ? "?MorningNightEnum=" + morningNightMap.findIndex(x => x == morningNight) : "");
}

function startWorldView() {
    isFirstHomeView = false;
    document.getElementById('world-body-container').scrollIntoView({ behavior: 'smooth' });
    setWorldView();

    if (isFirstWorldView) {
        if (param_userId) {
            changeView();
        } else {
            updateMarkers();
        }

        isFirstWorldView = false;
    }
}
function setWorldView() {
    document.body.classList.remove("home-view");
    document.body.classList.add("world-view");
}
function startHomeView() {
    //document.body.scrollIntoView({ behavior: 'smooth' });
    window.scrollTo({top: 0, behavior: 'smooth'});
    setHomeView();
    closePopup("home-popup");
    isFirstHomeView = false;
}
function setHomeView() {
    document.body.classList.remove("world-view");
    document.body.classList.add("home-view");
    closeExperiencePlacesClick();
}
function setHomeWorldViewSize() {
    let wrapperHeight = document.body.offsetHeight;
    let wrapperWidth = document.body.offsetWidth;
    //wrapper.style.height = (wrapperHeight + 100) + "px";
    //wrapper.style.width = (wrapperWidth + 350) + "px";
    /*if(document.getElementById("user-selected-body-container")) {
        document.getElementById("user-selected-body-container").style.height = (wrapperHeight) + "px";
        document.getElementById("user-selected-body-container").style.width = (wrapperWidth) + "px";
    }*/
    if(document.getElementById("home-body-container")) {
        document.getElementById("home-body-container").style.height = (wrapperHeight) + "px";
        document.getElementById("home-body-container").style.width = (wrapperWidth) + "px";
    }
    if(document.getElementById("world-body-container")) {
        document.getElementById("world-body-container").style.height = (wrapperHeight) + "px";
        document.getElementById("world-body-container").style.width = (wrapperWidth) + "px";
    }
}
//#endregion

//#region markers
async function updateMarkers() {
    try {
        setAppStatus(appStatusMap[1]);
        //await startRotate();
        stopPreviewMode();

        // container, value
        let placeListContainer = document.getElementById("place-list-container");
        let experienceListContainer = document.getElementById("experience-list-container");
        let userListContainer = document.getElementById("user-list-container");

        let placeListSubcontainer = placeListContainer.querySelector(".list-container");
        let experienceListSubcontainer = experienceListContainer.querySelector(".list-container");
        let userListSubcontainer = userListContainer.querySelector(".list-container");

        let placeListFlagcontainer = placeListContainer.querySelector(".flag-container");
        let experienceListFlagcontainer = experienceListContainer.querySelector(".flag-container");

        let morningNightEnumValue = morningNight ? morningNightMap.indexOf(morningNight) : 0;
        let searchTextValue = document.getElementById("experience-search").value;
        let searchButton = document.getElementById("search-button");


        // reset
        resetMarkers();

        // notification filter
        if (searchTextValue) {
            searchButton.classList.add("more");
        } else {
            searchButton.classList.remove("more");
        }

        // list, flag, text
        let placeMediaList;
        let experienceMediaList;
        let userList;

        let isToDo;
        let toDoText = "";
        let morningNightText = "";



        // ToDo
        if (param_userId && viewType == viewTypeMap[3]) {
            isToDo = true;
            toDoText = "[ToDo]";
        }

        // morning night enum
        morningNightText = morningNight && morningNight > morningNightMap[0] ? "[" + morningNight + "]" : "";


        // setto gli header flag
        placeListFlagcontainer.innerHTML = toDoText + " " + morningNightText;
        experienceListFlagcontainer.innerHTML = toDoText + " " + morningNightText;

        //loading
        placeListSubcontainer.innerHTML = "<small class='noresult'>Loading...</small>";
        experienceListSubcontainer.innerHTML = "<small class='noresult'>Loading...</small>";
        userListSubcontainer.innerHTML = "<small class='noresult'>Loading...</small>";


        // get PLACES
        placeMediaList = await GetPlaceList(morningNightEnumValue, searchTextValue, isToDo, true, 0, 5);
        placeListSubcontainer.innerHTML = "";
        if (!placeMediaList || placeMediaList.length <= 0) {
            placeListSubcontainer.innerHTML += "<small class='noresult'>No <b>Places</b> found! Come on! Go and explore the world! Try to search in <a href='/Search/Places'>Search Global Places</a></small>";
        } else {
            addMarkers(placeMediaList, placeListSubcontainer, 0, isToDo);
        }


        // get EXPERIENCES
        experienceMediaList = await GetExperienceList(null, morningNightEnumValue, isToDo, searchTextValue, false, true, 0, 5);
        experienceListSubcontainer.innerHTML = "";
        if (!experienceMediaList || experienceMediaList.length <= 0) {
            experienceListSubcontainer.innerHTML += "<small class='noresult'>No <b>Experiences</b> found! Come on! Go and explore the world! Try to search in <a href='/Search/Experiences'>Search Global Experiences</a></small>";
        } else {
            addMarkers(experienceMediaList, experienceListSubcontainer, 1, isToDo);
        }


        // get PEOPLE
        if (param_userId) {
            userList = await GetUserFriendListByUser(param_userId, searchTextValue, false, true, 0, 5);
        } else {
            userList = await GetUserList(searchTextValue, false, true, 0, 5);
        }
        userListSubcontainer.innerHTML = "";
        if (!userList || userList.length <= 0) {
            userListSubcontainer.innerHTML += "<small class='noresult'>There is no <b>People</b> with this description! Try to search in <a href='/Search/People'>Search Global People</a></small>";
        } else {
            addMarkers(userList, userListSubcontainer, 2, isToDo);
        }


        //stopRotate(markerList[0]);
        if (markerList[0]) {
            // myearth.goTo(markerList[0].location, { relativeDuration: 100, approachAngle: 5 });

            if (window.innerWidth > screenWidthLimitMax || (window.innerWidth <= screenWidthLimitMax && !document.body.classList.contains("place-list-open"))) {
                await openPhoto(markerList[0], markerList[0].isToDo, markerList[0].type);
                window.dispatchEvent(new CustomEvent('startPreviewMode'));
            } else {
                myearth.goTo(markerList[0].location, { relativeDuration: 100, approachAngle: 5 });
            }
        }
        setAppStatus(appStatusMap[2]);
    } catch (error) {
        document.getElementById("place-list-container").innerHTML = "<small class='noresult error-color'>There is an error! I don t know why! Try to <a href='javascript:window.location.reload(true)' title='Reload page'>reload the page</a></small>";

        //stopRotate();
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
function resetMarkers() {
    closePhoto();

    let placeListContainer = document.getElementById("place-list-container");
    let experienceListContainer = document.getElementById("experience-list-container");
    let userListContainer = document.getElementById("user-list-container");


    // Resetto i markers
    markerList.forEach((marker) => {
        marker.removeEventListener('click');
        marker.animate('scale', 0, { duration: 200 });
        marker.remove();
    });
    markerList = [];

    // Resetto la list
    placeList.forEach((place) => {
        place.removeEventListener('click', () => { });
    });
    placeList = [];

    // resetto la lista html
    placeListContainer.querySelector(".list-container").innerHTML = "";
    experienceListContainer.querySelector(".list-container").innerHTML = "";
    userListContainer.querySelector(".list-container").innerHTML = "";

    placeListContainer.querySelector(".flag-container").innerHTML = "";
    experienceListContainer.querySelector(".flag-container").innerHTML = "";
    placeListContainer.querySelector(".flag-container").innerHTML = "";
}
function addMarkers(mediaList, listContainer, type, isToDo) {
    // type = 0 -> place
    // type = 1 -> experience
    // type = 2 -> people

    // add photo pins
    for (let i = 0; i < mediaList.length; i++) {
        let media = mediaList[i];
        //let maxLength_description = 50;
        let place;
        let marker;
        let markerColor = media.isMine || media.isFriend ? "#1E90FF" : "#ea5a57";

        // creo oggetto place
        if (type == 0) {
            place = {
                id: media.id,
                name: media.name,
                description: media.description,
                longitude: media.longitude,
                latitude: media.latitude,
                pictureId: media.pictureId,
            };
        } else if (type == 1) {
            place = {
                id: media.placeId,
                name: media.placeName,
                description: media.placeDescription,
                longitude: media.placeLongitude,
                latitude: media.placeLatitude,
                pictureId: media.fileId,
            };
        } else if (type == 2) {
            place = {
                id: media.residencePlaceId,
                name: media.residencePlaceName,
                description: media.residencePlaceDescription,
                longitude: media.residencePlaceLongitude,
                latitude: media.residencePlaceLatitude,
                pictureId: media.profilePhotoFileId,
            };
        }

        // inserisco marker
        marker = myearth.addMarker({
            mesh: "Marker",
            color: markerColor,
            location: { lat: place.latitude, lng: place.longitude },
            scale: 0.01,
            offset: 1.6,
            visible: place.id ? true : false,
            transparent: true,
            hotspotRadius: 0.6,
            hotspotHeight: 1.25,
            //useLookAngle: true,

            // custom property
            photoInfo: media,
            place: place,
            isToDo: isToDo,
            type: type
        });

        marker.addEventListener('click', () => {
            stopPreviewMode();
            openPhoto(marker, isToDo, type);
        });

        // animate marker
        setTimeout(() => {
            marker.animate('scale', markerScale, { duration: 200 });
            marker.animate('offset', 0, { duration: 1100, easing: 'bounce' });
        }, 200 * i);

        markerList.push(marker);

        // set the list
        let placeItem = document.createElement("div");
        placeItem.classList.add("hidden");
        placeItem.classList.add("item");

        if (type == 0) {
            placeItem.dataset.placeid = media.id;
            placeItem.innerHTML += "<div class='place-name'><b>" + media.name + "</b></div>";
        } else if (type == 1) {
            let datetime = media.date;
            if (media.time) {
                datetime += ", " + media.time;
            }
            if (media.statusEnum != 1) {
                placeItem.classList.add("deactive");
            }

            placeItem.dataset.experienceid = media.id;
            placeItem.innerHTML += "<div class='experience-name'><b>" + media.name + "</b></div>";
            placeItem.innerHTML += "<address class='d-block experience-place'>" + media.placeName + "</address>";
            if (param_userId) {
                placeItem.innerHTML += "<small><time class='d-block experience-date' datetime='" + datetime + "'>" + datetime + "</time></small>";
            } else {
                placeItem.innerHTML += "<div class='d-block experience-user'>" + media.userNickname + "</div>";
            }
        } else if (type == 2) {
            if (media.statusEnum != 1 || (param_userId && media.friendStatusEnum != 1)) {
                placeItem.classList.add("deactive");
            }
            placeItem.dataset.userid = media.id;
            placeItem.innerHTML += "<div class='user-nickname'><b>" + media.nickname + "</b></div>";
            placeItem.innerHTML += "<div class='user-name'>" + media.name + " " + media.surname + "</div>";
            if (media.residencePlaceName) placeItem.innerHTML += "<address class='d-block user-residence'>" + media.residencePlaceName + "</address>";
            if (media.birthDate) placeItem.innerHTML += "<small><time class='d-block user-birthdate' datetime='" + media.birthDate + "'>" + media.birthDate + "</time></small>";
        }
        listContainer.append(placeItem);

        // click
        placeItem.addEventListener('click', () => {
            stopPreviewMode();
            openPhoto(marker, isToDo, type);
            if (window.innerWidth <= screenWidthLimitMax) closeExperiencePlacesClick();
        });
        placeList.push(placeItem);

        // effect list display
        setTimeout(() => {
            placeItem.classList.remove("hidden");
        }, 200 * i);
    }
}
//#endregion


//#region placelist menu 
function toggleExperiencePlacesClick() {
    document.body.classList.toggle("place-list-open");

    /*if (document.body.classList.contains("place-list-open")) {
        document.getElementById("experience-search").focus();
        document.getElementById("experience-search").select();
    } else {
        document.getElementById("experience-search").blur();
    }*/
    if (!document.body.classList.contains("place-list-open")) {
        document.getElementById("experience-search").blur();
    }
}
function closeExperiencePlacesClick() {
    document.body.classList.remove("place-list-open");

    document.getElementById("experience-search").blur();
}
function openExperiencePlacesClick() {
    document.body.classList.add("place-list-open");

    //document.getElementById("experience-search").focus();
    //document.getElementById("experience-search").select();
}
//#endregion


//#region photo overlay
async function setPhoto(photoInfo, photoPlace, isToDo, type) {
    let photo = document.querySelector('#photo');
    let mediaContainer = photo.querySelector("#photo-media-container");
    let mediaTitle = photo.querySelector('#photo-title');

    mediaContainer.classList.add("loading");

    try {
        resetPhoto();

        if (photo) {
            // get the media and visualize
            let file;
            let fileSrc;
            if (type == 0) {
                mediaTitle.innerHTML = "<img src='/images/icons/place.svg' alt='place' class='t-size mr-05' />" + photoPlace.name;
                /*file = await GetDefaultExperienceMediaByPlace(photoPlace.id, morningNight ? morningNightMap.indexOf(morningNight) : 0, isToDo);
                fileSrc = await GetExperienceMedia(file.id, true);
                */
                file = {
                    id: null,
                    fileType: "image/",
                    fileName: photoPlace.name
                };
                fileSrc = await GetDefaultPlacePicture(photoPlace.id, true);
            } else if (type == 1) {
                mediaTitle.innerHTML = "<img src='/images/icons/experiencemorning.svg' alt='experience' class='t-size mr-05' />" + photoInfo.name;
                file = {
                    id: photoInfo.fileId,
                    fileType: photoInfo.fileType,
                    fileName: photoInfo.fileName
                };
                fileSrc = await GetExperienceMedia(file.id, true);
            } else if (type == 2) {
                mediaTitle.innerHTML = "<img src='/images/icons/user_avatar_default.svg' alt='person' class='t-size mr-05' />" + photoInfo.nickname;
                if (photoInfo.profilePhotoFileId) {
                    file = {
                        id: photoInfo.profilePhotoFileId,
                        fileType: photoInfo.profilePhotoFileType,
                        fileName: photoInfo.profilePhotoFileName
                    };
                    fileSrc = await GetUserPicture(file.id, true); 
                }
            }
            
            if (!file || !fileSrc) {
                throw "Media not exists";
            } else if (file.fileType.startsWith("image/")) {
                let img = document.createElement("img");
                img.classList.add("overlay-media");
                img.setAttribute("id", 'overlay-img');
                img.setAttribute("alt", 'media');
                img.onerror = (e) => {
                    onPhotoError(mediaContainer, type, 1, e);
                }
                img.onload = (e) => {
                    onPhotoSuccess(mediaContainer, type, 1, e);
                }
                img.setAttribute("src", fileSrc);

                //mediaContainer.innerHTML = "<img src='" + fileSrc + "' alt='media' onerror='onPhotoError(this.parentElement)' class='overlay-media' id='overlay-img'/>";
            }
            else if (file.fileType.startsWith("video/")) {
                let video = document.createElement("video");
                video.classList.add("overlay-media");
                video.setAttribute("id", 'overlay-video');
                video.setAttribute("alt", 'media');
                video.autoplay = true;
                video.muted = true;
                video.onerror = (e) => {
                    onPhotoError(mediaContainer, type, 2, e);
                }
                video.onloadeddata = (e) => {
                    onPhotoSuccess(mediaContainer, type, 2, e);
                }
                video.setAttribute("src", fileSrc);
                
                //mediaContainer.innerHTML = "<video src='" + fileSrc + "' autoplay muted onerror='onPhotoError(this.parentElement)' class='overlay-media' id='overlay-video'></video>";
            }
            else if (file.fileType.startsWith("audio/")) {
                let audio = document.createElement("audio");
                audio.classList.add("overlay-media");
                audio.setAttribute("id", 'overlay-audio');
                audio.setAttribute("alt", 'media');
                audio.autoplay = true;
                audio.muted = true;
                audio.onerror = (e) => {
                    onPhotoError(mediaContainer, type, 3, e);
                }
                audio.onloadeddata = (e) => {
                    onPhotoError(mediaContainer, type, 3, e);
                }
                audio.setAttribute("src", fileSrc);

                //mediaContainer.innerHTML = "<audio src='" + fileSrc + "' autoplay muted onerror='onPhotoError(this.parentElement)' class='overlay-media' id='overlay-audio'></audio>";
            } else {
                throw "Media not supported";
            }
        }
    } catch (error) {
        onPhotoError(mediaContainer, type);
        throw error;
    }
}
function onPhotoError(container, type, fileType, event) {
    // fileType:
    // 1 -> image
    // 2 -> video
    // 3 -> audio
    
    if (container) {
        if (type == 2) {
            container.innerHTML = "<img src='/images/icons/user_avatar_default.svg' alt='person' class='overlay-media' id='overlay-error' />";
        } else if(fileType == 3) {
            container.innerHTML = "<img src='/images/icons/audio_default.svg' alt='audio' class='overlay-media' id='overlay-error' />";
        } else {
            container.innerHTML = "<img src='/images/icons/media_default.svg' alt='experience media error' class='overlay-media' id='overlay-error' />";
        }
        setTimeout(() => {
            container.classList.remove("loading");
        }, 200);
    }
}
function onPhotoSuccess(container, type, fileType, event) {
    if (container) {
        container.appendChild(event.target);
        setTimeout(() => {
            container.classList.remove("loading");
        }, 200);
    }
}
async function openPhoto(marker, isToDo, type, isPreviewMode) {
    // rotate earth
    startOpeningPhoto();
    if (!isPreviewMode) myearth.goTo(marker.location, { relativeDuration: 100/*, approachAngle: 12 */ });
    //myearth.animate( 'zoom', 2, { relativeDuration: 100 } );

    // close list of place
    /*if (window.innerWidth <= screenWidthLimitMax && !isPreviewMode) {
        closeExperiencePlacesClick();
    }*/

    // set marker
    if (marker != currentMarker) {
        let photo = document.querySelector('.earth-overlay #photo');

        // close current photo
        if (currentMarker) {
            await closePhoto();
        }

        setPhoto(marker.photoInfo, marker.place, isToDo, type);

        // animate marker
        marker.animate('scale', 0, { duration: 200 });

        // animate overlay
        mediaOverlay.location = marker.location;
        mediaOverlay.visible = true;


        currentMarker = marker;
        if (type == 0) {
            document.querySelector("#place-list-container .item[data-placeid='" + marker.photoInfo.id + "']").classList.add("active");
        } else if (type == 1) {
            document.querySelector("#experience-list-container .item[data-experienceid='" + marker.photoInfo.id + "']").classList.add("active");
        } else if (type == 2) {
            document.querySelector("#user-list-container .item[data-userid='" + marker.photoInfo.id + "']").classList.add("active");
        }

        // appear photo
        await sleep(200);
        photo.classList.add('photo-appear');
    }

    // stop autorotate
    if (!isPreviewMode) myearth.autoRotate = false;
}
async function closePhotoClick(event) {
    event.stopPropagation();
    stopPreviewMode();
    closePhoto();
    window.dispatchEvent(new CustomEvent('startPreviewMode'));
}
async function closePhoto() {
    if (!currentMarker) return;
    startClosingPhoto();

    //reset photo
    resetPhoto();
    if (document.querySelector("#place-list-container .item.active")) {
        document.querySelector("#place-list-container .item.active").classList.remove("active");
    }
    if (document.querySelector("#experience-list-container .item.active")) {
        document.querySelector("#experience-list-container .item.active").classList.remove("active");
    }
    if (document.querySelector("#user-list-container .item.active")) {
        document.querySelector("#user-list-container .item.active").classList.remove("active");
    }

    // animate overlay
    await sleep(200);
    mediaOverlay.visible = false;

    // animate marker
    if (currentMarker) {
        currentMarker.animate('scale', markerScale, { duration: 200 });
        currentMarker.opacity = 0.7;
        currentMarker = false;
    }

    // animate earth
    //myearth.animate( 'zoom', 1, { relativeDuration: 100 } );
    myearth.autoRotate = true;
    /*
    // fix no autorotate start in mobile
    myearth.dragging = false;   
    myearth.autoRotateStart = true;
    */
}
function photoClick(event) {
    let photo = document.querySelector('#photo');
    if (photo.querySelector("#overlay-video")) photo.querySelector("#overlay-video").pause();
    if (photo.querySelector("#overlay-audio")) photo.querySelector("#overlay-audio").pause();
    let href;

    if (currentMarker.type == 0) {
        href = viewUri + "Place?PlaceId=" + currentMarker.photoInfo.id;
        if (param_userId) href += "&UserId=" + param_userId;
        if (viewType) href += "&ViewTypeEnum=" + viewTypeMap.findIndex(x => x == viewType);
        if (morningNight) href += "&MorningNightEnum=" + morningNightMap.findIndex(x => x == morningNight);
    } else if (currentMarker.type == 1) {
        href = viewUri + "Experience?ExperienceId=" + currentMarker.photoInfo.id;
    } else if (currentMarker.type == 2) {
        href = viewUri + "User?UserId=" + currentMarker.photoInfo.id;
    }

    window.location.href = href;
}
function resetPhoto() {
    let photo = document.querySelector('#photo');

    if (photo) {
        //mediaOverlay.animate('scale', 0, { duration: 200 });
        photo.classList.remove('photo-appear');
        photo.style.backgroundImage = null;
        photo.querySelector('#photo-title').innerHTML = "";
        photo.querySelector('#photo-media-container').innerHTML = "";
    }
}
//#endregion
