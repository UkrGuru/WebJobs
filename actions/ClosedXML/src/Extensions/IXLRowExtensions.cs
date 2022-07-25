using ClosedXML.Excel;

namespace UkrGuru.WebJobs.Actions.ClosedXML;

internal static class IXLRowExtensions
{
    public static string[] GetHeadNames(this IXLRow firstRow)
    {
        ArgumentNullException.ThrowIfNull(firstRow);

        var colNames = new List<string>(); int c = 0;
        foreach (var cell in firstRow.Cells())
        {
            colNames.Add(Convert.ToString(cell.Value) ?? $"Column{c}");
            c++;
        }

        return colNames.ToArray();
    }
}
