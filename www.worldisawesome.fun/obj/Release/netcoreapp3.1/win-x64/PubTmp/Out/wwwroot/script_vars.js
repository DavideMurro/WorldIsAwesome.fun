//const apiUri = 'http://localhost:5000/Api/';
const apiUri = 'http://davidemurro-001-site1.htempurl.com/Api/';
const urlCreator = window.URL || window.webkitURL;

var userLogged;
var userStatus;
var userStatusMap = [
    "Default",
    "Logged",
    "NotLogged"
];

var appStatus;
var appStatusMap = [
    "Default",
    "Loading",
    "Ready",
    "Error"
];

var myearth;
var autoRotate;
var markerList = [];
var currentMarker;

// 0 -> Default
// 1 -> Morning
// 2 -> Night
// 3 -> Evening
var lightNight;
var lightNightMap = [
    "Default",
    "Morning",
    "Night",
    "Evening"
];

var currentExperienceList;
var currentExperienceIndex;



//#region USER STATUS
function setUserStatus(newUserStatus) {
    let loginButton = document.getElementById("login-button");

    userStatusMap.forEach(us => {
        loginButton.classList.remove(us.toLowerCase());
    });

    loginButton.classList.add(newUserStatus.toLowerCase());
    userStatus = newUserStatus;

    switch (userStatus) {
        case userStatusMap[1]:
            document.querySelector("#login-button #user-logged").innerHTML = userLogged.name;
            document.getElementById("newexperience-button").classList.add("active");
            break;
        case userStatusMap[2]:
            document.querySelector("#login-button #user-logged").innerHTML = "";
            document.getElementById("newexperience-button").classList.remove("active");
            
            break;

        default:
            break;
    }
}
//#endregion


//#region APP STATUS
function setAppStatus(newAppStatus) {
    appStatusMap.forEach(as => {
        document.body.classList.remove(as.toLowerCase());
    });

    document.body.classList.add(newAppStatus.toLowerCase());
    appStatus = newAppStatus;
}
//#endregion


//#region LIGHT NIGHT
function setLightNight(newLightNight) {
    lightNightMap.forEach(ln => {
        document.body.classList.remove(ln.toLowerCase());
    });

    document.body.classList.add(newLightNight.toLowerCase());
    lightNight = newLightNight;
}
//#endregion


//#region POPUPS
async function openPopup(id) {
    let popup = document.getElementById(id);
    popup.style.opacity = 0;
    popup.classList.add("active");
    setTimeout(() => {
        popup.style.opacity = 1;
        return true;
    }, 200);

}
async function closePopup(id) {
    let popup = document.getElementById(id);
    popup.style.opacity = 1;
    popup.style.opacity = 0;
    setTimeout(() => {
        popup.classList.remove("active");
        return true;
    }, 200);
}
async function closeAllPopup() {
    let popupList = document.getElementsByClassName("popup-container");
    for (let index = 0; index < popupList.length; index++) {
        let popup = popupList[index];
        popup.classList.remove("active");
    }
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
//#endregion

/*
window.addEventListener("DOMContentLoaded", () => {
    // close popup when click outside
    Array.from(document.getElementsByClassName("popup-container")).forEach((popup) => {
        popup.addEventListener('click', (e) => {
            if (e.target.classList.contains("popup-container")) {
                closePopup(e.target.id);
            }
        });
    });
});
*/
