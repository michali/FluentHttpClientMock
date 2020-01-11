using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientMock
{
    public class HttpClientMockBuilder
    {
        private static ConditionSet _conditionSet;
        internal HttpMessageHandlerStub MessageHandler { private get; set; }

        public string AbsoluteRequestUri => MessageHandler.AbsoluteRequestUri;
        public string RequestContent => MessageHandler.RequestContent;

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
            if (MessageHandler == null)
            {
                MessageHandler = new HttpMessageHandlerStub();
            }
            return new HttpClient(MessageHandler);
        }

        public HttpClientMockBuilder SetRequestMessageString(string requestContent)
        {
            var handler = new HttpMessageHandlerStub(async (request, cancellationToken) =>
            {          
                var responseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(requestContent)
                };

                return await Task.FromResult(responseMessage);
            });

            MessageHandler = handler;

            return this;
        }
    }
}
