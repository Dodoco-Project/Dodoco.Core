namespace Dodoco.Core.Test.Unit.Util.FileSystem;

using Dodoco.Core.Util.FileSystem;

using NUnit.Framework;

[TestFixture]
[TestOf(typeof(DataUnitFormatter))]
public class DataUnitFormatterTest {

    private static object[] UseNone_Cases = {
        new object[] { 0, "0" },                        // 0 B
        new object[] { 1, "1" },                        // 1 B
        new object[] { 500, "500" },                    // 500 B
        new object[] { 999, "999" },                    // 999 B
        new object[] { 1000, "1" },                     // 1 KB
        new object[] { 500000, "500" },                 // 500 KB
        new object[] { 999999, "999.99" },              // 999.99 KB
        new object[] { 1000000, "1" },                  // 1 MB
        new object[] { 500000000, "500" },              // 500 MB
        new object[] { 999999999, "999.99" },           // 999.99 MB
        new object[] { 1000000000, "1" },               // 1 GB
        new object[] { 500000000000, "500" },           // 500 GB
        new object[] { 999999999999, "999.99" },        // 999.99 GB
        new object[] { 1000000000000, "1" },            // 1 TB
        new object[] { 500000000000000, "500" },        // 500 TB
        new object[] { 999999999999999, "999.99" }      // 999.99 TB
    };

    private static object[] UseSymbol_Cases = {
        new object[] { 0, "0 B" },
        new object[] { 1, "1 B" },
        new object[] { 500, "500 B" },
        new object[] { 999, "999 B" },
        new object[] { 1000, "1 kB" },
        new object[] { 500000, "500 kB" },
        new object[] { 999999, "999.99 kB" },
        new object[] { 1000000, "1 MB" },
        new object[] { 500000000, "500 MB" },
        new object[] { 999999999, "999.99 MB" },
        new object[] { 1000000000, "1 GB" },
        new object[] { 500000000000, "500 GB" },
        new object[] { 999999999999, "999.99 GB" },
        new object[] { 1000000000000, "1 TB" },
        new object[] { 500000000000000, "500 TB" },
        new object[] { 999999999999999, "999.99 TB" }
    };

    private static object[] UseName_Cases = {
        new object[] { 0, "0 bytes" },
        new object[] { 1, "1 bytes" },
        new object[] { 500, "500 bytes" },
        new object[] { 999, "999 bytes" },
        new object[] { 1000, "1 kilobytes" },
        new object[] { 500000, "500 kilobytes" },
        new object[] { 999999, "999.99 kilobytes" },
        new object[] { 1000000, "1 megabytes" },
        new object[] { 500000000, "500 megabytes" },
        new object[] { 999999999, "999.99 megabytes" },
        new object[] { 1000000000, "1 gigabytes" },
        new object[] { 500000000000, "500 gigabytes" },
        new object[] { 999999999999, "999.99 gigabytes" },
        new object[] { 1000000000000, "1 terabytes" },
        new object[] { 500000000000000, "500 terabytes" },
        new object[] { 999999999999999, "999.99 terabytes" }
    };

    [TestCaseSource(nameof(UseNone_Cases)), Description("Should convert but not format the input")]
    public void Test_ShouldConvertButNotFormatTheInput(double input, string expected) {

        Assert.That(DataUnitFormatter.Format(input, DataUnitFormatterOption.USE_NONE), Is.EqualTo(expected));

    }

    [TestCaseSource(nameof(UseSymbol_Cases)), Description("Should convert and format the input with units' symbols")]
    public void Test_ShouldConvertAndFormatTheInputWithSymbols(double input, string expected) {

        Assert.That(DataUnitFormatter.Format(input, DataUnitFormatterOption.USE_SYMBOL), Is.EqualTo(expected));

    }

    [TestCaseSource(nameof(UseName_Cases)), Description("Should convert and format the input with units' names")]
    public void Test_ShouldConvertAndFormatTheInputWithUnitsNames(double input, string expected) {

        Assert.That(DataUnitFormatter.Format(input, DataUnitFormatterOption.USE_FULLNAME), Is.EqualTo(expected));

    }

}