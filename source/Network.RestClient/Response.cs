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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Finebits.Network.RestClient
{
    public class Response
    {
        public HeaderCollection Headers { get; internal set; }
        protected internal virtual Task<bool> ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }

    public class EmptyResponse : Response
    { }

    public class StringResponse : Response
    {
        public string Content { get; protected set; }

        protected internal override async Task<bool> ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content is null)
            {
                return false;
            }

            if (!IsTextMediaType(content))
            {
                return false;
            }

            Content = await content.ReadAsStringAsync().ConfigureAwait(false);
            return true;
        }

        private static bool IsTextMediaType(HttpContent content)
        {
            var contentType = content?.Headers?.ContentType;

            return contentType != null &&
                    (
                        string.Equals(contentType.MediaType, MediaTypeNames.Text.Plain, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(contentType.MediaType, MediaTypeNames.Text.Xml, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(contentType.MediaType, MediaTypeNames.Text.Html, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(contentType.MediaType, MediaTypeNames.Text.RichText, StringComparison.OrdinalIgnoreCase)
                    );
        }
    }

    public class JsonResponse<TContent> : Response
    {
        private const string JsonMediaType = "application/json";
        public TContent Content { get; protected set; }
        public JsonSerializerOptions Options { get; set; }

        protected internal override async Task<bool> ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content is null)
            {
                return false;
            }

            if (!IsJsonMediaType(content))
            {
                return false;
            }

            Content = await content.ReadFromJsonAsync<TContent>(Options, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private static bool IsJsonMediaType(HttpContent content)
        {
            return string.Equals(content?.Headers?.ContentType?.MediaType, JsonMediaType, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class HeadResponse : Response
    {
        public HeaderCollection ContentHeaders { get; protected set; }

        protected internal override Task<bool> ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content is null)
            {
                return Task.FromResult(false);
            }

            ContentHeaders = new HeaderCollection(content.Headers, false);
            return Task.FromResult(true);
        }

        public HeaderCollection GetAllHeaders()
        {
            return new HeaderCollection(Headers.Concat(ContentHeaders), false);
        }
    }

    public class StreamResponse : Response, IDisposable
    {
        private bool _disposedValue;

        public Stream Stream { get; private set; }

        public StreamResponse()
        {
            Stream = new MemoryStream();
        }

        public StreamResponse(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            Stream = stream;
        }

        protected internal override async Task<bool> ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            if (content is null)
            {
                return false;
            }

            await content.CopyToAsync(Stream).ConfigureAwait(false);
            Stream.Position = 0;
            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stream?.Dispose();
                    Stream = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class FlexibleResponse : Response, IDisposable
    {
        private bool _disposedValue;
        private IEnumerable<Response> _responses;
        public Response PickedResponse { get; protected set; }

        public FlexibleResponse(IEnumerable<Response> responses)
        {
            if (responses is null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            _responses = responses;
        }

        protected internal override async Task<bool> ReadContentAsync(HttpContent content, CancellationToken cancellationToken)
        {
            foreach (var response in _responses)
            {
                if (await response.ReadContentAsync(content, cancellationToken).ConfigureAwait(false))
                {
                    PickedResponse = response;
                    return true;
                }
            }

            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    PickedResponse = null;
                    _responses?.ToList().ForEach(response => DisposeObject(response));
                    _responses = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static void DisposeObject(object obj)
        {
            if (obj is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
