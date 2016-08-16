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
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GraduateEntity;
using GraduateEntity.Model;
using GraduateEntityProxy;

namespace GraduateWeb {
	
	[ServiceContract]
	public interface IEndpoint {

		[OperationContract]
		[WebInvoke(Method = "POST",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/sign-in")]
		Response<SessionProxy> checkUser(LoginRequest usr);

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
		Response<string> createUser(CreateProxy create);

		[OperationContract]
		[WebInvoke(Method = "POST",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/search")]
		Response<List<PersonProxy>> queryPeople(DataSet query);

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/workplace-titles")]
		Response<List<TitleProxy>> provideTitles();

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/workplace-industries")]
		Response<List<IndustryProxy>> provideIndustries();

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/workplace-departments")]
		Response<List<DepartmentProxy>> provideDepartments();

        [OperationContract]
        [WebInvoke(Method = "GET",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare,
                   UriTemplate = "/graduation-types/{type}")]//only for cap or graduation
        Response<List<GraduationTypeProxy>> provideGraduateTypes(string type);

        [OperationContract]
        [WebInvoke(Method = "GET",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare,
                   UriTemplate = "/years")]
        Response<List<YearAvailable>> provideYears();

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/cities/{countryid}")]
        Response<List<CityProxy>> provideCities(string countryid);

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/filters")]
		Response<List<FilterProxy>> provideFilters();

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/has-authority")]
		Response<bool> checkIfUserHaveAdministrationPreviliges();

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/activate-user/{uid}")]
		Response<bool> activateUser(string uid);

        [OperationContract]
        [WebInvoke(Method = "GET",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare,
                   UriTemplate = "/reset-password/{id}")]
        Response<UserProxy> resetUserPassword(string id);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare,
                   UriTemplate = "/change-password")]
        Response<bool> changePassword(ChangePassword change);

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/countries")]
		Response<List<CountryProxy>> provideCountries();

        [OperationContract]
        [WebInvoke(Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare,
                   UriTemplate = "/filter")]
        Response<List<PersonProxy>> filterPeople(FilterSelection selection);

        [OperationContract]
        [WebInvoke(Method = "POST",
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json,
                   BodyStyle = WebMessageBodyStyle.Bare,
                   UriTemplate = "/workplace-change")]
        Response<bool> changeWorkplace(WorkplaceInfoProxy proxy);

		[OperationContract]
		[WebInvoke(Method = "GET",
				   RequestFormat = WebMessageFormat.Json,
				   ResponseFormat = WebMessageFormat.Json,
				   BodyStyle = WebMessageBodyStyle.Bare,
				   UriTemplate = "/version")]
		Response<int> version();
		//TODO add some auto complete text here for using in auto-complete in various inputs
	}

	[DataContract]
	public class DataSet { 
		[DataMember(Name = "q")]
		public string Query { get; set; }
	}

	[DataContract]
	public class LoginRequest { 
		[DataMember]
		public string UserName { get; set; }
		[DataMember]
		public string Password { get; set; }
	}

    [DataContract]
    public class ChangePassword {
        [DataMember]
        public string ResetToken { get ; set; }
        [DataMember]
        public string NewPassword { get; set; }
    }

    [DataContract]
    public class FilterSelection {
        [DataMember]
        public int FilterID { get; set; }
        [DataMember]
        public int ContentID { get; set; }
    }
}

