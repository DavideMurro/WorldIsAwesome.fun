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
            method: apiMethod
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
            throw response.statusText;
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
        setUserStatus(userStatusMap[1]);
        return result;
    } catch (e) {
        setUserStatus(userStatusMap[2]);
        throw e;
    }
}
async function Login(username, password) {
    try {
        let params = { username: username, password: password };
        let result = await apiCall("Login", "POST", params, 'json');
        userLogged = result;
        setUserStatus(userStatusMap[1]);
        return result;
    } catch (e) {
        setUserStatus(userStatusMap[2]);
        throw e;
    }
}
async function GetPlaceMediaList(lightNightEnum) {
    try {
        let params = { "morningNightEnum": lightNightEnum };
        return await apiCall("GetPlaceMediaList", "GET", params, "json");
    } catch (e) {
        throw e;
    }
}
async function GetMedia(mediaId) {
    try {
        let params = { "mediaId": mediaId };
        return await apiCall("GetMedia", "GET", params, 'blob');
    } catch (e) {
        throw e;
    }
}
async function GetDefaultMediaPlace(placeId) {
    try {
        let params = { "placeId": placeId };
        return await apiCall("GetDefaultMediaPlace", "GET", params, 'blob');
    } catch (e) {
        throw e;
    }
}
async function UpsertExperience(experienceId, dateTime, name, description, morningNightEnum, fileId, file, placeId, placeName, placeDescription, placeLatitude, placeLongitude) {
    try {
        let params = {
            dateTime: dateTime.toISOString(),
            name: name,
            description: description,
            morningNightEnum: morningNightEnum,
        };
        if (experienceId) params.id = experienceId;
        if (fileId) params.fileId = fileId;
        if (placeId) params.placeId = placeId;

        if (file) params.file = file;
        if (placeName) params.placeName = placeName;
        if (placeDescription) params.placeDescription = placeDescription;
        if (placeLatitude) params.placeLatitude = placeLatitude;
        if (placeLongitude) params.placeLongitude = placeLongitude;

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
async function GetExperienceListByPlace(placeId) {
    try {
        let params = {
            placeId: placeId
        };
        return await apiCall("GetExperienceListByPlace", "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
async function GetExperience(experienceId) {
    try {
        let params = { "experienceId": experienceId };
        return await apiCall("GetExperience", "GET", params, 'json');
    } catch (e) {
        throw e;
    }
}
//#endregion

