//organization name validation
var isOrganizationNotValid = function (str) {
    return str === null || str.length < 3;
};
//grab pricker lib
var $pickerLib = $('.ui-picker-lib');   
//check jquery.ui registered, this library ok but messed my Ractive implementations it should not be too any restrictions on content change dynamism
if(typeof jQuery.ui != 'undefined') {
    $pickerLib.selectable({
        filter: '.ui-picker-selectable-handler',
        selecting: function(event, ui) {
            //selection action
            var $selectingParent = $(ui.selecting).parent();
            $selectingParent.addClass('tile-brand-accent ui-picker-selected');

            $('.ui-picker-info').addClass('ui-picker-info-active').removeClass('ui-picker-info-null');
            $('.ui-picker-info-desc-wrap').html($selectingParent.find('.ui-picker-info-desc').html());
            $('.ui-picker-info-title-wrap').html($selectingParent.find('.ui-picker-info-title').html());
        },
        unselecting: function(event, ui) {
            var $unselectingParent = $(ui.unselecting).parent();

            $unselectingParent.removeClass('tile-brand-accent ui-picker-selected');
            
            //multiple selection handle
            if (!$('.ui-picker-selected').length) {
                $('.ui-picker-info').addClass('ui-picker-info-null');
                $('.ui-picker-info-desc-wrap').html('');
                $('.ui-picker-info-title-wrap').html('');
            } else {
                //only one selected, may be multiple selection can be closed for better or worse
                var $first = $($('.ui-picker-selected')[0]);

                $('.ui-picker-info-desc-wrap').html($first.find('.ui-picker-info-desc').html());
                $('.ui-picker-info-title-wrap').html($first.find('.ui-picker-info-title').html());
            }
        }
    });            
};
//picker handlers
$(document).on('click', '.ui-picker-info-close', function () {
    $('.ui-picker-info').removeClass('ui-picker-info-active');
});          
//get cookie for name
var findCookie = function(key) {
    var name = key + '=';
    var ca = document.cookie.split(';');
    for(var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return '';
};        
//query validation
var isQueryNotValid = function(str) {
    return str === null || str.length < 3;    
};
//notify users
var notifyUser = function(str, handle) {
    $('body').snackbar({
        alive: 3000,
        content: '<a data-dismiss="snackbar" class="notification-click">OK</a><div class="snackbar-text">' + str + '</div>'
    });            
    if(handle) {
        $('.notification-click').on('click', handle);
    }
};
//filters
var filters;
$.get('/v1/endpoint/filters')
    .then(function (response) {
        if (response.Code == 200) {
            filters = response.Data;
            //append null values object to use floating labels
            filters.splice(0, 0, { FilterID: null, FilterType: null, FilterValue: null });
        } else {
            //show error 
            notifyUser(response.Message, null);
        }
    });

//binder
var bindView = function(url) {
    return $.get(url);
};
//callback for query results
var callback = {
    onSuccess: function(response) {
        if(response.Code === 200) {
            PeopleViewModel.set("People", response.Data);
        } else if (response.Code == 401) {
            var str = response.Message;
            notifyUser(str, new function () {
                window.location.href = "/sign-in";
            });
            setTimeout(new function () {
                window.location.href = "/sign-in";
            }, 3000);
        } else {           
            var str = response.Message;
            notifyUser(str, null);
        }
        toggleProgress();
    },
    onError: function(err) {
        var str = err.toString();
        notifyUser(str, null);
        toggleProgress();
    }
};
//api constants
var constants = {
    Url : '/v1/endpoint/search',
    HttpMethod: 'POST',
    ContentType: 'application/json; charset=utf-8',
    Headers: {
        'X-Access-Token': findCookie('X-Access-Token')
    },
    DataType: 'json',
    Callback: callback
};
//ajax
var request = function(dataSet) {
    $.ajax({
        url: constants.Url,
        type: constants.HttpMethod,
        data: JSON.stringify(dataSet),
        headers: constants.Headers,
        contentType: constants.ContentType,
        dataType: constants.DataType,
        success: constants.Callback.onSuccess,
        failure: constants.Callback.onError
    });    
};
//change event handler
var changeEventHandler = function() {
    var $n = $($(this));
    var query = $n.val();
    var isQueryValid = !isQueryNotValid(query);
    if(!isQueryValid) {
        notifyUser("You should enter at least 3 characters!", function() {
           $n.focus(); 
        });
        return;
    }
    toggleProgress();
    var dataSet = { q: query };
    request(dataSet);
};
//search event handler
var searchEventHandler = function() {
    var $n = $('#uiSearch');
    var query = $n.val(); 
    var isQueryValid = !isQueryNotValid(query);
    if(!isQueryValid) {
        notifyUser("You should enter at least 3 characters!", function() {
           $n.focus(); 
        });
        return;
    }
    toggleProgress();
    var dataSet = { q: query };
    request(dataSet);
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
//define
var PeopleViewModel;
//retrieve viewModel of People
bindView('js/views/people.view.htm')
    .then(function(view) {
        var People = Ractive.extend({
            template: view,              
            partials: { },    
            lazy: true,
            addPerson: function(person) {
                this.push('People', person);
            },
            deletePerson: function(index) {
                this.splice('People', index, 1);
            },
            getPerson: function(index) {
                var people = this.get().People;
                return people[index];
            },
            addAll: function (people) {
                //lool array and add those into model
                for (var i = 0, z = people.length; i < z; i++) {
                    this.addPerson(people[i]);
                }
            },
            clearAll: function() {
                this.set("People", null);//set data as null
            },             
            transitions: {
                fade: fadeTransition
            }
        });

        PeopleViewModel = new People({
            el: 'output',
            data: {
                People: []
            }
        });

        //contorls if user has previlages
        $.ajax({ 
            url: "/v1/endpoint/has-authority",
            type: 'GET',
            headers: {
                "X-Access-Token": findCookie('X-Access-Token')
            },
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (response) {
                if (response.Code == 200) {
                    //if user is administrator
                    PeopleViewModel.set("isAdmin", response.Data);
                } else {
                    notifyUser("can not check previlages, please try later.", null);
                }
            },
            failure: function () {
                notifyUser("can not check previlages, please try later.", null);
            }
        });
    });
//filter viewModel
var FilterViewModel;
bindView("/js/views/filter.view.htm")
    .then(function (view) {

        var Filters = Ractive.extend({
            template: view,
            partials: {},
            lazy: true,
            addFilter: function (filter) {
                this.push("filters", filter);
            },
            deleteFilter: function (index) {
                this.splice("filters", index, 1);
            },
            oninit: function (opitions) {
                this.on({
                    selectedFilter: function (event) {
                        //bind filter result
                        var filterID = this.get().Filter.FilterID;
                        //find index of filter
                        var findFilter = function (filterID, data) {
                            for (var i = 0, z = data.length; i < z; i++) {
                                var filter = data[i];
                                if (filter.FilterID == filterID) {
                                    return i;
                                }
                            }
                            return -1;
                        };
                        var index = findFilter(filterID, this.get().filters)
                        if (index == -1) return;//if its -1 then what the hell
                        var url = this.get().filters[index].FilterValue;
                        var bind = this;
                        if (url) {
                            $.get(url)
                                .then(function (response) {
                                    if (response.Code == 200) {
                                        bind.set('contents', toContentArray(response.Data));
                                    } else {
                                        notifyUser("we can not retrieve filter data, try again later", null);
                                    }
                                });
                        } else {
                            bind.set("contents", null);
                            bind.set("Filter.ContentID", null);//this will not let user select willingless state of combo
                        }
                    },
                    clearContents: function (event) {
                        this.set("contents", null);
                        this.set("Filter.ContentID", null);//this will not let user select willingless state of combo
                    },
                    queryFilter: function (event) {
                        var contentID = this.get().Filter.ContentID;
                        var filterID = this.get().Filter.FilterID;
                        
                        this.fire("clearContents");//clear contents
                        PeopleViewModel.set("People", null);//clear currents
                        toggleProgress();

                        //start request of filter
                        $.ajax({
                            url: "/v1/endpoint/filter",
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            headers: {
                                "X-Access-Token": findCookie("X-Access-Token")
                            },
                            data: JSON.stringify({ FilterID: filterID, ContentID: contentID }),
                            success: function (response) {
                                if (response.Code == 200) {
                                    PeopleViewModel.set("People", response.Data);
                                } else {
                                    notifyUser(response.Message, null);
                                }
                                toggleProgress();
                            },
                            failure: function () {
                                notifyUser("An Netwrok Error occured, please try again later.", null);
                                toggleProgress();
                            }
                        });
                    }
                }); 
            },
            transitions: {
                fade: fadeTransition        
            }
        });

        if (!filters) {//if we don't have any filters pre fetched we go in here else... while(!filters) { } was ok but messing with load time grrr!
            $.ajax({
                url: '/v1/endpoint/filters',
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.Code == 200) {
                        filters = response.Data;
                        filters.splice(0, 0, {
                            FilterID: null,
                            FilterType: null,
                            FilterValue: null
                        });

                        if (FilterViewModel) {
                            FilterViewModel.set("filters", filters);
                        }
                    }
                }
            })
        }

        FilterViewModel = new Filters({
            el: 'filter-output',
            data: {
                Filter: { FilterID: null, ContentID: null },
                filters: filters,
                contents: null
            }
        });
    });
