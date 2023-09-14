//#region events onload
var isInfiniteScrollPlacesEnabled = true;
var infiniteScrollPlacesOffset = 0;
var infiniteScrollPlacesCount = 10;

window.addEventListener("myOnPageLoad", async () => {
    // debounce 
    document.getElementById("places-search").addEventListener('input', debounce(infiniteScrollSearchPlaces, 300));

    // init method to call
    try {
        // first random search to show something
        if (param_userId) {
            changeView();
        } else {
            infiniteScrollSearchPlaces();
        }

        // set app to ready
        setAppStatus(appStatusMap[2]);
    } catch (error) {
        setAppStatus(appStatusMap[3]);
        throw error;
    }
});
//#endregion


//#region search places
async function searchPlaces() {
    document.getElementById("places-list-container").innerHTML = "";
    infiniteScrollPlacesOffset = 0;
    isInfiniteScrollPlacesEnabled = true;
    await addSearchPlaces();
}
async function addSearchPlaces() {
    isInfiniteScrollPlacesEnabled = false;

    let placeListContainer = document.getElementById("places-list-container");
    let placeSearchInput = document.getElementById("places-search");

    let noResult = document.createElement("small");
    noResult.classList.add("noresult");
    noResult.classList.add("p-1");
    noResult.innerHTML = "Loading...";
    placeListContainer.appendChild(noResult);

    let placeList = await GetPlaceList(morningNight, placeSearchInput.value, viewType == viewTypeMap[3], false, infiniteScrollPlacesOffset, infiniteScrollPlacesCount);
    //if (param_userId) document.getElementById("place-count").innerHTML = "Total: " + placeList.length;

    if(placeListContainer.querySelector(".noresult")) placeListContainer.querySelector(".noresult").remove();

    if (placeList.length > 0) {
        var params = "";
        if (param_userId) params += "&UserId=" + param_userId;
        if (morningNight) params += "&MorningNightEnum=" + morningNightMap.findIndex(x => x == morningNight);
        if (viewType) params += "&ViewTypeEnum=" + viewTypeMap.findIndex(x => x == viewType);

        for (let i = 0; i < placeList.length; i++) {
            let place = placeList[i];

            let placeItem = document.createElement("div");
            //placeItem.classList.add("hidden");
            placeItem.classList.add("button");
            placeItem.classList.add("item");
            // if (place.statusEnum != 1) placeItem.classList.add("deactive");
            if (place.isMine) placeItem.classList.add("ismine");
            //placeItem.href = viewUri + "Place?PlaceId=" + place.id + params;
            placeItem.setAttribute("onclick", "location.href='" + viewUri + "Place?PlaceId=" + place.id + "'");
            let content = "<div class='place-picture-content-container loading'></div>";
            content += "<div class='flex-auto o-hidden'>";
            content += "<strong class='d-block place-name'>" + place.name + "</strong>";
            if (place.description) content += "<small class='place-description " + (place.description.length > 150 ? "description-height-limit" : "") + "'>" + place.description + "</small>";
            content += "</div>";
            placeItem.innerHTML = content;

            placeListContainer.appendChild(placeItem);
            addSearchPlacesPicture(place, placeItem);
            /*
            setTimeout(() => {
                placeItem.classList.remove("hidden");
            }, 200 * i);
            */
        }

        infiniteScrollPlacesOffset += infiniteScrollPlacesCount;
        isInfiniteScrollPlacesEnabled = true;
    } else {
        let noResult = document.createElement("small");
        noResult.classList.add("noresult");
        noResult.classList.add("p-1");
        noResult.innerHTML = "The Place you are looking for is still undiscovered! You can be the first one!";
        placeListContainer.appendChild(noResult);
        isInfiniteScrollPlacesEnabled = false;
    }
}
async function addSearchPlacesPicture(place, placeItem) {
    let container = placeItem.querySelector('.place-picture-content-container');

    try {
        let file = await GetDefaultPlacePicture(place.id, true);

        let img = document.createElement("img");
        img.classList.add("place-picture");
        img.setAttribute("alt", 'place picture');
        img.onerror = (e) => {
            onMediaError(container, e);
        }
        img.onload = (e) => {
            onMediaSuccess(container, e);
        }
        img.setAttribute("src", file);
    } catch (error) {
        onMediaError(container);
        throw error;
    }
}
function onMediaError(container, event) {
    let imgDefault = document.createElement("img");
    imgDefault.classList.add("place-picture");
    imgDefault.setAttribute("src", '/images/icons/place_picture_default.svg');
    imgDefault.setAttribute("alt", 'place picture not exists');
    container.appendChild(imgDefault);

    setTimeout(() => {
        container.classList.remove("loading");
    }, 200);
}
function onMediaSuccess(container, event) {
    container.appendChild(event.target);
    setTimeout(() => {
        container.classList.remove("loading");
    }, 200);
}
//#endregion


//#region change enum
async function changeMorningNight() {
    try {
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

        infiniteScrollSearchPlaces();

    } catch (error) {
        if (!error.Status || error.Status == 500) {
            openPopupError("There is an error! I don t know why. Please, try again or <a href='javascript:window.location.reload(true)' title='Reload page'>reload the page</a>");
        } else if (error.Status == 400) {
            openPopupError(error.ErrorMessage);
        }
        throw error;
    }
}
function changeView() {
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

    infiniteScrollSearchPlaces();
}
//#endregion


//#region infinitescroll
async function infiniteScrollSearchPlaces() {
    await searchPlaces();
    while (document.documentElement.offsetHeight >= document.documentElement.scrollHeight && isInfiniteScrollPlacesEnabled) {
        await addSearchPlaces();
    }
    document.removeEventListener("scroll", infiniteScrollEndCallPlaces, false);
    document.addEventListener("scroll", infiniteScrollEndCallPlaces, false);
}
async function infiniteScrollEndCallPlaces() {
    if (isInfiniteScrollPlacesEnabled) {
        let container = document.documentElement;
        if (container.scrollTop + container.offsetHeight + 100 > container.scrollHeight) {
            await addSearchPlaces();
        }
    }
}
//#endregion