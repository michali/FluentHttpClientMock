using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientMock
{
    internal class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;
        public string AbsoluteRequestUri { get; private set; }
        public string RequestContent { get; private set; }

        internal HttpMessageHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        {
            _sendAsync = sendAsync;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AbsoluteRequestUri = request.RequestUri.AbsoluteUri;

            if (request.Method != HttpMethod.Get) // todo: better handling of empty request content
                RequestContent = await request.Content?.ReadAsStringAsync();

            return await _sendAsync(request, cancellationToken);
        }
    }
}