//email validation
var isEmailNotValid = function(str) {
    var regex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return str === null || !regex.test(str);
};
//password content validation
var isPasswordNotValid = function(str) {
    return str === null || str.length < 8;       
};
//if two password not match
var isPasswordsNotMatch = function(s1, s2) {
    return s1 === null || s2 === null || s1 !== s2;
};
//identity no validation
var isIdentityNoNotValid = function(str) {
    return str === null || str.length < 5;
};
//first name validaiton
var isFirstNameNotValid = function(str) {
    return str === null || str.length < 3; 
};
//last name validation, lenght 2 since case there 2 letter surnames...sight!
var isLastNameNotValid = function(str) {
    return str === null || str.length < 2;   
};
//linkedin validation
var isLinkedinNotValid = function(str) {
    return str === null || str.length < 2; 
};
//organization name validation
var isOrganizationNotValid = function(str) {
    return str === null || str.length < 3;
};
//validation
var isDepartmentNameNotValid = function(str) {
    return str === null || str.length < 3;
};
//validations
var isUniversityNameNotValid = function(str) {
    return str === null || str.length < 8;//is there any universities that is name as low as 8 chararcters ?
};
//null or empty
var isNotNullOrEmpty = function(v) {
    if(v instanceof String) {
        return v !== null || v !== '';
    } else {
        return v !== null;
    }
};
//fetch countries
var countries;
$.ajax({
    url: '/v1/endpoint/countries',
    type: 'GET',
    contentType: 'application/json; charset=utf-8',
    dataType: 'json',
    success: function (response) {
        if (response.Code == 200) {
            countries = response.Data;
            countries.splice(0, 0, { CountryID: null, CountryName: null });        
        } else {
            notifyUser('Countries data is not available, please try later', null);
        }
    }
});
//fetch years
var years;
$.ajax({
    url: '/v1/endpoint/years',
    type: 'GET',
    contentType: 'application/json; charset=utf-8',
    dataType: 'json',
    success: function (response) {
        if (response.Code == 200) {
            years = response.Data;
            years.splice(0, 0, { YearID: null, Year: null });
        } else {
            notifyUser('Years data is not available, please try later', null);
        }
    }
});
//notify user
var notifyUser = function(str, handle) {
    $('body').snackbar({
        alive: 3000,
        content: '<a data-dismiss="snackbar" class="notification-click">OK</a><div class="snackbar-text">' + str + '</div>'
    }); 
    //if we have handle
    if(handle) {
        $('.notificatiın-click').on('click', handle);
    }
};
//callback for create
var callback = {
    onSuccess: function(response) {
        if (response.Code === 200) {
            //show message to user
            notifyUser(response.Message, function () {
                window.location.href = "/sign-in";
            });
            //then redirect for users who do not like to click OK buttons
            setTimeout(function () {
                window.location.href = "/sign-in";
            }, 5000);//after 5 sec user did nothing then we exit
        } else {
            var str = response.Message;
            notifyUser(str, null);
        }
        clearToggle();
    },
    onError: function(err) {
        var str = err.toString();
        notifyUser(str, null);
        clearToggle();
    }
};
//api constants
var constants = {
    Url: '/v1/endpoint/create-an-account',
    HttpMethod: 'POST',
    ContentType: 'application/json; charset=utf-8',
    DataType: 'json',
    Callback: callback
};
//create request
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
//view templates
var bindView = function(url) {
    return $.get(url);
};
//state change of btnSaveAccount
var toggleButton = function(str) {
    var $btn = $(str);
    var d = 'disabled';
    var state = $btn.hasClass(d);
    if(state) {
        $btn.removeClass(d);
    } else {
        $btn.addClass(d);
    }   
};
//AccountViewModel Starts
var AccountViewModel;
//bind it on account view model
bindView('js/views/account.view.htm')
    .then(function(view) {
        //definition
        var Accounts = Ractive.extend({
            template: view,
            partials: { },    
            lazy: true,
            transitions: {
                fade: fadeTransition
            },
            oninit: function(options) {
                this.on({
                    validateUserName: function(event) {
                        var isUserNameValid = !isEmailNotValid(this.get().UserName);
                        if(!isUserNameValid) {
                            notifyUser("Username is not valid, should be email format.", function() {
                                $('#ui_username').focus();    
                            });
                            return;//exit here
                        }
                        //check if username already registered db
                        $.ajax({
                            url: '/v1/endpoint/check-if-exists',
                            type: 'POST',
                            data: JSON.stringify(this.get().UserName),
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function(response) {
                                if (response.Code === 200) {
                                    var alreadyExits = response.Data;//if user not exists it will return true, else false
                                    if (!alreadyExits) {
                                        notifyUser("Username is already exists, try another one.", function () {
                                            $('#ui_username').focus();
                                        });
                                    }
                                } else {
                                    notifyUser("username check is not available, please try later", null);
                                }
                            }                            
                        });
                    },
                    validatePassword: function(event) {
                        var isPasswordValid = !isPasswordNotValid(this.get().Password);
                        if(!isPasswordValid) {
                            notifyUser("Password you entered not valid, at least 8 characters.", function() {
                                $('$ui_password').focus();
                            });
                        }
                    },
                    validatePasswordRe: function(event) {
                        var isPasswordMatch = !isPasswordsNotMatch(this.get().Password, this.get().PasswordRepeat);
                        if(!isPasswordMatch) {
                            notifyUser('Passwords you entered are not match, try again.', function(){
                                $('#ui_password_re').focus();    
                            });
                        }
                    } 
                });
            }
        });
        //bind data and echo
        AccountViewModel = new Accounts({
            el: 'account-output',
            data: {                
                UserName: null,
                Password: null,
                PasswordRepeat: null
            }
        });
});
//getter
var retrieveAccount = function() {
    return AccountViewModel.get();
};
//model
var PeopleViewModel;
//bind
bindView('js/views/person.view.htm')
    .then(function(view) {
        
        var People = Ractive.extend({
            template: view,
            partials: { },
            lazy: true,
            oninit: function(options) {
                this.on({
                    validateIdentity: function(event) {
                        var isIdentityNoValid = !isIdentityNoNotValid(this.get().IdentityNo);
                        if(!isIdentityNoValid) {
                            notifyUser("You should enter valid identity no.", function() {
                                $('#ui_identity').focus();    
                            });
                        }
                    },
                    validateFirstName: function(event) {
                        var isFirstNameValid = !isFirstNameNotValid(this.get().FirstName);
                        if(!isFirstNameValid) {
                            notifyUser("You shoud enter valid first name.", function() {
                                $('#ui_firstname').focus();    
                            });
                        }
                    },
                    validateLastName: function(event) {
                        var isLastNameValid = !isLastNameNotValid(this.get().LastName);
                        if(!isLastNameValid) {
                            notifyUser("You should enter valid last name.", function() {
                                $('#ui_lastname').focus();    
                            });
                        }
                    }
                })
            },
            transitions: {
                fade: fadeTransition
            }
        });
    
        PeopleViewModel = new People({
            el: 'people-output',
            data: {
                IdentityNo: null,
                FirstName: null,
                MiddleName: null,
                LastName: null,
                Phone: null
            }    
        }); 
    
});
//getter
var retrievePerson = function() {
    return PeopleViewModel.get();
}	
//model
var SocialViewModel;
//bind
bindView('js/views/social.view.htm')
    .then(function(view) {
    
    var Socials = Ractive.extend({
        template: view,
        partials: {},
        lazy: true,
        oninit: function(options) {
            this.on({
               validateLinkedin: function(event) {
                   var isLinkedinValid = !isLinkedinNotValid(this.get().Linkedin);
                   if(!isLinkedinValid) {
                        notifyUser("You should enter valid linkedin address, try again.", function() {
                            $('#ui_linkedin').focus();       
                        });
                   }
               } 
            });
        },
        transitions: {
            fade: fadeTransition
        }
    });
    
    SocialViewModel = new Socials({
        el: 'social-output',
        data: {
            Linkedin: null,
            Twitter: null,
            Facebook: null
        }
    });
});
//getter
var retrieveSocial = function() {
    return SocialViewModel.get();
};
//model
var GraduationViewModel;
//bind
bindView('js/views/graduation.view.htm')
    .then(function (view) {

        var Graduations = Ractive.extend({
            template: view,
            partials: {},
            lazy: true,
            debug: true,
            addGraduation: function(graduation) {
                this.push("Graduations", graduation);
                //fire event
                var event = { index: { i: this.get().Graduations.indexOf(graduation) } };
                this.fire("fetchTypes", event);
                this.fire("fetchYears", event);
                this.fire("fetchCountries", event);
            },
            deleteGraduation: function(index) {
                this.splice("Graduations", index, 1);
            },
            oninit: function(options) {
                this.on({
                    remove: function(event) {
                        this.deleteGraduation(event.index.i);
                    },
                    validateDepartmentName: function(event) {
                        var index = event.index.i;
                        var Graduations = this.get().Graduations;
                        var str = Graduations[index].DepartmentName;
                        var isDepartmentNameValid = !isDepartmentNameNotValid(str);
                        if(!isDepartmentNameValid) {
                            notifyUser('Department name is not valid, try again.', function() {
                                $('#ui_department' + index).focus();//direct user there
                            });
                        }
                    },
                    validateUniversityName: function(event) {
                        var index = event.index.i;
                        var Graduations = this.get().Graduations;
                        var str = Graduations[index].University;
                        var isUniversityNameValid = !isUniversityNameNotValid(str);
                        if(!isUniversityNameValid) {
                            notifyUser('University name is not valid, try again.', function() {
                                $('#ui_university' + index).focus();//direct user there
                            })
                        }
                    },
                    fetchTypes: function(event) {
                        var bind = this;
                        var index = event.index.i;
                        $.get('/v1/endpoint/graduation-types/graduate')
                            .then(function(response) {
                                if(response.Code === 200) {
                                    var data = response.Data;
                                    data.splice(0, 0, { GraduateTypeID: null, GraduateTypeName: null });
                                    bind.set("Graduations." + index + ".types", data);
                                } else {
                                    notifyUser('can not retrieve graduation tpyes, please try later.', null);
                                }
                        })
                    },
                    fetchYears: function(event) {
                        var index = event.index.i;
                        if (!years) {//no cache avaialable fetch it
                            $.ajax({
                                url: '/v1/endpoint/years',
                                type: 'GET',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    if (response.Code == 200) {
                                        years = response.Data;
                                        years.splice(0, 0, { YearID: null, Year: null });
                                        GraduationViewModel.set("Graduations." + index + ".years", years);
                                    } else {
                                        notifyUser('Years data is not available, please try later', null);
                                    }
                                }
                            });
                        } else {
                            //use cached one
                            this.set("Graduations." + index + ".years", years);
                        }
                    },
                    fetchCountries: function(event) {                        
                        var index = event.index.i;
                        if (!countries) {//no cache, fetch
                            $.ajax({
                                url: '/v1/endpoint/countries',
                                type: 'GET',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    if (response.Code == 200) {
                                        countries = response.Data;
                                        countries.splice(0, 0, { CountryID: null, CountryName: null });
                                        GraduationViewModel.set("Graduations." + index + ".countries", countries);
                                    } else {
                                        notifyUser('Countries data is not available, please try later', null);
                                    }
                                }
                            });
                        } else {//use cache
                            this.set("Graduations." + index + ".countries", countries);
                        }
                    },
                    countryChanged: function(event) {
                        var bind = this;
                        var index = event.index.i;
                        //might need to change this for get request url
                        var cityNode = $('#ui_university_city' + index);
                        var selectedCountry = this.get().Graduations[index].Country;
                        if (selectedCountry !== null) {
                            //enable city combo
                            cityNode.blur();//try to sawp css of city
                            $.get('/v1/endpoint/cities/' + selectedCountry)
                                .then(function (response) {
                                    if (response.Code === 200) {
                                        var data = response.Data;
                                        data.splice(0, 0, { CityID: null, CityName: null })
                                        bind.set("Graduations." + index + ".cities", data);
                                    } else {
                                        notifyUser("can not retrieve cities for this country, please try later.", null);
                                    }
                                });
                        } else {
                            bind.set("Graduations." + index + ".cities", null);//clear all
                            bind.set("Graduations." + index + ".City", null);//set selection null if there is.
                            cityNode.blur();//just to be safe change focus state.
                        }
                    }
                });
            },
            transitions: {
                fade: fadeTransition
            }
        });       

        GraduationViewModel = new Graduations({
            el: 'graduation-output',
            data: {
                Graduations: []
            }
        });

        GraduationViewModel.addGraduation({
                GraduateYear: null,
                GraduateType: null,
                DepartmentName: null,
                University: null,
                City: null,
                Country: null
        });
});
//getter
var retrieveGraduations = function() {
    return GraduationViewModel.get();
};
//model
var CapViewModel;
//bind
bindView('/js/views/cap.view.htm')
    .then(function(view) {
        
        var Caps = Ractive.extend({
            template: view,
            partials: {},
            lazy: true,
            addCap: function(cap) {
                this.set("Cap", cap);
                this.fire("fetchYears");
                this.fire("fetchTypes");
            },
            deleteCap: function() {
                this.set("Cap", null);
            },
            oninit: function(options) {
                this.on({
                    fetchYears: function (event) {
                        if (!years) {//no cahce, fetch
                            $.ajax({
                                url: '/v1/endpoint/years',
                                type: 'GET',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    if (response.Code == 200) {
                                        years = response.Data;
                                        years.splice(0, 0, { YearID: null, Year: null });
                                        CapViewModel.set("years", years);
                                    } else {
                                        notifyUser('Years data is not available, please try later', null);
                                    }
                                }
                            });
                        } else {//use cahce
                            this.set("years", years);
                        }
                    },
                    fetchTypes: function(event) {
                        var bind = this;
                        $.get('/v1/endpoint/graduation-types/cap')
                            .then(function(response) {
                                if(response.Code === 200) {
                                    var data = response.Data;
                                    data.splice(0, 0, { GraduateTypeID: null, GraduateTypeName: null });
                                    bind.set("types", data);
                                } else {
                                    notifyUser('can not retrieve types, please try later.', null);
                                }       
                        });
                    },
                    validateDepartmentName: function(event) {
                        var value = this.get().Cap.DepartmentName;
                        if(isDepartmentNameNotValid(value)) {
                            notifyUser("You should enter valid department name", function() {
                                $('ui_cap_department').focus();    
                            });
                        }
                    }
                });
            },
            transitions: {
                fade: fadeTransition
            }
        });
    
        CapViewModel = new Caps({
            el: 'cap-output',
            data: { }
        });
        //add cap
        CapViewModel.addCap({ Year: null, GraduateType: null, DepartmentName: null});
});
//retrieve caps
var retrieveCap = function() {
    return CapViewModel.get();
}
//model
var ExchangeViewModel;
//bind
bindView('js/views/exchange.view.htm')
    .then(function(view) {
        
        var Exchanges = Ractive.extend({
            template: view,
            partial: {},
            lazy: true,
            addExchange: function(exchange) {
                this.set("Exchange", exchange);
                this.fire("fetchYears");
                this.fire("fetchCountries");
            },
            deleteExchange: function() {
                this.set("Exchange", null);
            },
            oninit: function(options) {
                this.on({
                    fetchYears: function (event) {
                        if (!years) {//no cahce, fetch
                            $.ajax({
                                url: '/v1/endpoint/years',
                                type: 'GET',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    if (response.Code == 200) {
                                        years = response.Data;
                                        years.splice(0, 0, { YearID: null, Year: null });
                                        ExchangeViewModel.set("years", years);
                                    } else {
                                        notifyUser('Years data is not available, please try later', null);
                                    }
                                }
                            });
                        } else {//use cahce
                            this.set("years", years);
                        }
                    },
                    fetchCountries: function (event) {
                        if (!countries) {//no cache, fetch
                            $.ajax({
                                url: '/v1/endpoint/countries',
                                type: 'GET',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    if (response.Code == 200) {
                                        countries = response.Data;
                                        countries.splice(0, 0, { CountryID: null, CountryName: null });
                                        ExchangeViewModel.set("countries", countries);
                                    } else {
                                        notifyUser('Countries data is not available, please try later', null);
                                    }
                                }
                            });
                        } else {//use cache
                            this.set("countries", countries);
                        }
                    },
                    countryChanged: function(event) {
                        var bind = this;
                        //might need to change this for get request url
                        var cityNode = $('#ui_exchange_city');
                        var selectedCountry = this.get().Exchange.Country;
                        if(selectedCountry !== null) {
                            //enable city combo
                            cityNode.blur();//try to sawp css of city
                            $.get('/v1/endpoint/cities/' + selectedCountry)
                                .then(function(response) {
                                    if(response.Code === 200) {
                                        var data = response.Data;
                                        data.splice(0, 0, { CityID: null, CityName: null })
                                        bind.set("cities", data);
                                    } else {
                                        notifyUser("can not retrieve cities for this country, please try later.", null);
                                    }  
                            });
                        } else {                            
                            bind.set("cities", null);//clear all
                            bind.set("Exchange.City", null);//set selection null if there is.
                            cityNode.blur();//just to be safe change focus state.
                        }
                    },
                    validateUniversityName: function(event) {
                        var value = this.get().Exchange.University;
                        if(isUniversityNameNotValid(value)) {
                            notifyUser("You should enter valid university name.", function() {
                                $('#ui_exchange_university').focus();    
                            });
                        }
                    }    
                });
            },
            transitions: {
                fade: fadeTransition
            }
        });
    
        ExchangeViewModel = new Exchanges({
            el: 'exchange-output',
            data: { }
        });
    
        ExchangeViewModel.addExchange({ Year: null, University: null, Country: null, City: null });
});
//getter
var retrieveExchange = function() {
    return ExchangeViewModel.get();
};
//model
var WorkplaceViewModel;
//bind
bindView('js/views/workplace.view.htm')
    .then(function(view) {
        
        var Workplaces = Ractive.extend({
            template: view,
            partials: {},
            lazy: true,
            addWorkplace: function(workplace) {
                this.set("Workplace", workplace);
                //fire events to fetch
                this.fire("fetchTitles");//have to manually fire these events
                this.fire("fetchDepartments");//have to manually fire these events
                this.fire("fetchIndustries");//have to manually fire these events
                this.fire("fetchCountries");
            },
            oninit: function(options) { //workplace definition needs it
                this.on({
                    remove: function(event) {
                        this.reset();
                    },
                    validateOrganizationName: function(event) {
                        var isOrganizationNameValid = !isOrganizationNotValid(this.get().Workplace.OrganizationName);
                        if(!isOrganizationNameValid) {
                            notifyUser("You should enter valid organization name.", function() {
                                $('#ui_organization_name').focus();    
                            });
                        }
                    },
                    //fetches titles
                    fetchTitles: function(event) {
                        var bind = this;
                        $.get('/v1/endpoint/workplace-titles')
                            .then(function(response) {
                                if(response.Code === 200) {
                                    var data = response.Data;
                                    data.splice(0, 0, { TitleID: null, TitleName: null });
                                    bind.set("titles", data);
                                } else {
                                    notifyUser("can not retrieve titles, please try later.", null);
                                } 
                        });    
                    },
                    //fetches industries
                    fetchIndustries: function(event) {
                        var bind = this;
                        $.get('/v1/endpoint/workplace-industries')
                            .then(function(response) {
                                if(response.Code === 200) {
                                    var data = response.Data;
                                    data.splice(0, 0, { IndustryID: null, IndustryName: null });
                                    bind.set("industries", data);
                                } else {
                                    notifyUser('can not retrieve industries, please try later.', null);
                                }  
                        });
                    },
                    //fetches departments
                    fetchDepartments: function(event) {
                        var bind = this;
                        $.get('/v1/endpoint/workplace-departments')
                            .then(function(response) {
                                if(response.Code === 200) {
                                    var data = response.Data;
                                    data.splice(0, 0, { DepartmentID: null, DepartmentName: null });
                                    bind.set("departments", data);
                                } 
                        });
                    },
                    //fetch countries
                    fetchCountries: function (event) {
                        if (!countries) {//no cache, fetch
                            $.ajax({
                                url: '/v1/endpoint/countries',
                                type: 'GET',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    if (response.Code == 200) {
                                        countries = response.Data;
                                        countries.splice(0, 0, { CountryID: null, CountryName: null });
                                        WorkplaceViewModel.set("countries", countries);
                                    } else {
                                        notifyUser('Countries data is not available, please try later', null);
                                    }
                                }
                            });
                        } else {//use cache
                            this.set("countries", countries);
                        }
                    },
                    //when change occurs, query it like that, be aware of null!
                    countryChanged: function(event) {            
                        var bind = this;
                        //might need to change this for get request url
                        var cityNode = $('#ui_workplace_city');
                        var selectedCountry = this.get().Workplace.Country;
                        if(selectedCountry !== null) {
                            //enable city combo
                            cityNode.blur();//try to sawp css of city
                            $.get('/v1/endpoint/cities/' + selectedCountry)
                                .then(function(response) {
                                    if(response.Code === 200) {
                                        var data = response.Data;
                                        data.splice(0, 0, { CityID: null, CityName: null })
                                        bind.set("cities", data);
                                    } else {
                                        notifyUser("can not retrieve cities for this country, please try later.", null);
                                    }  
                            });
                        } else {                            
                            bind.set("cities", null);//clear all
                            bind.set("Workplace.City", null);//set selection null if there is.
                            cityNode.blur();//just to be safe change focus state.
                        }
                    }
                });
            },
            transitions: {
                fade: fadeTransition
            }
        });
    
        WorkplaceViewModel = new Workplaces({
            el: 'workplace-output',
            data: { }
        });
});
//getter
var retrieveWorkplaces = function() {
    return WorkplaceViewModel.get();
};
//add education event handler
var addNewEducationEventHandler = function() {
    GraduationViewModel.addGraduation({
        GraduateYear: null,
        GraduateType: null,
        DepartmentName: null,
        University: null,
        City: null,
        Country: null
    });    
};
//add workplace event handler
var addWorkplaceEventHandler = function() {
    if(WorkplaceViewModel != null) {
        WorkplaceViewModel.addWorkplace({
            Title: null,
            Department: null,
            OrganizationName: null,
            Industry: null,
            Country: null,
            City: null
        });
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
//save account event handler
var saveAccountEventHandler = function() {
    //disable actions for now and show progress indicator
    clearToggle();//same as showing, yeah WTF!
    
    //check account
    var account = retrieveAccount();
    if(account != null) {
        //try validate username, not gone check again if exists in server because it is async payload... :(
        if(isEmailNotValid(account.UserName)) {
            notifyUser("You should enter valid username, please try again.", function() {
                $('#ui_username').focus(); 
            });
            clearToggle();//toggles are cleared
            return;
        }
        //try valdiate first password
        if(isPasswordNotValid(account.Password)) {
            notifyUser("You should enter valid password, please try again.", function() {
                $('#ui_password').focus();    
            });  
            clearToggle();
            return;
        }
        //try validate password match
        if(isPasswordNotValid(account.Password, account.PasswordRepeat)) {
            notifyUser("Passwords you have entered does not match, please try again.", function() {
                $('#ui_password_re').focus();    
            });
            clearToggle();
            return;
        }
    } else {
        notifyUser("Fill Account", function() {
            $('#ui_username').focus();    
        });
        clearToggle();
        return;
    }
    //check person
    var person = retrievePerson();
    if(person != null) {
        //try validate identity no
        if(isIdentityNoNotValid(person.IdentityNo)) {
            notifyUser("You should enter valid identityNo, please try again.", function() {
                $('#ui_identity').focus();
            });
            clearToggle();
            return;
        }
        //try validate first name
        if(isFirstNameNotValid(person.FirstName)) {
            notifyUser("You should enter valid firstName, please try again.", function() {
                $('#ui_fistname').focus();
            });
            clearToggle();
            return;
        }
        //try validate last name
        if(isLastNameNotValid(person.LastName)) {
            notifyUser("You should enter valid lastName, please try again.", function() {
               $('#ui_lastname').focus(); 
            });
            clearToggle();
            return;
        }
    } else {
        notifyUser("Fill Person", function() {
            $('#ui_identity').focus();
        });
        clearToggle();
        return;
    }
    //check social
    var social = retrieveSocial();
    if(social !== null) {
        //try validate linkedin
        if(isLinkedinNotValid(social.Linkedin)) {
            notifyUser("You should enter valid linkedin, please try again.", function() {
               $('#ui_linkedin').focus(); 
            });
            clearToggle();
            return;
        }
    } else {
        notifyUser("Fill Social", function() {
            $('#ui_linkedin').focus();
        });
        clearToggle();
        return;
    }
    //check graduation or graduations
    var graduations = retrieveGraduations().Graduations;
    if(graduations != null) {
        //size cached loop
        for(var i = 0, z = graduations.length; i < z; i++) {
            var graduation = graduations[i];
            if(graduation != null) {
                //check year is not empty
                if(!isNotNullOrEmpty(graduation.GraduateYear)) {
                    notifyUser("You should select a year, please try again.", function() {
                        $('#ui_graduate_year' + i).focus();    
                    });
                    clearToggle();
                    return;
                }
                //check type is not empty
                if(!isNotNullOrEmpty(graduation.GraduateType)) {
                    notifyUser("You should select a type, please try again.", function() {
                        $('ui_graduate_type' + i).focus();
                    });
                    clearToggle();
                    return;
                }
                //try validate department name
                if(isDepartmentNameNotValid(graduation.DepartmentName)) {
                    notifyUser("You should enter valid department name, please try again.", function() {
                        $('#ui_department' + i).focus();    
                    });
                    clearToggle();
                    return;
                }
                //try validate univeristy name
                if(isUniversityNameNotValid(graduation.University)) {
                    notifyUser("You should enter valid university name, please try again.", function() {
                        $('#ui_university' + i).focus();
                    });
                    clearToggle();
                    return;
                }
                //check country not null
                if(!isNotNullOrEmpty(graduation.Country)) {
                    notifyUser("You should select country, please try again.", function() {
                        $('#ui_university_country' + i).focus();    
                    });
                    clearToggle();
                    return;
                }
                //check city not null
                if(!isNotNullOrEmpty(graduation.City)) {
                    notifyUser("You should select city, please try again.", function() {
                        $('#ui_university_city' + i).focus();
                    });
                    clearToggle();
                    return;
                }
            } else {
                notifyUser("Fill Education(s)", function() {
                    $('#ui_graduate_year' + i).focus();
                });
                clearToggle();
                return;
            }
        }
    } else {
        notifyUser("Fill Education", function() {
            $('#ui_graduate_year0').focus();
        });
        clearToggle();
        return;
    }
    
    var workplace = retrieveWorkplaces().Workplace;
    //if not null
    if(workplace != null) {
        //title is not null
        if(!isNotNullOrEmpty(workplace.Title)) {
            notifyUser("You should select a title or remove workplace, please try again.", function() {
                $('#ui_workplace_title').focus();    
            });
            clearToggle();
            return;
        }
        //department is not null
        if(!isNotNullOrEmpty(workplace.Department)) {
            notifyUser("You should select a department or remove workplace, please try again.", function() {
                $('#ui_workplace_department').focus();    
            });
            clearToggle();
            return;
        }
        //organization name
        if(isOrganizationNotValid(workplace.OrganizationName)) {
            notifyUser("You should enter a valid organization name or remove workplace, please try again.", function() {
                $('#ui_organization_name').focus();    
            });
            clearToggle();
            return;
        }
        //industry
        if(!isNotNullOrEmpty(workplace.Industry)) {
            notifyUser("You should select a industry or remove workplace, please try again.", function() {
                $('#ui_industry').focus();    
            });
            clearToggle();
            return;
        }
        //country
        if(!isNotNullOrEmpty(workplace.Country)) {
            notifyUser("You should select a country or remove workplace, please try again.", function() {
                $('#ui_workplace_country').focus();    
            });
            clearToggle();
            return;
        }
        //city
        if(!isNotNullOrEmpty(workplace.City)) {
            notifyUser("You should select a city or remove workplace, please try again.", function() {
                $('#ui_workplace_city').focus();    
            });
            clearToggle();
            return;
        }
    }
    
    var exchange = ExchangeViewModel.get().Exchange;
    var cap = CapViewModel.get().Cap;
    //post this data
    var dataSet = { 
        Account: { 
            UserName: account.UserName, 
            Password: md5(account.Password).toUpperCase() 
        }, 
        Person: person, 
        Social: social, 
        Graduations: toArray(graduations),//there cities here need to get rid of them
        Cap: cap.Year && cap.GraduateType && cap.Department ? cap : null,
        Exchange: exchange.Year && exchange.University && exchange.Country && exchange.City ? exchange : null,
        Workplace: workplace ? workplace : null
    };
    //post it

    request(dataSet);
    
    //we managed to come all the way down here, that's equal to winnig gold-medal in olympics
};
//only use with graduations
var toArray = function(data) {
    var array = [];
    for(var i = 0, z = data.length; i < z; i++) {
        var item = data[i];
        array.splice(0, 0, {
            City: item.City,
            Country: item.Country,
            DepartmentName: item.DepartmentName,
            GraduateType: item.GraduateType,
            GraduateYear: item.GraduateYear,
            University: item.University
        });
    }
    return array;
};
//checks toggle
var clearToggle = function() {
    toggleButton('#buttonSaveAccount');
    toggleButton('#buttonCancelAccount');
    toggleProgress();
};
//go to login
var cancelEventHandler = function() {
    window.location.href = '/sign-in';
};
//city model will be bind to every where so it should be loaded in runtime 
//and that might be binded for each item in education list if possible

$(document).ready(function() {            
    //bind Buttons
    $('#btnSaveAccount').on('click', saveAccountEventHandler);
    $('#btnCancelAccount').on('click', cancelEventHandler);
    $('#btnAddEducation').on('click', addNewEducationEventHandler);
    $('#btnAddWorkplace').on('click', addWorkplaceEventHandler);
});