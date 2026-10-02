using System.Collections;

namespace OOP.Domain;

public static class ReportExtensions
{
    public static string ToReportTable<T>(this IEnumerable<T> items)
    {
        return string.Join(Environment.NewLine, items.Select(x => "- " + x));
    }
}
