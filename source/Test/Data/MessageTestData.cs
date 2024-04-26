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

using System.Net;

namespace Finebits.Network.RestClient.Test.Data
{
    internal class MessageTestData
    {
        internal static class UriSet
        {
            public static readonly Uri Host = new("https://test-host/");

            public static readonly Uri OkEndpoint = new("/ok", UriKind.Relative);
            public static readonly Uri BadRequestEndpoint = new("/bad-request", UriKind.Relative);

            public static readonly Uri HttpStatusCodeEndpoint = new("/http-status-code", UriKind.Relative);

            public static readonly Uri StringPayloadEndpoint = new("/string/payload", UriKind.Relative);
            public static readonly Uri StringTextOkEndpoint = new("/string/ok-text", UriKind.Relative);
            public static readonly Uri StringHtmlOkEndpoint = new("/string/ok-html", UriKind.Relative);
            public static readonly Uri StringXmlOkEndpoint = new("/string/ok-xml", UriKind.Relative);
            public static readonly Uri StringRtfOkEndpoint = new("/string/ok-rtf", UriKind.Relative);
            public static readonly Uri StringBadRequestEndpoint = new("/string/bad-request", UriKind.Relative);

            public static readonly Uri FormUrlEncodedPayloadEndpoint = new("/from-url-encoded/payload", UriKind.Relative);

            public static readonly Uri JsonPayloadEndpoint = new("/json/payload", UriKind.Relative);
            public static readonly Uri JsonOkEndpoint = new("/json/ok", UriKind.Relative);
            public static readonly Uri JsonBadRequestEndpoint = new("/json/bad-request", UriKind.Relative);

            public static readonly Uri JsonOkStringEndpoint = new("/json/ok-string", UriKind.Relative);
            public static readonly Uri JsonBadStringEndpoint = new("/json/bad-string", UriKind.Relative);
            public static readonly Uri JsonBadMimeTypeEndpoint = new("/json/bad-mime-type", UriKind.Relative);

            public static readonly Uri StreamOkEndpoint = new("/stream/ok", UriKind.Relative);
            public static readonly Uri StreamBadRequestEndpoint = new("/stream/bad-request", UriKind.Relative);
            public static readonly Uri StreamOkStringEndpoint = new("/stream/ok-string", UriKind.Relative);

            public static readonly Uri ContentHeaderOkEndpoint = new("/content-header/ok", UriKind.Relative);
            public static readonly Uri HeaderSuccessRequestEndpoint = new("/header/success", UriKind.Relative);
            public static readonly Uri HeaderBadRequestEndpoint = new("/header/bad-request", UriKind.Relative);
            public static readonly Uri CustomHeaderEndpoint = new("/header/custom", UriKind.Relative);

            public static readonly Uri FlexibleBadRequestEndpoint = new("/flexible/bad-request", UriKind.Relative);
            public static readonly Uri FlexibleOkStringEndpoint = new("/flexible/ok-string", UriKind.Relative);
            public static readonly Uri FlexibleOkJsonEndpoint = new("/flexible/ok-json", UriKind.Relative);
            public static readonly Uri FlexibleOkStreamEndpoint = new("/flexible/ok-stream", UriKind.Relative);

            public const string HttpStatusCodeQueryParam = "code";

            public static Uri GetHttpStatusCodeEndpoint(HttpStatusCode code)
            {
                return new($"{HttpStatusCodeEndpoint}?{HttpStatusCodeQueryParam}={code}", UriKind.Relative);
            }
        }

        internal static class DataSet
        {
            public const string HeaderKey = "header-key";
            public const string ContentHeaderKey = "content-header-key";

            public const string UrlCodeKey = "code";
            public const string UrlKey = "url-key";
            public const string UrlExtraKey = "url-extra-key";

            public const string Utf8Value = "šomē-ütf8-valúe-你好世界";
            public const string ExtraUtf8Value = "šomē-ēxtra-ütf8-valúē-你好世界";
            public const string ErrorValue = "ērror-ùtf8-valūē-你好世界";
            public const string ErrorDescriptionValue = "ērror-dēscrīptїon-ûtf8-valūē-你好世界";
            public const string EmptyValue = "";

            public const string HtmlValue = """
                <html>
                <h1>šomē-ütf8-valúe-你好世界</h1>
                </html>
                """;

            public const string XmlValue = """
                <?xml version="1.0" encoding="UTF-8"?>
                <Root>
                    <Value>šomē-ütf8-valúe-你好世界</Value>
                </Root>
                """;

            public const string RtfValue = """{\rtf1\uc1 {foo bar}}""";
        }
    }
}
