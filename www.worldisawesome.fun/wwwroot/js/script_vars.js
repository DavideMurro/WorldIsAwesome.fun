//const siteUrl = 'https://localhost:5001/';
const siteUrl = window.location.origin + "/";
const apiUri = siteUrl + 'Api/';
const viewUri = siteUrl + 'View/';
const urlCreator = window.URL || window.webkitURL;

// screen size
const screenWidthLimitMax = 767;

// http parameters GET
const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);
const param_userId = urlParams.get('UserId');
const param_placeId = urlParams.get('PlaceId');
const param_experienceId = urlParams.get('ExperienceId');
const param_morningNightEnum = urlParams.get('MorningNightEnum');
const param_viewTypeEnum = urlParams.get('ViewTypeEnum');
const param_redirect = urlParams.get('Redirect');

// myInitialized
const myPageLoadEvent = new CustomEvent('myOnPageLoad');
const myInitCompletedEvent = new CustomEvent('myOnInitCompleted');
var isPageLoad = false;
var isInitCompleted = false;

var isHomeWorldViewScrollEnable = true;
var isFirstWorldView = true;
var isFirstHomeView = true;


// vars
var appStatus;
var appStatusMap = [
    "Default",
    "Loading",
    "Ready",
    "Error"
];

var userLogged;
var userStatus;
var userStatusMap = [
    "Default",
    "Logged",
    "NotLogged"
];

var currentUser;
var isUserMine;
var currentUserFriend;

var placeList = [];
var currentPlace;
var isPlaceMine;

//var experienceList = [];
var currentExperience;
var isExperienceMine;


var myearth;
var autoRotate;
var markerList = [];
var currentMarker;
var markerScale = 1;


var viewType;
var viewTypeMap = [
    "Default",
    "ViewMyPlaces",
    "ViewGlobalPlaces",
    "ViewExperiencesToDo",
    "ViewFriends"
];


// 0 -> Default
// 1 -> Morning
// 2 -> Night
// 3 -> Evening
var morningNight;
var morningNightMap = [
    "Default",
    "Morning",
    "Night",
    "Evening"
];




var editor_configuration = {
    buttonList: [
        ['undo', 'redo'],
        [/*'font',*/ 'fontSize'/*, 'formatBlock'*/],
        /*['paragraphStyle', 'blockquote'],*/
        ['bold', 'underline', 'italic', 'strike'/*, 'subscript', 'superscript'*/],
        ['fontColor', 'hiliteColor'/*, 'textStyle'*/],
        //['removeFormat'],
        //'/', // Line break
        //['outdent', 'indent'],
        ['align', 'horizontalRule', 'list', 'lineHeight'],
        ['table', 'link', 'image', 'video', 'audio' /** ,'math' */], // You must add the 'katex' library at options to use the 'math' plugin.
        /** ['imageGallery'] */ // You must add the "imageGalleryUrl".
        /*['fullScreen', 'showBlocks', 'codeView'],
        ['preview', 'print'],
        ['save', 'template']*/
    ],
    width: '100%',
    minHeight: '20em',
    stickyToolbar: null,
    showPathLabel: false,
    charCounter: true,
    charCounterType: 'byte-html', // char byte byte-html
    charCounterLabel: 'Size :',
    maxCharCount: 1000000
};


//#region events onload
window.addEventListener("load", async () => {
    // add hammer if exists
    try {
        let popupContainerElements = document.getElementsByClassName("popup-container");
        Array.from(popupContainerElements).forEach(popupContainerElement => {
            let popupContainersHammer = new Hammer(popupContainerElement);
            popupContainersHammer.on("tap", (ev) => {
                //closePopup(ev.target.id);
                if (ev.target.classList.contains("popup-container") && ev.target.onclose) {
                    ev.target.onclose();
                }
            });
        });
    } catch (error) {
        console.warn("Hammerjs doesn t exists or error");
    }

    // myInitialized
    isPageLoad = true;
    window.dispatchEvent(myPageLoadEvent);
});
//#endregion


var debounce = (func, delay) => {
    let debounceTimer
    return function () {
        let context = this;
        let args = arguments;
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(() => func.apply(context, args), delay);
    }
}

var capitalize = (s) => {
    if (typeof s !== 'string') return '';
    return s.charAt(0).toUpperCase() + s.slice(1);
}

async function sleep(timeout) {
    return new Promise(resolve => {
        setTimeout(resolve, timeout);
    });
}

function isEquals(a, b, isCaseSensitive = false) {
    if (isCaseSensitive) {
        return a === b;
    } else {
        return typeof a === 'string' && typeof b === 'string'
            ? a.localeCompare(b, undefined, { sensitivity: 'accent' }) === 0
            : a === b;
    }
}


//#region USER STATUS
function setUserStatus(newUserStatus) {

    userStatusMap.forEach(us => {
        document.body.classList.remove(us.toLowerCase());
    });

    document.body.classList.add(newUserStatus.toLowerCase());
    userStatus = newUserStatus;
}
//#endregion