//model
var WorkplaceViewModel;
//bind
bindView("/js/views/workplace.dialog.view.htm")
    .then(function (view) {
        var Workplaces = Ractive.extend({
            template: view,
            partials: {},
            lazy: true,
            getWorkplace: function () {
                return this.get().Workplace;
            },
            clearWorkplace: function () {
                this.set("Workplace.Title", null);
                this.set("Workplace.Department", null);
                this.set("Workplace.OrganizationName", null);
                this.set("Workplace.Industry", null);
                this.set("Workplace.Country", null);
                this.set("Workplace.City", null);
            },
            oninit: function (opitions) {
                this.on({
                    saveWorkplace: function (event) {
                        //validate
                        var obj = this.getWorkplace();
                        if (!obj.Title) {
                            notifyUser("Invalid title, select one", function () {
                                $('#ui_workplace_title').focus();
                            });
                            return;
                        }
                        if (!obj.Department) {
                            notifyUser("Invalid department, select one", function () {
                                $('#ui_workpalce_department').focus();
                            });
                            return;
                        }
                        if (isOrganizationNotValid(obj.OrganizationName)) {
                            notifyUser("Invalid organization name", function () {
                                $('#ui_organization_name').focus();
                            });
                            return;
                        }
                        if (!obj.Industry) {
                            notifyUser("Invalid industry, select one", function () {
                                $('#ui_industry').focus();
                            });
                            return;
                        }
                        if (!obj.Country) {
                            notifyUser("Invalid country, select one", function () {
                                $('#ui_workplace_country').focus();
                            });
                            return;
                        }
                        if (!obj.City) {
                            notifyUser("Invalid city, select one", function () {
                                $('#ui_workplace_city').focus();
                            });
                            return;
                        }
                        //ajax
                        $.ajax({
                            url: '/v1/endpoint/workplace-change',
                            type: 'POST',
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            headers: {
                                "X-Access-Token": findCookie("X-Access-Token")
                            },
                            data: JSON.stringify(obj),
                            success: function (response) {
                                if (response.Code == 200) {
                                    notifyUser("Saved new workplace successfully.", null);
                                } else {
                                    notifyUser(response.Message, null);
                                }
                            }
                        });
                    },
                    cancelWorkplace: function (event) {
                        this.clearWorkplace();
                    },
                    countryChanged: function(event) {
                        var bind = this;
                        //might need to change this for get request url
                        var cityNode = $('#ui_workplace_city');
                        var selectedCountry = this.get().Workplace.Country;
                        if (selectedCountry !== null) {
                            //enable city combo
                            cityNode.blur();//try to sawp css of city
                            $.get('/v1/endpoint/cities/' + selectedCountry)
                                .then(function (response) {
                                    if (response.Code === 200) {
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
                    },
                    validateOrganizationName: function (event) {
                        var isOrganizationNameValid = !isOrganizationNotValid(this.getWorkplace().OrganizationName);
                        if (!isOrganizationNameValid) {
                            notifyUser("You should enter valid organization name.", function () {
                                $('#ui_organization_name').focus();
                            });
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
            data: {
                Workplaces: {
                    Title: null,
                    Department: null,
                    OrganizationName: null,
                    Industry: null,
                    Country: null,
                    City: null
                }
            }
        });
        //fetch titles
        $.ajax({
            url: '/v1/endpoint/workplace-titles',
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response.Code == 200) {
                    var data = response.Data;
                    data.splice(0, 0, {
                        TitleID: null,
                        TitleName: null
                    });
                    WorkplaceViewModel.set("titles", data);
                } else {
                    notifyUser(response.Data, null);
                }
            }
        })
        //fetch departments
        $.ajax({
            url: '/v1/endpoint/workplace-departments',
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response.Code == 200) {
                    var data = response.Data;
                    data.splice(0, 0, {
                        DepartmentID: null,
                        DepartmentName: null
                    });
                    WorkplaceViewModel.set("departments", data);
                } else {
                    notifyUser(response.Data, null);
                }
            }
        })
        //fetch industries
        $.ajax({
            url: '/v1/endpoint/workplace-industries',
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response.Code == 200) {
                    var data = response.Data;
                    data.splice(0, 0, {
                        IndustryID: null,
                        IndustryName: null
                    });
                    WorkplaceViewModel.set("industries", data);
                } else {
                    notifyUser(response.Data, null);
                }
            }
        })
        //fetch countries
        $.ajax({
            url: '/v1/endpoint/countries',
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response.Code == 200) {
                    var data = response.Data;
                    data.splice(0, 0, {
                        CountryID: null,
                        CountryName: null
                    });
                    WorkplaceViewModel.set("countries", data);
                } else {
                    notifyUser(response.Data, null);
                }
            }
        })
    });
//filter returns will be wrapped by this fn, only handles Title, Department, Industry and GraduateType for
//more you should register this array parser.. sorry :(
var toContentArray = function(data) {
    var array = [];
    if (data != null) {
        for(var i = 0, z = data.length; i < z; i++) {
            var item = data[i];
            if(item.TitleID) {//Titles
                array.splice(0, 0, { 
                    ContentID: item.TitleID, 
                    ContentName: item.TitleName 
                });
            } else if(item.DepartmentID) {//Departments
                array.splice(0, 0, { 
                    ContentID: item.DepartmentID, 
                    ContentName: item.DepartmentName 
                });
            } else if(item.IndustryID) {//Industries
                array.splice(0, 0, { 
                    ContentID: item.IndustryID, 
                    ContentName: item.IndustryName 
                });
            } else if(item.GraduationTypeID) {//GraduateTypes
                array.splice(0, 0, { 
                    ContentID: item.GraduationTypeID,
                    ContentName: item.GraduationTypeName
                });
            } else {
                notifyUser("Filter content type is unknown, you can register it here", null);
            }
        }
    }
    //append null for floating label
    array.splice(0, 0, {
        ContentID: null,
        ContentName: null
    });
    return array;
};

//toggle state of combo box given class
var filterValueComboToggle = function (str, add) {
    var items = $(str);
    if (items) {
        for (var i = 0, z = items.length; i < z; i++) {
            var $item = $(items[i]);
            if (add) {
                $item.removeClass('disabled');
            } else {
                $item.addClass('disabled');
            }
        }
    }
};
//clear handler
var clearEventHandler = function () {
    if (PeopleViewModel) {
        PeopleViewModel.clearAll();
    }
};
//activate user event handler
var activateEventHandler = function (index) {
    if (PeopleViewModel) {
        var person = PeopleViewModel.get().People[index];
        var uri = '/v1/endpoint/activate-user/' + person.UserID;
        //toggle state of user
        $.ajax({
            url: uri,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            headers: {
                "X-Access-Token": findCookie("X-Access-Token")
            },
            dataType: 'json',
            success: function (response) {
                if (response.Code == 200) {
                    PeopleViewModel.set("People." + index + ".isApproved", response.Data);//set this on model also                    
                    $('#btnActivate' + index + ' span').text(response.Data ? "DEACTIVATE" : "ACTIVATE");
                } else {
                    notifyUser(response.Message, null);
                }
            },
            failure: function () {
                notifyUser("service is not available, please try later.", null);
            }
        });
    }
};

var resetPasswordEventHandler = function (index) {
    if (PeopleViewModel) {
        var person = PeopleViewModel.get().People[index];
        var uri = '/v1/endpoint/reset-password/' + person.UserID;
        //change password state
        $.ajax({
            url: uri,
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            headers: {
                "X-Access-Token": findCookie("X-Access-Token")
            },
            dataType: 'json',
            success: function (response) {
                if (response.Code == 200) {
                    var uri = window.location.host + '/change-password/' + response.Data.ResetToken;
                    var to = response.Data.UserName;
                    window.location.href = "mailto:" + to + "?subject=Password Reset&body=You can reset your password with following link. " + encodeURI(uri);                   
                } else {
                    notifyUser(response.Message, null);
                }
            },
            failure: function () {
                notifyUser("service is not avaialable, please try later.", null);
            }
        });
    }
};

//token validation
var isTokenNotValid = function(token) { 
    return token === null || token.length < 32;
};
//bind items when document is ok.
$(document).ready(function() {  
    //check token
    var token = findCookie('X-Access-Token');
    var isTokenValid = !isTokenNotValid(token);
    if(!isTokenValid) {
        window.location.href = "/sign-in";
    }
    //catch change event
    $('#uiSearch').on('change', changeEventHandler);
    //catch click event
    $('#btnSearch').on('click', searchEventHandler);
    //clear handler
    $('#btnClear').on('click', clearEventHandler);
});
