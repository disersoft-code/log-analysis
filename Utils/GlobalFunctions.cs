using System.Globalization;
using System.Text.RegularExpressions;

namespace LogAnalysis.Utils;

internal static class GlobalFunctions
{
    private static readonly Regex _versionRegex = new(@"versi.n\s*([^:]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex _serialRegex = new(@"NUMERO\s+DE\s+SERIE\s*-\s*>\s*([A-Z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex _dateRegex = new(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3}", RegexOptions.Compiled);

    private static readonly Regex _userRegex = new(@"Usuario:\s*([^\s]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool IsValidFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (!File.Exists(path))
            return false;

        var fileInfo = new FileInfo(path);

        return fileInfo.Length > 0; // tiene contenido
    }

    public static bool ContainsIgnoreAccents(string text, string value)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(value))
            return false;

        return CultureInfo.CurrentCulture.CompareInfo.IndexOf(
            text,
            value,
            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
    }


    public static int ContainsIgnoreAccentsIndex(string text, string value)
    {
        return CultureInfo.CurrentCulture.CompareInfo
            .IndexOf(text, value, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
    }

    public static string? TryExtractVersion(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var match = _versionRegex.Match(line);
        if (!match.Success)
            return null;

        return match.Groups[1].Value.Trim();
    }

    public static string? TryExtractSerial(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var match = _serialRegex.Match(line);
        if (!match.Success)
            return null;

        return match.Groups[1].Value.Trim();
    }

    public static DateTime? TryParseDate(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var match = _dateRegex.Match(line);
        if (!match.Success)
            return null;

        if (DateTime.TryParseExact(
            match.Value,
            "yyyy-MM-dd HH:mm:ss,fff",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date))
        {
            return date;
        }

        return null;
    }

    public static string? TryExtractUser(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;

        var match = _userRegex.Match(line);
        if (!match.Success)
            return null;

        return match.Groups[1].Value.Trim();
    }



}
