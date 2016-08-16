//
//  Copyright 2016  
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using GraduateEntity;
using GraduateEntity.Model;
using System.ServiceModel;
using GraduateEntityProxy;

namespace GraduateWeb {

	public class Endpoint : IEndpoint {

		private const string KEY_ACCESS_TOKEN = "X-Access-Token";
        private const int START_YEAR = 1995;

        private const string TYPE_CAP   = "cap";
        private const string TYPE_GRAD  = "graduate";

        private const string FILTER_TITLE           = "ByTitle";
        private const string FILTER_DEPARTMENT      = "ByDepartment";
        private const string FILTER_INDUSTRY        = "ByIndustry";
        private const string FILTER_GRADUATE_TYPE   = "ByGraduateType";

		private GraduateDbContext dbContext = new GraduateDbContext();

		/// <summary>
		/// /sign-in user method handle
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="usr">Usr.</param>
		public Response<SessionProxy> checkUser(LoginRequest usr) {
			if(usr == null) {
				return new Response<SessionProxy>() {
					Code = 404,
					Message = "Enter username and password.",
					Data = null
				};
			}

			User user = dbContext.Users
			                     .Where(u => u.UserName.Equals(usr.UserName))
			                     .FirstOrDefault();

			if(user == null) {
				return new Response<SessionProxy>() {
					Code = 404,
					Message = "No such user, username is case sensetive.",
					Data = null
				};
			}

            if(!user.IsApproved) {
                return new Response<SessionProxy>() {
                    Code = 404,
                    Message = "user is waiting activation.",
                    Data = null
                };
            }

			if(!user.Password.Equals(usr.Password)) {
				return new Response<SessionProxy>() {
					Code = 404,
					Message = "Invalid password, password is case sensetive.",
					Data = null
				};
			}

			Session session = new Session() {
				UserID = user.UserID,
				Token = newToken(),
				CreateDate = DateTime.Now,
				DueDate = DateTime.Now.AddMinutes(30)
			};

			dbContext.Sessions.Add(session);
			dbContext.SaveChanges();

			//return session
			return new Response<SessionProxy>() {
				Code = 200,
				Message = "Success.",
				Data = new SessionProxy(session)
			};
		}

