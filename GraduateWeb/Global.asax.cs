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
using System.Web;
using System.Web.Routing;
using System.ServiceModel.Activation;
using System;

namespace GraduateWeb {
	public class Global : HttpApplication {
		protected void Application_Start() {
			//Register our routes
			RegisterRoutes(RouteTable.Routes);
		}

		protected void RegisterRoutes(RouteCollection routes) {
			//ignore
			//routes.Ignore("{resource}.axd/{*pathInfo}");
			//service binding
			//pages start
			routes.MapPageRoute("index", "sign-in", "~/index.aspx");
			routes.MapPageRoute("create-an-account", "create-an-account", "~/create.aspx");
			routes.MapPageRoute("query", "search", "~/query.aspx");
            routes.MapPageRoute("change-password", "change-password/{*anyString}", "~/reset.aspx");

            routes.Add(new ServiceRoute("v1/endpoint", new WebServiceHostFactory(), typeof(Endpoint)));

		}
	}
}
