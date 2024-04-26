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
using System.Net.Http.Json;

using NUnit.Framework;

using DataSet = Finebits.Network.RestClient.Test.Data.MessageTestData.DataSet;

namespace Finebits.Network.RestClient.Test
{
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Class is instantiated via NUnit Framework")]
    internal class ResponseTests
    {
        [Test]
        public void Construct_StringResponse_Success()
        {
            Assert.DoesNotThrow(() => { StringResponse response = new(); });
        }

        [Test]
        public void ReadContentAsync_StringResponse_NullContent_Success()
        {
            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                StringResponse response = new();
                result = await response.ReadContentAsync(null, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReadContentAsync_StringResponse_CorrectContent_Success()
        {
            using StringContent content = new(DataSet.Utf8Value);

            bool? result = null;
            string? text = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                StringResponse response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);
                text = response.Content;
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(text, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void ReadContentAsync_StringResponse_EmptyContent_Success()
        {
            using StringContent content = new(DataSet.EmptyValue);

            bool? result = null;
            string? text = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                StringResponse response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);
                text = response.Content;
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(text, Is.EqualTo(DataSet.EmptyValue));
            });
        }

        [Test]
        public void ReadContentAsync_StringResponse_WrongContent_Success()
        {
            using StreamContent content = new(new MemoryStream());

            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                StringResponse response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void Construct_StreamResponse_Success()
        {
            Assert.DoesNotThrow(() => { using StreamResponse streamResponse = new(); });
            Assert.DoesNotThrow(() => { using StreamResponse streamResponse = new(new MemoryStream()); });
        }

        [Test]
        public void Construct_StreamResponse_NullParam_Exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => { using StreamResponse streamResponse = new(null); });

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.ParamName, Is.EqualTo("stream"));
        }

        [Test]
        public void Dispose_StreamResponse_Success()
        {
            Assert.DoesNotThrow(() =>
            {
                using StreamResponse streamResponse = new();
                streamResponse.Stream.Dispose();
            });

            Assert.DoesNotThrow(() =>
            {
                using StreamResponse streamResponse = new();
                streamResponse.Dispose();
            });
        }

        [Test]
        public void ReadContentAsync_StreamResponse_NullContent_Success()
        {
            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                using StreamResponse streamResponse = new();
                result = await streamResponse.ReadContentAsync(null, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReadContentAsync_StreamResponse_CorrectContent_Success()
        {
            using StreamContent content = new(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(DataSet.Utf8Value)));

            bool? result = null;
            string? text = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                using StreamResponse streamResponse = new();
                result = await streamResponse.ReadContentAsync(content, default).ConfigureAwait(false);

                using StreamReader reader = new(streamResponse.Stream);
                text = reader.ReadToEnd();
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(text, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void ReadContentAsync_StreamResponse_EmptyContent_Success()
        {
            using StreamContent content = new(new MemoryStream());

            bool? result = null;
            string? text = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                using StreamResponse streamResponse = new();
                result = await streamResponse.ReadContentAsync(content, default).ConfigureAwait(false);

                using StreamReader reader = new(streamResponse.Stream);
                text = reader.ReadToEnd();
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(text, Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void ReadContentAsync_StreamResponse_SomeContent_Success()
        {
            using StringContent content = new(DataSet.Utf8Value);

            bool? result = null;
            string? text = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                using StreamResponse streamResponse = new();
                result = await streamResponse.ReadContentAsync(content, default).ConfigureAwait(false);

                using StreamReader reader = new(streamResponse.Stream);
                text = reader.ReadToEnd();
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(text, Is.EqualTo(DataSet.Utf8Value));
            });
        }

        [Test]
        public void Construct_JsonResponse_Success()
        {
            Assert.DoesNotThrow(() => { JsonResponse<Fakes.JsonData> response = new(); });
            Assert.DoesNotThrow(() =>
            {
                JsonResponse<Fakes.JsonData> response = new()
                {
                    Options = new System.Text.Json.JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                    }
                };
            });
        }

        [Test]
        public void ReadContentAsync_JsonResponse_NullContent_Success()
        {
            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                JsonResponse<Fakes.JsonData> response = new();
                result = await response.ReadContentAsync(null, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReadContentAsync_JsonResponse_CorrectContent_Success()
        {
            using HttpContent content = JsonContent.Create(new
            {
                error = DataSet.ErrorValue,
                error_description = DataSet.ErrorDescriptionValue,
                value = DataSet.Utf8Value
            });

            bool? result = null;
            Fakes.JsonData? json = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                JsonResponse<Fakes.JsonData> response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);

                json = response.Content;
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(json, Is.Not.Null);
            });

            Assert.Multiple(() =>
            {
                Assert.That(json.Value, Is.EqualTo(DataSet.Utf8Value));
                Assert.That(json.Error, Is.EqualTo(DataSet.ErrorValue));
                Assert.That(json.ErrorDescription, Is.EqualTo(DataSet.ErrorDescriptionValue));
            });
        }

        [Test]
        public void ReadContentAsync_JsonResponse_EmptyContent_Success()
        {
            using StreamContent content = new(new MemoryStream());

            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                JsonResponse<Fakes.JsonData> response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReadContentAsync_JsonResponse_WrongContent_Success()
        {
            using StringContent content = new(DataSet.Utf8Value);

            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                JsonResponse<Fakes.JsonData> response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void Construct_HeadResponse_Success()
        {
            Assert.DoesNotThrow(() => { HeadResponse response = new(); });
        }

        [Test]
        public void ReadContentAsync_HeadResponse_NullContent_Success()
        {
            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                HeadResponse response = new();
                result = await response.ReadContentAsync(null, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReadContentAsync_HeadResponse_CorrectContent_Success()
        {
            using StringContent content = new(DataSet.EmptyValue);

            content.Headers.Add(DataSet.HeaderKey, DataSet.Utf8Value);
            content.Headers.Add(DataSet.HeaderKey, DataSet.ExtraUtf8Value);

            bool? result = null;
            HeaderCollection? headers = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                HeadResponse response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);

                headers = response.ContentHeaders;
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(headers, Is.Not.Null);
            });

            Assert.That(headers, Does.Contain(new KeyValuePair<string, IEnumerable<string>>(DataSet.HeaderKey, new[] { DataSet.Utf8Value, DataSet.ExtraUtf8Value })));
        }

        [Test]
        public void ReadContentAsync_HeadResponse_EmptyContent_Success()
        {
            using StreamContent content = new(new MemoryStream());

            bool? result = null;
            HeaderCollection? headers = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                HeadResponse response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);

                headers = response.ContentHeaders;
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(headers, Is.Not.Null);
            });

            Assert.That(headers, Is.Empty);
        }

        [Test]
        public void ReadContentAsync_HeadResponse_SomeContent_Success()
        {
            using StringContent content = new(DataSet.Utf8Value);

            bool? result = null;
            HeaderCollection? headers = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                HeadResponse response = new();
                result = await response.ReadContentAsync(content, default).ConfigureAwait(false);

                headers = response.ContentHeaders;
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(headers, Is.Not.Null);
            });

            Assert.That(headers, Is.Not.Empty);
        }

        [Test]
        public void Construct_FlexibleResponse_Success()
        {
            Assert.DoesNotThrow(() => { using FlexibleResponse flexibleResponse = new(Enumerable.Empty<Response>()); });
        }

        [Test]
        public void Construct_FlexibleResponse_NullParam_Exception()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => { using FlexibleResponse flexibleResponse = new(null); });

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.ParamName, Is.EqualTo("responses"));
        }

        [Test]
        public void Dispose_FlexibleResponse_Success()
        {
            Assert.DoesNotThrow(() =>
            {
                using FlexibleResponse flexibleResponse = new(Enumerable.Empty<Response>());
                flexibleResponse.Dispose();
            });
        }

        [Test]
        public void ReadContentAsync_FlexibleResponse_NullParam_Success()
        {
            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                using FlexibleResponse streamResponse = new(Enumerable.Empty<Response>());
                result = await streamResponse.ReadContentAsync(null, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReadContentAsync_FlexibleResponse_WrongParam_Success()
        {
            using StreamContent content = new(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(DataSet.Utf8Value)));

            bool? result = null;
            Assert.DoesNotThrowAsync(async () =>
            {
                using FlexibleResponse streamResponse = new(new Response[]
                {
                    new JsonResponse<Fakes.JsonData>(),
                    new StringResponse()
                });
                result = await streamResponse.ReadContentAsync(content, default).ConfigureAwait(false);
            });

            Assert.That(result, Is.False);
        }

        [Test]
        public void ReadContentAsync_FlexibleResponse_CorrectParam_Success()
        {
            using StringContent content = new(DataSet.Utf8Value);

            bool? result = null;
            string? text = null;

            Assert.DoesNotThrowAsync(async () =>
            {
                using FlexibleResponse streamResponse = new(new Response[]
                {
                    new JsonResponse<Fakes.JsonData>(),
                    new StringResponse()
                });
                result = await streamResponse.ReadContentAsync(content, default).ConfigureAwait(false);

                if (streamResponse.PickedResponse is StringResponse stringResponse)
                {
                    text = stringResponse.Content;
                }
            });

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(text, Is.EqualTo(DataSet.Utf8Value));
            });
        }
    }
}
