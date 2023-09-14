// media cache  ES. -> experienceMediaId: mediaUrl
var experienceMediaCache = {};
var userProfilePictureCache = {};
var placeProfilePictureCache = {};
var experienceMediaPreviewCache = {};
var userProfilePicturePreviewCache = {};
var placeProfilePicturePreviewCache = {};

//#region API Calls
// apiActionName -> name of method to call
// apiMethod -> 'GET', 'POST'...
// params -> object like {nameParameter: valueParameter, nameParameter2: valueParameter2}
// responseType -> can be json, blob...
async function apiCall(apiActionName, apiMethod, params, responseType) {
    try {
        // create url and init
        let url = new URL(apiUri + apiActionName);
        let myInit = {
            method: apiMethod,
            credentials: 'include'
        };

        // params
        switch (apiMethod) {
            case "GET":
                // add params
                if (params) Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));

                break;
            case "POST":
                // add params form data
                let paramsForm = new FormData();
                if (params) Object.keys(params).forEach(key => paramsForm.append(key, params[key]));
                myInit.body = paramsForm;

                break;

            default:
                break;
        }

        // call method
        if (responseType == 'blob') {
            return url;
        } else {

            let response = await fetch(url, myInit);
            if (response.ok) {
                let data;

                switch (responseType) {
                    case 'json':
                        data = await response.json();
                        break;
                    case 'blob':
                        data = await response.blob();
                        break;

                    default:
                        data = await response;
                        break;
                }

                // response
                // console.log(url.href, params, data);
                return data;
            } else {
                // throw response.statusText;

                var errorResponse = await response.json();
                errorResponse.Status = response.status;
                let errorMessage;
                switch (errorResponse.Status) {
                    case 400:

                        break;
                    case 401:
                        errorMessage = errorResponse.ErrorMessage;
                        errorMessage += "<br/><br/>Please, <a href='/Login?Redirect=" + window.location.href + "' title='Login'>Login</a>";
                        setAppStatus(appStatusMap[3], errorMessage);
                        break;
                    case 404:
                        errorMessage = errorResponse.ErrorMessage;
                        errorMessage += "<br/><br/>Please, <a href='/' title='Home'>Go Home</a>";
                        setAppStatus(appStatusMap[3], errorMessage);
                        break;
                    case 500:

                        break;

                    default:

                        break;
                }

                throw errorResponse;

            }
        }

    } catch (error) {
        // console.error(error);
        throw error;
    }
}
async function CheckLogin() {
    try {
        let result = await apiCall("CheckLogin", "GET", null, 'json');
        userLogged = result;

        // add ismine user
        if (param_userId && userLogged && isEquals(param_userId, userLogged.id)) {
            isUserMine = true;
            document.body.classList.add("ismine");
        }

        setUserStatus(userStatusMap[1]);
        return result;
    } catch (e) {
        setUserStatus(userStatusMap[2]);
        throw e;
    }
}
async function Login(username, password, isRememberMe) {
    try {
        let params = { username: username, password: password, isRememberMe: isRememberMe };
        let result = await apiCall("Login", "POST", params, 'json');
        userLogged = result;

        setUserStatus(userStatusMap[1]);
        return result;
    } catch (e) {
        setUserStatus(userStatusMap[2]);
        throw e;
    }
}
async function Logout() {
    try {
        let result = await apiCall("Logout", "POST", null, 'json');
        userLogged = null;

        setUserStatus(userStatusMap[2]);
    } catch (e) {
        throw e;
    }
}

