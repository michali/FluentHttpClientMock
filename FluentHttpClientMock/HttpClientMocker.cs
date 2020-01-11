using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientMock
{
    public class HttpClientMocker
    {
        private static ConditionSet _conditionSet;
        internal HttpMessageHandlerStub MessageHandler { private get; set; }

        public string AbsoluteRequestUri => MessageHandler.AbsoluteRequestUri;
        public string RequestContent => MessageHandler.RequestContent;

        private HttpClientMocker()
        {
            _conditionSet = new ConditionSet(this);
        }

        public static HttpClientMocker Create()
        {
            return new HttpClientMocker();
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
    }
}
