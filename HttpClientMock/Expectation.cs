using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientMock
{
    public class Expectation
    {
        private readonly HttpClientMockBuilder _httpClientWithExpectationsBuilder;
        private readonly Predicate<HttpRequestMessage> _condition;

        public Expectation(HttpClientMockBuilder httpClientWithExpectationsBuilder,
            Predicate<HttpRequestMessage> condition)
        {
            _httpClientWithExpectationsBuilder = httpClientWithExpectationsBuilder;
            _condition = condition;
        }

        public HttpClientMockBuilder ResponseShouldBe(string expectedResponseContent, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var handler = new HttpMessageHandlerStub(async (request, cancellationToken) =>
            {
                if (!_condition(request))
                {
                    return await Task.FromResult(new HttpResponseMessage
                    {
                        Content = new ByteArrayContent(new byte[]{})
                    });
                }                

                var responseMessage = new HttpResponseMessage(expectedStatusCode)
                {
                    Content = expectedResponseContent !=  null ? new StringContent(expectedResponseContent)
                    : new ByteArrayContent(new byte[]{})
                };

                return await Task.FromResult(responseMessage);
            });

            _httpClientWithExpectationsBuilder.MessageHandler = handler;
            return _httpClientWithExpectationsBuilder;
        }
    }
}