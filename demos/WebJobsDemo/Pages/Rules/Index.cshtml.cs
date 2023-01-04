using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Rules;

public class IndexModel : PageModel
{
    public List<Rule> Rules { get; set; }

    public async Task OnGetAsync()
    {
        Rules = await DbHelper.ExecAsync<List<Rule>>("WJbRules_Grd_Demo");
    }
}
