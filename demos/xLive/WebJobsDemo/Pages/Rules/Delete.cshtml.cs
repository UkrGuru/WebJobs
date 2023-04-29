using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Rules;

public class DeleteModel : PageModel
{
    [BindProperty]
    public Rule Rule { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        Rule = await DbHelper.ExecAsync<Rule>("WJbRules_Get_Demo", id);

        if (Rule.RuleId == 0) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null) return NotFound();

        await DbHelper.ExecAsync<Rule>("WJbRules_Del_Demo", id);

        return RedirectToPage("./Index");
    }
}
