using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HttpClientMock
{
    /// <summary>
    /// Encapsulates the conditions to be set on a mocked <c>HttpClient</c>
    /// </summary>
    public class ConditionSet
    {
        private readonly HttpClientMocker _httpClientWithExpectationsBuilder;
        private ICollection<Predicate<HttpRequestMessage>> _conditions;
        internal ConditionSet(HttpClientMocker httpClientWithExpectationsBuilder)
        {
            _httpClientWithExpectationsBuilder = httpClientWithExpectationsBuilder;
            _conditions = new HashSet<Predicate<HttpRequestMessage>>();
        }

        /// <summary>
        /// Allows to chain additional conditions to a mocked <c>HttpClient</c>
        /// </summary>
        public ConditionSet And => this;

        /// <summary>
        /// Checks for a specific content in an HTTP request
        /// </summary>
        /// <param name="expectedRequestContent">The content in the HTTP request body to set the condition on</param>
        /// <returns></returns>
        public ConditionSet RequestMessageStringIs(string expectedRequestContent)
        {
            _conditions.Add(message => message.Content.ReadAsStringAsync().Result == expectedRequestContent);
            return this;
        }

        /// <summary>
        /// Checks for a spefifi Absolute URI that is used by the mocked <c>HttpClient</c>
        /// </summary>
        /// <param name="absoluteUri">The URI to set the condition on</param>
        /// <returns></returns>
        public ConditionSet AbsoluteUrlIs(string absoluteUri)
        {
            _conditions.Add(message => message.RequestUri.AbsoluteUri == absoluteUri);
            return this;
        }

        /// <summary>
        /// Closes the series of conditions and allows to set expectations on those conditions
        /// </summary>
        /// <returns></returns>
        public Expectation Then => new Expectation(_httpClientWithExpectationsBuilder, _conditions);
    }
}