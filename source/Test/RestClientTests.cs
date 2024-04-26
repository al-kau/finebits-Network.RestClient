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

using UriSet = Finebits.Network.RestClient.Test.Data.MessageTestData.UriSet;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class RestClientTests
    {
        [Test]
        public void Construct_NullParam_Exception()
        {
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new FakeRestClient(null, null));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("httpClient"));
            }

            {
                var exception = Assert.Throws<ArgumentNullException>(() => new FakeRestClient(null, new Uri("https://any")));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("httpClient"));
            }
        }

        [Test]
        public void Send_NullParam_Exception()
        {
            using HttpClient httpClient = new();
            FakeRestClient client = new(httpClient, UriSet.Host);

            {
                var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SendMessageAsync(null).ConfigureAwait(false));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("message"));
            }

            {
                var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await client.SendMessageAsync(null, new CancellationTokenSource().Token).ConfigureAwait(false));

                Assert.That(exception, Is.Not.Null);
                Assert.That(exception.ParamName, Is.EqualTo("message"));
            }
        }

        [Test]
        [TestCaseSource(typeof(Test.Data.HttpStatusCodeTestData), nameof(Test.Data.HttpStatusCodeTestData.ErrorHttpStatusCodeCases))]
        public void Send_UnsuccessfulStatusCode_Exception(HttpStatusCode code)
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FakeMessage<EmptyResponse> message = new(UriSet.GetHttpStatusCodeEndpoint(code));

            var exception = Assert.ThrowsAsync<HttpRequestException>(async () => await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(exception, Is.Not.Null);
                Assert.That(message.HttpStatus, Is.EqualTo(code));
            });
        }

        [Test]
        [TestCaseSource(typeof(Test.Data.HttpStatusCodeTestData), nameof(Test.Data.HttpStatusCodeTestData.SuccessHttpStatusCodeCases))]
        public void Send_SuccessStatusCode_Success(HttpStatusCode code)
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FakeMessage<EmptyResponse> message = new(UriSet.GetHttpStatusCodeEndpoint(code));
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

            Assert.DoesNotThrowAsync(async () => httpStatusCode = await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(httpStatusCode, Is.EqualTo(code));
                Assert.That(message.HttpStatus, Is.EqualTo(code));
            });
        }

        [Test]
        public void Send_RequestCancellationToken_Exception()
        {
            using CancellationTokenSource cts = new();
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FakeMessage<EmptyResponse> message = new(UriSet.OkEndpoint);

            var exception = Assert.CatchAsync<OperationCanceledException>(async () => await client.SendMessageAsync(message, cts.Token).ConfigureAwait(false));
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void Send_CancellationToken_Exception()
        {
            using CancellationTokenSource cts = new();
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.CreateCancellationToken(cts).Object);
            FakeRestClient client = new(httpClient, UriSet.Host);

            using FakeMessage<EmptyResponse> message = new(UriSet.OkEndpoint);

            cts.Cancel();

            var exception = Assert.CatchAsync<OperationCanceledException>(async () => await client.SendMessageAsync(message, cts.Token).ConfigureAwait(false));
            Assert.That(exception, Is.Not.Null);
        }

        [Test]
        public void Send_NullBaseUri_Success()
        {
            using HttpClient httpClient = new(Mocks.HttpMessageHandlerCreator.Create().Object);
            FakeRestClient client = new(httpClient, null);

            using FakeMessage<EmptyResponse> message = new(new Uri(UriSet.Host, UriSet.OkEndpoint));

            var httpStatusCode = HttpStatusCode.BadRequest;
            Assert.DoesNotThrowAsync(async () => httpStatusCode = await client.SendMessageAsync(message).ConfigureAwait(false));

            Assert.Multiple(() =>
            {
                Assert.That(httpStatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(message.HttpStatus, Is.EqualTo(HttpStatusCode.OK));
            });
        }
    }
}