		/// <summary>
		/// Checks the user name if exists.
		/// </summary>
		/// <returns>The user name if exists.</returns>
		/// <param name="usrname">Usrname.</param>
		public Response<bool> checkUserNameIfExists(string usrname) {
			return new Response<bool>() {
				Code = 200,
				Message = "success",
				Data = dbContext.Users
								.Where(s => s.UserName.Equals(usrname))
								.Count() <= 0
			};
		}

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="create"></param>
        /// <returns></returns>
		public Response<string> createUser(CreateProxy create) {
            try {
                AccountProxy acc = create.Account;
                if(acc == null) {
                    return new Response<string>() {
                        Code = 404,
                        Message = "invalid account",
                        Data = null
                    };
                }

                User newUser = new User() {
                    UserName = acc.UserName,
                    Password = acc.Password,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    ResetTokenDue = DateTime.Now,
                    IsApproved = false,
                    HaveAdministratorPrivileges = false
                };

                dbContext.Users.Add(newUser);
                dbContext.SaveChanges();

                InfoProxy person = create.Person;
                if(person == null) {
                    return new Response<string>() {
                        Code = 404,
                        Message = "invalid person",
                        Data = null
                    };
                }

                Person newPerson = new Person() {
                    UserID = newUser.UserID,
                    FirstName = person.FirstName,
                    MiddleName = string.IsNullOrEmpty(person.MiddleName) ? null : person.MiddleName,
                    LastName = person.LastName,
                    IdentityNo = person.IdentityNo,
                    Phone = string.IsNullOrEmpty(person.Phone) ? null : person.Phone,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };

                dbContext.People.Add(newPerson);
                dbContext.SaveChanges();

                Education newEducation = new Education() {
                    PersonID = newPerson.PersonID,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };
                dbContext.Educations.Add(newEducation);
                dbContext.SaveChanges();

                newPerson.Education = newEducation;

                if(create.Exchange != null) {
                    //if has Exchange data
                    University exUniversity = new University() {
                        UniversityName = create.Exchange.University,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };

                    dbContext.Universities.Add(exUniversity);
                    dbContext.SaveChanges();

                    Exchange newExchange = new Exchange() {
                        CityID = create.Exchange.City.GetValueOrDefault(-1),
                        CountryID = create.Exchange.Country.GetValueOrDefault(-1),
                        Year = create.Exchange.Year.GetValueOrDefault(-1),
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        UniveristyID = exUniversity.UniversityID,
                        EducationID = newEducation.EducationID
                    };

                    dbContext.Exchanges.Add(newExchange);
                    dbContext.SaveChanges();
                }

                if(create.Cap != null) {
                    Cap newCap = new Cap() {
                        DepartmentName = create.Cap.DepartmentName,
                        GraduationTypeID = create.Cap.GraduateType.GetValueOrDefault(-1),
                        Year = create.Cap.Year.GetValueOrDefault(-1),
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        EducationID = newEducation.EducationID
                    };

                    dbContext.Caps.Add(newCap);
                    dbContext.SaveChanges();
                }

                if(!create.Graduations.IsNullOrEmpty()) {
                    create.Graduations.ForEach(gr => dbContext.Graduations.Add(new Graduation() {
                        CityID = gr.City,
                        CountryID = gr.Country,
                        GraduateYear = gr.GraduateYear,
                        GraduationTypeID = gr.GraduateType,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        UniversityID = invoke<int>(() => {
                            //create entity
                            University newUniversity = new University() {
                                UniversityName = gr.University,
                                UniveristyDepartment = gr.DepartmentName,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now
                            };
                            //save it    
                            dbContext.Universities.Add(newUniversity);
                            dbContext.SaveChanges();
                            return newUniversity.UniversityID;//return id 
                        }),
                        EducationID = newEducation.EducationID
                    }));

                    dbContext.SaveChanges();
                }

                if(create.Social != null) {
                    Social newSocial = new Social() {
                        Linkedin = create.Social.Linkedin,
                        Twitter = string.IsNullOrEmpty(create.Social.Twitter) ? null : create.Social.Twitter,
                        Facebook = string.IsNullOrEmpty(create.Social.Facebook) ? null : create.Social.Facebook,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        PersonID = newPerson.PersonID
                    };

                    dbContext.Socials.Add(newSocial);
                    dbContext.SaveChanges();

                    newPerson.Social = newSocial;
                }

                if(create.Workplace != null) {
                    Workplace newWorkplace = new Workplace() {
                        OrganizationName = create.Workplace.OrganizationName,
                        TitleID = create.Workplace.Title.GetValueOrDefault(-1),
                        IndustryID = create.Workplace.Industry.GetValueOrDefault(-1),
                        DepartmentID = create.Workplace.Department.GetValueOrDefault(-1),
                        CityID = create.Workplace.City.GetValueOrDefault(-1),
                        CountryID = create.Workplace.Country.GetValueOrDefault(-1),
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        PersonID = newPerson.PersonID
                    };

                    dbContext.Workplaces.Add(newWorkplace);
                    dbContext.SaveChanges();

                    newPerson.Workplace = newWorkplace;
                }

                dbContext.SaveChanges();

                return new Response<string>() {
                    Code = 200,
                    Message = "your account is created, you can sign-in after we check if information you provied is valid.",
                    Data = "account saved successfully."
                };
            } catch(Exception e) {
                return new Response<string>() {
                    Code = 404,
                    Message = e.InnerException.Message,
                    Data = e.InnerException.StackTrace
                };
            }
		}

