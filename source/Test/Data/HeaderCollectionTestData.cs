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

using System.Collections;

using NUnit.Framework;

namespace Finebits.Network.RestClient.Test.Data
{
    internal partial class HeaderCollectionTestData
    {
        public class TupleTestData
        {
            public IEnumerable<(string? key, string? value)> Headers { get; }

            public IEnumerable<(string key, string value)> Result { get; }

            public bool IsEmpty => !Result.Any();

            public TupleTestData(IEnumerable<(string? key, string? value)> headers, IEnumerable<(string key, string value)> result)
            {
                Headers = headers;
                Result = result;
            }
        }

        public class KeyValuePairTestData
        {
            public IEnumerable<KeyValuePair<string, IEnumerable<string?>?>> Headers { get; }

            public IEnumerable<(string key, string value)> Result { get; }

            public bool IsEmpty => !Result.Any();

            public KeyValuePairTestData(IEnumerable<KeyValuePair<string, IEnumerable<string?>?>> headers, IEnumerable<(string key, string value)> result)
            {
                Headers = headers;
                Result = result;
            }
        }

        public class AdditionalTupleTestData : TupleTestData
        {
            public IEnumerable<(string? key, string? value)> AdditionalHeaders { get; }

            public AdditionalTupleTestData(
                IEnumerable<(string? key, string? value)> headers,
                IEnumerable<(string? key, string? value)> additionalHeaders,
                IEnumerable<(string key, string value)> result)
                : base(headers, result)
            {
                AdditionalHeaders = additionalHeaders;
            }
        }

        public struct EmptyTestStructure
        { }

        public static IEnumerable HeaderTupleCases
        {
            get
            {
                yield return new TestCaseData(
                    "few",
                    new TupleTestData(headers: new (string?, string?)[] { ("header1", "value1"), ("header2", "value2"), ("header3", "value3"), ("header4", "value4") },
                                      result: new[] { ("header1", "value1"), ("header2", "value2"), ("header3", "value3"), ("header4", "value4") })
                );

                yield return new TestCaseData(
                    "same",
                    new TupleTestData(headers: new (string?, string?)[] { ("header-name", "value1"), ("header-name", "value2"), ("header-name", "value3"), ("header-name", "value4") },
                                      result: new[] { ("header-name", "value1"), ("header-name", "value2"), ("header-name", "value3"), ("header-name", "value4") })
                );

                yield return new TestCaseData(
                    "null",
                    new TupleTestData(headers: new (string?, string?)[] { ("header1", "value1"), (null, "value2"), ("header3", null), (null, null) },
                                      result: new[] { ("header1", "value1"), ("header3", string.Empty) })
                );

                yield return new TestCaseData(
                    "empty",
                    new TupleTestData(headers: new List<(string?, string?)>(),
                                      result: new List<(string, string)>())
                );
            }
        }

        public static IEnumerable HeaderKeyValuePairCases
        {
            get
            {
                yield return new TestCaseData(
                    "correct",
                    new KeyValuePairTestData(
                        headers: new Dictionary<string, IEnumerable<string?>?>
                        {
                            { "header1", new [] { "value1", "value2", "value3" } },
                            { "header2", new [] { "value1", "value2" } },
                            { "header3", new string?[] { "value1", null } },
                            { "header4", new string?[] { null } },
                            { "header5", null },
                            { "header-name", new [] { "value1" } },
                            { "wrong-name-@#$%^)(*&^%", new [] { "value1" } },
                        },
                        result: new[]
                        {
                            ( "header1", "value1" ),
                            ( "header1", "value2" ),
                            ( "header1", "value3" ),
                            ( "header2", "value1" ),
                            ( "header2", "value2" ),
                            ( "header3", "value1" ),
                            ( "header4", string.Empty ),
                            ( "header-name", "value1" ),
                        })
                );

                yield return new TestCaseData(
                    "incorrect",
                    new KeyValuePairTestData(
                        headers: new Dictionary<string, IEnumerable<string?>?>
                        {
                            { "header5", null },
                            { "wrong-name-@#$%^)(*&^%", new [] { "value1" } },
                        },
                        result: new List<(string, string)>())
                );

                yield return new TestCaseData(
                    "empty",
                    new KeyValuePairTestData(
                        headers: new Dictionary<string, IEnumerable<string?>?>(),
                        result: new List<(string, string)>())
                );
            }
        }

        public static IEnumerable AdditionalHeadersCases
        {
            get
            {
                yield return new TestCaseData(
                    "correct",
                    new AdditionalTupleTestData(
                        headers: new (string?, string?)[]
                        {
                            ( "header1", "value1" ),
                            ( "header1", "value2" ),
                            ( "header1", "value3" ),
                            ( "header2", "value1" ),
                            ( "header2", "value2" ),
                            ( "header3", "value1" ),
                            ( "header4", string.Empty ),
                        },
                        additionalHeaders: new (string?, string?)[]
                        {
                            ( "header1", "value4" ),
                            ( "header2", "value3" ),
                            ( "header2", "value4" ),
                            ( "header4", "value1" ),
                            ( "header5", "value1" ),
                            ( "wrong-name-@#$%^()*&^%", "value1" ),
                        },
                        result: new[]
                        {
                            ( "header1", "value1" ),
                            ( "header1", "value2" ),
                            ( "header1", "value3" ),
                            ( "header1", "value4" ),
                            ( "header2", "value1" ),
                            ( "header2", "value2" ),
                            ( "header2", "value3" ),
                            ( "header2", "value4" ),
                            ( "header3", "value1" ),
                            ( "header4", "value1" ),
                            ( "header5", "value1" ),
                        })
                );

                yield return new TestCaseData(
                    "incorrect",
                    new AdditionalTupleTestData(
                        headers: new (string?, string?)[]
                        {
                            ( null, null ),
                            ( null, "value1" ),
                            ( "wrong-name-@#$%^()*&^%", "value1" ),
                        },
                        additionalHeaders: new (string?, string?)[]
                        {
                            ( null, null ),
                            ( null, "value1" ),
                            ( "wrong-name-@#$%^()*&^%", "value1" ),
                        },
                        result: new List<(string, string)>())
                );

                yield return new TestCaseData(
                    "empty",
                    new AdditionalTupleTestData(
                        headers: new List<(string?, string?)>(),
                        additionalHeaders: new List<(string?, string?)>(),
                        result: new List<(string, string)>())
                );
            }
        }
    }
}
