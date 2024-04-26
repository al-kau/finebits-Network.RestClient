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
using System.Text.Json.Serialization;

namespace Finebits.Network.RestClient.Test.Fakes
{
    internal abstract class FakeMessage<TResponse, TRequest> : CommonMessage<TResponse, TRequest>
        where TResponse : Response
        where TRequest : Request
    {
        public override Uri Endpoint { get; }

        public override HttpMethod Method { get; }

        public FakeMessage(Uri endpoint, HttpMethod? method = null)
        {
            Endpoint = endpoint;
            Method = method ?? HttpMethod.Get;
        }
    }

    internal class FakeMessage<TResponse> : Message<TResponse>
        where TResponse : Response, new()
    {
        public override Uri Endpoint { get; }

        public override HttpMethod Method { get; }

        public FakeMessage(Uri endpoint, HttpMethod? method = null)
        {
            Endpoint = endpoint;
            Method = method ?? HttpMethod.Get;
        }
    }

    internal class StringMessage : FakeMessage<StringResponse>
    {
        public StringMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }
    }

    internal class JsonMessage : FakeMessage<JsonResponse<JsonData>>
    {
        public JsonMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }
    }

    internal class StreamMessage : FakeMessage<StreamResponse>
    {
        public StreamMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }
    }

    internal class HeaderMessage : FakeMessage<HeadResponse, EmptyRequest>
    {
        public HeaderCollection? Headers { get; set; }

        public HeaderMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }

        protected override EmptyRequest CreateRequest()
        {
            return new EmptyRequest()
            {
                Headers = Headers
            };
        }

        protected override HeadResponse CreateResponse()
        {
            return new HeadResponse();
        }
    }

    internal class StringPayloadMessage : FakeMessage<StringResponse, StringRequest>
    {
        public required StringRequest StringRequest { get; init; }

        public StringPayloadMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }

        protected override StringRequest CreateRequest()
        {
            return StringRequest;
        }

        protected override StringResponse CreateResponse()
        {
            return new StringResponse();
        }
    }

    internal class JsonPayloadMessage : FakeMessage<JsonResponse<JsonPayloadMessage.ResponseContent>, JsonRequest<JsonPayloadMessage.RequestPayload>>
    {
        public required RequestPayload Payload { get; init; }

        public JsonPayloadMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }

        protected override JsonRequest<RequestPayload> CreateRequest()
        {
            return new JsonRequest<RequestPayload>()
            {
                Payload = Payload,
                Options = new System.Text.Json.JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                }
            };
        }

        protected override JsonResponse<ResponseContent> CreateResponse()
        {
            return new JsonResponse<ResponseContent>()
            {
                Options = new System.Text.Json.JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                }
            };
        }

        public struct RequestPayload
        {
            [JsonInclude]
            public string Code { get; set; }

            [JsonInclude]
            public string Value { get; set; }
        }

        public struct ResponseContent
        {
            [JsonInclude]
            public string Value { get; set; }
        }
    }

    internal class FormUrlEncodedPayloadMessage : FakeMessage<StringResponse, FormUrlEncodedRequest>
    {
        public required NameValueCollection Collection { get; init; }

        public FormUrlEncodedPayloadMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }

        protected override FormUrlEncodedRequest CreateRequest()
        {
            return new FormUrlEncodedRequest(Collection.AllKeys.Select(key => new KeyValuePair<string?, string?>(key, Collection[key])));
        }

        protected override StringResponse CreateResponse()
        {
            return new StringResponse();
        }
    }

    internal class FlexibleMessage : FakeMessage<FlexibleResponse, EmptyRequest>
    {
        public FlexibleMessage(Uri endpoint, HttpMethod? method = null) : base(endpoint, method)
        { }

        protected override EmptyRequest CreateRequest()
        {
            return new EmptyRequest();
        }

        protected override FlexibleResponse CreateResponse()
        {
            return new FlexibleResponse(new Response[]
            {
                new JsonResponse<JsonData>(),
                new StringResponse(),
                new StreamResponse(),
            });
        }
    }

    public record JsonData
    {
        [JsonInclude]
        [JsonPropertyName("error")]
        public string? Error { get; init; }

        [JsonInclude]
        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; init; }

        [JsonInclude]
        [JsonPropertyName("value")]
        public string? Value { get; init; }
    }
}