		/// <summary>
		/// Queries the people.
		/// </summary>
		/// <returns>The people.</returns>
		/// <param name="query">Query.</param>
		public Response<List<PersonProxy>> queryPeople(DataSet query) {
			string token = hasAccessToken(WebOperationContext.Current.IncomingRequest); 
			if(string.IsNullOrEmpty(token)) {
                return new Response<List<PersonProxy>>() {
					Code = 401,
					Message = "unauthroized.",
					Data = null
				};
			}

			Session session = dbContext.Sessions
									   .Where(se => se.Token.Equals(token, StringComparison.InvariantCultureIgnoreCase))
									   .FirstOrDefault();

			if(session == null || session.DueDate < DateTime.Now) {
				if(session != null) {
					dbContext.Sessions.Remove(session);
					dbContext.SaveChanges();
				}

                return new Response<List<PersonProxy>>() {
					Code = 401,
					Message = "session is expired, sign in again.",
					Data = null
				};
			}

			string[] args = invoke<string[]>(() => {
				return query.Query.Contains(',') ? query.Query.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray() 
                                                 : (query.Query + ",").Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();//one arg query might not contain that ugly comma
			});

			if(args == null || args.Length <= 0) {
                return new Response<List<PersonProxy>>() {
					Code = 404,
					Message = "query malformed.",
					Data = null
				};
			}

			//if has only one member which is easy part
			if(args.Length == 1) {
				//check if it's year
				int year;
				bool isYear = Int32.TryParse(args[0], out year);
				if(isYear) {
                    return new Response<List<PersonProxy>>() {
						Code = 200,
						Message = "success.",
						Data = PersonProxy.FromPeople(dbContext.People.Where(p => p.Education.Graduations.Where(g => g.GraduateYear == year).Count() > 0).ToList())
					};
				} else {
                    string name = args[0].Trim();//can not pass indexed array in linq ? sad very sad.
                    return new Response<List<PersonProxy>>() {
						Code = 200,
						Message = "success.",
						Data = PersonProxy.FromPeople(dbContext.People.Where(p => p.FirstName.Contains(name) || (p.MiddleName != null && p.MiddleName.Contains(name))).ToList())
					};
				}
			} else if(args.Length == 2) {
				//we do assume 2nd one is year else (quite screwed)
				string firstName = args[0].Trim();
				int year;
				bool isYear = Int32.TryParse(args[1], out year);
				if(isYear) {
                    return new Response<List<PersonProxy>>() {
						Code = 200,
						Message = "success.",
						Data = PersonProxy.FromPeople(dbContext.People.Where(p => (p.FirstName.Contains(firstName) || (p.MiddleName != null && p.MiddleName.Contains(firstName))) && p.Education.Graduations.Where(g => g.GraduateYear == year).Count() > 0).ToList())
					};
				} else {
					//can't parse as int, alright it's now last name
					string lastName = args[1].Trim();

                    List<Person> people = dbContext.People.Where(p => (p.FirstName.Contains(firstName) || (p.MiddleName != null && p.MiddleName.Contains(firstName))) && p.LastName.Contains(lastName)).ToList();

                    return new Response<List<PersonProxy>>() {
						Code = 200,
						Message = "success.",
						Data = PersonProxy.FromPeople(people)
					};
				}
			} else if(args.Length == 3) {
				//sweetest one ever!
				string name = args[0].Trim();
				string lastName = args[1].Trim();
				int year;
				Int32.TryParse(args[2], out year);
                return new Response<List<PersonProxy>>() {
					Code = 200,
					Message = "success.",
					Data = PersonProxy.FromPeople(dbContext.People.Where(p => (p.FirstName.Contains(name) || (p.MiddleName != null && p.MiddleName.Contains(name))) && p.LastName.Contains(lastName) && p.Education.Graduations.Where(g => g.GraduateYear == year).Count() > 0).ToList())
				};
			}


            return new Response<List<PersonProxy>>() {
				Code = 404,
				Message = "too many arguments.",
				Data = null
			};
		}

