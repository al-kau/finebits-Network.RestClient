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
    internal class HeaderCollectionTests
    {
        [Test]
        public void Construct_NullParam_Exception()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<KeyValuePair<string, IEnumerable<string>>>? headers = null;
                var headerCollection = new HeaderCollection(headers, false);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<(string, string)>? headers = null;
                var headerCollection = new HeaderCollection(headers, false);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<KeyValuePair<string, string>>? headers = null;
                var headerCollection = new HeaderCollection(headers, false);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                var headerCollection = HeaderCollection.Create(null);
            });

            var headerCollection = HeaderCollection.Create(new Data.HeaderCollectionTestData.EmptyTestStructure());

            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<KeyValuePair<string, IEnumerable<string>>>? headers = null;
                headerCollection.Add(headers);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<(string, string)>? headers = null;
                headerCollection.Add(headers);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                IEnumerable<KeyValuePair<string, string>>? headers = null;
                headerCollection.Add(headers);
            });
        }


        [TestCaseSource(typeof(Data.HeaderCollectionTestData), nameof(Data.HeaderCollectionTestData.HeaderTupleCases))]
        public void Construct_TupleParam_Success(string _, Data.HeaderCollectionTestData.TupleTestData data)
        {
            var headerCollection = new HeaderCollection(data.Headers);

            using var request = new HttpRequestMessage();
            headerCollection.UpdateHeaders(request);

            Assert.That(request.Headers, data.IsEmpty ? Is.Empty : Is.Not.Empty);

            Assert.Multiple(() =>
            {
                foreach ((string header, string value) in data.Result)
                {
                    var values = request.Headers.GetValues(header);
                    Assert.That(values, Does.Contain(value));
                }
            });
        }

        [TestCaseSource(typeof(Data.HeaderCollectionTestData), nameof(Data.HeaderCollectionTestData.HeaderKeyValuePairCases))]
        public void Construct_KeyValuePairParam_Success(string _, Data.HeaderCollectionTestData.KeyValuePairTestData data)
        {
            var headerCollection = new HeaderCollection(data.Headers, false);

            using var request = new HttpRequestMessage();
            headerCollection.UpdateHeaders(request);

            Assert.That(request.Headers, data.IsEmpty ? Is.Empty : Is.Not.Empty);

            Assert.Multiple(() =>
            {
                foreach ((string header, string value) in data.Result)
                {
                    var values = request.Headers.GetValues(header);
                    Assert.That(values, Does.Contain(value));
                }
            });
        }


        [TestCaseSource(typeof(Data.HeaderCollectionTestData), nameof(Data.HeaderCollectionTestData.AdditionalHeadersCases))]
        public void Add_TupleParam_Success(string _, Data.HeaderCollectionTestData.AdditionalTupleTestData data)
        {
            var headerCollection = new HeaderCollection(data.Headers)
            {
                data.AdditionalHeaders
            };

            using var request = new HttpRequestMessage();
            headerCollection.UpdateHeaders(request);

            Assert.That(request.Headers, data.IsEmpty ? Is.Empty : Is.Not.Empty);

            Assert.Multiple(() =>
            {
                foreach ((string header, string value) in data.Result)
                {
                    var values = request.Headers.GetValues(header);
                    Assert.That(values, Does.Contain(value));
                }
            });
        }
    }
}
