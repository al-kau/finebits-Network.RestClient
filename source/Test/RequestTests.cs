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
using System.Net;
using System.Net.Mime;
using System.Text;

using NUnit.Framework;

using DataSet = Finebits.Network.RestClient.Test.Data.MessageTestData.DataSet;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class RequestTests
    {
        [Test]
        public void Construct_EmptyRequest_Success()
        {
            Assert.DoesNotThrow(() => { EmptyRequest request = new(); });
        }

        [Test]
        public void CreateContentAsync_EmptyRequest_Success()
        {
            HttpContent? content = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                EmptyRequest request = new();
                content = await request.CreateContentAsync(default).ConfigureAwait(false);
            });

            Assert.That(content, Is.Null);
        }

        [Test]
        public void Construct_StringRequest_CorrectParam_Success()
        {
            Assert.DoesNotThrow(() => { StringRequest request = new(DataSet.EmptyValue); });
        }

        [Test]
        public void Construct_StringRequest_NullParam_Exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => { StringRequest request = new(null); });

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.ParamName, Is.EqualTo("payload"));
        }

        [Test]
        public void CreateContentAsync_StringRequest_Success()
        {
            HttpContent? content = null;
            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    StringRequest request = new(DataSet.EmptyValue);
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }

            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    StringRequest request = new(DataSet.Utf8Value);
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }

            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    StringRequest request = new(DataSet.Utf8Value)
                    {
                        Encoding = Encoding.UTF8
                    };
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }

            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    StringRequest request = new(DataSet.Utf8Value)
                    {
                        Encoding = Encoding.UTF8,
                        MediaType = MediaTypeNames.Text.Plain
                    };
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }
        }

        [Test]
        public void Construct_JsonRequest_Success()
        {
            Assert.DoesNotThrow(() => { JsonRequest<Fakes.JsonData> request = new(); });
        }

        [Test]
        public void CreateContentAsync_JsonRequest_Success()
        {
            HttpContent? content = null;
            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    JsonRequest<Fakes.JsonData> request = new();
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }

            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    JsonRequest<Fakes.JsonData> request = new()
                    {
                        Payload = new Fakes.JsonData
                        {
                            Value = DataSet.Utf8Value
                        }
                    };
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }
        }

        [Test]
        public void Construct_FormUrlEncodedRequest_CorrectParam_Success()
        {
            Assert.DoesNotThrow(() =>
            {
                FormUrlEncodedRequest request = new(Enumerable.Empty<KeyValuePair<string, string>>());
            });
        }

        [Test]
        public void Construct_FormUrlEncodedRequest_NullParam_Success()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                FormUrlEncodedRequest request = new(null);
            });

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.ParamName, Is.EqualTo("payload"));
        }

        [Test]
        public void CreateContentAsync_FormUrlEncodedRequest_Success()
        {
            HttpContent? content = null;
            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    FormUrlEncodedRequest request = new(Enumerable.Empty<KeyValuePair<string, string>>());
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }

            {
                Assert.DoesNotThrowAsync(async () =>
                {
                    FormUrlEncodedRequest request = new(new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>(DataSet.UrlCodeKey, nameof(HttpStatusCode.OK)),
                        new KeyValuePair<string, string>(DataSet.UrlKey, DataSet.Utf8Value),
                        new KeyValuePair<string, string>(DataSet.UrlExtraKey, DataSet.ExtraUtf8Value),
                    });
                    content = await request.CreateContentAsync(default).ConfigureAwait(false);
                });

                Assert.That(content, Is.Not.Null);
            }
        }
    }
}
