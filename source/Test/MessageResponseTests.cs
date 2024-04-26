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

using Finebits.Network.RestClient.Test.Fakes;

using NUnit.Framework;

using DataSet = Finebits.Network.RestClient.Test.Data.MessageTestData.DataSet;
using UriSet = Finebits.Network.RestClient.Test.Data.MessageTestData.UriSet;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class MessageResponseTests
    {
        [Test]
        public void Send_BadResponse_StringContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.StringBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_BadResponse_EmptyStringContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.Null);
            });
        }

        [Test]
        public void Send_OkResponse_TextStringContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.StringTextOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_OkResponse_HtmlStringContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.StringHtmlOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.HtmlValue));
            });
        }

        [Test]
        public void Send_OkResponse_XmlStringContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.StringXmlOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.XmlValue));
            });
        }

        [Test]
        public void Send_OkResponse_RtfStringContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.StringRtfOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(DataSet.RtfValue));
            });
        }

        [Test]
        public void Send_BadResponse_JsonContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.JsonBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.That(exception, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content.Error, Is.EqualTo(DataSet.ErrorValue));
                Assert.That(message.Response.Content.ErrorDescription, Is.EqualTo(DataSet.ErrorDescriptionValue));
                Assert.That(message.Response.Content.Value, Is.EqualTo(default));
            });
        }

        [Test]
        public void Send_BadResponse_EmptyJsonContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.Response.Content, Is.EqualTo(default(JsonData)));
            });
        }

        [Test]
        public void Send_OkResponse_JsonContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.JsonOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content.Error, Is.EqualTo(default));
                Assert.That(message.Response.Content.ErrorDescription, Is.EqualTo(default));
                Assert.That(message.Response.Content.Value, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_OkResponse_JsonOkString_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.JsonOkStringEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content.Value, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_OkResponse_JsonBadString_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.JsonBadStringEndpoint);

            var exception = Assert.ThrowsAsync<System.Text.Json.JsonException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(default(JsonData)));
            });
        }

        [Test]
        public void Send_OkResponse_JsonBadMimeType_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using JsonMessage message = new(UriSet.JsonBadMimeTypeEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.Response.Content, Is.EqualTo(default(JsonData)));
            });
        }

        [Test]
        public void Send_BadResponse_StreamContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StreamMessage message = new(UriSet.StreamBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Stream, Is.Not.Null);
                using StreamReader reader = new(message.Response.Stream);
                Assert.That(reader.ReadToEnd(), Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_BadResponse_EmptyStreamContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StreamMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Stream, Is.Not.Null);
                Assert.That(message.Response.Stream.Length, Is.Zero);
            });
        }

        [Test]
        public void Send_OkResponse_StreamStringContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StreamMessage message = new(UriSet.StreamOkStringEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.Stream, Is.Not.Null);
                using StreamReader reader = new(message.Response.Stream);
                Assert.That(reader.ReadToEnd(), Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_OkResponse_StreamContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StreamMessage message = new(UriSet.StreamOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.Stream, Is.Not.Null);
                using StreamReader reader = new(message.Response.Stream);
                Assert.That(reader.ReadToEnd(), Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_BadResponse_FlexibleContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FlexibleMessage message = new(UriSet.FlexibleBadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.Response, Is.Not.Null);
            });

            Assert.Multiple(() =>
            {
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
            });

            var response = message.Response.PickedResponse as JsonResponse<JsonData>;
            Assert.That(response, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.Content.Error, Is.EqualTo(DataSet.ErrorValue));
                Assert.That(response.Content.ErrorDescription, Is.EqualTo(DataSet.ErrorDescriptionValue));
                Assert.That(response.Content.Value, Is.EqualTo(default));
            });
        }

        [Test]
        public void Send_BadResponse_EmptyFlexibleContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FlexibleMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.Response, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));
            });

            var response = message.Response.PickedResponse as StreamResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Stream, Is.Not.Null);
            Assert.That(response.Stream.Length, Is.Zero);
        }

        [Test]
        public void Send_OkResponse_FlexibleStringContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FlexibleMessage message = new(UriSet.FlexibleOkStringEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

            var response = message.Response.PickedResponse as StringResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Content, Is.EqualTo(DataSet.Utf8Value));
        }

        [Test]
        public void Send_OkResponse_FlexibleJsonContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FlexibleMessage message = new(UriSet.FlexibleOkJsonEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
            var response = message.Response.PickedResponse as JsonResponse<JsonData>;
            Assert.That(response, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.Content.Error, Is.EqualTo(default));
                Assert.That(response.Content.ErrorDescription, Is.EqualTo(default));
                Assert.That(response.Content.Value, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Send_OkResponse_FlexibleStreamContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FlexibleMessage message = new(UriSet.FlexibleOkStreamEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

            var response = message.Response.PickedResponse as StreamResponse;
            Assert.That(response, Is.Not.Null);

            Assert.That(response.Stream, Is.Not.Null);
            using StreamReader reader = new(response.Stream);
            Assert.That(reader.ReadToEnd(), Is.EqualTo(DataSet.Utf8Value));
        }

        [Test]
        public void Send_MethodGet_BadResponse_Headers_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.HeaderBadRequestEndpoint, HttpMethod.Get);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Content, Is.EqualTo(DataSet.Utf8Value));
                Assert.That(message.Response.Headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.Utf8Value })));
            });
        }

        [Test]
        public void Send_MethodGet_OkResponse_Headers_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.HeaderSuccessRequestEndpoint, HttpMethod.Get);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.Content, Is.EqualTo(DataSet.Utf8Value));
                Assert.That(message.Response.Headers, Does.Contain(
                    new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.Utf8Value, DataSet.ExtraUtf8Value }))
                    );
            });
        }

        [Test]
        public void Send_MethodHead_NoContent_Headers_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using StringMessage message = new(UriSet.HeaderSuccessRequestEndpoint, HttpMethod.Head);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.NoContent));

                Assert.That(message.Response.Content, Is.Null);
                Assert.That(message.Response.Headers, Does.Contain(
                    new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.Utf8Value, DataSet.ExtraUtf8Value }))
                    );
            });
        }

        [Test]
        public void Send_OkResponse_HeaderContent_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using HeaderMessage message = new(UriSet.ContentHeaderOkEndpoint);

            Assert.DoesNotThrowAsync(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));

                Assert.That(message.Response.GetAllHeaders(), Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.Utf8Value, DataSet.ExtraUtf8Value })));
                Assert.That(message.Response.GetAllHeaders(), Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.ContentHeaderKey, new[] { DataSet.Utf8Value })));
            });
        }

        [Test]
        public void Send_BadResponse_HeaderContent_Exception()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using HeaderMessage message = new(UriSet.BadRequestEndpoint);

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.BadRequest));

                Assert.That(message.Response.Headers, Is.Empty);
                Assert.That(message.Response.ContentHeaders, Does.Contain(new KeyValuePair<string, IEnumerable<string>>("Content-Length", new[] { "0" })));
            });
        }
    }
}
