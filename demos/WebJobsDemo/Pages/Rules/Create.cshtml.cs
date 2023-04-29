using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;
using Action = UkrGuru.WebJobs.Data.Action;

namespace WebJobsDemo.Pages.Rules;

public class CreateModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        Rule = new RuleInput();

        var actions = await DbHelper.ExecAsync<List<Action>>("WJbActions_Lst_Demo");

        ViewData["Actions"] = new SelectList(actions, "ActionId", "ActionName");

        return Page();
    }

    [BindProperty]
    public RuleInput Rule { get; set; } 

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await DbHelper.ExecAsync("WJbRules_Ins_Demo", Rule);

        return RedirectToPage("./Index");
    }
}
