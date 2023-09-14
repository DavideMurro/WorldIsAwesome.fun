//#region events onload
var isInfiniteScrollPeopleEnabled = true;
var infiniteScrollPeopleOffset = 0;
var infiniteScrollPeopleCount = 10;

window.addEventListener("myOnPageLoad", async () => {
    // debounce 
    document.getElementById("people-search").addEventListener('input', debounce(infiniteScrollSearchPeople, 300));

    // set params
    try {
        infiniteScrollSearchPeople();

        // set app to ready
        setAppStatus(appStatusMap[2]);
    } catch (error) {
        setAppStatus(appStatusMap[3]);
        throw error;
    }

});
//#endregion


//#region search people
async function searchPeople() {
    document.getElementById("people-list-container").innerHTML = "";
    infiniteScrollPeopleOffset = 0;
    isInfiniteScrollPeopleEnabled = true;
    await addSearchPeople();
}
async function addSearchPeople() {
    isInfiniteScrollPeopleEnabled = false;

    let userListContainer = document.getElementById("people-list-container");
    let userSearchInput = document.getElementById("people-search");

    let noResult = document.createElement("small");
    noResult.classList.add("noresult");
    noResult.classList.add("p-1");
    noResult.innerHTML = "Loading...";
    userListContainer.appendChild(noResult);

    let userList;
    if (param_viewTypeEnum && viewTypeMap[param_viewTypeEnum] == viewTypeMap[3]) {
        userList = await GetUserToDoListByExperience(param_experienceId, userSearchInput.value, infiniteScrollPeopleOffset, infiniteScrollPeopleCount);
    } else if (param_viewTypeEnum && viewTypeMap[param_viewTypeEnum] == viewTypeMap[4]) {
        userList = await GetUserFriendListByUser(param_userId, userSearchInput.value, true, false, infiniteScrollPeopleOffset, infiniteScrollPeopleCount);
    } else {
        userList = await GetUserList(userSearchInput.value, true, false, infiniteScrollPeopleOffset, infiniteScrollPeopleCount);
    }
    //if (param_userId) document.getElementById("user-count").innerHTML = "Total: " + userList.length;

    if(userListContainer.querySelector(".noresult")) userListContainer.querySelector(".noresult").remove();

    if (userList.length > 0) {
        for (let i = 0; i < userList.length; i++) {
            let user = userList[i];

            let userItem = document.createElement("div");
            userItem.dataset.userid = user.id;
            //userItem.classList.add("hidden");
            userItem.classList.add("button");
            userItem.classList.add("item");
            if (user.isMine || user.isFriend) userItem.classList.add("ismine");
            if (user.statusEnum != 1) userItem.classList.add("deactive");
            //userItem.href = viewUri + "User?UserId=" + user.id;
            userItem.setAttribute("onclick", "location.href='" + viewUri + "User?UserId=" + user.id + "'");
            let content = "<div class='user-picture-content-container loading'></div>";
            content += "<div class='flex-auto o-hidden'>";
            content += "<strong class='d-block user-nickname'>" + user.nickname + "</strong>";
            content += "<small class='d-block user-name'>" + user.name + " " + user.surname + "</small>";
            //content += "<small><time class='user-birthdate' datetime='" + user.date + "'>" + user.date + "</time></small>";
            if (user.residencePlaceName) {
                content += "<address class='d-block'>" + user.residencePlaceName + "</address>";
            }
            if (user.description) content += "<small class='user-description " + (user.description.length > 150 ? "description-height-limit" : "") + "'>" + user.description + "</small>";
            // friends
            if (user.friendStatusEnum == 0) {
                content += "<div class='user-pending-request-container mt-1'>";
                if (user.friendIsConfirmable) {
                    content += "<div>";
                    content += "<button class='button success mr-05' onclick='event.preventDefault(); event.stopPropagation(); acceptUserFriendRequest(\"" + user.id + "\")'>Accept</button>";
                    content += "<button class='button error' onclick='event.preventDefault(); event.stopPropagation(); declineUserFriendRequest(\"" + user.id + "\")'>Decline</button>";
                    content += "</div>";
                } else {
                    content += "<b>Pending Friend request...</b>";
                }
                content += "</div>";
            }
            content += "</div>";
            userItem.innerHTML = content;

            userListContainer.appendChild(userItem);
            addSearchPeoplePicture(user, userItem);

            /*
            setTimeout(() => {
                userItem.classList.remove("hidden");
            }, 200 * i);
            */
        }

        infiniteScrollPeopleOffset += infiniteScrollPeopleCount;
        isInfiniteScrollPeopleEnabled = true;
    } else {
        let noResult = document.createElement("small");
        noResult.classList.add("noresult");
        noResult.classList.add("p-1");
        noResult.innerHTML = "No more people here :(";
        userListContainer.appendChild(noResult);
        isInfiniteScrollPeopleEnabled = false;
    }
}
async function addSearchPeoplePicture(user, userItem) {
    let container = userItem.querySelector('.user-picture-content-container');

    try {
        if (user.profilePhotoFileId) {
            let file = await GetUserPicture(user.profilePhotoFileId, true);

            let img = document.createElement("img");
            img.classList.add("user-picture");
            img.setAttribute("alt", 'user picture');
            img.onerror = (e) => {
                onMediaError(container, e);
            }
            img.onload = (e) => {
                onMediaSuccess(container, e);
            }
            img.setAttribute("src", file);
        } else {
            onMediaError(container);
        }
    } catch (error) {
        onMediaError(container);
        throw error;
    }
}
function onMediaError(container, event) {
    let imgDefault = document.createElement("img");
    imgDefault.classList.add("user-picture");
    imgDefault.setAttribute("src", '/images/icons/user_avatar_default.svg');
    imgDefault.setAttribute("alt", 'user picture not exists');
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


//#region friends requests
async function acceptUserFriendRequest(userFriendId) {
    try {
        setAppStatus(appStatusMap[1]);
        await ConfirmUserFriend(userFriendId);
        document.querySelector("#people-list-container .item[data-userid='" + userFriendId + "'] .user-pending-request-container").remove();
        document.querySelector("#people-list-container .item[data-userid='" + userFriendId + "']").classList.add("ismine");
        setAppStatus(appStatusMap[2]);
    } catch (error) {
        setAppStatus(appStatusMap[2]);
        openPopupError("Can't accept this request :(");
        throw error;
    }
}
async function declineUserFriendRequest(userFriendId) {
    try {
        setAppStatus(appStatusMap[1]);
        await RemoveUserFriend(userFriendId);
        document.querySelector("#people-list-container .item[data-userid='" + userFriendId + "']").remove();
        setAppStatus(appStatusMap[2]);
    } catch (error) {
        setAppStatus(appStatusMap[2]);
        openPopupError("Can't decline this request");
        throw error;
    }
}
//#endregion


//#region infinitescroll
async function infiniteScrollSearchPeople() {
    await searchPeople();
    while (document.documentElement.offsetHeight >= document.documentElement.scrollHeight && isInfiniteScrollPeopleEnabled) {
        await addSearchPeople();
    }
    document.removeEventListener("scroll", infiniteScrollEndCallPeople, false);
    document.addEventListener("scroll", infiniteScrollEndCallPeople, false);
}
async function infiniteScrollEndCallPeople() {
    if (isInfiniteScrollPeopleEnabled) {
        let container = document.documentElement;
        if (container.scrollTop + container.offsetHeight + 100 > container.scrollHeight) {
            await addSearchPeople();
        }
    }
}
//#endregion