        /// <summary>
        /// Filters People for selected creteria
        /// </summary>
        /// <param name="selection"></param>
        /// <returns></returns>
        public Response<List<PersonProxy>> filterPeople(FilterSelection selection) {
           string token = hasAccessToken(WebOperationContext.Current.IncomingRequest); 
            if(string.IsNullOrEmpty(token)) {
                return new Response<List<PersonProxy>>() {
                    Code = 401,
                    Message = "unauthroized.",
                    Data = null
                };
            }

            Session session = dbContext.Sessions
                                       .Where(se => se.Token.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                                       .FirstOrDefault();

            if(session == null || session.DueDate < DateTime.Now) {
                if(session != null) {
                    dbContext.Sessions.Remove(session);
                    dbContext.SaveChanges();
                }

                return new Response<List<PersonProxy>>() {
                    Code = 401,
                    Message = "session is expired, sign in again.",
                    Data = null
                };
            }

            if(selection == null) {
                return new Response<List<PersonProxy>>() {
                    Code = 404,
                    Message = "Unknown filters",
                    Data = null                   
                };
            }

            Filter filter = dbContext.Filters.Where(f => f.FilterID == selection.FilterID).FirstOrDefault();
            if(filter != null) {
                List<PersonProxy> dataSet;
                if(filter.FilterType.Equals(FILTER_TITLE, StringComparison.OrdinalIgnoreCase)) {
                    dataSet = PersonProxy.FromPeople(dbContext.People.Where(p => p.Workplace != null && p.Workplace.TitleID == selection.ContentID).ToList());
                } else if(filter.FilterType.Equals(FILTER_INDUSTRY, StringComparison.OrdinalIgnoreCase)) {
                    dataSet = PersonProxy.FromPeople(dbContext.People.Where(p => p.Workplace != null && p.Workplace.IndustryID == selection.ContentID).ToList());
                } else if(filter.FilterType.Equals(FILTER_DEPARTMENT, StringComparison.OrdinalIgnoreCase)) {
                    dataSet = PersonProxy.FromPeople(dbContext.People.Where(p => p.Workplace != null && p.Workplace.DepartmentID == selection.ContentID).ToList());
                } else if(filter.FilterType.Equals(FILTER_GRADUATE_TYPE, StringComparison.OrdinalIgnoreCase)) {
                    dataSet = PersonProxy.FromPeople(dbContext.People.Where(p => p.Education.Graduations.Where(g => g.GraduationTypeID == 1).Count() > 0).ToList());
                } else {
                    return new Response<List<PersonProxy>>() {
                        Code = 404,
                        Message = "no such filter has registered",
                        Data = null                         
                    };
                }

                return new Response<List<PersonProxy>>() {
                    Code = 200,
                    Message = "success.",
                    Data = dataSet
                };
            }
            //death error check
            return new Response<List<PersonProxy>>() {
                Code = 404,
                Message = "no such filter ? not possbile but you managed to get this error, well-done!",
                Data = null
            };
        }


		/// <summary>
		/// Version this instance.
		/// </summary>
		public Response<int> version() {
			return new Response<int>() {
				Code = 200,
				Message = "success.",
				Data = 1
			};
		}

		/// <summary>
		/// Provides the titles.
		/// </summary>
		/// <returns>The titles.</returns>
		public Response<List<TitleProxy>> provideTitles() {
			return new Response<List<TitleProxy>>() {
				Code = 200,
				Message = "success.",
				Data = TitleProxy.FromTitles(dbContext.Titles.ToList())
			};
		}

		/// <summary>
		/// Provides the departments.
		/// </summary>
		/// <returns>The departments.</returns>
		public Response<List<DepartmentProxy>> provideDepartments() {
            return new Response<List<DepartmentProxy>>() {
				Code = 200,
				Message = "success.",
				Data = DepartmentProxy.FromDepartments(dbContext.Departments.ToList())
			};
		}

		/// <summary>
		/// Provides the industries.
		/// </summary>
		/// <returns>The industries.</returns>
		public Response<List<IndustryProxy>> provideIndustries() {
            return new Response<List<IndustryProxy>>() {
				Code = 200,
				Message = "success.",
				Data = IndustryProxy.FromIndustries(dbContext.Industries.ToList())
			};
		}

		/// <summary>
		/// Provides the cities.
		/// </summary>
		/// <returns>The cities.</returns>
        /// <param name="countryid">Identifier.</param>
        public Response<List<CityProxy>> provideCities(string countryid) {
            int id;
            bool parsed = Int32.TryParse(countryid, out id);
            if(!parsed) {
                return new Response<List<CityProxy>>() {
                    Code = 404,
                    Message = "invalid id paramater.",
                    Data = null
                };
            }

			return new Response<List<CityProxy>>() {
				Code = 200,
				Message = "success.",
				Data = CityProxy.FromCities(dbContext.Cities.Where(c => c.CountryID == id).OrderBy(o => o.CityName).ToList())
			};
		}

		/// <summary>
		/// Provides the countires.
		/// </summary>
		/// <returns>The countires.</returns>
		public Response<List<CountryProxy>> provideCountries() {
			return new Response<List<CountryProxy>>() {
				Code = 200,
				Message = "success.",
				Data = CountryProxy.FromCountries(dbContext.Countries.ToList())
			};
		}

        /// <summary>
        /// Provides years for selectable in graduation, can not select future too bad :P
        /// </summary>
        /// <returns></returns>
        public Response<List<YearAvailable>> provideYears() {
            int Calendar = DateTime.Now.AddYears(1).Year;//for looping year + 1 added, if server has date problems... then please shut down it!
            return new Response<List<YearAvailable>>() { 
                Code = 200,
                Message = "success.",
                Data = Enumerable.Range(0, Calendar - START_YEAR)
                                 .Select(y => new YearAvailable() {
                                     YearID = y + 1,
                                     Year = y + START_YEAR
                                 }).ToList()
            };
        }

		/// <summary>
		/// Provides the graduate types.
		/// </summary>
		/// <returns>The graduate types.</returns>
		/// <param name="type">Type.</param>
        public Response<List<GraduationTypeProxy>> provideGraduateTypes(string type) {
			//those are the ids for cap
			int[] filters = dbContext.GraduationTypes
			                         .Where(g => g.GraduationTypeName.Equals("MAJOR") || g.GraduationTypeName.Equals("MINOR"))
			                         .Select(s => s.GraduationTypeID)
			                         .ToArray();
            return type.Equals(TYPE_CAP, StringComparison.InvariantCultureIgnoreCase) ? new Response<List<GraduationTypeProxy>>() {
                Code = 200,
                Message = "success.",
                Data =  GraduationTypeProxy.FromGraduationTypes(dbContext.GraduationTypes.Where(g => filters.Contains(g.GraduationTypeID)).ToList())
            } : type.Equals(TYPE_GRAD, StringComparison.InvariantCultureIgnoreCase) ? new Response<List<GraduationTypeProxy>>() {
                Code = 200,
                Message = "success.",
                Data = GraduationTypeProxy.FromGraduationTypes(dbContext.GraduationTypes.Where(g => !filters.Contains(g.GraduationTypeID) && !g.GraduationTypeName.Equals("EXCHANGE")).ToList())
            } : new Response<List<GraduationTypeProxy>>() {
                Code = 404,
                Message = "unknow graduations type",
                Data = null
            };
        }

		/// <summary>
		/// Provides the filters.
		/// </summary>
		/// <returns>The filters.</returns>
		public Response<List<FilterProxy>> provideFilters() {
			return new Response<List<FilterProxy>>() {
				Code = 200,
				Message = "success.",
				Data = FilterProxy.FromFilters(dbContext.Filters.ToList())
			};
		}

		/// <summary>
		/// Checks if user have administration previliges.
		/// </summary>
		/// <returns>The if user have administration previliges.</returns>
		public Response<bool> checkIfUserHaveAdministrationPreviliges() {
			string token = hasAccessToken(WebOperationContext.Current.IncomingRequest);
			if(string.IsNullOrEmpty(token)) {
				return new Response<bool>() { 
					Code = 401,
					Message = "unauthorized.",
					Data = false
				};
			}
			//get session and check its user and if it exists then ask for its previlege level
			Session session = dbContext.Sessions.Where(s => s.Token.Equals(token)).FirstOrDefault();
			if(session != null) {
				User user = dbContext.Users.Where(u => u.UserID == session.UserID).FirstOrDefault();
				if(user != null) {
					return new Response<bool>() {
						Code = 200,
						Message = "success.",
						Data = user.HaveAdministratorPrivileges
					};
				}
			}

			return new Response<bool>() { 
				Code = 404,
				Message = "not found",
				Data = false
			};
		}

        /// <summary>
        /// change workplace value
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public Response<bool> changeWorkplace(WorkplaceInfoProxy proxy) {
            string token = hasAccessToken(WebOperationContext.Current.IncomingRequest);
            if(string.IsNullOrEmpty(token)) {
                return new Response<bool>() {
                    Code = 401,
                    Message = "unauthroized.",
                    Data = false
                };
            }

            Session session = dbContext.Sessions
                                       .Where(se => se.Token.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                                       .FirstOrDefault();

            if(session == null || session.DueDate < DateTime.Now) {
                if(session != null) {
                    dbContext.Sessions.Remove(session);
                    dbContext.SaveChanges();
                }

                return new Response<bool>() {
                    Code = 401,
                    Message = "session is expired, sign in again.",
                    Data = false
                };
            }
            //reach workplaces
            Person person = dbContext.People.Where(p => p.UserID == session.UserID).FirstOrDefault();
            if(person != null) {
                Workplace workplace = dbContext.Workplaces.Where(w => w.PersonID == person.PersonID).FirstOrDefault();
                if(workplace != null) {
                    if(proxy.Country.HasValue) {
                        workplace.CountryID = proxy.Country.Value;
                    }
                    if(proxy.City.HasValue) {
                        workplace.CityID = proxy.City.Value;
                    }
                    if(proxy.Title.HasValue) {
                        workplace.TitleID = proxy.Title.Value;
                    }
                    if(proxy.Department.HasValue) {
                        workplace.DepartmentID = proxy.Department.Value;
                    }
                    if(proxy.Industry.HasValue) {
                        workplace.IndustryID = proxy.Industry.Value;
                    }
                    workplace.OrganizationName = proxy.OrganizationName;
                    workplace.UpdateDate = DateTime.Now;

                    dbContext.SaveChanges();

                    return new Response<bool>() {
                        Code = 200,
                        Message = "success.",
                        Data = true
                    };
                } else { 
                    //already does not have it
                    workplace = new Workplace() {
                         CountryID =proxy.Country.Value,
                         CityID = proxy.City.Value,
                         TitleID = proxy.Title.Value,
                         DepartmentID = proxy.Department.Value,
                         IndustryID = proxy.Industry.Value,
                         OrganizationName = proxy.OrganizationName,
                         PersonID = person.PersonID,
                         CreateDate = DateTime.Now,
                         UpdateDate = DateTime.Now
                    };

                    dbContext.Workplaces.Add(workplace);
                    dbContext.SaveChanges();

                    return new Response<bool>() {
                        Code = 200,
                        Message = "success.",
                        Data = true
                    };
                }               
            }
            //multiple reason for failure
            return new Response<bool>() {
                Code = 404,
                Message = "not available.",
                Data = false
           };;
        }


		/// <summary>
		/// Activates the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="UserID">User identifier.</param>
		public Response<bool> activateUser(string uid) {
			string token = hasAccessToken(WebOperationContext.Current.IncomingRequest);
			if(string.IsNullOrEmpty(token)) {
				return new Response<bool>() {
					Code = 401,
					Message = "unauthorized.",
					Data = false
				};
			}

            int UserID;
            bool parsed = Int32.TryParse(uid, out UserID);
            if(!parsed) {
                return new Response<bool>() {
                    Code = 404,
                    Message = "invalid user id.",
                    Data = false
                };
            }

			Session session = dbContext.Sessions.Where(se => se.Token.Equals(token)).FirstOrDefault();
			if(session != null) {
				User sUser = dbContext.Users.Where(us => us.UserID == session.UserID).FirstOrDefault();
				if(sUser != null) {
					if(sUser.HaveAdministratorPrivileges) {
						User activate = dbContext.Users.Where(u => u.UserID == UserID).FirstOrDefault();
						if(activate != null) {
							activate.IsApproved = !activate.IsApproved;//this enables us to activate-deacctivate users state as we wish 
							dbContext.SaveChanges();

							return new Response<bool>() {
								Code = 200,
								Message = "success.",
								Data = activate.IsApproved
							};
						}
					}
				}
			}

			return new Response<bool>() {
				Code = 404,
				Message = "unavailable.",
				Data = false
			};
		}

        public Response<UserProxy> resetUserPassword(string uid) {
            string token = hasAccessToken(WebOperationContext.Current.IncomingRequest);
            if(string.IsNullOrEmpty(token)) {
                return new Response<UserProxy>() {
                    Code = 401,
                    Message = "unauthorized.",
                    Data = null
                };
            }

            int UserID;
            bool parsed = Int32.TryParse(uid, out UserID);
            if(!parsed) {
                return new Response<UserProxy>() {
                    Code = 404,
                    Message = "invalid user id.",
                    Data = null
                };
            }

            Session session = dbContext.Sessions.Where(se => se.Token.Equals(token)).FirstOrDefault();
            if(session != null) {
                User sUser = dbContext.Users.Where(us => us.UserID == session.UserID).FirstOrDefault();
                if(sUser != null) {
                    if(sUser.HaveAdministratorPrivileges) {
                        User activate = dbContext.Users.Where(u => u.UserID == UserID).FirstOrDefault();
                        if(activate != null) {
                            activate.IsApproved = false;//we first deactivate user which wants to change her/his password
                            activate.ResetToken = newToken();
                            activate.ResetTokenDue = DateTime.Now.AddDays(3);//3 days fair enough for any user to change his/her password
                            
                            dbContext.SaveChanges();

                            return new Response<UserProxy>() {
                                Code = 200,
                                Message = "success.",
                                Data = new UserProxy(activate)
                            };
                        }
                    }
                }
            }

            return new Response<UserProxy>() {
                Code = 404,
                Message = "unavailable.",
                Data = null
            };
        }

        public Response<bool> changePassword(ChangePassword change) {
            if(change == null) {
                return new Response<bool>() {
                    Code = 404,
                    Message = "invlaid params.",
                    Data = false
                };
            }

            User activateUser = dbContext.Users.FirstOrDefault(u => u.ResetToken.Equals(change.ResetToken));
            if(activateUser == null || string.IsNullOrEmpty(change.NewPassword)) {
                return new Response<bool>() {
                    Code = 404,
                    Message = "you do not have such request.",
                    Data = false
                };
            }

            if(activateUser.ResetTokenDue < DateTime.Now) {
                return new Response<bool>() {
                    Code = 404,
                    Message = "your change time limit has expired, request new one.",
                    Data = false
                };
            }

            activateUser.ResetToken = null;
            activateUser.Password = change.NewPassword;
            activateUser.IsApproved = true;

            dbContext.Entry(activateUser).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            //change occured
            return new Response<bool>() {
                Code = 200,
                Message = "success.",
                Data = true
            };            
        }

		//HELPER METHODS START, invokes lamda anywhere and returns result
		protected T invoke<T>(Func<T> fn) {
			return fn();
		}

        /// <summary>
        /// Checks where access-token required to consume this endpoint instance, in header ;)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
		protected string hasAccessToken(IncomingWebRequestContext request) {
			return invoke<string>(() => {
				List<string> keys = request.Headers
				                           .AllKeys.ToList();
				
				return keys.Where(k => k.Equals(KEY_ACCESS_TOKEN))
					       .Select(key => request.Headers.Get(keys.IndexOf(key)))
					       .FirstOrDefault();
			});
		}

        /// <summary>
        /// Generates Access Token for each session, love of guid
        /// </summary>
        /// <returns></returns>
		protected string newToken() {
			return Guid.NewGuid().ToString()
				                 .Replace("-", string.Empty)
				                 .ToUpper();
		}
	}
}

