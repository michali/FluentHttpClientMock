using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HttpClientMock
{
    public class ConditionSet
    {
        private readonly HttpClientMockBuilder _httpClientWithExpectationsBuilder;
        private ICollection<Predicate<HttpRequestMessage>> _conditions;
        public ConditionSet(HttpClientMockBuilder httpClientWithExpectationsBuilder)
        {
            _httpClientWithExpectationsBuilder = httpClientWithExpectationsBuilder;
            _conditions = new HashSet<Predicate<HttpRequestMessage>>();
        }

        public ConditionSet And => this;

        public ConditionSet RequestMessageStringIs(string expectedRequestContent)
        {
            _conditions.Add(message => message.Content.ReadAsStringAsync().Result == expectedRequestContent);
            return this;
        }

        public ConditionSet AbsoluteUrlIs(string absoluteUri)
        {
            _conditions.Add(message => message.RequestUri.AbsoluteUri == absoluteUri);
            return this;
        }

        public Expectation Then => new Expectation(_httpClientWithExpectationsBuilder, _conditions);
    }
}