using System;
using System.Net.Http;

namespace HttpClientMock
{
    public class ConditionSet
    {
        private readonly HttpClientMockBuilder _httpClientWithExpectationsBuilder;
        private Predicate<HttpRequestMessage> _condition;
        public ConditionSet(HttpClientMockBuilder httpClientWithExpectationsBuilder)
        {
            _httpClientWithExpectationsBuilder = httpClientWithExpectationsBuilder;
        }

        public ConditionSet And => this;

        public ConditionSet RequestMessageStringIs(string expectedRequestContent)
        {
            _condition = message => message.Content.ReadAsStringAsync().Result == expectedRequestContent;
            return this;
        }

        public ConditionSet AbsoluteUrlIs(string absoluteUrl)
        {
            _condition = message => message.RequestUri.AbsoluteUri == absoluteUrl;
            return this;
        }

        public Expectation Then => new Expectation(_httpClientWithExpectationsBuilder, _condition);
    }
}