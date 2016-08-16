using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using GraduateEntity;
using System.Security.Principal;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Data.Entity.Core.Metadata.Edm;

namespace GraduateWeb {
	public class Endpoint : IEndpoint {

		private const string KEY_ACCESS_TOKEN = "X-Access-Token";

		private GraduateDbContext dbContext = new GraduateDbContext();

		/// <summary>
		/// /sign-in user method handle
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="usr">Usr.</param>
		public Response<Session> checkUser(LoginRequest usr) {
			if(usr == null) {
				return new Response<Session>() {
					Code = 404,
					Message = "Enter username and password.",
					Data = null
				};
			}

			User user = dbContext.Users
			                     .Where(u => u.UserName.Equals(usr.UserName))
			                     .FirstOrDefault();

			if(user == null) {
				return new Response<Session>() {
					Code = 404,
					Message = "No such user, username is case sensetive.",
					Data = null
				};
			}

			if(!user.Password.Equals(usr.Password)) {
				return new Response<Session>() {
					Code = 404,
					Message = "invalid password, password is case sensetive.",
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
			return new Response<Session>() {
				Code = 200,
				Message = "success.",
				Data = session
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

		public Response<string> createUser() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Queries the people.
		/// </summary>
		/// <returns>The people.</returns>
		/// <param name="query">Query.</param>
		public Response<List<Person>> queryPeople(DataSet query) {
			string token = hasAccessToken(WebOperationContext.Current.IncomingRequest);
			if(string.IsNullOrEmpty(token)) {
				return new Response<List<Person>>() {
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

				return new Response<List<Person>>() {
					Code = 401,
					Message = "session is expired, sign in again.",
					Data = null
				};
			}

			string[] args = invoke<string[]>(() => {
				return query.Query.Contains(',') ? query.Query.Split(',') : (query.Query + ",").Split(',');//one arg query might not contain that ugly comma
			});

			if(args == null || args.Length <= 0) {
				return new Response<List<Person>>() {
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
					return new Response<List<Person>>() {
						Code = 200,
						Message = "success.",
						Data = dbContext.People.Where(p => p.Education.Graduations
																		.Where(g => g.GraduateYear.Year == year).Count() > 0)
												.ToList()
					};
				} else {
					return new Response<List<Person>>() {
						Code = 200,
						Message = "success.",
						Data = dbContext.People.Where(p => p.FirstName.Contains(args[0]) || (p.MiddleName != null && p.MiddleName.Contains(args[0])))
											   .ToList()
					};
				}
			} else if(args.Length == 2) {
				//we do assume 2nd one is year else (quite screwed)
				string firstName = args[0];
				int year;
				bool isYear = Int32.TryParse(args[1], out year);
				if(isYear) {
					return new Response<List<Person>>() {
						Code = 200,
						Message = "success.",
						Data = dbContext.People.Where(p => (p.FirstName.Contains(firstName)
													  || (p.MiddleName != null && p.MiddleName.Contains(firstName)))
													  && p.Education.Graduations.Where(g => g.GraduateYear.Year == year).Count() > 0)
												.ToList()
					};
				} else {
					//can't parse as int, alright it's now last name
					string lastName = args[1];
					return new Response<List<Person>>() {
						Code = 200,
						Message = "success.",
						Data = dbContext.People.Where(p => (p.FirstName.Contains(firstName)
															|| (p.MiddleName != null && p.MiddleName.Contains(firstName)))
													  && p.LastName.Contains(lastName))
												.ToList()
					};
				}
			} else if(args.Length == 3) {
				//sweetest one ever!
				string name = args[0];
				string lastName = args[1];
				int year;
				Int32.TryParse(args[2], out year);
				return new Response<List<Person>>() {
					Code = 200,
					Message = "success.",
					Data = dbContext.People.Where(p => (p.FirstName.Contains(name) || (p.MiddleName != null && p.MiddleName.Contains(name)))
												  && p.LastName.Contains(lastName)
												  && p.Education.Graduations.Where(g => g.GraduateYear.Year == year).Count() > 0)
											.ToList()
				};
			}


			return new Response<List<Person>>() {
				Code = 404,
				Message = "too many arguments.",
				Data = null
			};
		}

		protected T invoke<T>(Func<T> fn) {
			return fn();
		}

		protected string hasAccessToken(IncomingWebRequestContext request) {
			return invoke<string>(() => {
				List<string> keys = request.Headers
				                           .AllKeys.ToList();
				
				return keys.Where(k => k.Equals(KEY_ACCESS_TOKEN))
					       .Select(key => request.Headers.Get(keys.IndexOf(key)))
					       .FirstOrDefault();
			});
		}

		protected string newToken() {
			byte[] sink = Guid.NewGuid().ToByteArray();
			StringBuilder str = new StringBuilder();
			sink.ToList()
			    .ForEach(b => str.Append(b.ToString("X2")));
			return str.ToString();
		}
	}
}

