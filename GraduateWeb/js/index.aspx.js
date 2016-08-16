//username validation fn
var isEmailNotValid = function(str) {
    var regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return str === null || !regex.test(str);//if str null or not match regex condition we can say it's not valid
};

//password validation fn
var isPasswordNotValid = function(str) {
    return str === null || str.length < 8;//if str null or not at least 8 chars we can say it's not valid  
};

//this will show notification
var notifyUser = function(str, handle) {
    $('body').snackbar({
        alive: 3000,
        content: '<a data-dismiss="snackbar" class="notification-click">OK</a><div class="snackbar-text">' + str + '</div>'
    });
    //if we have handle
    if(handle) {
        $('.notification-click').on('click', handle);
    }
};

//callback for our ajax request, for login
var callback = {
    onSuccess: function(response) {
        if(response.Code == 200) {
            var token = response.Data.Token;
			storeCookie('X-Access-Token', token);
			window.location.href = '/search';
        } else {
            var str = response.Message;
            notifyUser(str, null);
        }
        toggleButtonSignin();
        toggleProgress();
    },
    onError: function(err) {
        var str = err.toString();
        notifyUser(err, null);
        toggleButtonSignin();
        toggleProgress();
    }
};

//handling state of progress
var toggleProgress = function() {
    var proggress = $('#ui_progress');
    var on = 'el-loading';
    var off = 'el-loading-done';
    var state = proggress.hasClass(off);
    if(!state) {
        proggress.removeClass(on);
        proggress.addClass(off);
    } else {
        proggress.removeClass(off);
        proggress.addClass(on);
    }
};

//handle state of button
var toggleButtonSignin = function() {
    var $btn = $('#btnSignin');
    var off = 'disabled';
    var isDisabled = $btn.hasClass(off);
    if(isDisabled) {
        $btn.removeClass(off);
    } else {
        $btn.addClass(off);
    }
};

//api constants
var constants = {
    Url: '/v1/endpoint/sign-in',    
    HttpMethod: 'POST',
    ContentType:'application/json; charset=utf-8', 
    DataType: 'json',
    Callback: callback
};
//where we do ajax request
var request = function(dataSet) {
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

//btnSignIn event Handler
var signInEventHandler = function() {
    //until we done we need to unbind our click handle    
    toggleButtonSignin();
    //check if data is valid
    var userName = $('#ui_username').val();
    var password = $('#ui_password').val();
    //username validation
    var isUserNameValid = !isEmailNotValid(userName);
    if(!isUserNameValid) {
        notifyUser('Please Enter Valid Username', function() {
            $('#ui_username').focus();
        });
        //failed bind click again
        toggleButtonSignin();
        return;//exit
    }
    //password validation
    var isPasswordValid = !isPasswordNotValid(password);
    if(!isPasswordValid) {
        notifyUser('Please Enter Valid Password', function() {
            $('#ui_password').focus();    
        });
        //failed bind click again
        toggleButtonSignin();
        return;//exit
    }
    //show proggress
    toggleProgress();
    
    //no need local for upperCase since it will provide hexStr
    password = md5(password).toUpperCase();
    var dataSet = { UserName: userName, Password: password };
    request(dataSet);
};

//store data in cookie valid for 30 mins
var storeCookie = function(key, value) {
    var now = new Date();
    now.setTime(now.getTime() + 30 * 60 * 1000);//cookie will valid for 30 mins
    var dueStr = now.toUTCString();
    document.cookie = key + '=' + value + '; expires=' + dueStr;          
};

//btnNeedHelp event handler
var needHelpEventHandler = function(e) {
	e.preventDefault();
	window.location.href = "mailto:info@mis.boun.edu.tr?subject=YOUR_SUBJECT_HERE&body=YOUR_MESSAGE_HERE"; //TODO CHANGE THIS 
};

//btnCreateAnAccount event handler
var createAnAccountEventHandler = function() {
	window.location.href = "/create-an-account";//Global.asax route-redirect thing
};
	
//register events when document ready
$(document).ready(function() {            
    //button register event listener for click
	$('#btnSignin').on('click', signInEventHandler);
	$('#btnNeedHelp').on('click', needHelpEventHandler);
	$('#btnCreateAnAccount').on('click', createAnAccountEventHandler);
});
