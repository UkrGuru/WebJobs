using System.Collections.Generic;
using System.Linq;

namespace WebJobsTests;

public class WorkerTests
{
    private const int NO_DELAY = 0;
    private const int MIN_DELAY = 100;
    private const int NEW_DELAY = 1000;
    private const int MAX_DELAY = 20000;

    public static IEnumerable<object[]> GetData4TestDelay(int numTests)
    {
        // job_err, job_found, next_err, next_found, expected
        var allData = new List<object[]>
        {
            new object[] { false, false, false, false, MIN_DELAY + NEW_DELAY },
            new object[] { false, false, false, true, MIN_DELAY + NEW_DELAY },
            new object[] { false, false, true, false, MIN_DELAY + NEW_DELAY },
            new object[] { false, false, true, true, MIN_DELAY + NEW_DELAY },

            new object[] { false, true, false, false, MIN_DELAY },
            new object[] { false, true, false, true, NO_DELAY },
            new object[] { false, true, true, false, MIN_DELAY + NEW_DELAY },
            new object[] { false, true, true, true, MIN_DELAY + NEW_DELAY },

            new object[] { true, false, false, false, MIN_DELAY + NEW_DELAY },
            new object[] { true, false, false, true, MIN_DELAY + NEW_DELAY },
            new object[] { true, false, true, false, MIN_DELAY + NEW_DELAY },
            new object[] { true, false, true, true, MIN_DELAY + NEW_DELAY },

            new object[] { true, true, false, false, MIN_DELAY + NEW_DELAY },
            new object[] { true, true, false, true, MIN_DELAY + NEW_DELAY },
            new object[] { true, true, true, false, MIN_DELAY + NEW_DELAY },
            new object[] { true, true, true, true, MIN_DELAY + NEW_DELAY },
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetData4TestDelay), parameters: 16)]
    public void TestDelay(object job_err, object job_found, object next_err, object next_found, object expected)
    {
        int delay = MIN_DELAY;

        try
        {
            if ((bool)job_err) throw new(); 

            if ((bool)job_found)
            {
                bool next_result = false;
                try
                {
                    if ((bool)next_err) throw new();

                    next_result = (bool)next_found;

                    delay = next_result ? NO_DELAY : MIN_DELAY;
                }
                catch
                {
                    delay += NEW_DELAY;
                }
            }
            else
            {
                if (delay < MAX_DELAY) delay += NEW_DELAY;
            }
        }
        catch
        {
            delay += NEW_DELAY;
        }

        Assert.Equal(expected, delay);

        //if (delay > 0)
        //{
        //    // delay
        //}
        //else
        //{
        //    delay = MIN_DELAY;
        //}
    }
}
