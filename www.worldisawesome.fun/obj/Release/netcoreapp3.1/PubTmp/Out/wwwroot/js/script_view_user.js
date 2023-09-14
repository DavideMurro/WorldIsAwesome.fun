
//#region login / user
//logout
async function logout() {
    openPopup("logout-confirm-popup");
}
async function logoutSubmit() {
    setAppStatus(appStatusMap[1]);

    try {
        await Logout();
        window.location.href = "/";
    } catch (error) {
        // throw error;
        window.location.href = "/";
    }
}

// edit password
async function openProfileUserPopup_editPassword() {
    openPopup("user-profile-password-form-popup");
}
function closeUserProfilePasswordFormPopup() {
    resetUserProfilePasswordFormPopup();

    closePopup("user-profile-password-form-popup");
}
function resetUserProfilePasswordFormPopup() {
    document.getElementById("user-credentials-password-id").value = "";
    document.getElementById("user-credentials-password").value = "";
    document.getElementById("user-credentials-passwordnew").value = "";
    document.getElementById("user-credentials-passwordnewrepeat").value = "";

    resetUserProfilePasswordFormPopup_errors();
}
function resetUserProfilePasswordFormPopup_errors() {
    document.getElementById("user-credentials-password-error").innerHTML = "";

    document.getElementById("user-credentials-password-container").classList.remove("error");
    document.getElementById("user-credentials-passwordnew-container").classList.remove("error");
    document.getElementById("user-credentials-passwordnewrepeat-container").classList.remove("error");
}
async function userProfilePasswordSubmit(event) {
    event.preventDefault();

    let isFormValid = false;
    setAppStatus(appStatusMap[1]);
    resetUserProfilePasswordFormPopup_errors();
    let passwordInput = document.getElementById("user-credentials-password");
    let passwordInputContainer = document.getElementById("user-credentials-password-container");
    let passwordNewInput = document.getElementById("user-credentials-passwordnew");
    let passwordNewInputContainer = document.getElementById("user-credentials-passwordnew-container");
    let passwordNewRepeatInput = document.getElementById("user-credentials-passwordnewrepeat");
    let passwordNewRepeatInputContainer = document.getElementById("user-credentials-passwordnewrepeat-container");

    try {
        if (!(passwordInput.value)) {
            passwordInput.focus();
            passwordInputContainer.classList.add("error");
            throw "Password doesn t set";
        }
        if (!(passwordNewInput.value && passwordNewInput.value.length >= 8)) {
            passwordNewInput.focus();
            passwordNewInputContainer.classList.add("error");
            throw "Password doesn t set or is shorter then 8 chars";
        }
        if (!(passwordNewRepeatInput.value && passwordNewRepeatInput.value == passwordNewInput.value)) {
            passwordNewRepeatInput.focus();
            passwordNewRepeatInputContainer.classList.add("error");
            throw "Password doesn t set or doesn t match with the password above";
        }
        isFormValid = true;
    } catch (error) {
        isFormValid = false;
    }

    if (isFormValid) {
        try {
            await UpdateUser_Password(passwordInput.value, passwordNewInput.value);
            closeUserProfilePasswordFormPopup();
            openPopupSuccess("Password changed!! Now log back in!", "/Login");
            await Logout();
            setAppStatus(appStatusMap[2]);
        } catch (error) {
            if (!error.Status || error.Status == 500) {
                document.getElementById("user-credentials-password-error").innerHTML = "Something is wrong, my friend. Try in a few minutes";
            } else if (error.Status == 400) {
                document.getElementById("user-credentials-password-error").innerHTML = error.ErrorMessage;
            }
            if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
            throw error;
        }
    }

    if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
}

