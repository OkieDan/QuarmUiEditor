using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LayoutEditor.Common;

public static class IniSorter
{
    /// <summary>
    /// Loads an ini file, sorts its sections naturally, and returns sorted content.
    /// </summary>
    public static string SortIniFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("INI file not found", filePath);

        string[] lines = File.ReadAllLines(filePath);

        // Compute original hashes
        var originalHashes = ComputeSectionHashes(lines);

        // Sort the content
        string sortedContent = SortIniContent(lines);

        // Verify hashes
        VerifySectionHashes(lines, sortedContent);

        return sortedContent;
    }

    private static void VerifySectionHashes(string[] originalLines, string sortedContent)
    {
        var sortedLines = sortedContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var originalHashes = ComputeSectionHashes(originalLines);
        var sortedHashes = ComputeSectionHashes(sortedLines);

        foreach (var kvp in originalHashes)
        {
            if (!sortedHashes.TryGetValue(kvp.Key, out var sortedHash))
                throw new InvalidOperationException($"Section '{kvp.Key}' missing in sorted output.");

            if (kvp.Value != sortedHash)
                throw new InvalidOperationException($"Hash mismatch in section '{kvp.Key}'.");
        }
    }
    /// <summary>
    /// Sorts ini content provided as lines.
    /// </summary>
    public static string SortIniContent(string[] lines)
    {
        var sections = new Dictionary<string, List<string>>();
        string? currentSection = null;

        foreach (var line in lines)
        {
            if (Regex.IsMatch(line.Trim(), @"^\[.*\]$"))
            {
                currentSection = line.Trim();
                if (!sections.ContainsKey(currentSection))
                    sections[currentSection] = new List<string>();
            }
            else if (currentSection != null)
            {
                sections[currentSection].Add(line);
            }
        }

        var sortedSections = sections.Keys
            .OrderBy(s => s.Trim('[', ']'), new NaturalStringComparer())
            .ToList();

        var sb = new StringBuilder();
        for (int i = 0; i < sortedSections.Count; i++)
        {
            var section = sortedSections[i];
            sb.AppendLine(section);

            var contentLines = sections[section];
            for (int j = 0; j < contentLines.Count; j++)
            {
                sb.Append(contentLines[j]);
                // Add newline unless it's the last line of the last section
                if (j < contentLines.Count - 1 || i < sortedSections.Count - 1)
                    sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Computes a SHA256 hash for each section (excluding the header).
    /// </summary>
    public static Dictionary<string, string> ComputeSectionHashes(string[] lines)
    {
        var sections = new Dictionary<string, List<string>>();
        string? currentSection = null;

        foreach (var line in lines)
        {
            if (Regex.IsMatch(line.Trim(), @"^\[.*\]$"))
            {
                currentSection = line.Trim();
                if (!sections.ContainsKey(currentSection))
                    sections[currentSection] = new List<string>();
            }
            else if (currentSection != null)
            {
                sections[currentSection].Add(line);
            }
        }

        return sections.ToDictionary(
            kvp => kvp.Key,
            kvp => ComputeHash(string.Join("\n", kvp.Value))
        );
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

/// <summary>
/// Natural string comparer: numbers compared numerically, everything else literally (spaces included).
/// </summary>
public class NaturalStringComparer : IComparer<string>
{
    private static readonly Regex _regex = new Regex(@"\d+|\D+", RegexOptions.Compiled);

    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var xParts = _regex.Matches(x);
        var yParts = _regex.Matches(y);
        int i = 0;

        while (i < xParts.Count && i < yParts.Count)
        {
            string xPart = xParts[i].Value;
            string yPart = yParts[i].Value;

            int result;

            bool xIsNumber = int.TryParse(xPart, out int xNum);
            bool yIsNumber = int.TryParse(yPart, out int yNum);

            if (xIsNumber && yIsNumber)
            {
                result = xNum.CompareTo(yNum);
            }
            else
            {
                result = string.Compare(xPart, yPart, StringComparison.OrdinalIgnoreCase);
            }

            if (result != 0)
                return result;

            i++;
        }

        // If all parts match so far, shorter string comes first
        return xParts.Count.CompareTo(yParts.Count);
    }
}
