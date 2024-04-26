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

using Finebits.Network.RestClient.Utils;

using NUnit.Framework;

namespace Finebits.Network.RestClient.Test.Data
{
    internal partial class QueryParametersTestData
    {
        public struct StructTestData
        {
            public string StringParam { get; set; }
            public int IntParam { get; set; }
            public double DoubleParam { get; set; }
            public bool? BoolParam { get; set; }
        }

        public struct AttributeTestData
        {
            [ParameterName("WellNamedParam")]
            public int? WrongNamedParam { get; set; }

            [ParameterConverter(typeof(AttributeTestData), nameof(Convert))]
            public int? ConvertParam { get; set; }

            public static string? Convert(object? obj)
            {
                return obj is null ? null : $"Value-is({obj})";
            }
        }

        public struct TupleTestData
        {
            public IEnumerable<(string? key, string? value)> TupleQuery { get; }

            public TupleTestData(IEnumerable<(string? key, string? value)> query)
            {
                TupleQuery = query;
            }
        }

        public static IEnumerable QueryStringCases
        {
            get
            {
                yield return new TestCaseData("", "");
                yield return new TestCaseData("param1=value1&param2=value2", "param1=value1&param2=value2");
                yield return new TestCaseData("param2=value2&param1=value1", "param2=value2&param1=value1");
                yield return new TestCaseData("param1=&param2=", "param1=&param2=");
                yield return new TestCaseData("param1=foo bar", "param1=foo+bar");
                yield return new TestCaseData("param1=!_.0-9A-Za-z(*)", "param1=!_.0-9A-Za-z(*)");
                yield return new TestCaseData("""param1=@#$%^=":;<>,?/\|""", "param1=%40%23%24%25%5e%3d%22%3a%3b%3c%3e%2c%3f%2f%5c%7c");
                yield return new TestCaseData("param1=%2b&param2=%26&param3=%&param4= &param5=%20", "param1=%2b&param2=%26&param3=%25&param4=+&param5=+");
                yield return new TestCaseData("param1=%40%23%24%25%5e%3d%22%3a%3b%3c%3e%2c%3f%2f%5c%7c", "param1=%40%23%24%25%5e%3d%22%3a%3b%3c%3e%2c%3f%2f%5c%7c");
            }
        }

        public static IEnumerable QueryTupleCases
        {
            get
            {
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { ("param1", "value1"), ("param2", "value2") }), "param1=value1&param2=value2");
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { ("param2", "value2"), ("param1", "value1") }), "param2=value2&param1=value1");
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { ("param1", null), ("param2", string.Empty), ("param3", " ") }), "param2=&param3=+");
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { ("param", "foo bar") }), "param=foo+bar");
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { ("param", "!_.0-9A-Za-z(*)") }), "param=!_.0-9A-Za-z(*)");
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { ("param", """'@#$%^=":;<>,?/\|+&""") }), "param=%27%40%23%24%25%5e%3d%22%3a%3b%3c%3e%2c%3f%2f%5c%7c%2b%26");
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { ("param", null) }), string.Empty);
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { (null, null) }), string.Empty);
                yield return new TestCaseData(new TupleTestData(new (string?, string?)[] { (null, "value"), (" ", "value"), (string.Empty, "value") }), string.Empty);
            }
        }

        public static IEnumerable QueryPairCases
        {
            get
            {
                yield return new TestCaseData(new Dictionary<string, string> { { "param1", "value1" }, { "param2", "value2" } }, "param1=value1&param2=value2");
                yield return new TestCaseData(new Dictionary<string, string> { { "param2", "value2" }, { "param1", "value1" } }, "param2=value2&param1=value1");
                yield return new TestCaseData(new Dictionary<string, string?> { { "param", "value" }, { "param2", null } }, "param=value");
                yield return new TestCaseData(new Dictionary<string, string> { { "param", "foo bar" } }, "param=foo+bar");
                yield return new TestCaseData(new Dictionary<string, string> { { "param", "!_.0-9A-Za-z(*)" } }, "param=!_.0-9A-Za-z(*)");
                yield return new TestCaseData(new Dictionary<string, string> { { "param", """'@#$%^=":;<>,?/\|+&""" } }, "param=%27%40%23%24%25%5e%3d%22%3a%3b%3c%3e%2c%3f%2f%5c%7c%2b%26");
            }
        }

        public static IEnumerable QueryStructCases
        {
            get
            {
                yield return new TestCaseData(new StructTestData
                {
                    StringParam = "!_.0-9A-Za-z(*)",
                    IntParam = int.MinValue,
                    DoubleParam = double.E,
                }, $"{nameof(StructTestData.StringParam)}=!_.0-9A-Za-z(*)&{nameof(StructTestData.IntParam)}={int.MinValue}&{nameof(StructTestData.DoubleParam)}={double.E}");

                yield return new TestCaseData(new StructTestData
                {
                    StringParam = """' @#$%^=":;<>,?/\|+&""",
                    IntParam = int.MaxValue,
                    DoubleParam = double.Pi,
                    BoolParam = false,
                }, $"{nameof(StructTestData.StringParam)}=%27+%40%23%24%25%5e%3d%22%3a%3b%3c%3e%2c%3f%2f%5c%7c%2b%26&{nameof(StructTestData.IntParam)}={int.MaxValue}&{nameof(StructTestData.DoubleParam)}={double.Pi}&{nameof(StructTestData.BoolParam)}={bool.FalseString}");

                yield return new TestCaseData(new AttributeTestData
                {
                    WrongNamedParam = 0,
                    ConvertParam = int.MaxValue,
                }, $"WellNamedParam=0&{nameof(AttributeTestData.ConvertParam)}={AttributeTestData.Convert(int.MaxValue)}");

                yield return new TestCaseData(new
                {
                    ZeroValue = 0,
                    MaxValue = int.MaxValue,
                    MinValue = int.MinValue,
                    EmptyValue = string.Empty,
                    StringValue = "string",
                }, $"ZeroValue=0&MaxValue={int.MaxValue}&MinValue={int.MinValue}&EmptyValue={string.Empty}&StringValue=string");
            }
        }
    }
}