//#region APP STATUS
function setAppStatus(newAppStatus, text) {
    document.querySelector("#app-status-container #app-status-error-container").innerHTML = "";
    document.querySelector("#app-status-container #app-status-loading-container").innerHTML = "";

    appStatusMap.forEach(as => {
        document.body.classList.remove(as.toLowerCase());
    });

    // aggiungo l errore
    if (newAppStatus == appStatusMap[3]) {
        document.querySelector("#app-status-container #app-status-error-container").innerHTML = text ? text : "There is an error. <br/> <a href='/' title='Home'>Go Home</a>";
    } else if (newAppStatus == appStatusMap[1]) {
        document.querySelector("#app-status-container #app-status-loading-container").innerHTML = text ? text : "";
    }

    document.body.classList.add(newAppStatus.toLowerCase());
    appStatus = newAppStatus;
}
//#endregion


//#region MORNING NIGHT
function setMorningNight(newMorningNight) {
    morningNightMap.forEach(ln => {
        document.body.classList.remove(ln.toLowerCase());
    });

    document.body.classList.add(newMorningNight.toLowerCase());
    morningNight = newMorningNight;
}
//#endregion


//#region VIEW TYPE
function setView(vtNew) {
    viewTypeMap.forEach(vt => {
        document.body.classList.remove(vt.toLowerCase());
    });

    if (vtNew && viewTypeMap.findIndex(x => x == vtNew) > 0) {
        document.body.classList.add(vtNew.toLowerCase());
        viewType = vtNew;
    }
}
//#endregion


//#region POPUPS
async function openPopup(id) {
    let popup = document.getElementById(id);
    popup.classList.add("active");
    await sleep(200);
    //myearth.draggable = false;
    document.body.classList.add("popup-active");
    return true;
}
async function closePopup(id) {
    let popup = document.getElementById(id);
    popup.classList.remove("active");
    await sleep(200);
    //myearth.draggable = true;
    document.body.classList.remove("popup-active");
    return true;
}
async function closeAllPopup() {
    let popupList = document.getElementsByClassName("popup-container");
    for (let index = 0; index < popupList.length; index++) {
        let popup = popupList[index];
        popup.classList.remove("active");
    }
    document.body.classList.remove("popup-active");
    return true;
}
function isPopupOpen(id) {
    let popup = document.getElementById(id);
    return popup.classList.contains("active");
}

async function openPopupError(error) {
    document.querySelector("#error-popup .popup-body").innerHTML = error;
    openPopup('error-popup').then(() => {
        document.getElementById("error-ok-button").focus();
    });
    return true;
}
async function openPopupSuccess(seccessText, buttonOkLink) {
    document.querySelector("#success-popup .popup-body").innerHTML = "";
    document.getElementById("success-ok-button").onclick = null;

    document.querySelector("#success-popup .popup-body").innerHTML = seccessText;
    if (buttonOkLink) {
        document.getElementById("success-ok-button").onclick = () => { window.location.href = buttonOkLink };
    }

    openPopup('success-popup').then(() => {
        document.getElementById("success-ok-button").focus();
    });

    return true;
}
//#endregion


//#region search location 
async function searchLocation(containerId, onSelectCallback) {
    let placeAutocomplete = document.querySelector("#" + containerId + " .input");
    let resultContainer = document.querySelector("#" + containerId + " .search-results-container");
    let searchRemoveButton = document.querySelector("#" + containerId + " .remove-button");

    if (!resultContainer) {
        let item = document.createElement("div");
        item.className = "search-results-container";
        document.getElementById(containerId).appendChild(item);
        resultContainer = document.querySelector("#" + containerId + " .search-results-container");
    }
    resultContainer.classList.remove("d-none");
    resultContainer.innerHTML = "<small class='search-noresult'>Loading...</small>";
    if (searchRemoveButton) searchRemoveButton.classList.add("d-none");

    try {

        let response = await fetch("https://api.teleport.org/api/cities/?search=" + placeAutocomplete.value/* + "&limit=5"*/);
        data = await response.json();

        if (data._embedded["city:search-results"].length <= 0) {
            resultContainer.innerHTML = "<small class='search-noresult'>No results! Where did you go??</small>";
        } else {
            resultContainer.innerHTML = "";
            data._embedded["city:search-results"].forEach((result, index) => {
                let item = document.createElement("label");
                item.setAttribute("for", "search-result-location" + index);
                item.className = "search-result";
                item.innerHTML = "<input type='radio' name='search-result-location' id='search-result-location" + index + "' value='" + result._links["city:item"].href + "' >" + result.matching_full_name;
                item.addEventListener("change", (e) => { selectResultLocation(e.target, containerId, onSelectCallback) });
                resultContainer.appendChild(item);
            });
        }
    } catch (error) {
        resultContainer.innerHTML = "<small class='search-noresult error-color'>There s an error! I don t know why. Please try again in a few minutes</small>";
        throw error;
    }
}
async function selectResultLocation(input, containerId, onSelectCallback) {
    let searchResultContainer = document.querySelector("#" + containerId + " .search-results-container");
    let searchResultList = document.querySelectorAll("#" + containerId + " .search-result");
    let searchInput = document.querySelector("#" + containerId + " .input");
    let searchRemoveButton = document.querySelector("#" + containerId + " .remove-button");
    searchResultList.forEach(result => {
        result.classList.remove("active");
    });
    input.parentElement.classList.add("active");

    searchInput.value = input.parentElement.textContent;
    searchResultContainer.classList.add("d-none");
    if (searchRemoveButton) searchRemoveButton.classList.remove("d-none");
    if (onSelectCallback) onSelectCallback();
}
//#endregion