// edit mail
function openProfileUserPopup_editMail() {
    openPopup("user-profile-mail-form-popup");
}
function closeUserProfileMailFormPopup() {
    resetUserProfileMailFormPopup();

    closePopup("user-profile-mail-form-popup");
}
function resetUserProfileMailFormPopup() {
    document.getElementById("user-credentials-mail-id").value = "";
    document.getElementById("user-credentials-mail-password").value = "";
    document.getElementById("user-credentials-mailnew").value = "";

    resetUserProfileMailFormPopup_errors();
}
function resetUserProfileMailFormPopup_errors() {
    document.getElementById("user-credentials-mail-error").innerHTML = "";

    document.getElementById("user-credentials-mail-password-container").classList.remove("error");
    document.getElementById("user-credentials-mailnew-container").classList.remove("error");
}
async function userProfileMailSubmit(event) {
    event.preventDefault();

    let isFormValid = false;
    setAppStatus(appStatusMap[1]);
    resetUserProfileMailFormPopup_errors();
    let passwordInput = document.getElementById("user-credentials-mail-password");
    let passwordInputContainer = document.getElementById("user-credentials-mail-password-container");
    let mailNewInput = document.getElementById("user-credentials-mailnew");
    let mailNewInputContainer = document.getElementById("user-credentials-mailnew-container");

    try {
        if (!(mailNewInput.value && (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mailNewInput.value)))) {
            mailNewInput.focus();
            mailNewInputContainer.classList.add("error");
            throw "New mail doesn t set or is not formatted as a mail";
        }
        if (!(passwordInput.value)) {
            passwordInput.focus();
            passwordInputContainer.classList.add("error");
            throw "Password doesn t set";
        }
        isFormValid = true;
    } catch (error) {
        isFormValid = false;
    }

    if (isFormValid) {
        try {
            await UpdateUser_Mail(passwordInput.value, mailNewInput.value);
            closeUserProfileMailFormPopup();
            openPopupSuccess("Mail changed!! Now confirm your new mail and log back in!", "/Login");
            await Logout();
            // openPopup("login-popup");
            setAppStatus(appStatusMap[2]);
        } catch (error) {
            if (!error.Status || error.Status == 500) {
                document.getElementById("user-credentials-mail-error").innerHTML = "Something is wrong, my friend. Try in a few minutes";
            } else if (error.Status == 400) {
                document.getElementById("user-credentials-mail-error").innerHTML = error.ErrorMessage;
            }
            if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
            throw error;
        }
    }

    if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
}
//#endregion

//#region CONTACT ME
function contactmeClick() {
    openPopup("contactme-confirm-popup").then(() => {
        document.getElementById("contactme-message").focus();
        document.getElementById("contactme-message").select();
    });
}
async function contactmeSubmit() {
    try {
        setAppStatus(appStatusMap[1]);
        await RequestUser_Contact(param_userId, document.querySelector("#contactme-confirm-popup #contactme-message").value);
        closeContactmePopup("contactme-confirm-popup");
        setAppStatus(appStatusMap[2]);
        openPopup("contactme-success");
    } catch (error) {
        let errorContainer = document.querySelector("#contactme-confirm-popup #contactme-confirm-error");
        if (!error.Status || error.Status == 500) {
            errorContainer.innerHTML = "I can't contact this user! :(";
        } else if (error.Status == 400) {
            errorContainer.innerHTML = error.ErrorMessage;
        }
        errorContainer.scrollIntoView();
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
async function closeContactmePopup() {
    closePopup("contactme-confirm-popup");
    document.querySelector("#contactme-confirm-popup #contactme-confirm-error").innerHTML = "";
    document.querySelector("#contactme-confirm-popup #contactme-message").value = "";
}
//#endregion

//#region FRIENDS
async function removeFriendClick() {
    openPopup("remove-friend-confirm-popup");
}
async function closeRemoveFriendPopup() {
    closePopup("remove-friend-confirm-popup");
}
async function removeFriendSubmit() {
    try {
        setAppStatus(appStatusMap[1]);
        await RemoveUserFriend(param_userId);
        closeRemoveFriendPopup();
        setAppStatus(appStatusMap[2]);
        window.location.reload();
    } catch (error) {
        closeRemoveFriendPopup();
        if (!error.Status || error.Status == 500) {
            openPopupError("I can't remove this friend! :(");
        } else if (error.Status == 400) {
            openPopupError(error.ErrorMessage);
        }
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
async function addFriendClick() {
    try {
        setAppStatus(appStatusMap[1]);
        await AddUserFriend(param_userId);
        setAppStatus(appStatusMap[2]);
        window.location.reload();
    } catch (error) {
        if (!error.Status || error.Status == 500) {
            openPopupError("I can't add this friend! :(");
        } else if (error.Status == 400) {
            openPopupError(error.ErrorMessage);
        }
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
async function confirmFriendClick() {
    try {
        setAppStatus(appStatusMap[1]);
        await ConfirmUserFriend(param_userId);
        setAppStatus(appStatusMap[2]);
        window.location.reload();
    } catch (error) {
        if (!error.Status || error.Status == 500) {
            openPopupError("I can't confirm this friend! :(");
        } else if (error.Status == 400) {
            openPopupError(error.ErrorMessage);
        }
        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);
        throw error;
    }
}
//#endregion

