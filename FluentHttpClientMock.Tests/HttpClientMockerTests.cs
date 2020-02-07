using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace HttpClientMock.Tests
{
    public class HttpClientMockerTests
    {
        [Fact]
        public async Task WhenUriIsSet_ExpectUri()
        {
            var responseContent = "content";
            var requestUrl = "https://path.to.url/";
            var mocker = HttpClientMocker.Create();

            mocker.When.AbsoluteUrlIs(requestUrl)
            .Then.ResponseShouldBe(responseContent);

            var client = mocker.Build();

            var response = await client.GetAsync(requestUrl);

            Assert.Equal(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenNotExpectedRequestUriIsCalled_DoNotReturnExpectedContentasAResponse()
        {            
            var responseContent = "content";

            var mocker = HttpClientMocker.Create();

            mocker.When.AbsoluteUrlIs("https://path.to.url/")
            .Then.ResponseShouldBe(responseContent);

            var client = mocker.Build();

            var response = await client.GetAsync("https://path.to.anotherurl/");

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenSettingExpectedRequestContent_ShouldReturnExpectedResponse()
        {
            var requestContent = "request content";
            var responseContent = "response content";

            var mocker = HttpClientMocker.Create();

            mocker.When.RequestMessageStringIs(requestContent)
            .Then.ResponseShouldBe(responseContent);

            var client = mocker.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent(requestContent));

            Assert.Equal(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenExpectedRequestContentIsNotSent_ShouldNotReturnExpectedResponse()
        {
            var responseContent = "response content";
            var mocker = HttpClientMocker.Create();

            mocker.When.RequestMessageStringIs(responseContent)
            .Then.ResponseShouldBe("response content");

            var client = mocker.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("other request content"));

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ShouldReturnEmptyResponse()
        {
            var mocker = HttpClientMocker.Create();
            mocker.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe(null);

            var client = mocker.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("request content"));

            var content = Assert.IsType<ByteArrayContent>(response.Content);
            Assert.Empty(await content.ReadAsByteArrayAsync());
        }

        [Fact]
        public async Task SetsResponseStatusCode()
        {
            var mocker = HttpClientMocker.Create();
            mocker.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe(null, HttpStatusCode.Accepted);

            var client = mocker.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("request content"));

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

       [Fact]
       public async Task RetrievesAbsoluteUri()
       {
            var expectedUrl = "https://path.to.url/";
            var mocker = HttpClientMocker.Create();

            var client = mocker.Build();

            await client.PostAsync(expectedUrl, new ByteArrayContent(new byte[]{}));

            Assert.Equal(expectedUrl, mocker.AbsoluteRequestUri);
       }

       [Fact]
       public async Task RetrievesRequestContent()
       {
            var expectedRequestContent = "request content";
            var mocker = HttpClientMocker.Create();

            var client = mocker.Build();

            await client.PostAsync("https://path.to.url/", new StringContent(expectedRequestContent));

            Assert.Equal(expectedRequestContent, mocker.RequestContent);
       }

        [Fact]
        public async Task CanSetTwoExpectations()
        {
            var expectedUrl = "https://path.to.url/";
            var expectedRequestContent = "request content";
            var mocker = HttpClientMocker.Create();
            
            mocker.When.AbsoluteUrlIs(expectedUrl)
            .And.RequestMessageStringIs(expectedRequestContent)
            .Then.ResponseShouldBe("response content");

            var client = mocker.Build();

            await client.PostAsync(expectedUrl, new StringContent(expectedRequestContent));

            Assert.Equal(expectedUrl, mocker.AbsoluteRequestUri);
            Assert.Equal(expectedRequestContent, mocker.RequestContent);
        }

        [Fact]
        public async Task CanSetTwoExpectations_WhenSomeExpectationsAreNotMet_ExpectedResponseShouldNotBeReturned()
        {
            var requestUrl = "https://path.to.url/";
            var expectedRequestContent = "request content";
            var responseContent = "response content";
            var mocker = HttpClientMocker.Create();
            
            mocker.When.AbsoluteUrlIs(requestUrl)
            .And.RequestMessageStringIs(expectedRequestContent)
            .Then.ResponseShouldBe(responseContent);

            var client = mocker.Build();

            var response = await client.PostAsync("https://path.to.otherurl/", new StringContent(expectedRequestContent));

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task HttpGet_WhenExpectationFails_NoContentBodyIsReturned()
        {
            var requestUrl = "https://path.to.url";
            var responseContent = "response content";
            var mocker = HttpClientMocker.Create();
            
            mocker.When.AbsoluteUrlIs(requestUrl)
            .Then.ResponseShouldBe(responseContent);

            var client = mocker.Build();

            var response = await client.GetAsync("https://path.to.otherurl/");

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Delete_WhenNoRequestContentIsSent_RequestContentShouldBeNull()
        {
            var mocker = HttpClientMocker.Create();
            
            var client = mocker.Build();

            await client.DeleteAsync("https://path.to.url");

            Assert.Null(mocker.RequestContent);
        }

        [Fact]
        public async Task Put_WhenRequestContentIsSent_GetsResponse()
        {
            var requestContent = "request content";
            var mocker = HttpClientMocker.Create();

            var client = mocker.Build();

            await client.PutAsync("https://path.to.url", new StringContent(requestContent));

            Assert.Equal(requestContent, mocker.RequestContent);
        }

        [Fact]
        public async Task Patch_WhenRequestContentIsSent_GetsResponse()
        {
            var requestContent = "request content";
            var mocker = HttpClientMocker.Create();

            var client = mocker.Build();

            await client.PatchAsync("https://path.to.url", new StringContent(requestContent));

            Assert.Equal(requestContent, mocker.RequestContent);
        }

        [Fact]
        public async Task SetBaseUrl()
        {
            var requestContent = "request content";
            var mocker = HttpClientMocker.Create();
            mocker.SetBaseRequestUri("http://www.path.to");

            var client = mocker.Build();

            await client.PatchAsync("endpoint", new StringContent(requestContent));

            Assert.Equal("http://www.path.to/endpoint", mocker.AbsoluteRequestUri);
        }
    }
}
