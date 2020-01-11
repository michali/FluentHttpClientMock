using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientMock
{
    public class Expectation
    {
        private readonly HttpClientMockBuilder _httpClientWithExpectationsBuilder;
        private readonly ICollection<Predicate<HttpRequestMessage>> _conditions;

        public Expectation(HttpClientMockBuilder httpClientWithExpectationsBuilder,
            ICollection<Predicate<HttpRequestMessage>> conditions)
        {
            _httpClientWithExpectationsBuilder = httpClientWithExpectationsBuilder;
            _conditions = conditions;
        }

        public HttpClientMockBuilder ResponseShouldBe(string expectedResponseContent, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
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