
function isEmailValid(email) {
    if (email != '') {
        var Regex = /^(?:\w+\.?)*\w+@(?:\w+\.)*\w+$/;
        return Regex.test(email);
    }
    else {
        return true;
    }
}