using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Dysnomia.Common.WebAPIWrapper.Exceptions;

namespace Dysnomia.Common.WebAPIWrapper {
	public class WebAPIQuerier {
		private readonly IHttpClientFactory _clientFactory;
		private readonly Dictionary<string, string> _defaultHeaders;

		public WebAPIQuerier(IHttpClientFactory clientFactory, Dictionary<string, string> defaultHeaders = null) {
			_clientFactory = clientFactory;
			_defaultHeaders = defaultHeaders;
		}

		private HttpClient GetClient() {
			var client = _clientFactory.CreateClient();

			if (_defaultHeaders != null) {
				foreach (var header in _defaultHeaders) {
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}
			}

			return client;
		}

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
			var response = await GetClient().GetAsync(url);

			await this.ThrowAPIErrors(response);

			return await response.Content.ReadAsStringAsync();
		}

		protected async Task<T> Get<T>(string url) {
			return JsonSerializer.Deserialize<T>(await GetString(url));
		}

		protected async Task Post(string url, HttpContent content) {
			var response = await GetClient().PostAsync(url, content);

			await this.ThrowAPIErrors(response);
		}

		protected async Task<string> PostString(string url, HttpContent content) {
			var response = await GetClient().PostAsync(url, content);

			await this.ThrowAPIErrors(response);

			return await response.Content.ReadAsStringAsync();
		}

		protected async Task<T> Post<T>(string url, HttpContent content) {
			return JsonSerializer.Deserialize<T>(await PostString(url, content));
		}
	}
}
