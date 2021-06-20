using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Dysnomia.Common.WebAPIWrapper.Exceptions;

namespace Dysnomia.Common.WebAPIWrapper {
	public class WebAPIQuerier {
		protected async Task ThrowAPIErrors(HttpResponseMessage response) {
			switch (response.StatusCode) {
				case HttpStatusCode.Unauthorized: // 401
				case HttpStatusCode.Forbidden: // 403
					throw new ForbiddenException(await response.Content.ReadAsStringAsync());

				case HttpStatusCode.TooManyRequests:
					throw new TooManyRequestsException(await response.Content.ReadAsStringAsync());

				case HttpStatusCode.InternalServerError: // 500
				case HttpStatusCode.BadGateway: // 502
				case HttpStatusCode.ServiceUnavailable: // 503
				case HttpStatusCode.GatewayTimeout: // 504
					throw new InternalServerErrorException(await response.Content.ReadAsStringAsync());
			}
		}

		protected async Task<string> GetString(string url) {
			using HttpClient httpClient = new HttpClient();

			var response = await httpClient.GetAsync(url);

			await this.ThrowAPIErrors(response);

			return await response.Content.ReadAsStringAsync();
		}

		protected async Task<T> Get<T>(string url) {
			return JsonSerializer.Deserialize<T>(await GetString(url));
		}

		protected async Task Post(string url, HttpContent content) {
			using HttpClient httpClient = new HttpClient();

			var response = await httpClient.PostAsync(url, content);

			await this.ThrowAPIErrors(response);
		}

		protected async Task<string> PostString(string url, HttpContent content) {
			using HttpClient httpClient = new HttpClient();

			var response = await httpClient.PostAsync(url, content);

			await this.ThrowAPIErrors(response);

			return await response.Content.ReadAsStringAsync();
		}

		protected async Task<T> Post<T>(string url, HttpContent content) {
			return JsonSerializer.Deserialize<T>(await PostString(url, content));
		}
	}
}
