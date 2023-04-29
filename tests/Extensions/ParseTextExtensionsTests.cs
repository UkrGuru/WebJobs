using Xunit;
using UkrGuru.WebJobs.Data;
using System.Linq;
using static UkrGuru.WebJobs.Data.ParsingGoalExtensions;

namespace WebJobsTests.Extensions;

public class ParseTextExtensionsTests
{
    public static string Text = @"Order #123 from 01/07/2022

Customer: Company #1
Salesperson: Maria

Order Details
Product|Qty|Unit Price|Discount|Total Price|Status
Pears|30|30.00|10.00%|810.00|Invoiced
Apples|30|53.00|10.00%|1431.00|Invoiced

Grand Total: 2241.00

Payment Information
Payment Type: Check
Payment Date: 01/07/2022
Payment/Order Notes:
";

    public static ParsingGoal[] Goals = new ParsingGoal[] {
        new ParsingGoal(){ Name = "OrderId", Start = "Order #", End = "from" },
        new ParsingGoal(){ Name = "From", Start = "from", End = "\r\n" },
        new ParsingGoal(){ Name = "Customer", Start = "Customer:",  End = "\r\n" },
        new ParsingGoal(){ Name = "Salesperson", Start = "Salesperson:",  End = "\r\n" },
        new ParsingGoal(){ Name = "Order Details", Start = "Order Details",  End = "Grand Total" },
        new ParsingGoal(){ Name = "Grand Total", Start = "Grand Total:",  End = "\r\n" },
        new ParsingGoal(){ Name = "Payment Information", Start = "Payment Information" },
        new ParsingGoal(){ Name = "Payment Type", Parent = "Payment Information", Start = "Payment Type:",  End = "\r\n" },
        new ParsingGoal(){ Name = "Payment Date", Parent = "Payment Information", Start = "Payment Date:",  End = "\r\n" },
        new ParsingGoal(){ Name = "Payment Notes", Parent = "Payment Information", Start = "Payment/Order Notes:",  End = "" },
    };

    public static string Result = @"{""OrderId"":""123"",""From"":""01/07/2022"",""Customer"":""Company #1"",""Salesperson"":""Maria"",""Order Details"":""Product|Qty|Unit Price|Discount|Total Price|Status\r\nPears|30|30.00|10.00%|810.00|Invoiced\r\nApples|30|53.00|10.00%|1431.00|Invoiced"",""Grand Total"":""2241.00"",""Payment Type"":""Check"",""Payment Date"":""01/07/2022"",""Payment Notes"":""""}";

    [Theory]
    [InlineData("OrderId", "", "123")]
    [InlineData("From", "", "01/07/2022")]
    [InlineData("Customer", "", "Company #1")]
    [InlineData("Salesperson", "", "Maria")]
    [InlineData("Grand Total", "", "2241.00")]
    [InlineData("Payment Type", "Payment Information", "Check")]
    [InlineData("Payment Date", "Payment Information", "01/07/2022")]
    [InlineData("Payment Notes", "Payment Information", "")]
    public void GoalsParseValueTest(string name, string? parent, string? expected = default)
    {
        var goals = Goals.AppendRootNode(Text);

        var goal = goals.FirstOrDefault(e => e.Name.Equals(name) && e.Parent == parent);

        Assert.Equal(expected, goals.ParseValue(goal));
    }

    [Theory]
    [InlineData("OrderId", true)]
    [InlineData("From", true)]
    [InlineData("Customer", true)]
    [InlineData("Payment Type", true)]
    [InlineData("Payment Notes", true)]
    [InlineData("Payment Information", false)]
    [InlineData("", false)]
    public void GoalsIsLeafTest(string name, bool expected = default)
    {
        var goals = Goals.AppendRootNode(Text);

        Assert.Equal(expected, goals.IsLeaf(name));
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData(null, null, "", null)]
    [InlineData(null, null, "0", null)]

    [InlineData(null, "", null, null)]
    [InlineData(null, "", "", null)]
    [InlineData(null, "", "0", null)]

    [InlineData(null, "0", null, null)]
    [InlineData(null, "0", "", null)]
    [InlineData(null, "0", "0", null)]

    [InlineData("", null, null, "")]
    [InlineData("", null, "", "")]
    [InlineData("", null, "0", null)]

    [InlineData("", "", null, "")]
    [InlineData("", "", "", "")]
    [InlineData("", "", "0", null)]

    [InlineData("", "0", null, null)]
    [InlineData("", "0", "", null)]
    [InlineData("", "0", "0", null)]

    [InlineData("0", null, null, "0")]
    [InlineData("0", null, "", "0")]
    [InlineData("0", null, "0", "")]

    [InlineData("0", "", null, "0")]
    [InlineData("0", "", "", "0")]
    [InlineData("0", "", "0", "")]

    [InlineData("0", "0", null, "")]
    [InlineData("0", "0", "", "")]
    [InlineData("0", "0", "0", null)]

    [InlineData("01", null, null, "01")]
    [InlineData("01", null, "", "01")]
    [InlineData("01", null, "0", "")]
    [InlineData("01", null, "1", "0")]

    [InlineData("01", "", null, "01")]
    [InlineData("01", "", "", "01")]
    [InlineData("01", "", "0", "")]
    [InlineData("01", "", "1", "0")]

    [InlineData("01", "0", null, "1")]
    [InlineData("01", "0", "", "1")]
    [InlineData("01", "0", "0", null)]
    [InlineData("01", "0", "1", "")]

    [InlineData("012", null, null, "012")]
    [InlineData("012", null, "", "012")]
    [InlineData("012", null, "0", "")]
    [InlineData("012", null, "1", "0")]
    [InlineData("012", null, "2", "01")]
    [InlineData("012", null, "3", null)]

    [InlineData("012", "", null, "012")]
    [InlineData("012", "", "", "012")]
    [InlineData("012", "", "0", "")]
    [InlineData("012", "", "1", "0")]
    [InlineData("012", "", "2", "01")]
    [InlineData("012", "", "3", null)]

    [InlineData("012", "0", null, "12")]
    [InlineData("012", "0", "", "12")]
    [InlineData("012", "0", "0", null)]
    [InlineData("012", "0", "1", "")]
    [InlineData("012", "0", "2", "1")]
    [InlineData("012", "0", "3", null)]

    [InlineData("012", "1", null, "2")]
    [InlineData("012", "1", "", "2")]
    [InlineData("012", "1", "0", null)]
    [InlineData("012", "1", "1", null)]
    [InlineData("012", "1", "2", "")]
    [InlineData("012", "2", "1", null)]
    [InlineData("012", "1", "3", null)]

    public void CropTest(string? text, string? start, string? end = default, string? expected = default)
    {
        Assert.Equal(expected, ParsingGoalExtensions.Crop(text, start, end));
    }
}