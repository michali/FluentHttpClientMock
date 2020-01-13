using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientMock
{
    /// <summary>
    /// Encapsulates the expectations set on a mocked <c>HttpClient</c>
    /// </summary>
    public class Expectation
    {
        private readonly HttpClientMocker _httpClientWithExpectationsBuilder;
        private readonly ICollection<Predicate<HttpRequestMessage>> _conditions;

        internal Expectation(HttpClientMocker httpClientWithExpectationsBuilder,
            ICollection<Predicate<HttpRequestMessage>> conditions)
        {
            _httpClientWithExpectationsBuilder = httpClientWithExpectationsBuilder;
            _conditions = conditions;
        }

        /// <summary>
        /// Sets expectations on the response of a mocked <c>HttpClient</c>
        /// </summary>
        /// <param name="expectedResponseContent">The expected content in the HTTP response body</param>
        /// <param name="expectedStatusCode">The expected HTTP Status Code</param>
        /// <returns></returns>
        public HttpClientMocker ResponseShouldBe(string expectedResponseContent, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var handler = new HttpMessageHandlerStub(async (request, cancellationToken) =>
            {
                foreach (var condition in _conditions)
                {
                    if (!condition(request))
                    {
                        return await Task.FromResult(new HttpResponseMessage
                        {
                            Content = new ByteArrayContent(new byte[] { })
                        });
                    }
                }

                var responseMessage = new HttpResponseMessage(expectedStatusCode)
                {
                    Content = expectedResponseContent != null ? new StringContent(expectedResponseContent)
                    : new ByteArrayContent(new byte[] { })
                };

                return await Task.FromResult(responseMessage);
            });

            _httpClientWithExpectationsBuilder.MessageHandler = handler;
            return _httpClientWithExpectationsBuilder;
        }
    }
}