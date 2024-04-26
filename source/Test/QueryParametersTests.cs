// ---------------------------------------------------------------------------- //
//                                                                              //
//   Copyright 2024 Finebits (https://finebits.com/)                            //
//                                                                              //
//   Licensed under the Apache License, Version 2.0 (the "License"),            //
//   you may not use this file except in compliance with the License.           //
//   You may obtain a copy of the License at                                    //
//                                                                              //
//       http://www.apache.org/licenses/LICENSE-2.0                             //
//                                                                              //
//   Unless required by applicable law or agreed to in writing, software        //
//   distributed under the License is distributed on an "AS IS" BASIS,          //
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.   //
//   See the License for the specific language governing permissions and        //
//   limitations under the License.                                             //
//                                                                              //
// ---------------------------------------------------------------------------- //

using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class QueryParametersTests
    {
        [Test]
        [TestCaseSource(typeof(Data.QueryParametersTestData), nameof(Data.QueryParametersTestData.QueryStringCases))]
        public void Construct_StringParam_Success(string query, string result)
        {
            var queryParams = new QueryParameters(query);
            Assert.That(queryParams.ToString(), Is.EqualTo(result));
        }

        [Test]
        [TestCaseSource(typeof(Data.QueryParametersTestData), nameof(Data.QueryParametersTestData.QueryTupleCases))]
        [TestCaseSource(typeof(Data.QueryParametersTestData), nameof(Data.QueryParametersTestData.QueryTupleUnicodeCases))]
        public void Construct_TupleParam_Success(Data.QueryParametersTestData.TupleTestData data, string result)
        {
            var queryParams = new QueryParameters(data.TupleQuery);
            Assert.That(queryParams.ToString(), Is.EqualTo(result));
        }

        [Test]
        [TestCaseSource(typeof(Data.QueryParametersTestData), nameof(Data.QueryParametersTestData.QueryPairCases))]
        public void Construct_KeyValuePairParam_Success(IEnumerable<KeyValuePair<string, string?>> query, string result)
        {
            var queryParams = new QueryParameters(query);
            Assert.That(queryParams.ToString(), Is.EqualTo(result));
        }

        [Test]
        [TestCaseSource(typeof(Data.QueryParametersTestData), nameof(Data.QueryParametersTestData.QueryStructCases))]
        public void Create_StructParam_Success(object query, string result)
        {
            var queryParams = QueryParameters.Create(query);
            Assert.That(queryParams.ToString(), Is.EqualTo(result));
        }

        [Test]
        public void Construct_NullParam_Exception()
        {
            {
                string? query = null;
                Assert.Throws<ArgumentNullException>(() => new QueryParameters(query));
            }

            {
                IEnumerable<KeyValuePair<string, string>>? query = null;
                Assert.Throws<ArgumentNullException>(() => new QueryParameters(query));
            }

            {
                IEnumerable<(string key, string value)>? query = null;
                Assert.Throws<ArgumentNullException>(() => new QueryParameters(query));
            }

            Assert.Throws<ArgumentNullException>(() => QueryParameters.Create(null));
        }
    }
}
