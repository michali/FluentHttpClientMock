using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace HttpClientMock.Tests
{
    public class HttpClientMockBuilderTests
    {
        [Fact]
        public async Task WhenUriIsSet_ExpectUri()
        {
            var responseContent = "content";
            var requestUrl = "https://path.to.url/";
            var builder = HttpClientMockBuilder.Create();

            builder.When.AbsoluteUrlIs(requestUrl)
            .Then.ResponseShouldBe(responseContent);

            var client = builder.Build();

            var response = await client.GetAsync(requestUrl);

            Assert.Equal(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenNotExpectedRequestUriIsCalled_DoNotReturnExpectedContentasAResponse()
        {            
            var responseContent = "content";

            var builder = HttpClientMockBuilder.Create();

            builder.When.AbsoluteUrlIs("https://path.to.url/")
            .Then.ResponseShouldBe(responseContent);

            var client = builder.Build();

            var response = await client.GetAsync("https://path.to.anotherurl/");

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenSettingExpectedRequestContent_ShouldReturnExpectedResponse()
        {
            var requestContent = "request content";
            var responseContent = "response content";

            var builder = HttpClientMockBuilder.Create();

            builder.When.RequestMessageStringIs(requestContent)
            .Then.ResponseShouldBe(responseContent);

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent(requestContent));

            Assert.Equal(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenExpectedRequestContentIsNotSent_ShouldNotReturnExpectedResponse()
        {
            var responseContent = "response content";
            var builder = HttpClientMockBuilder.Create();

            builder.When.RequestMessageStringIs(responseContent)
            .Then.ResponseShouldBe("response content");

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("other request content"));

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ShouldReturnEmptyResponse()
        {
            var builder = HttpClientMockBuilder.Create();
            builder.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe(null);

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("request content"));

            var content = Assert.IsType<ByteArrayContent>(response.Content);
            Assert.Empty(await content.ReadAsByteArrayAsync());
        }

        [Fact]
        public async Task SetsResponseStatusCode()
        {
            var builder = HttpClientMockBuilder.Create();
            builder.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe(null, HttpStatusCode.Accepted);

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("request content"));

            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

       [Fact]
       public async Task RetrievesAbsoluteUri()
       {
            var expectedUrl = "https://path.to.url/";
            var builder = HttpClientMockBuilder.Create();

            var client = builder.Build();

            await client.PostAsync(expectedUrl, new ByteArrayContent(new byte[]{}));

            Assert.Equal(expectedUrl, builder.AbsoluteRequestUri);
       }

       [Fact]
       public async Task RetrievesRequestContent()
       {
            var expectedRequestContent = "request content";
            var builder = HttpClientMockBuilder.Create();

            var client = builder.Build();

            await client.PostAsync("https://path.to.url/", new StringContent(expectedRequestContent));

            Assert.Equal(expectedRequestContent, builder.RequestContent);
       }

        [Fact]
        public async Task CanSetTwoExpectations()
        {
            var expectedUrl = "https://path.to.url/";
            var expectedRequestContent = "request content";
            var builder = HttpClientMockBuilder.Create();
            
            builder.When.AbsoluteUrlIs(expectedUrl)
            .And.RequestMessageStringIs(expectedRequestContent)
            .Then.ResponseShouldBe("response content");

            var client = builder.Build();

            await client.PostAsync(expectedUrl, new StringContent(expectedRequestContent));

            Assert.Equal(expectedUrl, builder.AbsoluteRequestUri);
            Assert.Equal(expectedRequestContent, builder.RequestContent);
        }

        [Fact]
        public async Task CanSetTwoExpectations_WhenSomeExpectationsAreNotMet_ExpectedResponseShouldNotBeReturned()
        {
            var requestUrl = "https://path.to.url/";
            var expectedRequestContent = "request content";
            var responseContent = "response content";
            var builder = HttpClientMockBuilder.Create();
            
            builder.When.AbsoluteUrlIs(requestUrl)
            .And.RequestMessageStringIs(expectedRequestContent)
            .Then.ResponseShouldBe(responseContent);

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.otherurl/", new StringContent(expectedRequestContent));

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task HttpGet_WhenExpectationFails_NoContentBodyIsReturned()
        {
            var requestUrl = "https://path.to.url";
            var responseContent = "response content";
            var builder = HttpClientMockBuilder.Create();
            
            builder.When.AbsoluteUrlIs(requestUrl)
            .Then.ResponseShouldBe(responseContent);

            var client = builder.Build();

            var response = await client.GetAsync("https://path.to.otherurl/");

            Assert.NotEqual(responseContent, await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Delete_WhenNoRequestContentIsSent_RequestContentShouldBeNull()
        {
            var builder = HttpClientMockBuilder.Create();
            
            var client = builder.Build();

            await client.DeleteAsync("https://path.to.url");

            Assert.Null(builder.RequestContent);
        }

        [Fact]
        public async Task Put_WhenRequestContentIsSent_GetsResponse()
        {
            var requestContent = "request content";
            var builder = HttpClientMockBuilder.Create();

            var client = builder.Build();

            await client.PutAsync("https://path.to.url", new StringContent(requestContent));

            Assert.Equal(requestContent, builder.RequestContent);
        }

        [Fact]
        public async Task Patch_WhenRequestContentIsSent_GetsResponse()
        {
            var requestContent = "request content";
            var builder = HttpClientMockBuilder.Create();

            var client = builder.Build();

            await client.PatchAsync("https://path.to.url", new StringContent(requestContent));

            Assert.Equal(requestContent, builder.RequestContent);
        }
    }
}
