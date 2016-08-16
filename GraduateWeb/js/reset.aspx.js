//password validation fn
var isPasswordNotValid = function (str) {
    return str === null || str.length < 8;//if str null or not at least 8 chars we can say it's not valid  
};

//this will show notification
var notifyUser = function (str, handle) {
    $('body').snackbar({
        alive: 3000,
        content: '<a data-dismiss="snackbar" class="notification-click">OK</a><div class="snackbar-text">' + str + '</div>'
    });
    //if we have handle
    if (handle) {
        $('.notification-click').on('click', handle);
    }
};

//callback for our ajax request, for login
var callback = {
    onSuccess: function (response) {
        if (response.Code == 200) {
            notifyUser("your password changed successfully, you can login now", null);
            setInterval(function () {
                window.location.href = "/sign-in";
            }, 3000);
        } else {
            var str = response.Message;
            notifyUser(str, null);
        }
        toggleButtonChange();
        toggleProgress();
    },
    onError: function (err) {
        var str = err.toString();
        notifyUser(err, null);
        toggleButtonChange();
        toggleProgress();
    }
};

//handling state of progress
var toggleProgress = function () {
    var proggress = $('#ui_progress');
    var on = 'el-loading';
    var off = 'el-loading-done';
    var state = proggress.hasClass(off);
    if (!state) {
        proggress.removeClass(on);
        proggress.addClass(off);
    } else {
        proggress.removeClass(off);
        proggress.addClass(on);
    }
};

//handle state of button
var toggleButtonChange = function () {
    var $btn = $('#btnChange');
    var off = 'disabled';
    var isDisabled = $btn.hasClass(off);
    if (isDisabled) {
        $btn.removeClass(off);
    } else {
        $btn.addClass(off);
    }
};

//api constants
var constants = {
    Url: '/v1/endpoint/change-password',
    HttpMethod: 'POST',
    ContentType: 'application/json; charset=utf-8',
    DataType: 'json',
    Callback: callback
};
//where we do ajax request
var request = function (dataSet) {
    $.ajax({
        url: constants.Url,
        type: constants.HttpMethod,
        data: JSON.stringify(dataSet),
        contentType: constants.ContentType,
        dataType: constants.DataType,
        success: constants.Callback.onSuccess,
        failure: constants.Callback.onError
    });
};
//first
var first = function () {
    if ($('#ui_password').val().length >= 8) {
        return md5($('#ui_password').val()).toUpperCase();
    } else {
        notifyUser("password must be at least 8 chars length, try again", function () {
            $('#ui_password').focus();
        });
    }
    return null;
};
var second = function () {
    if ($('#ui_password_2').val().length >= 8) {
        return md5($('#ui_password_2').val()).toUpperCase();
    } else {
        notifyUser("password must be at least 8 chars length, try again", function () {
            $('#ui_password_2').focus();
        });
    }
    return null;
};

var hasUrlToken = function () {
    var array = window.location.href.split('/');
    return array[array.length - 1];
};

var isTokenValid = function () {
    var token = hasUrlToken();
    var regex = /^[A-F0-9]{32}$/; //our token will be 32 chars length and only contain A-F or 0-9 chars
    return regex.test(token);
};

//event handler
var changeEventHandler = function () {   
    if (first() != null && second() != null) {
        toggleProgress();
        toggleButtonChange();
        if (first() != second()) {
            notifyUser("the passwords you entered are not match, try again.", function () {
                $('#ui_password_2').focus();
            });
            toggleButtonChange();
            toggleProgress();
            return;
        }
        request({ ResetToken: hasUrlToken(), NewPassword: first() });
    }
};

$(document).ready(function () {
    if (!isTokenValid()) {
        window.location.href = '/sign-in';
    }
    $("#btnChange").on("click", changeEventHandler);
});
