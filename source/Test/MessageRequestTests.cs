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

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web;

using Finebits.Network.RestClient.Test.Fakes;

using NUnit.Framework;

using DataSet = Finebits.Network.RestClient.Test.Data.MessageTestData.DataSet;
using UriSet = Finebits.Network.RestClient.Test.Data.MessageTestData.UriSet;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class MessageRequestTests
    {
        [Test]
        public void Send_StringPayload_BadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringPayloadMessage message = new(UriSet.StringPayloadEndpoint)
            {
                StringRequest = new StringRequest(nameof(HttpStatusCode.BadRequest))
                {
                    MediaType = MediaTypeNames.Text.Plain
                }
            };

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(nameof(HttpStatusCode.BadRequest)));
            });
        }

        [Test]
        public void Send_CustomStringPayload_BadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringPayloadMessage message = new(UriSet.StringPayloadEndpoint)
            {
                StringRequest = new StringRequest(DataSet.EmptyValue)
                {
                    Encoding = Encoding.UTF8,
                    MediaType = MediaTypeNames.Text.Plain
                }
            };

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.EmptyValue));
            });
        }

        [Test]
        public void Send_StringPayload_OkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringPayloadMessage message = new(UriSet.StringPayloadEndpoint)
            {
                StringRequest = new StringRequest(nameof(HttpStatusCode.OK))
                {
                    Encoding = Encoding.UTF8,
                }
            };

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(nameof(HttpStatusCode.OK)));
            });
        }

        [Test]
        public void Send_JsonPayload_BadResponse_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonPayloadMessage message = new(UriSet.JsonPayloadEndpoint)
            {
                Payload = new JsonPayloadMessage.RequestPayload
                {
                    Code = nameof(HttpStatusCode.BadRequest),
                    Value = DataSet.EmptyValue
                }
            };

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content.Value, Is.EqualTo(DataSet.EmptyValue));
            });
        }

        [Test]
        public void Send_JsonContent_OkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonPayloadMessage message = new(UriSet.JsonPayloadEndpoint)
            {
                Payload = new JsonPayloadMessage.RequestPayload
                {
                    Code = nameof(HttpStatusCode.OK),
                    Value = DataSet.Utf8Value
                }
            };

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content.Value, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_CustomHeader_OkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using HeaderMessage message = new(UriSet.CustomHeaderEndpoint)
            {
                Headers = new HeaderCollection(new (string, string)[]
                {
                    (DataSet.HeaderKey, DataSet.Utf8Value),
                    (DataSet.HeaderKey, DataSet.ExtraUtf8Value),
                })
            };

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.Utf8Value, DataSet.ExtraUtf8Value })));
            });
        }

        [Test]
        public void Send_FormUrlEncodedPayload_OkResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            var collection = new NameValueCollection
            {
                { DataSet.UrlCodeKey , nameof(HttpStatusCode.OK) },
                { DataSet.UrlKey , DataSet.Utf8Value },
                { DataSet.UrlExtraKey , DataSet.ExtraUtf8Value },
            };

            using FormUrlEncodedPayloadMessage message = new(UriSet.FormUrlEncodedPayloadEndpoint, HttpMethod.Post)
            {
                Collection = collection
            };

            var query = string.Join("&", collection.AllKeys.Select(a => HttpUtility.UrlEncode(a) + "=" + HttpUtility.UrlEncode(collection[a])));

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(string.Equals(message.Response.Content, query, StringComparison.OrdinalIgnoreCase), Is.True);
            });
        }

        [Test]
        public void Send_FormUrlEncodedPayload_BadResponse_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FormUrlEncodedPayloadMessage message = new(UriSet.FormUrlEncodedPayloadEndpoint, HttpMethod.Post)
            {
                Collection = new()
            };

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.Empty);
            });
        }
    }
}
