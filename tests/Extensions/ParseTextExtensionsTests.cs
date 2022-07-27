using Xunit;
using UkrGuru.WebJobs.Data;
using static UkrGuru.WebJobs.Actions.ParseTextAction;
using System.Linq;

namespace WebJobsTests.Extensions;

public class ParseTextExtensionsTests
{
    private readonly ParsingGoal[] goals;

    public ParseTextExtensionsTests()
    {
        var text = @"Order #123 from 01/07/2022

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

        goals = new ParsingGoal[] { new ParsingGoal("OrderId", "", "Order #", "from"),
            new ParsingGoal("From", "", "from", "\n"),
            new ParsingGoal("Customer", "", "Customer:", "\n"),
            new ParsingGoal("Salesperson", "", "Salesperson:", "\n"),
            new ParsingGoal("Order Details", "", "Order Details", "Grand Total"),
            new ParsingGoal("Grand Total", "", "Grand Total:", "\n"),
            new ParsingGoal("Payment Information", "", "Payment Information"),
            new ParsingGoal("Payment Type", "Payment Information", "Payment Type:", "\n"),
            new ParsingGoal("Payment Date", "Payment Information", "Payment Date:", "\n"),
            new ParsingGoal("Payment Notes", "Payment Information", "Payment/Order Notes:", ""),
        };

        ParsingGoalExtensions.AppendRootNode(ref goals, text);
    }

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
        var goal = goals.FirstOrDefault(e => e.Name.Equals(name) && e.Parent == parent);

        Assert.Equal(expected, goals.ParseValue(goal));
    }
}