using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GraduateEntity;

namespace GraduateWeb {
	
	[ServiceContract]
	public interface IEndpoint {

		[OperationContract]
		[WebInvoke(Method = "POST",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/sign-in")]
		Response<Session> checkUser(LoginRequest usr);

		[OperationContract]
		[WebInvoke(Method = "POST",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/check-if-exists")]
		Response<bool> checkUserNameIfExists(string usrname);

		[OperationContract]
		[WebInvoke(Method = "POST",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/create-an-account")]
		Response<string> createUser();

		[OperationContract]
		[WebInvoke(Method = "POST",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/search")]
		Response<List<Person>> queryPeople(DataSet query);

		//TODO add some auto complete text here for using in auto-complete in various inputs
	}

	[DataContract]
	public class DataSet { 
		[DataMember(Name = "q")]
		public string Query { get; set; }
	}

	[DataContract]
	public class LoginRequest { 
		[DataMember(Name = "Username")]
		public string UserName { get; set; }
		[DataMember]
		public string Password { get; set; }
	}
}

