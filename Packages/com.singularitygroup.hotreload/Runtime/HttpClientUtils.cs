#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System.Net.Http;

namespace SingularityGroup.HotReload {

	public class HttpClientUtils { 
		public static HttpClient CreateHttpClient() {
			var handler = new HttpClientHandler {
				// Without this flag HttpClients don't work for PCs with double-byte characters in the name
				UseCookies = false
			};
            
			return new HttpClient(handler);
		}
	}

}

#endif