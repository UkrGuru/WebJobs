using ClosedXML.Excel;

namespace UkrGuru.WebJobs.Actions.ClosedXML;

internal static class DictionaryExtensions
{
    public static void Fill(this Dictionary<string, object> dict, IXLRow row, string[] head)
    {
        int c = 0;

        foreach (var cell in row.Cells())
        {
            dict[head[c++]] = cell.Value;
        }
    }
}
