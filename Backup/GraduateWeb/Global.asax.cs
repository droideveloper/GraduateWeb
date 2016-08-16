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
			routes.Ignore("{resource}.axd/{*pathInfo}");
			//three pages available only
			routes.Add(new ServiceRoute("v1/endpoint", new WebServiceHostFactory(), typeof(Endpoint)));
			routes.MapPageRoute("index", "sign-in", "~/index.html");
			routes.MapPageRoute("create-an-account", "create-an-account", "~/create.html");
			routes.MapPageRoute("query", "search", "~/query.html");
		}
	}

	//DELTE THIS TODO 
	public class ServiceRoute : RouteBase {
		public ServiceRoute(string str, WebServiceHostFactory factory, Type type) { }

		public override RouteData GetRouteData(HttpContextBase httpContext) {
			throw new NotImplementedException();
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
			throw new NotImplementedException();
		}
	}
}
