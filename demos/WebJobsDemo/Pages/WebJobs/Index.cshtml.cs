using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.WebJobs;

public class IndexModel : PageModel
{
    public List<Job> Jobs { get; set; }

    public string Date { get; set; }

    public async Task OnGetAsync()
    {
        Date = HttpContext.Request.Query["date"];

        if (string.IsNullOrEmpty(Date)) Date = DateTime.Today.ToString("yyyy-MM-dd");

        Jobs = await DbHelper.ExecAsync<List<Job>>("WJbHistory_Grd_Demo", Date);
    }
}