async function GetUserList(searchText, isAlsoNotActive = false, isRandom = false, takeOffset, takeCount) {
    try {
        let params = {
            isAlsoNotActive: isAlsoNotActive
        };
        if (searchText) params.searchText = searchText;
        if (isRandom) params.isRandom = isRandom;
        if (takeOffset) params.takeOffset = takeOffset;
        if (takeCount) params.takeCount = takeCount;
        return await apiCall("GetUserList", "GET", params, "json");
    } catch (e) {
        throw e;
    }
}
async function GetUser(userId) {
    try {
        let params = {
            userId: userId
        };
        return await apiCall("GetUser", "GET", params, "json");
    } catch (e) {
        throw e;
    }
}
async function GetUserPicture(pictureId, isPreview = false) {
    if (!isPreview && userProfilePictureCache[pictureId]) {
        return userProfilePictureCache[pictureId];
    } else if (isPreview && userProfilePicturePreviewCache[pictureId]) {
        return userProfilePicturePreviewCache[pictureId];
    } else {
        try {
            let params = {
                pictureId: pictureId,
                isPreview: isPreview
            };

            let userPicture = await apiCall("GetUserPicture", "GET", params, "blob");
            if (!isPreview) {
                userProfilePictureCache[pictureId] = userPicture;
            } else {
                userProfilePicturePreviewCache[pictureId] = userPicture;
            }
            return userPicture;
        } catch (e) {
            throw e;
        }
    }
}
async function GetUserToDoListByExperience(experienceId, searchText, takeOffset, takeCount) {
    try {
        let params = {
            experienceId: experienceId,
        };
        if (searchText) params.searchText = searchText;
        if (takeOffset) params.takeOffset = takeOffset;
        if (takeCount) params.takeCount = takeCount;
        return await apiCall("GetUserToDoListByExperience", "GET", params, "json");
    } catch (e) {
        throw e;
    }
}
async function GetUserFriendListByUser(userId, searchText, isAlsoNotActive, isRandom, takeOffset, takeCount) {
    try {
        let params = {
            userId: userId
        };
        if (searchText) params.searchText = searchText;
        if (isAlsoNotActive) params.isAlsoNotActive = isAlsoNotActive;
        if (isRandom) params.isRandom = isRandom;
        if (takeOffset) params.takeOffset = takeOffset;
        if (takeCount) params.takeCount = takeCount;
        return await apiCall("GetUserFriendListByUser", "GET", params, "json");
    } catch (e) {
        throw e;
    }
}
async function GetCurrentUserFriend(userId) {
    try {
        let params = {
            userId: userId
        };
        return await apiCall("GetCurrentUserFriend", "GET", params, "json");
    } catch (e) {
        throw e;
    }
}
async function InsertUser(nickname, name, surname, profilePhotoFile, description, birthdate, residencePlaceName, residencePlaceDescription, residencePlaceLatitude, residencePlaceLongitude, mail, password) {
    try {
        let params = {
            nickname: nickname,
            name: name,
            surname: surname,
            mail: mail,
            password: password
        };
        if (profilePhotoFile) params.profilePhotoFile = profilePhotoFile;
        if (description) params.description = description;
        if (birthdate) params.birthdate = birthdate;

        if (residencePlaceName) params.residencePlaceName = residencePlaceName;
        if (residencePlaceDescription) params.residencePlaceDescription = residencePlaceDescription;
        if (residencePlaceLatitude) params.residencePlaceLatitude = residencePlaceLatitude;
        if (residencePlaceLongitude) params.residencePlaceLongitude = residencePlaceLongitude;

        return await apiCall("InsertUser", "POST", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function UpdateUser_PersonalInformations(userId, name, surname, profilePhotoFileId, profilePhotoFile, description, birthdate, residencePlaceId, residencePlaceName, residencePlaceDescription, residencePlaceLatitude, residencePlaceLongitude) {
    try {
        let params = {
            userId: userId,
            name: name,
            surname: surname
        };
        if (profilePhotoFileId) params.profilePhotoFileId = profilePhotoFileId;
        if (residencePlaceId) params.residencePlaceId = residencePlaceId;

        if (profilePhotoFile) params.profilePhotoFile = profilePhotoFile;
        if (residencePlaceName) params.residencePlaceName = residencePlaceName;
        if (residencePlaceDescription) params.residencePlaceDescription = residencePlaceDescription;
        if (residencePlaceLatitude) params.residencePlaceLatitude = residencePlaceLatitude;
        if (residencePlaceLongitude) params.residencePlaceLongitude = residencePlaceLongitude;
        if (description) params.description = description;
        if (birthdate) params.birthdate = birthdate;

        return await apiCall("UpdateUser_PersonalInformations", "POST", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function UpdateUser_Password(password, passwordNew) {
    try {
        let params = {
            password: password,
            passwordNew: passwordNew
        };

        return await apiCall("UpdateUser_Password", "POST", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function UpdateUser_Mail(password, mailNew) {
    try {
        let params = {
            password: password,
            mailNew: mailNew
        };

        return await apiCall("UpdateUser_Mail", "POST", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function RequestUser_Contact(userId, message) {
    try {
        let params = {
            userId: userId,
            message: message
        };

        return await apiCall("RequestUser_Contact", "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}

async function GetPlaceList(morningNightEnum, searchText, isToDo, isRandom = false, takeOffset, takeCount) {
    let apiName = "GetPlaceList";

    try {
        let params = {};
        if (searchText) params.searchText = searchText;
        if (morningNightEnum) params.morningNightEnum = morningNightEnum;
        if (isToDo) params.isToDo = isToDo;
        if (param_userId) params.userId = param_userId;
        if (isRandom) params.isRandom = isRandom;
        if (takeOffset) params.takeOffset = takeOffset;
        if (takeCount) params.takeCount = takeCount;

        return await apiCall(apiName, "GET", params, "json");
    } catch (e) {
        console.error(e)
        throw e;
    }
}
async function GetPlacePicture(pictureId, isPreview = false) {
    if (!isPreview && placeProfilePictureCache[pictureId]) {
        return placeProfilePictureCache[pictureId];
    } else if (isPreview && placeProfilePicturePreviewCache[pictureId]) {
        return placeProfilePicturePreviewCache[pictureId];
    } else {
        try {
            let params = {
                pictureId: pictureId,
                isPreview: isPreview
            };

            let placePicture = await apiCall("GetPlacePicture", "GET", params, "blob");
            if (!isPreview) {
                placeProfilePictureCache[pictureId] = placePicture;
            } else {
                placeProfilePicturePreviewCache[pictureId] = placePicture;
            }
            return placePicture;
        } catch (e) {
            throw e;
        }
    }
}
async function GetDefaultPlacePicture(placeId, isPreview = true) {
    try {
        let params = {
            placeId: placeId,
            isPreview: isPreview
        };
        return await apiCall("GetDefaultPlacePicture", "GET", params, "blob");
    } catch (e) {
        throw e;
    }
}
async function GetExperienceMedia(mediaId, isPreview = false) {
    if (!isPreview && experienceMediaCache[mediaId]) {
        return experienceMediaCache[mediaId];
    } else if (isPreview && experienceMediaPreviewCache[mediaId]) {
        return experienceMediaPreviewCache[mediaId];
    } else {
        let apiName = "GetExperienceMedia";

        try {
            let params = {
                mediaId: mediaId,
                isPreview: isPreview
            };

            let experienceMedia = await apiCall(apiName, "GET", params, 'blob', true);
            if (!isPreview) {
                experienceMediaCache[mediaId] = experienceMedia;
            } else {
                experienceMediaPreviewCache[mediaId] = experienceMedia;
            }
            return experienceMedia;
        } catch (e) {
            throw e;
        }
    }
}
async function GetDefaultExperienceMediaByPlace(placeId, morningNightEnum, isToDo) {
    let apiName = "GetDefaultExperienceMediaByPlace";

    try {
        let params = {
            placeId: placeId,
            morningNightEnum: morningNightEnum
        };
        if (isToDo) params.isToDo = isToDo;
        if (param_userId) params.userId = param_userId;

        /*let media = await apiCall(apiName, "GET", params, 'json', true);
        return await GetExperienceMedia(media.id, true, isToDo);
        */
        return await apiCall(apiName, "GET", params, 'json', true);
    } catch (e) {
        throw e;
    }
}
async function UpsertExperience(experienceId, date, time, name, description, morningNightEnum, privacyLevel, fileId, file, placeId, placeName, placeDescription, placeLatitude, placeLongitude, isDraft) {
    try {
        let params = {
            name: name,
            statusEnum: isDraft ? 0 : 1
        };
        if (experienceId) params.id = experienceId;
        if (fileId) params.fileId = fileId;
        if (placeId) params.placeId = placeId;

        if (date) params.date = date;
        if (description) params.description = description;
        if (morningNightEnum) params.morningNightEnum = morningNightEnum;
        if (privacyLevel) params.privacyLevel = privacyLevel;
        if (file) params.file = file;
        if (placeName) params.placeName = placeName;
        if (placeDescription) params.placeDescription = placeDescription;
        if (placeLatitude) params.placeLatitude = placeLatitude;
        if (placeLongitude) params.placeLongitude = placeLongitude;

        if (time) params.time = time;

        return await apiCall("UpsertExperience", "POST", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function DeleteExperience(experienceId) {
    try {
        let params = {
            experienceId: experienceId
        };
        return await apiCall("DeleteExperience", "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function GetExperienceList(placeId, morningNightEnum, isToDo, searchText, isAlsoDraft = false, isRandom = false, takeOffset, takeCount) {
    let apiName = "GetExperienceList";

    try {
        let params = {};
        if (searchText) params.searchText = searchText;
        if (morningNightEnum) params.morningNightEnum = morningNightEnum;
        if (placeId) params.placeId = placeId;
        if (isToDo) params.isToDo = isToDo;
        if (param_userId) params.userId = param_userId;
        if (isAlsoDraft) params.isAlsoDraft = isAlsoDraft;
        if (isRandom) params.isRandom = isRandom;
        if (takeOffset) params.takeOffset = takeOffset;
        if (takeCount) params.takeCount = takeCount;

        return await apiCall(apiName, "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function GetExperience(experienceId) {
    let apiName = "GetExperience";

    try {
        let params = { "experienceId": experienceId };
        return await apiCall(apiName, "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}

async function AddUserExperienceToDo(experienceId) {
    let apiName = "AddUserExperienceToDo";

    try {
        let params = { "experienceId": experienceId };
        return await apiCall(apiName, "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function RemoveUserExperienceToDo(experienceId) {
    let apiName = "RemoveUserExperienceToDo";

    try {
        let params = { "experienceId": experienceId };
        return await apiCall(apiName, "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}

async function AddUserFriend(userFriendId) {
    let apiName = "AddUserFriend";

    try {
        let params = { "userFriendId": userFriendId };
        return await apiCall(apiName, "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function RemoveUserFriend(userFriendId) {
    let apiName = "RemoveUserFriend";

    try {
        let params = { "userFriendId": userFriendId };
        return await apiCall(apiName, "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function ConfirmUserFriend(userFriendId) {
    let apiName = "ConfirmUserFriend";

    try {
        let params = { "userFriendId": userFriendId };
        return await apiCall(apiName, "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
//#endregion