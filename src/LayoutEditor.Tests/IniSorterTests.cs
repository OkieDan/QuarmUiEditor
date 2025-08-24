using System.Linq;
using Xunit;
using LayoutEditor.Common;

namespace LayoutEditor.Tests;

public class IniSorterTests
{
    [Fact]
    public void TestIniSortingAndHashValidation()
    {
        string unsortedText = """            
            [Section 20]
            key1=value1


            [Section 3]
            key2=value2

            key3=value3
            [Section 10]
            keyX=valueX

            [Section  2]
            keyY=valueY            
            """;


        string[] unsortedIni = unsortedText.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);

        var originalHashes = IniSorter.ComputeSectionHashes(unsortedIni);
        string sortedContent = IniSorter.SortIniContent(unsortedIni);
        string[] sortedLines = sortedContent.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        var sortedHashes = IniSorter.ComputeSectionHashes(sortedLines);

        // Section counts must match
        Assert.Equal(originalHashes.Count, sortedHashes.Count);

        // Section content hashes must match
        foreach (var kvp in originalHashes)
        {
            Assert.True(sortedHashes.TryGetValue(kvp.Key, out var hash), $"Section {kvp.Key} missing in sorted output");
            Assert.Equal(kvp.Value, hash);
        }

        // Verify natural sort order
        var sortedKeys = sortedHashes.Keys.ToList();
        Assert.Equal("[Section 3]", sortedKeys[0]);
        Assert.Equal("[Section 10]", sortedKeys[1]);
        Assert.Equal("[Section 20]", sortedKeys[2]);
        Assert.Equal("[Section  2]", sortedKeys[3]); // double space comes last

    }
}
