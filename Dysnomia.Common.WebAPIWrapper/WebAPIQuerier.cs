using Dysnomia.Common.WebAPIWrapper.Exceptions;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dysnomia.Common.WebAPIWrapper {
	public class WebAPIQuerier {
		private readonly IHttpClientFactory _clientFactory;
		private readonly Dictionary<string, string> _defaultHeaders;

		public WebAPIQuerier(IHttpClientFactory clientFactory) {
			_clientFactory = clientFactory;
		}

		public WebAPIQuerier(IHttpClientFactory clientFactory, Dictionary<string, string> defaultHeaders) {
			_clientFactory = clientFactory;
			_defaultHeaders = defaultHeaders;
		}

		private HttpClient GetClient(Dictionary<string, string> headers = null) {
			var client = _clientFactory.CreateClient();

			if (_defaultHeaders != null) {
				foreach (var header in _defaultHeaders) {
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}
			}

			if (headers != null) {
				foreach (var header in headers) {
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}
			}

			return client;
		}

		private static void ThrowOnStatusCode(HttpStatusCode statusCode, string apiError) {
			switch (statusCode) {
				case HttpStatusCode.BadRequest:
					throw new BadRequestException(apiError);

				case HttpStatusCode.Unauthorized: // 401
				case HttpStatusCode.Forbidden: // 403
					throw new ForbiddenException(apiError);

				case HttpStatusCode.TooManyRequests:
					throw new TooManyRequestsException(apiError);

				case HttpStatusCode.InternalServerError: // 500
				case HttpStatusCode.BadGateway: // 502
				case HttpStatusCode.ServiceUnavailable: // 503
				case HttpStatusCode.GatewayTimeout: // 504
					throw new InternalServerErrorException(apiError);
				default:
					// Do nothing
					break;
			}
		}

		protected static async Task ThrowApiErrorsAsync(HttpResponseMessage response) {
			ArgumentNullException.ThrowIfNull(response);

			string apiError = null;
			if (response.Content is not null) {
				try {
					apiError = await response.Content.ReadAsStringAsync();
				} catch {
					// Do nothing
				}
			}

			ThrowOnStatusCode(response.StatusCode, apiError);
		}

		protected async Task<string> GetStringAsync(Uri url, Dictionary<string, string> headers) {
			var response = await GetClient(headers).GetAsync(url);

			await ThrowApiErrorsAsync(response);

			return await response.Content.ReadAsStringAsync();
		}

		protected async Task<string> GetStringAsync(Uri url) {
			return await GetStringAsync(url, null);
		}

		protected async Task<string> GetStringAsync(string url, Dictionary<string, string> headers) {
			return await GetStringAsync(new Uri(url), headers);
		}

		protected async Task<string> GetStringAsync(string url) {
			return await GetStringAsync(new Uri(url), null);
		}

		protected async Task<T> GetAsync<T>(Uri url, Dictionary<string, string> headers) {
			return JsonSerializer.Deserialize<T>(await GetStringAsync(url, headers));
		}

		protected async Task<T> GetAsync<T>(Uri url) {
			return await GetAsync<T>(url, null);
		}

		protected async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers) {
			return await GetAsync<T>(new Uri(url), headers);
		}

		protected async Task<T> GetAsync<T>(string url) {
			return await GetAsync<T>(new Uri(url), null);
		}

		protected async Task<string> PostStringAsync(Uri url, HttpContent content, Dictionary<string, string> headers) {
			var response = await GetClient(headers).PostAsync(url, content);

			await ThrowApiErrorsAsync(response);

			return await response.Content.ReadAsStringAsync();
		}

		protected async Task<string> PostStringAsync(Uri url, HttpContent content) {
			return await PostStringAsync(url, content, null);
		}

		protected async Task<string> PostStringAsync(string url, HttpContent content, Dictionary<string, string> headers) {
			return await PostStringAsync(new Uri(url), content, headers);
		}

		protected async Task<string> PostStringAsync(string url, HttpContent content) {
			return await PostStringAsync(new Uri(url), content, null);
		}

		protected async Task PostAsync(Uri url, HttpContent content, Dictionary<string, string> headers) {
			var response = await GetClient(headers).PostAsync(url, content);

			await ThrowApiErrorsAsync(response);
		}

		protected async Task PostAsync(Uri url, HttpContent content) {
			await PostAsync(url, content, null);
		}

		protected async Task PostAsync(string url, HttpContent content, Dictionary<string, string> headers) {
			await PostAsync(new Uri(url), content, headers);
		}

		protected async Task PostAsync(string url, HttpContent content) {
			await PostAsync(new Uri(url), content, null);
		}

		protected async Task<T> PostAsync<T>(Uri url, HttpContent content, Dictionary<string, string> headers) {
			return JsonSerializer.Deserialize<T>(await PostStringAsync(url, content, headers));
		}

		protected async Task<T> PostAsync<T>(Uri url, HttpContent content) {
			return await PostAsync<T>(url, content, null);
		}

		protected async Task<T> PostAsync<T>(string url, HttpContent content, Dictionary<string, string> headers) {
			return await PostAsync<T>(new Uri(url), content, headers);
		}

		protected async Task<T> PostAsync<T>(string url, HttpContent content) {
			return await PostAsync<T>(new Uri(url), content, null);
		}
	}
}
