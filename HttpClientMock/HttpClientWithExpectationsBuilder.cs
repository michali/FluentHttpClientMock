using System;
using System.Net.Http;

namespace HttpClientMock
{
    public class HttpClientMockBuilder
    {
        private static ConditionSet _conditionSet;
        internal HttpMessageHandlerStub MessageHandler { private get; set; }

        private HttpClientMockBuilder()
        {
            _conditionSet = new ConditionSet(this);
        }

        public static HttpClientMockBuilder Create()
        {
            return new HttpClientMockBuilder();
        }

        public ConditionSet When => _conditionSet;

        public HttpClient Build()
        {
            return new HttpClient(MessageHandler);
        }
    }
}
