//#region events onload
var isInfiniteScrollExperiencesEnabled = true;
var infiniteScrollExperiencesOffset = 0;
var infiniteScrollExperiencesCount = 10;

window.addEventListener("myOnPageLoad", async () => {
    // debounce 
    document.getElementById("experiences-search").addEventListener('input', debounce(infiniteScrollSearchExperiences, 300));

    // init method to call
    try {
        // first random search to show something
        if (param_userId) {
            changeView();
        } else {
            infiniteScrollSearchExperiences();
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
async function searchExperiences() { 
    document.getElementById("experiences-list-container").innerHTML = "";
    infiniteScrollExperiencesOffset = 0;
    isInfiniteScrollExperiencesEnabled = true;
    await addSearchExperiences();
}
async function addSearchExperiences() {
    isInfiniteScrollExperiencesEnabled = false;

    let experienceListContainer = document.getElementById("experiences-list-container");
    let experienceSearchInput = document.getElementById("experiences-search");

    let noResult = document.createElement("small");
    noResult.classList.add("noresult");
    noResult.classList.add("p-1");
    noResult.innerHTML = "Loading...";
    experienceListContainer.appendChild(noResult);

    let experienceList = await GetExperienceList(null, morningNight, viewType == viewTypeMap[3], experienceSearchInput.value, true, false, infiniteScrollExperiencesOffset, infiniteScrollExperiencesCount);
    //if (param_userId) document.getElementById("experience-count").innerHTML = "Total: " + experienceList.length;

    if (experienceListContainer.querySelector(".noresult")) experienceListContainer.querySelector(".noresult").remove();

    if (experienceList.length > 0) {
        var params = "";
        if (param_userId) params += "&UserId=" + param_userId;
        if (morningNight) params += "&MorningNightEnum=" + morningNightMap.findIndex(x => x == morningNight);
        if (viewType) params += "&ViewTypeEnum=" + viewTypeMap.findIndex(x => x == viewType);

        for (let i = 0; i < experienceList.length; i++) {
            let experience = experienceList[i];

            let experienceItem = document.createElement("div");
            //experienceItem.classList.add("hidden");
            experienceItem.classList.add("button");
            experienceItem.classList.add("item");
            if (experience.statusEnum != 1) experienceItem.classList.add("deactive");
            if (experience.isMine) experienceItem.classList.add("ismine");
            //experienceItem.href = viewUri + "Experience?ExperienceId=" + experience.id + params;
            experienceItem.setAttribute("onclick", "location.href='" + viewUri + "Experience?ExperienceId=" + experience.id + params + "'");

            let content = "<div class='experience-picture-container'>";
            content += "<div class='experience-picture-content-container loading'></div>";

            if (experience.privacyLevel == 2) {
                content += "<div><small class='privacylevel-tag'>Only Friends</small></div>";
            } else if (experience.privacyLevel == 3) {
                content += "<div><small class='privacylevel-tag'>Private</small></div>";
            }/* else {
                content += "<div><small class='privacylevel-tag'>Public</small></div>"; 
            }*/
            content += "</div>";
            content += "<div class='flex-auto o-hidden'>";
            content += "<strong class='d-block experience-name'>" + experience.name + "</strong>";
            if (experience.placeName) {
                content += "<address class='d-block experience-place'>" + experience.placeName + "</address>";
            }
            if (param_userId) {
                let datetime;
                if (experience.date && experience.time) datetime = experience.date + ", " + experience.time;
                else if (experience.date && !experience.time) datetime = experience.date;
                else if (!experience.date && experience.time) datetime = experience.time;

                if (datetime) content += "<small><time class='d-block experience-datatime' datetime='" + datetime + "'>" + datetime + "</time></small>";
            } else {
                content += "<div class='d-block experience-user'>" + experience.userNickname + "</div>";
            }
            if (experience.description) {
                content += "<small class='experience-description " + (experience.description.length > 150 ? "description-height-limit" : "") + "'>" + experience.description + "</small>";
            }
            content += "</div>";
            experienceItem.innerHTML = content;

            experienceListContainer.appendChild(experienceItem);
            addSearchExperiencesPicture(experience, experienceItem);

            /*
            setTimeout(() => {
                experienceItem.classList.remove("hidden");
            }, 200 * i);
            */
        }

        infiniteScrollExperiencesOffset += infiniteScrollExperiencesCount;
        isInfiniteScrollExperiencesEnabled = true;
    } else {
        let noResult = document.createElement("small");
        noResult.classList.add("noresult");
        noResult.classList.add("p-1");
        noResult.innerHTML = "The Experience you are looking for maybe doesn t exist!";
        experienceListContainer.appendChild(noResult);
        isInfiniteScrollExperiencesEnabled = false;
    }
}
async function addSearchExperiencesPicture(experience, experienceItem) {
    let container = experienceItem.querySelector('.experience-picture-content-container');

    try {
        if (experience.fileId) {
            let file = await GetExperienceMedia(experience.fileId, true);

            if (experience.fileType.startsWith("image/")) {
                let img = document.createElement("img");
                img.classList.add("experience-picture");
                img.classList.add("img");
                img.setAttribute("alt", 'experience media');
                img.onerror = (e) => {
                    onMediaError(container, 1, e);
                }
                img.onload = (e) => {
                    onMediaSuccess(container, 1, e);
                }
                img.setAttribute("src", file);

                //content += "<img src='" + file + "' onerror='onMediaError(this.parentElement)' class='experience-picture img' alt='experience media' />";
            }
            else if (experience.fileType.startsWith("video/")) {
                let video = document.createElement("video");
                video.classList.add("experience-picture");
                video.classList.add("video");
                video.autoplay = true;
                video.muted = true;
                video.onerror = (e) => {
                    onMediaError(container, 2, e);
                }
                video.onloadeddata = (e) => {
                    onMediaSuccess(container, 2, e);
                }
                video.setAttribute("src", file);

                //container.innerHTML += "<video autoplay muted src='" + file + "' onerror='onMediaError(this.parentElement)' class='experience-picture video'></video>";
            }
            else if (experience.fileType.startsWith("audio/")) {
                let audio = document.createElement("audio");
                audio.classList.add("experience-picture");
                audio.classList.add("audio");
                audio.autoplay = true;
                audio.muted = true;
                audio.onerror = (e) => {
                    onMediaError(container, 3, e);
                }
                audio.onloadeddata = (e) => {
                    onMediaError(container, 3, e);
                }
                audio.setAttribute("src", file);

                //container.innerHTML += "<audio autoplay muted src='" + file + "' onerror='onMediaError(this.parentElement)' class='experience-picture audio'></audio>";
            } else {
                onMediaError(container);
            }
        } else {
            let imgDefault = document.createElement("img");
            imgDefault.classList.add("experience-picture");
            imgDefault.setAttribute("src", '/images/icons/media_default.svg');
            imgDefault.setAttribute("alt", 'experience media not exists');
            container.appendChild(imgDefault);

            container.classList.remove("loading");
        }
    } catch (error) {
        onMediaError(container);
        throw error;
    }
}
function onMediaError(container, fileType, event) {
    if (container.querySelector(".experience-picture.video")) container.querySelector(".experience-picture.video").remove();
    if (container.querySelector(".experience-picture.audio")) container.querySelector(".experience-picture.audio").remove();
    if (container.querySelector(".experience-picture.img")) container.querySelector(".experience-picture.img").remove();

    let imgError = document.createElement("img");
    imgError.classList.add("experience-picture");
    imgError.setAttribute("alt", 'experience media error');
    if (fileType == 3) {
        imgError.setAttribute("src", '/images/icons/audio_default.svg');
    } else {  
        imgError.setAttribute("src", '/images/icons/media_default.svg');
    }
    container.appendChild(imgError);

    setTimeout(() => {
        container.classList.remove("loading");
    }, 200);
}
function onMediaSuccess(container, fileType, event) {
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

        infiniteScrollSearchExperiences();

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

    infiniteScrollSearchExperiences();
}
//#endregion


//#region infinitescroll
async function infiniteScrollSearchExperiences() {
    await searchExperiences();
    while (document.documentElement.offsetHeight >= document.documentElement.scrollHeight && isInfiniteScrollExperiencesEnabled) {
        await addSearchExperiences();
    }
    document.removeEventListener("scroll", infiniteScrollEndCallExperiences, false);
    document.addEventListener("scroll", infiniteScrollEndCallExperiences, false);
}
async function infiniteScrollEndCallExperiences() {
    if (isInfiniteScrollExperiencesEnabled) {
        let container = document.documentElement;
        if (container.scrollTop + container.offsetHeight + 100 > container.scrollHeight) {
            await addSearchExperiences();
        }
    }
}
//#endregion