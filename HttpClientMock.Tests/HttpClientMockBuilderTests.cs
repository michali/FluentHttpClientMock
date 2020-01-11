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
            var expectedUrl = "https://path.to.url/";
            var builder = HttpClientMockBuilder.Create();

            builder.When.AbsoluteUrlIs(expectedUrl)
            .Then.ResponseShouldBe("content");

            var client = builder.Build();

            var response = await client.GetAsync(expectedUrl);

            Assert.Equal("content", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenNotExpectedRequestUriIsCalled_DoNotReturnExpectedContentasAResponse()
        {
            var builder = HttpClientMockBuilder.Create();

            builder.When.AbsoluteUrlIs("https://path.to.url/")
            .Then.ResponseShouldBe("content");

            var client = builder.Build();

            var response = await client.GetAsync("https://path.to.anotherurl/");

            Assert.NotEqual("content", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenSettingExpectedRequestContent_ShouldReturnExpectedResponse()
        {
            var builder = HttpClientMockBuilder.Create();

            builder.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe("response content");

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("request content"));

            Assert.Equal("response content", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task WhenExpectedRequestContentIsNotSent_ShouldNotReturnExpectedResponse()
        {
            var builder = HttpClientMockBuilder.Create();

            builder.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe("response content");

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent("other request content"));

            Assert.NotEqual("response content", await response.Content.ReadAsStringAsync());
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
            builder.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe(null, expectedStatusCode: HttpStatusCode.OK);

            var client = builder.Build();

            var response = await client.PostAsync(expectedUrl, new StringContent("request content"));

            Assert.Equal(expectedUrl, builder.AbsoluteRequestUri);
       }

       [Fact]
       public async Task RetrievesRequestContent()
       {
            var expectedRequestContent = "request content";
            var builder = HttpClientMockBuilder.Create();
            builder.When.RequestMessageStringIs("request content")
            .Then.ResponseShouldBe(null, expectedStatusCode: HttpStatusCode.OK);

            var client = builder.Build();

            var response = await client.PostAsync("https://path.to.url/", new StringContent(expectedRequestContent));

            Assert.Equal(expectedRequestContent, builder.RequestContent);
       }
    }
}
