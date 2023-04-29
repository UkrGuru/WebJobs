using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Actions;

public class EditModel : PageModel
{
    [BindProperty]
    public ActionInput Action { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        Action = await DbHelper.ExecAsync<ActionInput>("WJbActions_Get_Demo", id);

        if (Action.ActionId == 0) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await DbHelper.ExecAsync("WJbActions_Upd_Demo", Action);

        return RedirectToPage("./Index");
    }
}
