
//#region login 
async function loginSubmit(event) {
    event.preventDefault();

    setAppStatus(appStatusMap[1]);
    document.getElementById("login-error").innerHTML = "";

    let usernameInput = document.getElementById("username");
    let passwordInput = document.getElementById("password");
    let isRememberMeInput = document.getElementById("isrememberme");
    try {
        let user = await Login(usernameInput.value, passwordInput.value, isRememberMeInput.checked);
        document.getElementById("login-error").innerHTML = "";
        document.getElementById("login-form").reset();
        // setAppStatus(appStatusMap[2]);
        
        if (param_redirect) {
            window.location.href = param_redirect;
        } else {
            window.location.href = "/?UserId="+user.id;
        }
    } catch (error) {
        let loginError = document.getElementById("login-error");
        if (!error.Status || error.Status == 500) {
            loginError.innerHTML = "Hey! There is something wrong! Try in a few minutes. If persists don't hesitate to <a href='/ContactUs' title='Contact us'>Contact us</a>";
        } else if (error.Status == 400) {
            loginError.innerHTML = error.ErrorMessage;
        }
        loginError.scrollIntoView();

        if (appStatus == appStatusMap[1]) setAppStatus(appStatusMap[2]);

        throw error;
    }
}
//#endregion

