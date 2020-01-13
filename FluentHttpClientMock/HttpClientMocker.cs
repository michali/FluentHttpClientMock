using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientMock
{
    /// <summary>
    /// Creates an HttpClient with conditions and expectations
    /// </summary>
    public class HttpClientMocker
    {
        private static ConditionSet _conditionSet;

        /// <summary>
        /// A fake <c>HttpMessageHandler</c> that does the work of accessing
        /// the web
        /// </summary>
        internal HttpMessageHandlerStub MessageHandler { private get; set; }

        /// <summary>
        /// Gets the Absolute URI used in the last request of the <c>HttpClient</c>
        /// </summary>
        public string AbsoluteRequestUri => MessageHandler.AbsoluteRequestUri;

        /// <summary>
        /// Gets the content of the request last sent through by the <c>HttpClient</c>
        /// </summary>
        public string RequestContent => MessageHandler.RequestContent;

        private HttpClientMocker()
        {
            _conditionSet = new ConditionSet(this);
        }

        /// <summary>
        /// Creates a mocker
        /// </summary>
        /// <returns>A new <c>HttpClientMocker</c> object</returns>
        public static HttpClientMocker Create()
        {
            return new HttpClientMocker();
        }

        /// <summary>
        /// Starts a set of conditions on the <c>HttpClient</c>
        /// </summary>
        public ConditionSet When => _conditionSet;

        /// <summary>
        /// Creates a mocked <c>HttpClient</c> with the conditions and expectations set.
        /// If no conditions and expectations are set, a default mock is created with
        /// no conditions and expectations.
        /// </summary>
        /// <returns>A mocked <c>HttpClient</c></returns>
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
